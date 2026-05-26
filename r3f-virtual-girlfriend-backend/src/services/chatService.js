// src/services/chatService.js
// Servicio principal del chat — orquesta LLM + TTS + Lipsync + DB
import { config } from "../config/index.js";

/**
 * Construye el mensaje de bienvenida cuando no hay input del usuario
 */
export const buildWelcomeMessage = (audioBase64, lipsyncJson) => ({
  messages: [
    {
      text: "¡Hola! ¿En qué puedo ayudarte hoy?",
      audio: audioBase64,
      lipsync: lipsyncJson,
      facialExpression: "smile",
      animation: "Talking_1",
    },
  ],
});

/**
 * Construye el mensaje de error cuando falta API key
 */
export const buildMissingKeyMessage = () => ({
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

/**
 * Construye respuesta de error genérico
 */
export const buildErrorMessage = (errMsg) => ({
  messages: [
    {
      text: errMsg || "Hubo un error procesando tu mensaje.",
      facialExpression: "sad",
      animation: "Idle",
      audio: null,
      lipsync: null,
    },
  ],
});

/**
 * Parsea respuesta del LLM (puede venir como { messages: [...] } o array directo)
 */
export const parseLLMResponse = (rawContent) => {
  if (!rawContent || typeof rawContent !== "string") {
    throw new Error("Invalid LLM response");
  }
  let parsed = JSON.parse(rawContent);
  if (parsed.messages) parsed = parsed.messages;
  if (!Array.isArray(parsed)) {
    throw new Error("LLM response must be an array");
  }
  return parsed;
};

/**
 * Construye el system prompt para el LLM
 */
export const buildSystemPrompt = () => `
        Eres un asistente virtual amigable e inteligente.
        Responde siempre en español.
        Responde con un JSON array de mensajes (máximo 3).
        Cada mensaje tiene: text, facialExpression, animation.
        Expresiones faciales: smile, sad, angry, surprised, funnyFace, default.
        Animaciones: Talking_0, Talking_1, Talking_2, Crying, Laughing, Rumba, Idle, Terrified, Angry.
        `;

/**
 * Aplica el modo de respuesta al mensaje
 */
export const applyResponseMode = (message, mode) => {
  if (mode === "text") {
    message.audio = null;
    message.lipsync = null;
  }
  if (mode === "audio") {
    message.displayText = "";
  }
  return message;
};

/**
 * Valida que el modo de respuesta sea válido
 */
export const validateMode = (mode) => {
  const validModes = ["text", "audio", "both"];
  return validModes.includes(mode) ? mode : "both";
};

/**
 * Valida el provider TTS
 */
export const validateProvider = (provider) => {
  const validProviders = ["edge", "elevenlabs"];
  return validProviders.includes(provider) ? provider : "edge";
};

/**
 * Determina si se debe generar audio según el modo
 */
export const shouldGenerateAudio = (mode) => mode !== "text";
