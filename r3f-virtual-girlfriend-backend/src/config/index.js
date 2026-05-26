// src/config/index.js
// Configuración centralizada
import dotenv from "dotenv";
dotenv.config();

export const config = {
  groq: {
    apiKey: process.env.GROQ_API_KEY || "",
    model: "llama-3.3-70b-versatile",
    baseURL: "https://api.groq.com/openai/v1",
  },
  supabase: {
    url: process.env.SUPABASE_URL || "",
    serviceKey: process.env.SUPABASE_SERVICE_KEY || "",
  },
  tts: {
    edgeVoice: "es-MX-DaliaNeural",
    elevenLabs: {
      apiKey: process.env.ELEVEN_LABS_API_KEY || "",
      voiceId: "21m00Tcm4TlvDq8ikWAM",
    },
  },
  server: {
    port: parseInt(process.env.PORT || "3000", 10),
  },
};

export const isWindows = () => process.platform === "win32";

export const getRhubarbBin = () =>
  isWindows() ? ".\\bin\\rhubarb.exe" : "./bin/rhubarb";

export default config;
