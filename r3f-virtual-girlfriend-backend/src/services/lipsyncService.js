// src/services/lipsyncService.js
// Servicio para generación de lipsync con Rhubarb
import { exec } from "child_process";
import { getRhubarbBin } from "../config/index.js";

/**
 * Ejecuta un comando del sistema
 * @param {string} command - Comando a ejecutar
 * @param {Function} execFn - Función exec inyectable (para tests)
 */
export const execCommand = (command, execFn = exec) => {
  return new Promise((resolve, reject) => {
    execFn(command, (error, stdout) => {
      if (error) return reject(error);
      resolve(stdout);
    });
  });
};

/**
 * Genera lipsync de un mensaje:
 *   1. Convierte MP3 → WAV con FFmpeg
 *   2. Genera visemes con Rhubarb
 * @param {number|string} messageId - ID del mensaje
 * @param {Function} execFn - Función exec inyectable
 */
export const lipSyncMessage = async (messageId, execFn = exec) => {
  const start = Date.now();
  const mp3 = `audios/message_${messageId}.mp3`;
  const wav = `audios/message_${messageId}.wav`;
  const json = `audios/message_${messageId}.json`;

  await execCommand(`ffmpeg -y -i ${mp3} ${wav}`, execFn);
  const ffmpegTime = Date.now() - start;

  await execCommand(
    `${getRhubarbBin()} -f json -o ${json} ${wav} -r phonetic`,
    execFn
  );
  const totalTime = Date.now() - start;

  return { ffmpegTime, totalTime, files: { mp3, wav, json } };
};
