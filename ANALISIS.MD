# 📋 ANÁLISIS DEL PROYECTO — Chatbot Avatar 3D

> **Fase 1 del SDLC bajo metodología DSDM (Dynamic Systems Development Method)**
> Versión 1.0 — Mayo 2026

---

## 📑 ÍNDICE

1. [Introducción](#1-introducción)
2. [Definición del Problema](#2-definición-del-problema)
3. [Stakeholders](#3-stakeholders)
4. [Análisis de Factibilidad](#4-análisis-de-factibilidad)
5. [Requerimientos Funcionales](#5-requerimientos-funcionales-rf)
6. [Requerimientos No Funcionales](#6-requerimientos-no-funcionales-rnf)
7. [Historias de Usuario](#7-historias-de-usuario)
8. [Priorización MoSCoW](#8-priorización-moscow-dsdm)
9. [Casos de Uso](#9-casos-de-uso-uml)
10. [Restricciones y Supuestos](#10-restricciones-y-supuestos)
11. [Análisis de Riesgos](#11-análisis-de-riesgos)
12. [Aplicación de DSDM](#12-aplicación-de-dsdm)

---

## 1. INTRODUCCIÓN

### 1.1 Propósito

Este documento presenta el **análisis formal** del proyecto _Chatbot Avatar 3D_, una
aplicación web interactiva que permite a los usuarios conversar en lenguaje natural con
un avatar 3D animado, integrando inteligencia artificial generativa, síntesis de voz y
sincronización labial (lipsync) en tiempo real.

### 1.2 Ámbito

El sistema se compone de:
- **Backend (Node.js / Express):** Orquesta IA, voz, lipsync y persistencia.
- **Frontend (React + Three.js):** Renderiza el avatar 3D y la interfaz de chat.
- **Base de datos (Supabase / PostgreSQL):** Almacena conversaciones e historial.

### 1.3 Definiciones, Acrónimos y Abreviaciones

| Término | Significado |
|---------|-------------|
| **LLM** | Large Language Model (modelo de lenguaje grande) |
| **TTS** | Text-to-Speech (síntesis de voz) |
| **Viseme** | Representación visual de un fonema (para lipsync) |
| **DSDM** | Dynamic Systems Development Method |
| **SRS** | Software Requirements Specification |
| **MoSCoW** | Must / Should / Could / Won't have |
| **RLS** | Row Level Security (Supabase) |
| **RF / RNF** | Requerimiento Funcional / No Funcional |

### 1.4 Referencias

- IEEE 830-1998 — Software Requirements Specifications
- DSDM Consortium — DSDM Agile Project Framework
- Pacagroup — Patrón de pruebas .NET MSTest

---

## 2. DEFINICIÓN DEL PROBLEMA

### 2.1 Contexto

Los chatbots tradicionales basados en texto son **fríos** y carecen de _engagement
emocional_. Los usuarios modernos demandan interacciones más humanas y multimodales:
voz natural, expresiones faciales, y respuestas contextualizadas.

### 2.2 Problemática

| Problema | Impacto |
|----------|---------|
| 💬 Chatbots planos sin personalidad | Baja retención de usuarios |
| 🔇 Respuestas solo en texto | Inaccesible para usuarios con dificultad lectora |
| 🎭 Falta de expresividad | Sin conexión emocional con el bot |
| 💸 Servicios premium caros (ElevenLabs, OpenAI) | Barrera de entrada económica |
| 🧱 Soluciones propietarias cerradas | Difícil personalizar |

### 2.3 Solución Propuesta

Un **chatbot 3D open-source** que combina:
- IA gratuita (Groq / Llama 3.3)
- Voz neuronal gratuita (Microsoft Edge TTS)
- Avatar 3D con animaciones faciales y lipsync
- Persistencia en la nube (Supabase free tier)
- Múltiples modos de respuesta (texto / audio / ambos)

### 2.4 Objetivos

#### Objetivo General
Desarrollar un sistema de chatbot 3D multimodal, gratuito y de código abierto,
que permita interacciones naturales por texto y voz con un avatar animado.

#### Objetivos Específicos
1. Integrar un LLM gratuito (Groq Llama 3.3) para respuestas conversacionales
2. Implementar síntesis de voz neuronal sin costo (Edge TTS)
3. Generar lipsync sincronizado con Rhubarb Lip-Sync
4. Renderizar avatar 3D con expresiones faciales dinámicas
5. Persistir el historial de conversaciones en PostgreSQL
6. Ofrecer fallback automático entre proveedores TTS (Edge ↔ ElevenLabs)
7. Lograr cobertura de pruebas ≥ 95%

---

## 3. STAKEHOLDERS

### 3.1 Matriz de Stakeholders (DSDM)

| Rol | Stakeholder | Responsabilidad | Influencia |
|-----|-------------|-----------------|-----------:|
| **Business Sponsor** | Cliente / Profesor | Aprueba el alcance y dirección | Alta |
| **Business Visionary** | Product Owner | Define la visión del producto | Alta |
| **Technical Coordinator** | Tech Lead / Arquitecto | Decisiones técnicas | Alta |
| **Team Leader** | Líder de equipo | Coordina al equipo | Media |
| **Solution Developer** | Desarrolladores | Implementan código | Alta |
| **Solution Tester** | QA / Testers | Validan calidad | Media |
| **Business Ambassador** | Usuario final | Provee feedback | Media |
| **Business Advisor** | Asesor académico | Guía metodológica | Baja |

### 3.2 Usuarios Finales

| Tipo de Usuario | Necesidad Principal | Frecuencia de Uso |
|----------------|--------------------|--------------------|
| Estudiante curioso | Probar tecnología AI multimodal | Esporádica |
| Desarrollador | Aprender integración 3D + LLM | Frecuente |
| Investigador | Experimentar con TTS/lipsync | Frecuente |
| Usuario casual | Conversar con avatar | Ocasional |

---

## 4. ANÁLISIS DE FACTIBILIDAD

### 4.1 Factibilidad Técnica ✅

| Aspecto | Evaluación |
|---------|-----------|
| Stack tecnológico maduro | ✅ Node.js, React, Three.js son estables |
| APIs disponibles | ✅ Groq, Supabase, Edge TTS tienen free tier |
| Lipsync open-source | ✅ Rhubarb es robusto y multiplataforma |
| Curva de aprendizaje | ⚠️ Three.js requiere conocimiento 3D |

### 4.2 Factibilidad Económica ✅

| Recurso | Costo |
|---------|------:|
| Groq API | **$0** (free tier 30 req/min) |
| Supabase | **$0** (free tier 500MB) |
| Edge TTS | **$0** (gratis ilimitado) |
| ElevenLabs | $0 (opcional con free tier 10k chars) |
| Hosting | **$0** (desarrollo local) |
| **TOTAL** | **$0/mes** ✅ |

### 4.3 Factibilidad Operacional ✅

- ✅ Equipo capacitado en JavaScript/React
- ✅ Documentación abundante de las APIs
- ✅ Herramientas de desarrollo estándar (VS Code)
- ✅ Sin necesidad de infraestructura especial

### 4.4 Factibilidad de Cronograma ✅

| Fase | Duración estimada |
|------|-------------------|
| Análisis | 1 semana |
| Diseño | 1 semana |
| Implementación | 3 semanas |
| Pruebas | 1 semana |
| **Total** | **6 semanas** |

---

## 5. REQUERIMIENTOS FUNCIONALES (RF)

### 5.1 Tabla de Requerimientos Funcionales

| ID | Descripción | Prioridad |
|----|-------------|-----------|
| **RF-01** | El sistema debe permitir enviar mensajes de texto al avatar | Must |
| **RF-02** | El sistema debe responder con texto generado por LLM (Groq) | Must |
| **RF-03** | El sistema debe sintetizar voz a partir del texto generado | Must |
| **RF-04** | El sistema debe generar lipsync sincronizado con el audio | Must |
| **RF-05** | El sistema debe animar al avatar 3D según la respuesta | Must |
| **RF-06** | El sistema debe permitir elegir entre 3 modos: text / audio / both | Must |
| **RF-07** | El sistema debe permitir elegir provider TTS: edge / elevenlabs | Should |
| **RF-08** | El sistema debe realizar fallback automático si ElevenLabs falla | Should |
| **RF-09** | El sistema debe persistir conversaciones en Supabase | Must |
| **RF-10** | El sistema debe permitir listar conversaciones anteriores | Should |
| **RF-11** | El sistema debe permitir borrar conversaciones | Should |
| **RF-12** | El sistema debe mostrar expresiones faciales (smile, sad, angry...) | Should |
| **RF-13** | El sistema debe ejecutar animaciones (Talking, Idle, Laughing...) | Should |
| **RF-14** | El sistema debe exponer endpoint GET /voices con voces disponibles | Could |
| **RF-15** | El sistema debe enviar mensaje de bienvenida sin input previo | Could |

### 5.2 Detalle de Requerimientos Críticos

#### RF-01: Envío de mensajes
- **Entrada:** Texto del usuario (máx. 500 caracteres)
- **Proceso:** POST /chat con `{ message, mode, ttsProvider }`
- **Salida:** Array de mensajes con texto, audio, lipsync, expresión, animación
- **Validación:** Mensaje no vacío

#### RF-02: Respuesta del LLM
- **Modelo:** Llama 3.3 70B (vía Groq API)
- **Formato:** JSON estructurado con máximo 3 mensajes
- **Idioma:** Español por defecto
- **Temperatura:** 0.6 (balance creatividad/coherencia)

#### RF-09: Persistencia
- **Tablas:** `conversations`, `messages`
- **Identificador:** `user_id` desde header `X-User-Id`
- **Almacenamiento:** Supabase PostgreSQL

---

## 6. REQUERIMIENTOS NO FUNCIONALES (RNF)

### 6.1 Rendimiento

| ID | Requerimiento | Métrica |
|----|---------------|---------|
| **RNF-01** | Tiempo de respuesta LLM | < 2 segundos (Groq es ~10× más rápido que OpenAI) |
| **RNF-02** | Tiempo de síntesis TTS | < 3 segundos por mensaje |
| **RNF-03** | Tiempo de generación lipsync | < 2 segundos por mensaje |
| **RNF-04** | Latencia total (input → audio reproduciéndose) | < 8 segundos |
| **RNF-05** | FPS del avatar 3D | ≥ 30 FPS estables |

### 6.2 Disponibilidad y Confiabilidad

| ID | Requerimiento |
|----|---------------|
| **RNF-06** | El backend debe tolerar errores de APIs externas sin caer |
| **RNF-07** | Handlers globales para `unhandledRejection` y `uncaughtException` |
| **RNF-08** | Si ElevenLabs falla, sistema debe usar Edge TTS automáticamente |
| **RNF-09** | Conexión a Supabase no debe bloquear el endpoint /chat |

### 6.3 Usabilidad

| ID | Requerimiento |
|----|---------------|
| **RNF-10** | Interfaz responsive (móvil + escritorio) |
| **RNF-11** | Botones intuitivos para cambiar modo y provider |
| **RNF-12** | Feedback visual durante carga (loading states) |
| **RNF-13** | Soporte para idioma español de México (es-MX-DaliaNeural) |

### 6.4 Seguridad

| ID | Requerimiento |
|----|---------------|
| **RNF-14** | Claves API almacenadas en `.env`, NO en repositorio |
| **RNF-15** | Supabase usa Service Role Key (server-side only) |
| **RNF-16** | Validación de input en backend (sanitización) |
| **RNF-17** | RLS habilitado en tablas Supabase |
| **RNF-18** | CORS configurado correctamente |

### 6.5 Mantenibilidad

| ID | Requerimiento |
|----|---------------|
| **RNF-19** | Código modular siguiendo Clean Architecture |
| **RNF-20** | Cobertura de pruebas ≥ 95% (Statements/Lines/Functions) |
| **RNF-21** | Documentación de cada función crítica |
| **RNF-22** | Logs descriptivos con emojis y categorías |

### 6.6 Portabilidad

| ID | Requerimiento |
|----|---------------|
| **RNF-23** | Funcionar en Windows, Linux, macOS |
| **RNF-24** | Detectar plataforma para usar binarios correctos (rhubarb.exe / rhubarb) |
| **RNF-25** | No requerir Docker para desarrollo local |

---

## 7. HISTORIAS DE USUARIO

### 7.1 Formato

```
COMO  [tipo de usuario]
QUIERO [acción]
PARA  [beneficio]
```

### 7.2 Backlog de Historias

#### Épica: Conversación Básica

**HU-001 — Enviar mensaje de texto**
```
COMO   usuario
QUIERO escribir un mensaje y enviarlo al avatar
PARA   recibir una respuesta inteligente
```
- **Criterios de aceptación:**
  - El input acepta texto libre
  - Al presionar Enter o botón Send, se envía
  - Mientras espera respuesta, muestra estado de loading

**HU-002 — Recibir respuesta del avatar**
```
COMO   usuario
QUIERO ver la respuesta del avatar en pantalla
PARA   leer lo que me dice
```

**HU-003 — Escuchar al avatar**
```
COMO   usuario
QUIERO escuchar la voz del avatar
PARA   tener una experiencia más natural
```

#### Épica: Modos de Respuesta

**HU-004 — Elegir modo solo texto**
```
COMO   usuario en lugar silencioso
QUIERO desactivar el audio del avatar
PARA   leer las respuestas sin molestar a otros
```

**HU-005 — Elegir modo solo audio**
```
COMO   usuario con dificultad visual
QUIERO escuchar al avatar sin texto en pantalla
PARA   accesibilidad
```

**HU-006 — Elegir modo audio + texto**
```
COMO   usuario regular
QUIERO ver texto Y escuchar al avatar al mismo tiempo
PARA   máxima comprensión
```

#### Épica: Personalización

**HU-007 — Cambiar provider de voz**
```
COMO   usuario que prueba calidades de voz
QUIERO alternar entre Edge TTS (gratis) y ElevenLabs (premium)
PARA   comparar las voces
```

**HU-008 — Ver expresiones faciales**
```
COMO   usuario
QUIERO que el avatar muestre emociones (alegre, triste, sorprendido...)
PARA   interacciones emocionalmente ricas
```

#### Épica: Historial

**HU-009 — Listar conversaciones previas**
```
COMO   usuario regular
QUIERO ver mis conversaciones anteriores
PARA   continuarlas o consultarlas
```

**HU-010 — Borrar conversaciones**
```
COMO   usuario preocupado por privacidad
QUIERO eliminar conversaciones que ya no quiero
PARA   mantener mi historial limpio
```

#### Épica: Control de Cámara

**HU-011 — Zoom del avatar**
```
COMO   usuario
QUIERO acercar/alejar la cámara del avatar
PARA   ver mejor detalles o vista completa
```

**HU-012 — Green screen**
```
COMO   creador de contenido
QUIERO activar fondo verde
PARA   superponer el avatar en mis videos
```

---

## 8. PRIORIZACIÓN MoSCoW (DSDM)

La técnica MoSCoW es central en DSDM para priorizar requerimientos.

### 8.1 MUST HAVE (Crítico — sin esto el producto no funciona)

| ID | Funcionalidad |
|----|---------------|
| M-1 | Enviar mensajes al chatbot vía POST /chat |
| M-2 | Recibir respuestas del LLM (Groq Llama 3.3) |
| M-3 | Sintetizar voz (Edge TTS por defecto) |
| M-4 | Generar lipsync con Rhubarb |
| M-5 | Renderizar avatar 3D con Three.js |
| M-6 | Aplicar morph targets para lipsync |
| M-7 | Modo "both" funcional (texto + audio) |
| M-8 | Conexión exitosa a Supabase |
| M-9 | Manejo de errores que NO tire el servidor |

### 8.2 SHOULD HAVE (Importante — agrega mucho valor)

| ID | Funcionalidad |
|----|---------------|
| S-1 | Modos "text" y "audio" como alternativos |
| S-2 | Provider ElevenLabs como alternativa |
| S-3 | Fallback automático Edge ↔ ElevenLabs |
| S-4 | Persistencia en tabla `conversations` y `messages` |
| S-5 | Listar conversaciones (GET /conversations) |
| S-6 | Borrar conversaciones (DELETE /conversations/:id) |
| S-7 | Expresiones faciales múltiples |
| S-8 | Animaciones corporales múltiples |

### 8.3 COULD HAVE (Deseable — si hay tiempo)

| ID | Funcionalidad |
|----|---------------|
| C-1 | Listar voces disponibles (GET /voices) |
| C-2 | Mensaje de bienvenida por defecto |
| C-3 | Control de zoom de cámara |
| C-4 | Modo green screen para grabación |
| C-5 | Selector de voz (es-MX, es-ES, es-AR) |
| C-6 | Histórico paginado |
| C-7 | Tema oscuro/claro |

### 8.4 WON'T HAVE (No en esta versión)

| ID | Funcionalidad |
|----|---------------|
| W-1 | Login con OAuth (Google, GitHub) |
| W-2 | Multi-usuario con cuentas |
| W-3 | Reconocimiento de voz del usuario (ASR) |
| W-4 | Soporte multi-idioma simultáneo |
| W-5 | Avatar personalizable |
| W-6 | Modo offline |
| W-7 | App móvil nativa |
| W-8 | Pagos / suscripciones |

### 8.5 Distribución Esperada (DSDM)

```
MUST:    ~60% del esfuerzo
SHOULD:  ~20%
COULD:   ~20%
WON'T:    0% (en esta iteración)
```

---

## 9. CASOS DE USO (UML)

### 9.1 Diagrama de Casos de Uso (Notación texto)

```
                        ┌──────────────────────────────────────┐
                        │      Sistema Chatbot Avatar 3D       │
                        │                                       │
   ┌──────────┐        │   ┌──────────────────────┐          │
   │          │────────┼──▶│  Enviar Mensaje      │          │
   │          │        │   └──────────────────────┘          │
   │          │        │              │ <<include>>           │
   │          │        │              ▼                       │
   │          │        │   ┌──────────────────────┐          │
   │          │        │   │  Generar Respuesta IA│──────────┼──▶ Groq API
   │          │        │   └──────────────────────┘          │
   │ Usuario  │        │              │ <<include>>           │
   │          │        │              ▼                       │
   │          │        │   ┌──────────────────────┐          │
   │          │        │   │  Sintetizar Voz      │──────────┼──▶ Edge TTS
   │          │        │   └──────────────────────┘          │       │
   │          │        │              │                       │       │
   │          │        │              │ <<extend>>            │       │
   │          │        │              │ (si ElevenLabs)       │       │
   │          │        │              ▼                       │       │
   │          │        │   ┌──────────────────────┐          │       │
   │          │        │   │  Fallback a Edge TTS │          │       │
   │          │        │   └──────────────────────┘          │       │
   │          │        │              │ <<include>>           │       │
   │          │        │              ▼                       │       │
   │          │        │   ┌──────────────────────┐          │       │
   │          │        │   │  Generar Lipsync     │──────────┼──▶ Rhubarb
   │          │        │   └──────────────────────┘          │
   │          │        │              │ <<include>>           │
   │          │        │              ▼                       │
   │          │        │   ┌──────────────────────┐          │
   │          │        │   │  Guardar en Supabase │──────────┼──▶ PostgreSQL
   │          │        │   └──────────────────────┘          │
   │          │        │              │                       │
   │          │────────┼──▶┌──────────────────────┐          │
   │          │        │   │  Ver Avatar 3D       │          │
   │          │        │   └──────────────────────┘          │
   │          │        │                                       │
   │          │────────┼──▶┌──────────────────────┐          │
   │          │        │   │  Cambiar Modo        │          │
   │          │        │   └──────────────────────┘          │
   │          │        │                                       │
   │          │────────┼──▶┌──────────────────────┐          │
   │          │        │   │  Listar Historial    │          │
   │          │        │   └──────────────────────┘          │
   │          │        │                                       │
   │          │────────┼──▶┌──────────────────────┐          │
   │          │        │   │  Borrar Conversación │          │
   └──────────┘        │   └──────────────────────┘          │
                        └──────────────────────────────────────┘
```

### 9.2 Especificación de Casos de Uso

#### CU-001: Enviar Mensaje

| Campo | Detalle |
|-------|---------|
| **Actor** | Usuario |
| **Precondición** | Backend corriendo, frontend cargado |
| **Flujo principal** | 1. Usuario escribe en input<br>2. Presiona Enter o Send<br>3. Sistema envía POST /chat<br>4. Sistema procesa respuesta<br>5. Avatar reproduce audio y se anima |
| **Flujo alterno** | 4a. Si LLM falla → mostrar mensaje de error con expresión "angry" |
| **Postcondición** | Conversación guardada en Supabase |

#### CU-002: Generar Respuesta IA

| Campo | Detalle |
|-------|---------|
| **Actor** | Sistema |
| **Precondición** | Mensaje del usuario recibido |
| **Flujo principal** | 1. Construir system prompt<br>2. Llamar Groq API<br>3. Parsear JSON de respuesta<br>4. Validar formato (array de mensajes) |
| **Flujo alterno** | 2a. Si Groq rechaza → throw error<br>3a. Si JSON malformado → throw error |

#### CU-003: Sintetizar Voz (con fallback)

| Campo | Detalle |
|-------|---------|
| **Actor** | Sistema |
| **Precondición** | Texto a sintetizar disponible |
| **Flujo principal (Edge)** | 1. Si provider=edge → llamar MsEdgeTTS<br>2. Guardar MP3 en `audios/message_X.mp3` |
| **Flujo alterno (ElevenLabs)** | 1. Si provider=elevenlabs → llamar API ElevenLabs<br>2. Si falla (401, 429, network) → fallback a Edge<br>3. Marcar `fallbackUsed: "edge"` |

#### CU-004: Generar Lipsync

| Campo | Detalle |
|-------|---------|
| **Actor** | Sistema |
| **Precondición** | Archivo MP3 generado |
| **Flujo principal** | 1. FFmpeg convierte MP3 → WAV<br>2. Rhubarb procesa WAV → JSON con visemes<br>3. Sistema lee JSON y lo adjunta a la respuesta |

#### CU-005: Listar Conversaciones

| Campo | Detalle |
|-------|---------|
| **Actor** | Usuario |
| **Precondición** | Usuario tiene `X-User-Id` (incluso "anonymous") |
| **Flujo principal** | 1. GET /conversations con header X-User-Id<br>2. Sistema consulta Supabase<br>3. Devuelve array ordenado por updated_at DESC |

---

## 10. RESTRICCIONES Y SUPUESTOS

### 10.1 Restricciones

| ID | Restricción |
|----|-------------|
| **R-1** | Debe usar APIs gratuitas (Groq, Edge TTS, Supabase free tier) |
| **R-2** | Frontend debe correr en navegador moderno (Chrome, Edge, Firefox) |
| **R-3** | Backend solo Node.js (no PHP, Python, Java) |
| **R-4** | Base de datos solo PostgreSQL (Supabase) |
| **R-5** | Lipsync con Rhubarb (no proprietary) |
| **R-6** | Voz en español por defecto |
| **R-7** | Sin autenticación en MVP (solo X-User-Id como identificador) |
| **R-8** | Avatar debe ser GLB/GLTF compatible con Three.js |

### 10.2 Supuestos

| ID | Supuesto |
|----|----------|
| **S-1** | El usuario tiene conexión a internet estable |
| **S-2** | El usuario habla español (es-MX por defecto) |
| **S-3** | Groq free tier mantendrá su política gratuita |
| **S-4** | Edge TTS continuará disponible sin restricciones |
| **S-5** | Supabase free tier es suficiente (500MB DB) |
| **S-6** | Los usuarios usan navegadores con WebGL habilitado |

---

## 11. ANÁLISIS DE RIESGOS

### 11.1 Matriz de Riesgos

| ID | Riesgo | Probabilidad | Impacto | Mitigación |
|----|--------|:------------:|:-------:|------------|
| **R-01** | Groq cambia política y deja de ser gratis | Media | Alto | Mantener compatibilidad con OpenAI/local LLM |
| **R-02** | Supabase free tier insuficiente | Baja | Medio | Optimizar tablas, migrar a Postgres self-hosted |
| **R-03** | ElevenLabs bloquea cuenta gratuita | Alta | Bajo | Fallback automático a Edge TTS ✅ |
| **R-04** | Rhubarb no detecta visemes correctamente | Media | Medio | Usar `-r phonetic` y calibrar |
| **R-05** | Avatar 3D pesa demasiado (>10MB) | Baja | Medio | Optimizar GLB, usar Draco compression |
| **R-06** | Latencia total > 10 segundos | Media | Alto | Streaming de audio, caching de visemes |
| **R-07** | Backend crashes por errores async | Alta | Alto | Handlers globales `unhandledRejection` ✅ |
| **R-08** | API keys filtradas en repo público | Baja | Crítico | `.env` en `.gitignore`, secret scanning |
| **R-09** | Three.js compatibilidad de navegadores | Baja | Medio | Detectar WebGL al inicio, fallback a texto |
| **R-10** | Costos inesperados de Supabase | Baja | Medio | Monitorear uso, alertas de billing |

### 11.2 Plan de Contingencia

#### Si Groq deja de ser gratis (R-01)
1. Cambiar a Together.ai (también con free tier)
2. Cambiar a Ollama local (sin red)
3. Mantener compatibilidad OpenAI

#### Si Supabase falla (R-02)
1. Migrar a PostgreSQL self-hosted con Docker
2. Usar SQLite para desarrollo
3. Implementar cache local con IndexedDB

---

## 12. APLICACIÓN DE DSDM

### 12.1 Los 8 Principios DSDM Aplicados

| # | Principio | Aplicación en este proyecto |
|---|-----------|----------------------------|
| 1 | **Focus on the business need** | Prioridad MUST clara: chat funcional 3D |
| 2 | **Deliver on time** | Timeboxing de 6 semanas |
| 3 | **Collaborate** | Frontend ↔ Backend ↔ DevOps integrados |
| 4 | **Never compromise quality** | Cobertura ≥ 95% obligatoria |
| 5 | **Build incrementally from firm foundations** | Backend primero, luego frontend, luego tests |
| 6 | **Develop iteratively** | Sprints: API → UI → TTS → Lipsync → DB |
| 7 | **Communicate continuously** | Documentación SRS + DISEÑO + Tests |
| 8 | **Demonstrate control** | Métricas: tests passing, cobertura %, latencia |

### 12.2 Roles DSDM Asumidos

| Rol DSDM | En este proyecto |
|----------|------------------|
| Business Sponsor | Profesor / Cliente |
| Business Visionary | Estudiante (autor) |
| Technical Coordinator | Estudiante (autor) |
| Solution Developer | Estudiante (autor) |
| Solution Tester | MSTest + Jest suites |
| Business Ambassador | Usuario final del prototipo |

### 12.3 Productos DSDM Generados

| Producto DSDM | Artefacto en este proyecto |
|---------------|----------------------------|
| Feasibility Assessment | Sección 4 de este documento |
| Business Case | Sección 2 (Solución Propuesta) |
| Prioritised Requirements List | Sección 8 (MoSCoW) |
| Functional Model | DISEÑO.MD (diagramas UML) |
| Design and Build Strategy | DISEÑO.MD (arquitectura) |
| Test Plan | Tests/SUPABASE_SETUP.sql + Tests/README.md |
| Solution Increment | Cada release del chatbot |

---

## 📊 RESUMEN EJECUTIVO

| Métrica | Valor |
|---------|-------|
| **Total Requerimientos Funcionales** | 15 (RF-01 a RF-15) |
| **Total Requerimientos No Funcionales** | 25 (RNF-01 a RNF-25) |
| **Historias de Usuario** | 12 (HU-001 a HU-012) |
| **Casos de Uso UML** | 5 principales |
| **Stakeholders identificados** | 8 |
| **Riesgos analizados** | 10 |
| **Costo total estimado** | $0 USD ✅ |
| **Duración estimada** | 6 semanas |
| **Metodología** | DSDM (Dynamic Systems Development Method) |

---

**Documento generado:** Mayo 2026
**Autor:** Equipo de Desarrollo Chatbot 3D
**Próximo paso:** Lectura de `DISEÑO.MD`
