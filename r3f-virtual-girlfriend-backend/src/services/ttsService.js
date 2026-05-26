// src/services/ttsService.js
// Servicio de Text-to-Speech con múltiples providers (Strategy Pattern)
import { createWriteStream } from "fs";
import axios from "axios";
import { config } from "../config/index.js";

/**
 * Genera audio MP3 usando Microsoft Edge TTS (gratuito)
 * @param {Object} ttsClient - Instancia de MsEdgeTTS
 * @param {string} text - Texto a sintetizar
 * @param {string} outPath - Ruta de salida del MP3
 */
export const synthesizeWithEdgeTTS = (ttsClient, text, outPath) => {
  if (!text || text.trim().length === 0) {
    return Promise.reject(new Error("Text cannot be empty"));
  }
  if (!outPath) {
    return Promise.reject(new Error("outPath is required"));
  }

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

/**
 * Genera audio MP3 usando ElevenLabs (premium)
 * @param {string} text - Texto a sintetizar
 * @param {string} outPath - Ruta de salida del MP3
 * @param {Object} httpClient - Cliente HTTP (axios) inyectable para tests
 */
export const synthesizeWithElevenLabs = async (
  text,
  outPath,
  httpClient = axios
) => {
  if (!text || text.trim().length === 0) {
    throw new Error("Text cannot be empty");
  }
  if (!outPath) throw new Error("outPath is required");

  const apiKey = config.tts.elevenLabs.apiKey;
  if (!apiKey) {
    throw new Error("ELEVEN_LABS_API_KEY no está configurada en .env");
  }

  const response = await httpClient({
    method: "POST",
    url: `https://api.elevenlabs.io/v1/text-to-speech/${config.tts.elevenLabs.voiceId}`,
    headers: {
      Accept: "audio/mpeg",
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

/**
 * Selecciona el provider de TTS según el parámetro
 * @param {Object} ttsClient - Cliente Edge TTS
 * @param {string} text - Texto a sintetizar
 * @param {string} outPath - Ruta del archivo de salida
 * @param {string} provider - "edge" | "elevenlabs"
 */
export const synthesizeToMp3 = (
  ttsClient,
  text,
  outPath,
  provider = "edge"
) => {
  if (provider === "elevenlabs") {
    return synthesizeWithElevenLabs(text, outPath);
  }
  return synthesizeWithEdgeTTS(ttsClient, text, outPath);
};
