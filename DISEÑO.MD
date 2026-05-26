# 🎨 DISEÑO DEL PROYECTO — Chatbot Avatar 3D

> **Fase 2 del SDLC bajo metodología DSDM**
> Versión 1.0 — Mayo 2026
> Documento complementario a `ANALISIS.MD`

---

## 📑 ÍNDICE

1. [Introducción](#1-introducción)
2. [Arquitectura General](#2-arquitectura-general)
3. [Stack Tecnológico](#3-stack-tecnológico)
4. [Patrones Arquitectónicos](#4-patrones-arquitectónicos)
5. [Diagrama de Componentes](#5-diagrama-de-componentes)
6. [Diagrama de Clases](#6-diagrama-de-clases-uml)
7. [Diagrama de Secuencia](#7-diagrama-de-secuencia-uml)
8. [Diagrama de Despliegue](#8-diagrama-de-despliegue)
9. [Modelo de Datos (ER)](#9-modelo-de-datos-er)
10. [Diccionario de Datos](#10-diccionario-de-datos)
11. [Diseño de API REST](#11-diseño-de-api-rest)
12. [Diseño de Interfaz (Wireframes)](#12-diseño-de-interfaz-wireframes)
13. [Patrones de Diseño Aplicados](#13-patrones-de-diseño-aplicados)
14. [Seguridad](#14-diseño-de-seguridad)
15. [Estructura de Carpetas](#15-estructura-de-carpetas)

---

## 1. INTRODUCCIÓN

### 1.1 Propósito

Este documento describe el **diseño técnico** del sistema _Chatbot Avatar 3D_,
detallando la arquitectura, componentes, modelos de datos, interfaces, y patrones
empleados para satisfacer los requerimientos definidos en `ANALISIS.MD`.

### 1.2 Audiencia

- Desarrolladores que implementarán el sistema
- Arquitectos que validarán las decisiones técnicas
- Testers que diseñarán los casos de prueba
- Equipo DevOps para despliegue

### 1.3 Trazabilidad

Cada decisión de diseño aquí presentada se mapea a un requerimiento de ANALISIS.MD:

| Diseño | Cumple Requerimiento |
|--------|----------------------|
| Sección 4 (Clean Arch) | RNF-19 (Mantenibilidad) |
| Sección 11 (API REST) | RF-01 a RF-15 |
| Sección 14 (Seguridad) | RNF-14 a RNF-18 |

---

## 2. ARQUITECTURA GENERAL

### 2.1 Visión 3-Tier

```
┌──────────────────────────────────────────────────────────────────────┐
│                      CAPA DE PRESENTACIÓN                            │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ Frontend (React 18 + Vite + Three.js)                       │   │
│  │  • Canvas 3D (Avatar.jsx con morph targets)                 │   │
│  │  • UI Components (Chat, Botones, Toggles)                   │   │
│  │  • Context Provider (useChat hook)                          │   │
│  └─────────────────────────────────────────────────────────────┘   │
└─────────────────────────────┬────────────────────────────────────────┘
                              │ HTTP REST (JSON)
                              │ http://localhost:3000
                              ▼
┌──────────────────────────────────────────────────────────────────────┐
│                      CAPA DE LÓGICA DE NEGOCIO                       │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ Backend (Node.js + Express.js)                              │   │
│  │  ┌──────────────────────────────────────────────────────┐   │   │
│  │  │  src/api/        — Endpoints REST                    │   │   │
│  │  │  src/services/   — Lógica de negocio                 │   │   │
│  │  │  src/utils/      — Utilidades (archivos, audio)      │   │   │
│  │  │  src/config/     — Configuración centralizada        │   │   │
│  │  └──────────────────────────────────────────────────────┘   │   │
│  └─────────────────────────────────────────────────────────────┘   │
└──────────┬─────────────────┬─────────────────┬─────────────────────┘
           │                 │                 │
           ▼                 ▼                 ▼
┌─────────────────┐ ┌────────────────┐ ┌─────────────────────┐
│   Groq API      │ │  Edge TTS      │ │  Rhubarb CLI        │
│  (LLM Llama 3.3)│ │  ElevenLabs    │ │  (FFmpeg + Rhubarb) │
└─────────────────┘ └────────────────┘ └─────────────────────┘
           │
           ▼
┌──────────────────────────────────────────────────────────────────────┐
│                     CAPA DE PERSISTENCIA                             │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ Supabase (PostgreSQL hosted)                                │   │
│  │  ┌──────────────┐  ┌──────────────┐                         │   │
│  │  │ conversations│  │   messages   │                         │   │
│  │  └──────────────┘  └──────────────┘                         │   │
│  └─────────────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────────────┘
```

### 2.2 Estilo Arquitectónico

| Estilo | Aplicación |
|--------|-----------|
| **Cliente-Servidor** | Frontend separado del backend |
| **REST API** | Comunicación via HTTP/JSON |
| **Layered (Capas)** | Presentación → Negocio → Datos |
| **Clean Architecture** | Backend dividido en Domain/App/Infra |
| **Strategy Pattern** | Múltiples TTS providers intercambiables |
| **Repository Pattern** | Abstrae acceso a Supabase |

---

## 3. STACK TECNOLÓGICO

### 3.1 Frontend

| Tecnología | Versión | Uso |
|-----------|---------|-----|
| **React** | 18.2 | Framework UI |
| **Vite** | 5.x | Build tool + Dev server |
| **Three.js** | r128+ | Renderizado 3D WebGL |
| **@react-three/fiber** | 8.x | Bindings React ↔ Three.js |
| **@react-three/drei** | 9.x | Helpers 3D reutilizables |
| **Tailwind CSS** | 3.x | Estilos utility-first |
| **Framer Motion** | 11.x | Animaciones de UI |

### 3.2 Backend

| Tecnología | Versión | Uso |
|-----------|---------|-----|
| **Node.js** | 18+ | Runtime JavaScript |
| **Express** | 4.x | Framework HTTP |
| **OpenAI SDK** | 4.x | Cliente para Groq (compatible) |
| **@supabase/supabase-js** | 2.x | Cliente Supabase |
| **msedge-tts** | latest | Microsoft Edge TTS |
| **axios** | 1.x | HTTP client (para ElevenLabs) |
| **dotenv** | 16.x | Variables de entorno |
| **cors** | 2.x | Cross-Origin Resource Sharing |

### 3.3 Servicios Externos

| Servicio | Función | Costo |
|----------|---------|------:|
| **Groq Cloud** | LLM Inference (Llama 3.3 70B) | $0 free tier |
| **Microsoft Edge TTS** | Síntesis de voz neural | $0 ilimitado |
| **ElevenLabs** | TTS premium (opcional) | $0 (10k chars/mes) |
| **Supabase** | PostgreSQL hosted + RLS | $0 (500MB free) |

### 3.4 Herramientas de Soporte

| Herramienta | Propósito |
|-------------|-----------|
| **FFmpeg** | Conversión MP3 → WAV |
| **Rhubarb Lip-Sync v1.13** | Generación de visemes |
| **MSTest (.NET 8)** | Pruebas unitarias (Tests/) |
| **Concurrently** | Lanzar frontend + backend juntos |
| **Nodemon** | Hot reload del backend |

---

## 4. PATRONES ARQUITECTÓNICOS

### 4.1 Clean Architecture (Backend)

```
┌────────────────────────────────────────────────────────────┐
│                   API LAYER (controllers)                  │
│  Recibe HTTP requests, valida, llama a servicios          │
└──────────────────────────────┬─────────────────────────────┘
                               │ depende de
                               ▼
┌────────────────────────────────────────────────────────────┐
│              APPLICATION LAYER (services)                  │
│  Casos de uso: ChatApplication, TtsApplication, etc.      │
└──────────────────────────────┬─────────────────────────────┘
                               │ depende de
                               ▼
┌────────────────────────────────────────────────────────────┐
│                 DOMAIN LAYER (entities)                    │
│  Reglas de negocio puras: Message, Conversation           │
└────────────────────────────────────────────────────────────┘
                               ▲
                               │ implementa contratos de
                               │
┌────────────────────────────────────────────────────────────┐
│            INFRASTRUCTURE LAYER (providers)                │
│  Adaptadores: GroqProvider, SupabaseClient, EdgeTtsProvider│
└────────────────────────────────────────────────────────────┘
```

### 4.2 Beneficios

- ✅ Testeable (mocks fáciles en cada capa)
- ✅ Cambio de provider TTS sin tocar lógica de negocio
- ✅ Cambio de BD (Supabase → MongoDB) sin tocar servicios
- ✅ Independencia del framework (Express podría ser Fastify)

---

## 5. DIAGRAMA DE COMPONENTES

```
                    ┌────────────────────────────────────────┐
                    │           NAVEGADOR WEB                │
                    │  ┌───────────────────────────────────┐ │
                    │  │       Frontend (Vite SPA)         │ │
                    │  │  ┌─────────────┐  ┌─────────────┐ │ │
                    │  │  │  App.jsx    │  │  UI.jsx     │ │ │
                    │  │  └─────────────┘  └─────────────┘ │ │
                    │  │  ┌─────────────┐  ┌─────────────┐ │ │
                    │  │  │ Avatar.jsx  │  │useChat.jsx  │ │ │
                    │  │  │ (Three.js)  │  │ (Context)   │ │ │
                    │  │  └─────────────┘  └─────────────┘ │ │
                    │  └───────────────────────────────────┘ │
                    └────────────────┬───────────────────────┘
                                     │ fetch /chat
                                     ▼
                    ┌────────────────────────────────────────┐
                    │       Backend Express (Node.js)        │
                    │  ┌──────────────────────────────────┐  │
                    │  │  Routes (chatRoutes.js)          │  │
                    │  │   • POST /chat                   │  │
                    │  │   • GET  /conversations          │  │
                    │  │   • POST /conversations          │  │
                    │  │   • GET  /voices                 │  │
                    │  │   • GET  /                       │  │
                    │  └──────────────────────────────────┘  │
                    │              │                         │
                    │              ▼                         │
                    │  ┌──────────────────────────────────┐  │
                    │  │  Services                        │  │
                    │  │   • chatService                  │  │
                    │  │   • ttsService                   │  │
                    │  │   • lipsyncService               │  │
                    │  │   • conversationService          │  │
                    │  └──────────────────────────────────┘  │
                    │              │                         │
                    │              ▼                         │
                    │  ┌──────────────────────────────────┐  │
                    │  │  Utils + Config                  │  │
                    │  │   • fileUtils                    │  │
                    │  │   • config/index.js              │  │
                    │  └──────────────────────────────────┘  │
                    └────────────────────────────────────────┘
                              │              │             │
                              ▼              ▼             ▼
                       ┌──────────┐   ┌──────────┐  ┌──────────┐
                       │  Groq    │   │ Edge TTS │  │ Supabase │
                       │  Cloud   │   │ Edge.com │  │ Postgres │
                       └──────────┘   └──────────┘  └──────────┘
                                             │
                                             ▼
                                     ┌──────────────┐
                                     │ Rhubarb CLI  │
                                     │ + FFmpeg     │
                                     └──────────────┘
```

---

## 6. DIAGRAMA DE CLASES (UML)

### 6.1 Backend — Diagrama de Clases

```
┌─────────────────────────────────────┐
│        <<entity>>                   │
│         Message                     │
├─────────────────────────────────────┤
│ - role: string                      │
│ - content: string                   │
│ - audio: string                     │
│ - lipsync: object                   │
│ - facialExpression: string          │
│ - animation: string                 │
├─────────────────────────────────────┤
│ + validate()                        │
│ + hasAudio(): boolean               │
│ + toJSON(): object                  │
└─────────────────────────────────────┘
              │
              │ contiene N
              ▼
┌─────────────────────────────────────┐
│        <<entity>>                   │
│       Conversation                  │
├─────────────────────────────────────┤
│ - id: number                        │
│ - user_id: string                   │
│ - title: string                     │
│ - created_at: timestamp             │
│ - updated_at: timestamp             │
├─────────────────────────────────────┤
│ + fromDatabase(row): Conversation   │
│ + toJSON(): object                  │
└─────────────────────────────────────┘


┌─────────────────────────────────────┐
│       <<interface>>                 │
│        ITtsProvider                 │
├─────────────────────────────────────┤
│ + synthesize(text, opts): Buffer    │
│ + getName(): string                 │
│ + isAvailable(): boolean            │
└─────────────────────────────────────┘
              △
              │ implementa
   ┌──────────┴──────────┐
   │                     │
┌────────────────┐  ┌────────────────┐
│ EdgeTtsProvider│  │ElevenLabsProv. │
├────────────────┤  ├────────────────┤
│ - voice        │  │ - apiKey       │
│ - tts: MsEdge  │  │ - voiceId      │
├────────────────┤  ├────────────────┤
│ + synthesize() │  │ + synthesize() │
└────────────────┘  └────────────────┘


┌─────────────────────────────────────┐
│        <<service>>                  │
│      ChatApplication                │
├─────────────────────────────────────┤
│ - llmProvider: GroqProvider         │
│ - ttsApp: TtsApplication            │
│ - lipsync: LipsyncApplication       │
│ - convRepo: ConversationRepository  │
├─────────────────────────────────────┤
│ + sendMessage(msg, opts): Result    │
│ + getHistory(limit): Conversation[] │
└─────────────────────────────────────┘
              │ usa
              ▼
┌─────────────────────────────────────┐
│       <<service>>                   │
│       TtsApplication                │
├─────────────────────────────────────┤
│ - providers: Map<string,ITtsProvider│
├─────────────────────────────────────┤
│ + synthesize(text, name): Buffer    │
│ + registerProvider(name, provider)  │
└─────────────────────────────────────┘


┌─────────────────────────────────────┐
│      <<repository>>                 │
│   ConversationRepository            │
├─────────────────────────────────────┤
│ - supabase: SupabaseClient          │
│ - tableName: 'conversations'        │
├─────────────────────────────────────┤
│ + listByUser(userId)                │
│ + create(userId, title)             │
│ + getById(id)                       │
│ + getMessages(convId)               │
│ + delete(id)                        │
│ + saveMessage(convId, role, content)│
└─────────────────────────────────────┘
```

### 6.2 Frontend — Diagrama de Clases (Componentes React)

```
┌─────────────────────────────────────┐
│      <<component>>                  │
│           App.jsx                   │
├─────────────────────────────────────┤
│ + render(): JSX                     │
└─────────────────────────────────────┘
       │              │
       │ wraps        │ contains
       ▼              ▼
┌─────────────┐  ┌──────────────────┐
│ChatProvider │  │     <Canvas>     │
└─────────────┘  │   (Three.js)     │
       │         └──────────────────┘
       │                  │
       │ provides         │ contains
       ▼                  ▼
┌─────────────┐  ┌──────────────────┐
│   useChat   │  │   Avatar.jsx     │
│   (hook)    │  ├──────────────────┤
└─────────────┘  │ - morphTargets   │
       │         │ - animations     │
       │ used by │ - currentMessage │
       ▼         ├──────────────────┤
┌─────────────┐  │ + useGLTF()      │
│   UI.jsx    │  │ + useFrame()     │
├─────────────┤  └──────────────────┘
│ - chatMode  │
│ - provider  │
│ - input     │
├─────────────┤
│ + sendMsg() │
│ + cycleMode │
└─────────────┘
```

---

## 7. DIAGRAMA DE SECUENCIA (UML)

### 7.1 Caso: Usuario envía un mensaje (mode=both, provider=edge)

```
Usuario  Frontend     Backend    Groq   EdgeTTS  Rhubarb  Supabase
  │         │           │         │       │        │         │
  │ escribe │           │         │       │        │         │
  ├────────▶│           │         │       │        │         │
  │         │           │         │       │        │         │
  │         │  POST     │         │       │        │         │
  │         │  /chat    │         │       │        │         │
  │         ├──────────▶│         │       │        │         │
  │         │           │         │       │        │         │
  │         │           │ INSERT  │       │        │         │
  │         │           │ user msg│       │        │         │
  │         │           ├─────────────────────────────────────▶
  │         │           │◀─────────────────────────────────────┤
  │         │           │         │       │        │         │
  │         │           │ chat    │       │        │         │
  │         │           │ complete│       │        │         │
  │         │           ├────────▶│       │        │         │
  │         │           │         │       │        │         │
  │         │           │ JSON    │       │        │         │
  │         │           │ response│       │        │         │
  │         │           │◀────────┤       │        │         │
  │         │           │         │       │        │         │
  │         │           │ parse   │       │        │         │
  │         │           │ messages│       │        │         │
  │         │           │         │       │        │         │
  │         │           │     synthesize  │        │         │
  │         │           ├────────────────▶│        │         │
  │         │           │                 │        │         │
  │         │           │     MP3 stream  │        │         │
  │         │           │◀────────────────┤        │         │
  │         │           │                 │        │         │
  │         │           │  ffmpeg+rhubarb │        │         │
  │         │           ├──────────────────────────▶│         │
  │         │           │                          │         │
  │         │           │    visemes JSON          │         │
  │         │           │◀──────────────────────────┤         │
  │         │           │                                   │
  │         │           │ INSERT assistant msg              │
  │         │           ├───────────────────────────────────▶
  │         │           │◀───────────────────────────────────┤
  │         │           │         │       │        │         │
  │         │ JSON:     │         │       │        │         │
  │         │ { msgs,   │         │       │        │         │
  │         │   audio,  │         │       │        │         │
  │         │   visemes}│         │       │        │         │
  │         │◀──────────┤         │       │        │         │
  │         │           │         │       │        │         │
  │         │ play audio│         │       │        │         │
  │         │ animate   │         │       │        │         │
  │         │ avatar    │         │       │        │         │
  │  ve y   │           │         │       │        │         │
  │  oye    │           │         │       │        │         │
  │◀────────┤           │         │       │        │         │
```

### 7.2 Caso: Fallback ElevenLabs → Edge TTS

```
Backend   ElevenLabs   EdgeTTS
   │          │           │
   │ POST     │           │
   ├─────────▶│           │
   │          │           │
   │ ERROR    │           │
   │ 401      │           │
   │◀─────────┤           │
   │          │           │
   │ catch    │           │
   │ fallback │           │
   │          │           │
   │  toStream            │
   ├──────────────────────▶│
   │                      │
   │ audio                │
   │◀──────────────────────┤
   │                      │
   │ marca fallbackUsed   │
   │ = "edge"             │
```

---

## 8. DIAGRAMA DE DESPLIEGUE

```
┌─────────────────────────────────────────────────────────────────┐
│                       MÁQUINA LOCAL (Dev)                       │
│                                                                 │
│  ┌─────────────────────────┐     ┌─────────────────────────┐   │
│  │  PORT 5173 (Frontend)   │     │  PORT 3000 (Backend)    │   │
│  │  ┌───────────────────┐  │     │  ┌───────────────────┐  │   │
│  │  │  Vite Dev Server  │  │     │  │  Express Server   │  │   │
│  │  │  (React + Three)  │  │     │  │  + Edge TTS init  │  │   │
│  │  └───────────────────┘  │     │  └───────────────────┘  │   │
│  └────────────┬────────────┘     └────────────┬────────────┘   │
│               │                                │                │
│               └──────── HTTP (CORS) ───────────┘                │
│                                                                 │
│  ┌───────────────────────────────────────────────────────────┐ │
│  │              Filesystem local                              │ │
│  │   D:\Programas\Visual Studio\chatbot-3d\                  │ │
│  │   ├── audios/             (MP3/WAV/JSON temporales)       │ │
│  │   ├── r3f-...-backend/                                    │ │
│  │   │   ├── bin/rhubarb.exe                                 │ │
│  │   │   └── .env (API keys)                                 │ │
│  │   └── r3f-...-frontend/                                   │ │
│  └───────────────────────────────────────────────────────────┘ │
└──────────────────────────┬──────────────────────────────────────┘
                           │ Internet
        ┌──────────────────┼──────────────────────┐
        ▼                  ▼                      ▼
┌──────────────┐  ┌──────────────────┐   ┌──────────────────┐
│  Groq Cloud  │  │   Edge TTS       │   │    Supabase      │
│              │  │  (Microsoft)     │   │  PostgreSQL      │
│ api.groq.com │  │ speech.platform  │   │ supabase.co      │
│              │  │     .bing.com    │   │                  │
└──────────────┘  └──────────────────┘   └──────────────────┘
```

---

## 9. MODELO DE DATOS (ER)

### 9.1 Diagrama Entidad-Relación

```
┌───────────────────────────────┐
│       conversations           │
├───────────────────────────────┤
│ PK  id          BIGSERIAL     │
│     user_id     TEXT          │
│     title       TEXT          │
│     created_at  TIMESTAMPTZ   │
│     updated_at  TIMESTAMPTZ   │
└──────────────┬────────────────┘
               │ 1
               │
               │ N
               ▼
┌───────────────────────────────┐
│         messages              │
├───────────────────────────────┤
│ PK  id                BIGSERIAL│
│ FK  conversation_id   BIGINT  │
│     role              TEXT    │
│     content           TEXT    │
│     animation         TEXT    │
│     facial_expression TEXT    │
│     created_at        TIMESTAMPTZ│
└───────────────────────────────┘

Relación: conversations(1) ──── tiene ────▶ (N) messages
          (ON DELETE CASCADE: borrar conversación borra sus mensajes)
```

### 9.2 Restricciones de Integridad

| Tabla | Constraint |
|-------|-----------|
| `messages.role` | CHECK IN ('user', 'assistant', 'system') |
| `messages.conversation_id` | FK → conversations(id) ON DELETE CASCADE |
| `conversations.title` | NOT NULL DEFAULT 'Nueva conversación' |
| `conversations.user_id` | NOT NULL DEFAULT 'anonymous' |

### 9.3 Índices

| Índice | Tabla | Columnas | Propósito |
|--------|-------|----------|-----------|
| `idx_conversations_user_id` | conversations | user_id | Filtrar por usuario |
| `idx_conversations_updated_at` | conversations | updated_at DESC | Ordenar por reciente |
| `idx_messages_conversation_id` | messages | conversation_id | JOIN rápido |
| `idx_messages_created_at` | messages | created_at | Ordenar cronológico |

---

## 10. DICCIONARIO DE DATOS

### 10.1 Tabla `conversations`

| Atributo | Tipo | NULL | Default | Descripción |
|----------|------|:----:|---------|-------------|
| `id` | BIGSERIAL | NO | auto | Identificador único |
| `user_id` | TEXT | NO | 'anonymous' | ID del usuario (header X-User-Id) |
| `title` | TEXT | NO | 'Nueva conversación' | Título descriptivo |
| `created_at` | TIMESTAMPTZ | NO | NOW() | Fecha de creación |
| `updated_at` | TIMESTAMPTZ | NO | NOW() | Última modificación |

### 10.2 Tabla `messages`

| Atributo | Tipo | NULL | Default | Descripción |
|----------|------|:----:|---------|-------------|
| `id` | BIGSERIAL | NO | auto | Identificador único |
| `conversation_id` | BIGINT | NO | — | FK a conversations |
| `role` | TEXT | NO | — | 'user' / 'assistant' / 'system' |
| `content` | TEXT | NO | — | Texto del mensaje |
| `animation` | TEXT | SÍ | NULL | Nombre de animación (Talking_1, Idle...) |
| `facial_expression` | TEXT | SÍ | NULL | Expresión (smile, sad, angry...) |
| `created_at` | TIMESTAMPTZ | NO | NOW() | Cuándo se envió |

### 10.3 Valores Enumerados

#### Animaciones
```
Talking_0, Talking_1, Talking_2,
Crying, Laughing, Rumba, Idle, Terrified, Angry
```

#### Expresiones Faciales
```
smile, sad, angry, surprised, funnyFace, default
```

#### Modos de Chat (parámetro request)
```
text, audio, both
```

#### Providers TTS (parámetro request)
```
edge, elevenlabs
```

---

## 11. DISEÑO DE API REST

### 11.1 Convenciones

- **Base URL:** `http://localhost:3000`
- **Formato:** JSON
- **Content-Type:** `application/json`
- **Charset:** UTF-8
- **Header de usuario:** `X-User-Id` (opcional, default `"anonymous"`)

### 11.2 Endpoints

#### GET `/`
**Descripción:** Health check del servidor

| Campo | Valor |
|-------|-------|
| **Request** | — (sin body) |
| **Response 200** | `Chatbot Avatar 3D — Backend running!` |
| **Errores** | — |

---

#### GET `/voices`
**Descripción:** Lista las voces TTS disponibles en español

| Campo | Valor |
|-------|-------|
| **Request** | — |
| **Response 200** | `{ "active": "es-MX-DaliaNeural", "available": [VoiceDto] }` |
| **Errores** | 500 (error de Edge TTS) |

---

#### POST `/chat` ⭐
**Descripción:** Procesa un mensaje del usuario y devuelve respuesta del avatar

| Campo | Valor |
|-------|-------|
| **Body** | `{ message, mode, ttsProvider, conversationId? }` |
| **Headers** | `X-User-Id` (opcional) |
| **Response 200** | `{ messages: [MessageDto], conversationId }` |
| **Errores** | 500 (error procesando) |

**Body schema:**
```json
{
  "message": "string (required, no vacío)",
  "mode": "text | audio | both (default: both)",
  "ttsProvider": "edge | elevenlabs (default: edge)",
  "conversationId": "number (opcional)"
}
```

**Response schema:**
```json
{
  "messages": [
    {
      "text": "string",
      "audio": "string (base64) | null",
      "lipsync": { "mouthCues": [...] } | null,
      "facialExpression": "smile|sad|...",
      "animation": "Talking_0|Idle|...",
      "displayText": "string (solo en mode=audio)",
      "fallbackUsed": "edge (solo si ElevenLabs falló)"
    }
  ],
  "conversationId": "number | null"
}
```

---

#### GET `/conversations`
**Descripción:** Lista las conversaciones del usuario

| Campo | Valor |
|-------|-------|
| **Headers** | `X-User-Id` (default "anonymous") |
| **Response 200** | `[ConversationDto]` ordenado por `updated_at DESC` |
| **Errores** | 500 (error de Supabase) |

---

#### POST `/conversations`
**Descripción:** Crea una nueva conversación

| Campo | Valor |
|-------|-------|
| **Body** | `{ "title": "string (opcional)" }` |
| **Headers** | `X-User-Id` (default "anonymous") |
| **Response 200** | `ConversationDto` con id asignado |
| **Errores** | 500 |

---

#### GET `/conversations/:id/messages`
**Descripción:** Obtiene los mensajes de una conversación

| Campo | Valor |
|-------|-------|
| **Path param** | `id` (number) |
| **Response 200** | `[MessageDto]` ordenado por `created_at ASC` |
| **Errores** | 500 |

---

#### DELETE `/conversations/:id`
**Descripción:** Elimina una conversación (y sus mensajes por cascade)

| Campo | Valor |
|-------|-------|
| **Path param** | `id` (number) |
| **Response 200** | `{ "success": true }` |
| **Errores** | 500 |

### 11.3 Códigos de Estado

| Código | Significado | Uso |
|-------|-------------|-----|
| 200 | OK | Operación exitosa |
| 400 | Bad Request | Body inválido (futuro) |
| 401 | Unauthorized | Auth fallida (futuro) |
| 500 | Internal Server Error | Error del backend |

---

## 12. DISEÑO DE INTERFAZ (WIREFRAMES)

### 12.1 Pantalla Principal — Vista Desktop

```
┌──────────────────────────────────────────────────────────────────────┐
│ ┌───────────────────────────┐                                       │
│ │   My Virtual GF           │                                  ┌─┐  │
│ │   I will always love you  │                                  │🔍│  │
│ │                           │                                  └─┘  │
│ │   [🔊 Audio + Texto]      │                                       │
│ │   [🆓 Edge TTS]           │                                  ┌─┐  │
│ └───────────────────────────┘                                  │📷│  │
│                                                                └─┘  │
│                                                                     │
│                                                                     │
│                                                                     │
│                            ┌──────────────┐                         │
│                            │              │                         │
│                            │   AVATAR 3D  │                         │
│                            │   (Three.js  │                         │
│                            │    Canvas)   │                         │
│                            │              │                         │
│                            │              │                         │
│                            │              │                         │
│                            └──────────────┘                         │
│                                                                     │
│                                                                     │
│                                                                     │
│   ┌───────────────────────────────────────────────────────┐        │
│   │ Escribe un mensaje...                          [SEND] │        │
│   └───────────────────────────────────────────────────────┘        │
└──────────────────────────────────────────────────────────────────────┘
```

### 12.2 Componentes UI Detallados

#### Panel Superior Izquierdo (Glassmorphism)
- Fondo: `bg-white bg-opacity-50 backdrop-blur-md`
- Título: "My Virtual GF" (negrita, 1.25rem)
- Subtítulo: emoji corazón
- Botón modo: pink-500, cicla entre 3 modos
- Botón provider: emerald (edge) / violet (elevenlabs)

#### Botones Derecha (Vertical Stack)
- Zoom in/out: SVG icon, fondo pink-500
- Green screen toggle: SVG icon

#### Input de Chat (Bottom Centered)
- Width: max-w-screen-sm
- Placeholder: "Escribe un mensaje..."
- Background: blanco translúcido con blur
- Botón Send: pink-500, opacity-30 si está cargando

### 12.3 Flujo de Interacción

```
[Usuario hace click en input] ─▶ [Escribe texto] ─▶ [Presiona Enter/Send]
                                                            │
                                                            ▼
                                                  [Botón se deshabilita]
                                                  [Mostrar loading]
                                                            │
                                                            ▼
                                                  [Backend procesa ~5s]
                                                            │
                                                            ▼
                                          [Avatar comienza a animar]
                                          [Audio reproduce]
                                          [Texto aparece en burbuja]
                                                            │
                                                            ▼
                                          [Input se habilita de nuevo]
```

---

## 13. PATRONES DE DISEÑO APLICADOS

### 13.1 Patrón Strategy (Proveedores TTS)

**Problema:** Necesitamos cambiar entre Edge TTS y ElevenLabs sin acoplar.

**Solución:** Interface común `ITtsProvider` con implementaciones intercambiables.

```javascript
// Domain layer
class ITtsProvider {
  synthesize(text, options) { /* abstract */ }
}

// Infrastructure layer
class EdgeTtsProvider extends ITtsProvider {
  async synthesize(text) { /* Edge implementation */ }
}

class ElevenLabsProvider extends ITtsProvider {
  async synthesize(text) { /* ElevenLabs implementation */ }
}

// Application layer
class TtsApplication {
  constructor(...providers) {
    this.providers = new Map(providers.map(p => [p.getName(), p]));
  }
  synthesize(text, providerName) {
    return this.providers.get(providerName).synthesize(text);
  }
}
```

### 13.2 Patrón Repository (Acceso a Datos)

**Problema:** Acoplar lógica de negocio a Supabase dificulta migrar BD.

**Solución:** Encapsular queries en una clase Repository.

```javascript
class ConversationRepository {
  constructor(supabaseClient) {
    this.supabase = supabaseClient;
  }

  async listByUser(userId) {
    const { data, error } = await this.supabase
      .from('conversations')
      .select('*')
      .eq('user_id', userId);
    if (error) throw new Error(error.message);
    return data || [];
  }
  // ...
}
```

### 13.3 Patrón Factory (Test Data)

**Problema:** Crear datos de prueba repetidamente es verboso.

**Solución:** Factory que genera DTOs preconfigurados.

```csharp
// TestDataFactory.cs
public static ChatRequest CreateValidChatRequest(string msg = "Hola") {
  return new ChatRequest { Message = msg, Mode = "both", TtsProvider = "edge" };
}
```

### 13.4 Patrón Singleton (SupabaseClient)

**Problema:** Múltiples instancias de cliente Supabase son ineficientes.

**Solución:** Una sola instancia global.

```javascript
class SupabaseClient {
  static instance = null;
  static getInstance(url, key) {
    if (!this.instance) this.instance = createClient(url, key);
    return this.instance;
  }
}
```

### 13.5 Patrón Dependency Injection

**Problema:** Acoplar servicios dificulta testing.

**Solución:** Inyectar dependencias por constructor.

```javascript
class ChatApplication {
  constructor(llmProvider, ttsApp, lipsync, convRepo) {
    this.llm = llmProvider;
    this.tts = ttsApp;
    this.lipsync = lipsync;
    this.convRepo = convRepo;
  }
}
```

### 13.6 Patrón Provider (React Context)

**Problema:** Pasar `chatMode`, `ttsProvider` por props es pesado.

**Solución:** Context global con `useChat()` hook.

```jsx
const ChatContext = createContext();

export const ChatProvider = ({ children }) => {
  const [chatMode, setChatMode] = useState('both');
  return <ChatContext.Provider value={{ chatMode, setChatMode }}>
    {children}
  </ChatContext.Provider>;
};

export const useChat = () => useContext(ChatContext);
```

### 13.7 Patrón Fallback (TTS)

**Problema:** ElevenLabs free tier puede fallar (401, 429).

**Solución:** Try/catch con fallback automático.

```javascript
try {
  audio = await synthesizeWithElevenLabs(text, path);
} catch (err) {
  console.log('Fallback a Edge TTS');
  audio = await synthesizeWithEdgeTTS(text, path);
  message.fallbackUsed = 'edge';
}
```

---

## 14. DISEÑO DE SEGURIDAD

### 14.1 Capa de Seguridad

```
┌─────────────────────────────────────────┐
│   1. Variables de entorno (.env)        │
│   • GROQ_API_KEY                        │
│   • SUPABASE_SERVICE_KEY                │
│   • ELEVEN_LABS_API_KEY                 │
│   .env en .gitignore (NUNCA committed) │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│   2. CORS                               │
│   cors({ origin: 'http://localhost:5173'})│
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│   3. Validación de input                │
│   • message no vacío                    │
│   • role válido (user|assistant|system) │
│   • mode válido (text|audio|both)       │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│   4. RLS en Supabase                    │
│   • Tablas con Row Level Security ON    │
│   • Políticas configurables             │
│   • Service Key bypassea RLS            │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│   5. Manejo de errores                  │
│   • unhandledRejection handler          │
│   • uncaughtException handler           │
│   • Try/catch en cada endpoint          │
└─────────────────────────────────────────┘
```

### 14.2 Amenazas Identificadas

| Amenaza | Severidad | Mitigación |
|---------|:--------:|-----------|
| API key filtrada en repo | Crítica | `.env` en `.gitignore`, secret scanning |
| Inyección SQL | Alta | Supabase usa prepared statements automáticos |
| XSS desde respuesta LLM | Media | React escapa por defecto, sanitizar HTML |
| Rate limit abuse | Media | Groq tiene su propio rate limit (30 req/min) |
| Denegación de servicio | Baja | Timeout en HttpClient (30s) |
| Datos sensibles en logs | Baja | No loggear claves ni body completo |

---

## 15. ESTRUCTURA DE CARPETAS

```
chatbot-3d/
├── ANALISIS.MD                       # Fase 1: Análisis
├── DISEÑO.MD                         # Fase 2: Diseño (este archivo)
├── package.json                      # Orquestador (concurrently)
├── start-chatbot.bat                 # Script Windows
│
├── r3f-virtual-girlfriend-backend/   # 🔧 Backend (Express + Node)
│   ├── index.js                      # Entry point (Express server)
│   ├── package.json
│   ├── .env                          # API keys (NO en repo)
│   │
│   ├── src/                          # Clean Architecture
│   │   ├── config/index.js           # Configuración centralizada
│   │   ├── services/
│   │   │   ├── chatService.js        # Lógica de chat
│   │   │   ├── ttsService.js         # Síntesis voz (Strategy)
│   │   │   ├── lipsyncService.js     # Rhubarb wrapper
│   │   │   └── conversationService.js# Repository pattern
│   │   └── utils/
│   │       └── fileUtils.js          # Helpers archivos
│   │
│   ├── bin/                          # Binarios externos
│   │   └── rhubarb.exe              # Rhubarb v1.13.0
│   │
│   └── audios/                       # Audios generados (gitignore)
│       ├── message_0.mp3
│       ├── message_0.wav
│       └── message_0.json
│
├── r3f-virtual-girlfriend-frontend/  # 🎨 Frontend (React + Three.js)
│   ├── index.html
│   ├── package.json
│   ├── vite.config.js
│   │
│   ├── public/
│   │   └── models/avatar.glb         # Modelo 3D
│   │
│   └── src/
│       ├── App.jsx                   # Root component
│       ├── main.jsx                  # Entry point
│       ├── components/
│       │   ├── Avatar.jsx            # Modelo 3D + lipsync
│       │   ├── UI.jsx                # Chat UI + toggles
│       │   └── Experience.jsx        # Setup escena Three
│       └── hooks/
│           └── useChat.jsx           # Context provider
│
└── Tests/                            # 🧪 Pruebas Unitarias MSTest
    ├── ChatBot.sln
    ├── README.md
    ├── SUPABASE_SETUP.sql            # Setup tablas
    ├── FIX_GSRS_POLICIES.sql         # Helper SQL
    │
    └── ChatBot.Application.Test/
        ├── ChatBot.Application.Test.csproj
        ├── coverage.runsettings
        ├── appsettings.test.json
        │
        ├── Base/
        │   └── BaseApiTest.cs        # [ClassInitialize]
        ├── Models/
        │   └── ChatModels.cs         # DTOs
        ├── Helpers/
        │   └── TestDataFactory.cs    # Factory pattern
        ├── Fixtures/
        │   └── ConversationFixtures.cs
        └── Tests/
            ├── HealthEndpointTest.cs       # TC-H01-03
            ├── VoicesEndpointTest.cs       # TC-V01-05
            ├── ChatEndpointTest.cs         # TC-C01-08
            ├── ChatModesTest.cs            # TC-M01-07
            ├── TtsProvidersTest.cs         # TC-T01-06
            └── ConversationsEndpointTest.cs# TC-CV01-11
```

---

## 📊 RESUMEN DE DECISIONES DE DISEÑO

| Decisión | Justificación |
|----------|---------------|
| Clean Architecture | Mantenibilidad + Testabilidad |
| REST sobre GraphQL | Simplicidad para MVP |
| PostgreSQL (Supabase) | Free tier + RLS + tipos avanzados |
| Three.js sobre Babylon | Comunidad más grande + más recursos |
| React sobre Vue/Svelte | Estándar de la industria, mejor ecosistema 3D |
| Groq sobre OpenAI | 10× más rápido + free tier generoso |
| Edge TTS sobre ElevenLabs (default) | Gratuito ilimitado |
| MSTest sobre xUnit | Solicitud explícita del cliente (patrón Pacagroup) |
| Cobertura ≥ 95% | Calidad enterprise |

---

## 🔗 TRAZABILIDAD ANÁLISIS → DISEÑO

| Requerimiento (ANALISIS.MD) | Componente que lo implementa |
|-----------------------------|------------------------------|
| RF-01 (enviar mensaje) | `POST /chat` (sección 11.2) |
| RF-02 (respuesta LLM) | `chatService.js` + Groq API |
| RF-03 (TTS) | `ttsService.js` (Strategy pattern) |
| RF-04 (lipsync) | `lipsyncService.js` + Rhubarb |
| RF-05 (avatar animado) | `Avatar.jsx` + morph targets |
| RF-08 (fallback) | Patrón Fallback (sección 13.7) |
| RF-09 (persistencia) | `ConversationRepository` |
| RNF-19 (mantenibilidad) | Clean Architecture (sección 4) |
| RNF-20 (cobertura 95%) | Tests/ChatBot.Application.Test/ |
| RNF-14 (API keys seguras) | `.env` + `.gitignore` (sección 14) |

---

**Documento generado:** Mayo 2026
**Autor:** Equipo de Desarrollo Chatbot 3D
**Documento previo:** `ANALISIS.MD`
**Próximo paso:** Implementación + Pruebas (ver `Tests/README.md`)
