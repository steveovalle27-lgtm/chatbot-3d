import { exec } from "child_process";
import cors from "cors";
import dotenv from "dotenv";
import express from "express";
import { promises as fs, createWriteStream } from "fs";
import OpenAI from "openai";
import { createClient } from "@supabase/supabase-js";
import { MsEdgeTTS, OUTPUT_FORMAT } from "msedge-tts";
import axios from "axios";
dotenv.config();

// Using Groq API (OpenAI-compatible) — free & fast
const openai = new OpenAI({
  apiKey: process.env.GROQ_API_KEY || "-",
  baseURL: "https://api.groq.com/openai/v1",
});

// Supabase client (service role — bypasses RLS for backend operations)
const supabase = createClient(
  process.env.SUPABASE_URL,
  process.env.SUPABASE_SERVICE_KEY
);

// Microsoft Edge TTS — gratis, ilimitado, voces neuronales en español
// Voces recomendadas:
//   es-MX-DaliaNeural   (mexicana femenina, joven y natural)
//   es-MX-JorgeNeural   (mexicano masculino)
//   es-ES-ElviraNeural  (española femenina)
//   es-AR-ElenaNeural   (argentina femenina)
const TTS_VOICE = "es-MX-DaliaNeural";

const ttsClient = new MsEdgeTTS();
await ttsClient.setMetadata(TTS_VOICE, OUTPUT_FORMAT.AUDIO_24KHZ_48KBITRATE_MONO_MP3);
console.log(`🎙️  Edge TTS configurado con voz: ${TTS_VOICE}`);

// Handlers globales — evitan que errores async no capturados tiren el servidor
process.on("unhandledRejection", (err) => {
  console.error("⚠️  Unhandled rejection:", err?.message || err);
});
process.on("uncaughtException", (err) => {
  console.error("⚠️  Uncaught exception:", err?.message || err);
});

// Genera un MP3 desde texto usando Edge TTS via stream (gratis, ilimitado)
const synthesizeWithEdgeTTS = async (text, outPath) => {
  return new Promise((resolve, reject) => {
    const { audioStream } = ttsClient.toStream(text);
    const fileStream = createWriteStream(outPath);

    audioStream.on("data", (chunk) => fileStream.write(chunk));
    audioStream.on("end", () => {
      fileStream.end();
      resolve();
    });
    audioStream.on("error", (err) => {
      fileStream.destroy();
      reject(err);
    });
    fileStream.on("error", reject);
  });
};

// Genera un MP3 desde texto usando ElevenLabs (premium, requiere API key)
const ELEVENLABS_VOICE_ID = "21m00Tcm4TlvDq8ikWAM"; // Rachel - voz pública
const synthesizeWithElevenLabs = async (text, outPath) => {
  const apiKey = process.env.ELEVEN_LABS_API_KEY;
  if (!apiKey) throw new Error("ELEVEN_LABS_API_KEY no está configurada en .env");

  const response = await axios({
    method: "POST",
    url: `https://api.elevenlabs.io/v1/text-to-speech/${ELEVENLABS_VOICE_ID}`,
    headers: {
      "Accept": "audio/mpeg",
      "Content-Type": "application/json",
      "xi-api-key": apiKey,
    },
    data: {
      text,
      model_id: "eleven_multilingual_v2",
      voice_settings: { stability: 0.5, similarity_boost: 0.75 },
    },
    responseType: "stream",
  });

  return new Promise((resolve, reject) => {
    const fileStream = createWriteStream(outPath);
    response.data.pipe(fileStream);
    fileStream.on("finish", resolve);
    fileStream.on("error", reject);
    response.data.on("error", reject);
  });
};

// Selecciona el provider de TTS según el parámetro recibido
const synthesizeToMp3 = async (text, outPath, provider = "edge") => {
  if (provider === "elevenlabs") {
    return synthesizeWithElevenLabs(text, outPath);
  }
  return synthesizeWithEdgeTTS(text, outPath);
};

const app = express();
app.use(express.json());
app.use(cors());
const port = 3000;

app.get("/", (req, res) => {
  res.send("Chatbot Avatar 3D — Backend running!");
});

app.get("/voices", async (req, res) => {
  try {
    const voices = await ttsClient.getVoices();
    const spanishVoices = voices.filter(v => v.Locale.startsWith("es-"));
    res.json({ active: TTS_VOICE, available: spanishVoices });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// ─── CONVERSATIONS ────────────────────────────────────────────────────────────

// Obtener todas las conversaciones de un usuario
app.get("/conversations", async (req, res) => {
  const userId = req.headers["x-user-id"] || "anonymous";
  const { data, error } = await supabase
    .from("conversations")
    .select("*")
    .eq("user_id", userId)
    .order("updated_at", { ascending: false });

  if (error) return res.status(500).json({ error: error.message });
  res.json(data);
});

// Crear nueva conversación
app.post("/conversations", async (req, res) => {
  const userId = req.headers["x-user-id"] || "anonymous";
  const { title } = req.body;
  const { data, error } = await supabase
    .from("conversations")
    .insert({ user_id: userId, title: title || "Nueva conversación" })
    .select()
    .single();

  if (error) return res.status(500).json({ error: error.message });
  res.json(data);
});

// Obtener mensajes de una conversación
app.get("/conversations/:id/messages", async (req, res) => {
  const { data, error } = await supabase
    .from("messages")
    .select("*")
    .eq("conversation_id", req.params.id)
    .order("created_at", { ascending: true });

  if (error) return res.status(500).json({ error: error.message });
  res.json(data);
});

// Borrar conversación
app.delete("/conversations/:id", async (req, res) => {
  const { error } = await supabase
    .from("conversations")
    .delete()
    .eq("id", req.params.id);

  if (error) return res.status(500).json({ error: error.message });
  res.json({ success: true });
});

// ─── CHAT ─────────────────────────────────────────────────────────────────────

const execCommand = (command) => {
  return new Promise((resolve, reject) => {
    exec(command, (error, stdout, stderr) => {
      if (error) reject(error);
      resolve(stdout);
    });
  });
};

const isWindows = process.platform === "win32";
const rhubarbBin = isWindows ? ".\\bin\\rhubarb.exe" : "./bin/rhubarb";

const lipSyncMessage = async (message) => {
  const time = new Date().getTime();
  console.log(`Starting conversion for message ${message}`);
  await execCommand(
    `ffmpeg -y -i audios/message_${message}.mp3 audios/message_${message}.wav`
  );
  console.log(`Conversion done in ${new Date().getTime() - time}ms`);
  await execCommand(
    `${rhubarbBin} -f json -o audios/message_${message}.json audios/message_${message}.wav -r phonetic`
  );
  console.log(`Lip sync done in ${new Date().getTime() - time}ms`);
};

app.post("/chat", async (req, res) => {
  try {
  const userMessage = req.body.message;
  const conversationId = req.body.conversationId;
  // mode: "both" | "audio" | "text" — controla qué se genera/devuelve
  const mode = req.body.mode || "both";
  // ttsProvider: "edge" | "elevenlabs"
  const ttsProvider = req.body.ttsProvider || "edge";
  const userId = req.headers["x-user-id"] || "anonymous";
  console.log(`📨 Chat request: mode=${mode}, provider=${ttsProvider}, msg="${userMessage?.slice(0,40)}..."`);

  if (!userMessage) {
    res.send({
      messages: [
        {
          text: "¡Hola! ¿En qué puedo ayudarte hoy?",
          audio: await audioFileToBase64("audios/intro_0.wav"),
          lipsync: await readJsonTranscript("audios/intro_0.json"),
          facialExpression: "smile",
          animation: "Talking_1",
        },
      ],
    });
    return;
  }

  if (!process.env.GROQ_API_KEY) {
    res.send({
      messages: [
        {
          text: "Falta GROQ_API_KEY en el archivo .env",
          audio: null,
          lipsync: null,
          facialExpression: "angry",
          animation: "Angry",
        },
      ],
    });
    return;
  }

  // Guardar mensaje del usuario en Supabase
  let activeConversationId = conversationId;
  if (activeConversationId) {
    await supabase.from("messages").insert({
      conversation_id: activeConversationId,
      role: "user",
      content: userMessage,
    });
    // Actualizar título si es el primer mensaje
    await supabase
      .from("conversations")
      .update({
        title: userMessage.slice(0, 50),
        updated_at: new Date().toISOString(),
      })
      .eq("id", activeConversationId)
      .eq("title", "Nueva conversación");
  }

  // Obtener historial de la conversación para contexto
  let history = [];
  if (activeConversationId) {
    const { data: prevMessages } = await supabase
      .from("messages")
      .select("role, content")
      .eq("conversation_id", activeConversationId)
      .order("created_at", { ascending: true })
      .limit(20);
    history = prevMessages || [];
  }

  const completion = await openai.chat.completions.create({
    model: "llama-3.3-70b-versatile",
    max_tokens: 1000,
    temperature: 0.6,
    response_format: { type: "json_object" },
    messages: [
      {
        role: "system",
        content: `
        Eres un asistente virtual amigable e inteligente.
        Responde siempre en español.
        Responde con un JSON array de mensajes (máximo 3).
        Cada mensaje tiene: text, facialExpression, animation.
        Expresiones faciales: smile, sad, angry, surprised, funnyFace, default.
        Animaciones: Talking_0, Talking_1, Talking_2, Crying, Laughing, Rumba, Idle, Terrified, Angry.
        `,
      },
      ...history,
      { role: "user", content: userMessage },
    ],
  });

  let messages = JSON.parse(completion.choices[0].message.content);
  if (messages.messages) messages = messages.messages;

  for (let i = 0; i < messages.length; i++) {
    const message = messages[i];
    const fileName = `audios/message_${i}.mp3`;

    // Si modo "text", saltar generación de audio completamente
    if (mode === "text") {
      message.audio = null;
      message.lipsync = null;
    } else {
      // Modos "audio" o "both" -> generar voz + lipsync con el provider elegido
      try {
        await synthesizeToMp3(message.text, fileName, ttsProvider);
        await lipSyncMessage(i);
        message.audio = await audioFileToBase64(fileName);
        message.lipsync = await readJsonTranscript(`audios/message_${i}.json`);
        console.log(`✅ Audio generado (provider=${ttsProvider}) para mensaje ${i}`);
      } catch (err) {
        console.error(`⚠️  Error generando audio/lipsync (provider=${ttsProvider}) para mensaje ${i}:`, err.message || err.code || err);
        message.audio = null;
        message.lipsync = null;
        // Si era ElevenLabs y falló, intentar fallback a Edge TTS
        if (ttsProvider === "elevenlabs") {
          try {
            console.log(`🔄 Intentando fallback a Edge TTS para mensaje ${i}`);
            await synthesizeWithEdgeTTS(message.text, fileName);
            await lipSyncMessage(i);
            message.audio = await audioFileToBase64(fileName);
            message.lipsync = await readJsonTranscript(`audios/message_${i}.json`);
            message.fallbackUsed = "edge";
          } catch (fbErr) {
            console.error(`❌ Fallback también falló:`, fbErr.message);
          }
        }
      }
    }

    // Si modo "audio", borrar el texto antes de devolverlo
    if (mode === "audio") {
      message.displayText = ""; // el frontend usa esto para no mostrar burbuja
    }

    // Guardar respuesta del asistente en Supabase
    if (activeConversationId) {
      try {
        await supabase.from("messages").insert({
          conversation_id: activeConversationId,
          role: "assistant",
          content: message.text,
          animation: message.animation,
          facial_expression: message.facialExpression,
        });
      } catch (err) {
        console.error(`⚠️  Error guardando en Supabase:`, err.message);
      }
    }
  }

  res.send({ messages, conversationId: activeConversationId });
  } catch (err) {
    console.error("❌ Error en /chat:", err.message || err);
    res.status(500).send({
      messages: [{
        text: "Hubo un error procesando tu mensaje. Revisa la consola del backend.",
        facialExpression: "sad",
        animation: "Idle",
        audio: null,
        lipsync: null,
      }]
    });
  }
});

// ─── HELPERS ──────────────────────────────────────────────────────────────────

const readJsonTranscript = async (file) => {
  const data = await fs.readFile(file, "utf8");
  return JSON.parse(data);
};

const audioFileToBase64 = async (file) => {
  const data = await fs.readFile(file);
  return data.toString("base64");
};

app.listen(port, () => {
  console.log(`🤖 Chatbot Avatar 3D — listening on port ${port}`);
  console.log(`📊 Supabase: ${process.env.SUPABASE_URL ? "✅ connected" : "❌ missing"}`);
  console.log(`🧠 Groq: ${process.env.GROQ_API_KEY ? "✅ ready" : "❌ missing"}`);
  console.log(`🎙️  Edge TTS: ✅ ready (voz: ${TTS_VOICE})`);
});
