// src/utils/fileUtils.js
// Utilidades de manejo de archivos
import { promises as fs } from "fs";

/**
 * Lee un archivo JSON y lo parsea
 * @param {string} file - Ruta al archivo
 * @returns {Promise<Object>}
 */
export const readJsonTranscript = async (file) => {
  if (!file) throw new Error("File path is required");
  const data = await fs.readFile(file, "utf8");
  return JSON.parse(data);
};

/**
 * Lee un archivo de audio y lo convierte a base64
 * @param {string} file - Ruta al archivo
 * @returns {Promise<string>}
 */
export const audioFileToBase64 = async (file) => {
  if (!file) throw new Error("File path is required");
  const data = await fs.readFile(file);
  return data.toString("base64");
};
