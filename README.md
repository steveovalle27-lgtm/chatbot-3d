# 🤖 Chatbot Avatar 3D

> Chatbot interactivo con avatar 3D, IA generativa, voz neuronal y sincronización labial en tiempo real.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Node](https://img.shields.io/badge/Node-18%2B-339933?logo=node.js&logoColor=white)](https://nodejs.org)
[![React](https://img.shields.io/badge/React-18.2-61DAFB?logo=react&logoColor=white)](https://react.dev)
[![Three.js](https://img.shields.io/badge/Three.js-r128-000000?logo=three.js&logoColor=white)](https://threejs.org)
[![Tests](https://img.shields.io/badge/Tests-40%20MSTest-512BD4?logo=.net&logoColor=white)](Tests/)

---

## 📸 Demo

Un avatar 3D que:
- 💬 Conversa contigo en español
- 🎙️ Habla con voz neuronal natural
- 👄 Sincroniza labios al hablar (lipsync real)
- 😊 Muestra expresiones faciales
- 💾 Recuerda tu historial

---

## ✨ Características

| Característica | Tecnología |
|---|---|
| 🧠 **IA generativa** | Groq Llama 3.3 70B (gratis, 10× más rápido que OpenAI) |
| 🎙️ **Síntesis de voz** | Microsoft Edge TTS (gratis ilimitado) + ElevenLabs (opcional) |
| 👄 **Lipsync** | Rhubarb Lip-Sync v1.13 (open source) |
| 🎭 **Avatar 3D** | Three.js + React Three Fiber con morph targets |
| 💾 **Persistencia** | Supabase PostgreSQL (free tier) |
| 🔄 **Fallback automático** | Si ElevenLabs falla → Edge TTS sin interrupciones |
| 🎨 **Modos de chat** | Solo texto / Solo audio / Audio + texto |
| 🧪 **Testing** | 40 pruebas MSTest + cobertura Visual Studio |

---

## 🚀 Quick Start

### 📋 Requisitos

- [Node.js 18+](https://nodejs.org)
- [FFmpeg](https://ffmpeg.org) (para conversión de audio)
- Cuenta gratuita en [Groq](https://console.groq.com) (API key)
- Cuenta gratuita en [Supabase](https://supabase.com)
- (Opcional) [.NET SDK 8](https://dotnet.microsoft.com/download) para correr tests

### 1️⃣ Clonar el repositorio

```bash
git clone https://github.com/TU_USUARIO/chatbot-3d.git
cd chatbot-3d
```

### 2️⃣ Configurar variables de entorno

```bash
# Backend
cd r3f-virtual-girlfriend-backend
cp .env.example .env
# Editar .env con tus claves reales

# Frontend (opcional, ya tiene default)
cd ../r3f-virtual-girlfriend-frontend
cp .env.example .env
```

### 3️⃣ Setup de Supabase

1. Crear proyecto en [supabase.com/dashboard](https://supabase.com/dashboard)
2. Ir a **SQL Editor** → ejecutar el script:
   ```
   Tests/SUPABASE_SETUP.sql
   ```
3. Copiar las claves a tu `.env`

### 4️⃣ Instalar dependencias

```bash
# Desde la raíz del proyecto
npm run install:all
```

### 5️⃣ Lanzar la app

**Opción A — Un solo comando:**
```bash
npm run dev
```

**Opción B — Doble-click (Windows):**
```
start-chatbot.bat
```

**Acceso:**
- 🌐 Frontend: http://localhost:5173
- 🔧 Backend: http://localhost:3000

---

## 📂 Estructura del Proyecto

```
chatbot-3d/
├── 📄 ANALISIS.MD                    # Documentación Fase 1 (DSDM)
├── 📄 DISEÑO.MD                       # Documentación Fase 2 (DSDM)
├── 📄 README.md                       # Este archivo
├── 📄 LICENSE                         # MIT License
├── 📄 package.json                    # Orquestador (concurrently)
├── 📄 start-chatbot.bat               # Lanzador Windows
│
├── 🔧 r3f-virtual-girlfriend-backend/
│   ├── src/                          # Clean Architecture
│   │   ├── config/                   # Configuración
│   │   ├── services/                 # Lógica de negocio
│   │   └── utils/                    # Helpers
│   ├── bin/rhubarb.exe              # Rhubarb v1.13.0
│   ├── audios/                       # Archivos generados (gitignored)
│   ├── index.js                      # Express entry
│   └── .env.example                  # Template variables
│
├── 🎨 r3f-virtual-girlfriend-frontend/
│   ├── src/
│   │   ├── components/               # Avatar.jsx, UI.jsx
│   │   ├── hooks/                    # useChat.jsx (Context)
│   │   └── App.jsx
│   ├── public/models/                # Modelos GLB
│   └── vite.config.js
│
└── 🧪 Tests/
    ├── ChatBot.sln                   # Solución MSTest
    ├── SUPABASE_SETUP.sql           # Setup BD
    └── ChatBot.Application.Test/    # 40 pruebas
```

---

## 🎮 Uso

### Cambiar modo de respuesta
Click en el botón superior izquierdo cicla entre:
- 🔊 **Audio + Texto** (default)
- 🎵 **Solo Audio**
- 💬 **Solo Texto**

### Cambiar provider de voz
- 🆓 **Edge TTS** — Gratis ilimitado (default)
- 💎 **ElevenLabs** — Premium (requiere API key)

Si ElevenLabs falla, el sistema **automáticamente** usa Edge TTS sin interrumpir.

### Atajos de teclado
- `Enter` — Enviar mensaje
- Botón cámara — Zoom in/out
- Botón pantalla — Activar green screen

---

## 🧪 Testing

### Pruebas Unitarias (MSTest)

```bash
cd Tests/ChatBot.Application.Test
dotnet test
```

### Cobertura en Visual Studio

1. Abrir `Tests/ChatBot.sln` en Visual Studio 2022
2. `Test → Analyze Code Coverage for All Tests`
3. Ver reporte visual con líneas verdes/rojas

### Casos de prueba: **40 tests** organizados en:
- `HealthEndpointTest` (3) — Health check
- `VoicesEndpointTest` (5) — Voces disponibles
- `ChatEndpointTest` (8) — Endpoint principal
- `ChatModesTest` (7) — Modos text/audio/both
- `TtsProvidersTest` (6) — Edge vs ElevenLabs
- `ConversationsEndpointTest` (11) — CRUD conversaciones

---

## 🏗️ Arquitectura

### Stack Tecnológico

```
┌─────────────────────────────────────┐
│ FRONTEND                            │
│ React 18 + Vite + Three.js          │
└──────────────┬──────────────────────┘
               │ HTTP REST
               ▼
┌─────────────────────────────────────┐
│ BACKEND                             │
│ Node.js + Express                   │
│ Clean Architecture (Domain/App/Infra)│
└─┬───────┬───────┬─────────┬─────────┘
  │       │       │         │
  ▼       ▼       ▼         ▼
Groq   EdgeTTS  Rhubarb  Supabase
LLM    ElevenLabs (CLI)  PostgreSQL
```

Ver `DISEÑO.MD` para arquitectura completa con diagramas UML.

---

## 📚 Documentación

| Archivo | Contenido |
|---------|-----------|
| [`ANALISIS.MD`](ANALISIS.MD) | Análisis del proyecto (SRS, MoSCoW, Casos de Uso, Riesgos) |
| [`DISEÑO.MD`](DISEÑO.MD) | Diseño técnico (UML, ER, API spec, Patrones) |
| [`Tests/README.md`](Tests/README.md) | Guía de pruebas y cobertura |

---

## 🔐 Seguridad

- ✅ API keys en `.env` (jamás en código)
- ✅ `.env` excluido por `.gitignore`
- ✅ Supabase RLS habilitado
- ✅ CORS configurado
- ✅ Service Role Key solo server-side

---

## 🛠️ Metodología

Desarrollado siguiendo **DSDM (Dynamic Systems Development Method)**:

1. **Fase 1 — Análisis** (ANALISIS.MD)
2. **Fase 2 — Diseño** (DISEÑO.MD)
3. **Fase 3 — Implementación iterativa**
4. **Fase 4 — Pruebas** (Tests/)
5. **Fase 5 — Despliegue**

Priorización **MoSCoW** aplicada en todo el proyecto.

---

## 🤝 Contribuir

```bash
# 1. Fork el proyecto
# 2. Crear rama de feature
git checkout -b feature/MiNuevaFeature

# 3. Commit cambios
git commit -m "feat: agrega nueva feature"

# 4. Push a tu fork
git push origin feature/MiNuevaFeature

# 5. Abrir Pull Request
```

---

## 📝 Roadmap

- [x] Backend con Groq + Edge TTS
- [x] Avatar 3D con lipsync
- [x] Persistencia Supabase
- [x] 40 pruebas MSTest
- [x] Documentación DSDM completa
- [ ] Login con OAuth (Google, GitHub)
- [ ] Reconocimiento de voz del usuario (ASR)
- [ ] Multi-idioma simultáneo
- [ ] Modo offline con Ollama local
- [ ] Despliegue en Vercel + Railway

---

## 📄 Licencia

MIT — ver [`LICENSE`](LICENSE) para más detalles.

---

## 👤 Autor

Desarrollado por **[Tu Nombre]** como proyecto académico aplicando **Clean Architecture + DSDM + MSTest**.

---

## 🙏 Créditos

- [Wawa Sensei](https://wawasensei.dev) — Plantilla original `r3f-virtual-girlfriend`
- [Groq](https://groq.com) — LLM inference gratis y ultrarrápido
- [Microsoft Edge TTS](https://github.com/Migushthe2nd/MsEdgeTTS) — Voz neuronal
- [Rhubarb Lip-Sync](https://github.com/DanielSWolf/rhubarb-lip-sync) — Lipsync
- [Supabase](https://supabase.com) — Postgres hosted
- [Three.js](https://threejs.org) — Engine 3D

---

⭐ Si este proyecto te fue útil, ¡dale una estrella!
