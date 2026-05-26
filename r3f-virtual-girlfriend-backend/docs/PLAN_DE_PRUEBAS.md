# PLAN DE PRUEBAS — Chatbot Avatar 3D

**Proyecto:** r3f-virtual-girlfriend-backend
**Versión:** 1.0.0
**Metodología:** DSDM (Dynamic Systems Development Method)
**Fecha:** 2026-05-25

---

## 1. OBJETIVO

Verificar el correcto funcionamiento de los módulos del backend del chatbot 3D mediante
pruebas unitarias automatizadas, alcanzando una cobertura mínima del **95%** en:
- Statements
- Branches
- Functions
- Lines

## 2. ALCANCE

### 2.1 Módulos bajo prueba

| Módulo | Archivo | Tipo |
|--------|---------|------|
| TTS Service | `src/services/ttsService.js` | Unit |
| Lipsync Service | `src/services/lipsyncService.js` | Unit |
| Conversation Service | `src/services/conversationService.js` | Unit |
| Chat Service | `src/services/chatService.js` | Unit |
| File Utils | `src/utils/fileUtils.js` | Unit |
| Config | `src/config/index.js` | Unit |
| API Endpoints | `index.js` | Integration |

### 2.2 Fuera de alcance

- Pruebas de rendimiento (performance)
- Pruebas de seguridad penetrante
- Pruebas E2E con el frontend 3D
- APIs externas reales (Groq, ElevenLabs) — se usan mocks

## 3. ESTRATEGIA DE PRUEBAS

### 3.1 Patrón AAA (Arrange-Act-Assert)

Todas las pruebas siguen el patrón estándar:
```javascript
test('Method_Scenario_ExpectedBehavior', () => {
  // ARRANGE — preparar datos
  // ACT — ejecutar el método
  // ASSERT — verificar resultado
});
```

### 3.2 Pirámide de pruebas

```
        E2E (0%)       <- No incluido
       /        \
      Integration (15%) <- chat.integration.test.js
     /            \
    Unit (85%)     <- Resto de tests
```

### 3.3 Herramientas

- **Framework:** Jest 29.x
- **Mocks:** jest.mock(), nock para HTTP
- **Cobertura:** Istanbul (incluido en Jest)
- **CI:** GitHub Actions (futuro)

## 4. CASOS DE PRUEBA

### TC-001: TTS Service — Edge TTS

| ID | Descripción | Entrada | Resultado Esperado | Severidad |
|----|-------------|---------|--------------------|-----------:|
| TC-001-01 | Sintetizar texto válido | "Hola" | Buffer no vacío | Alta |
| TC-001-02 | Texto vacío | "" | Throw "Text cannot be empty" | Alta |
| TC-001-03 | Texto null | null | Throw error | Media |
| TC-001-04 | Texto muy largo (5000 chars) | "a".repeat(5000) | Buffer válido | Baja |

### TC-002: TTS Service — ElevenLabs

| ID | Descripción | Entrada | Resultado Esperado | Severidad |
|----|-------------|---------|--------------------|-----------:|
| TC-002-01 | API key válida + texto | "Hola" | Buffer no vacío | Alta |
| TC-002-02 | API key faltante | (sin env) | Throw "API key missing" | Alta |
| TC-002-03 | API responde 401 | mock 401 | Throw axios error | Alta |
| TC-002-04 | API responde 429 (rate limit) | mock 429 | Throw axios error | Media |

### TC-003: TTS Provider Selection

| ID | Descripción | Entrada | Resultado Esperado |
|----|-------------|---------|--------------------|
| TC-003-01 | provider=edge | "edge" | Llama a Edge TTS |
| TC-003-02 | provider=elevenlabs | "elevenlabs" | Llama a ElevenLabs |
| TC-003-03 | provider undefined | undefined | Usa Edge (default) |
| TC-003-04 | provider inválido | "invalid" | Usa Edge (default) |

### TC-004: Lipsync Service

| ID | Descripción | Resultado Esperado |
|----|-------------|--------------------|
| TC-004-01 | Convertir MP3 a WAV con FFmpeg | Archivo .wav creado |
| TC-004-02 | Generar JSON con Rhubarb | Archivo .json válido |
| TC-004-03 | execCommand exitoso | Resolve con stdout |
| TC-004-04 | execCommand falla | Reject con error |

### TC-005: Conversation Service

| ID | Descripción | Resultado Esperado |
|----|-------------|--------------------|
| TC-005-01 | Crear conversación | Devuelve registro con ID |
| TC-005-02 | Listar conversaciones por usuario | Array ordenado por fecha |
| TC-005-03 | Obtener mensajes de conversación | Array de mensajes |
| TC-005-04 | Borrar conversación | success: true |
| TC-005-05 | Guardar mensaje de usuario | Insert exitoso |
| TC-005-06 | Guardar mensaje de asistente | Insert exitoso |

### TC-006: Chat Service (Lógica principal)

| ID | Descripción | Resultado Esperado |
|----|-------------|--------------------|
| TC-006-01 | Sin mensaje del usuario | Devuelve mensaje de bienvenida |
| TC-006-02 | Sin GROQ_API_KEY | Devuelve mensaje de error |
| TC-006-03 | Mensaje válido + mode=text | Sin audio, sin lipsync |
| TC-006-04 | Mensaje válido + mode=audio | Audio + lipsync + displayText vacío |
| TC-006-05 | Mensaje válido + mode=both | Audio + lipsync + text |
| TC-006-06 | ElevenLabs falla → fallback Edge | fallbackUsed = "edge" |

### TC-007: File Utils

| ID | Descripción | Resultado Esperado |
|----|-------------|--------------------|
| TC-007-01 | audioFileToBase64 con archivo válido | String base64 |
| TC-007-02 | audioFileToBase64 archivo inexistente | Throw ENOENT |
| TC-007-03 | readJsonTranscript JSON válido | Objeto parseado |
| TC-007-04 | readJsonTranscript JSON inválido | Throw SyntaxError |

## 5. CRITERIOS DE ACEPTACIÓN

✅ **CA-1:** Todas las pruebas unitarias pasan (0 fallos)
✅ **CA-2:** Cobertura de código ≥ 95% en todas las métricas
✅ **CA-3:** No hay tests con `skip()` o `xit()` sin justificación
✅ **CA-4:** Cada test sigue patrón AAA documentado
✅ **CA-5:** Tests se ejecutan en menos de 30 segundos
✅ **CA-6:** Reporte HTML generado en `coverage/lcov-report/index.html`

## 6. MATRIZ DE TRAZABILIDAD

| Requerimiento (SRS) | Casos de Prueba |
|---------------------|-----------------|
| RF-01: Síntesis de voz | TC-001, TC-002, TC-003 |
| RF-02: Generar lipsync | TC-004 |
| RF-03: Persistir conversaciones | TC-005 |
| RF-04: Procesar mensajes del usuario | TC-006 |
| RF-05: Modos de respuesta (text/audio/both) | TC-006-03, TC-006-04, TC-006-05 |
| RF-06: Fallback automático TTS | TC-006-06 |
| RNF-01: Tolerancia a fallos | TC-001-02, TC-002-02, TC-007-02 |

## 7. EJECUCIÓN

```powershell
# Todos los tests
npm test

# Solo unitarios
npm run test:unit

# Con cobertura
npm run test:coverage

# Abrir reporte HTML
start coverage\lcov-report\index.html
```

## 8. REPORTE

El reporte se genera automáticamente en:
- **Terminal:** Resumen de cobertura por archivo
- **HTML:** `coverage/lcov-report/index.html`
- **JSON:** `coverage/coverage-summary.json`
- **LCOV:** `coverage/lcov.info` (para CI/CD)

## 9. RESPONSABLES

| Rol | Responsabilidad |
|-----|-----------------|
| Developer | Escribir y mantener tests unitarios |
| QA | Validar casos de prueba |
| Tech Lead | Aprobar cobertura final |
