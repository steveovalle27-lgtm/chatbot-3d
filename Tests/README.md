# Tests MSTest — ChatBot Avatar 3D

Proyecto de pruebas unitarias **MSTest** (.NET 8.0) que valida la API del chatbot vía HTTP.

Equivalente al patrón **Pacagroup.Ecommerce.Application.Test**.

---

## 📋 Requisitos

- **Visual Studio 2022** (Community/Pro/Enterprise) ó VS Code con C# Dev Kit
- **.NET SDK 8.0+** ([descargar](https://dotnet.microsoft.com/download))
- **Backend corriendo** en `http://localhost:3000`

---

## 🚀 Cómo ejecutar las pruebas

### Opción 1: Visual Studio 2022 (Recomendado para Code Coverage)

1. **Abrir solución:**
   ```
   File > Open > Project/Solution
   → D:\Programas\Visual Studio\chatbot-3d\test\ChatBot.sln
   ```

2. **Levantar el backend antes** (en otra terminal):
   ```powershell
   cd "D:\Programas\Visual Studio\chatbot-3d"
   npm run dev
   ```

3. **Restore packages** (primera vez):
   ```
   Tools > NuGet Package Manager > Manage NuGet Packages for Solution
   → Restore
   ```

4. **Ejecutar tests:**
   - `Test > Run All Tests` (Ctrl + R, A)
   - O abrir **Test Explorer**: `View > Test Explorer`

5. **Ver Code Coverage:** ⭐
   - `Test > Analyze Code Coverage for All Tests`
   - Ó botón derecho en Test Explorer → `Analyze Code Coverage`
   - Resultado: **Code Coverage Results** window

6. **Aplicar runsettings** (para mejor cobertura):
   ```
   Test > Configure Run Settings > Select Solution Wide runsettings File
   → coverage.runsettings
   ```

### Opción 2: Línea de comandos (dotnet CLI)

```powershell
cd "D:\Programas\Visual Studio\chatbot-3d\test\ChatBot.Application.Test"

# Restore + Build
dotnet restore
dotnet build

# Ejecutar tests
dotnet test

# Con cobertura (formato cobertura/lcov)
dotnet test --collect:"XPlat Code Coverage" --settings coverage.runsettings

# Generar reporte HTML (necesita reportgenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:./TestResults/**/coverage.cobertura.xml -targetdir:./TestResults/CoverageReport -reporttypes:Html
start ./TestResults/CoverageReport/index.html
```

---

## 📁 Estructura

```
test/
├── ChatBot.sln                          # Solution para abrir en VS
└── ChatBot.Application.Test/
    ├── ChatBot.Application.Test.csproj  # Proyecto MSTest
    ├── coverage.runsettings             # Config cobertura VS
    ├── appsettings.test.json            # Configuración del proyecto
    │
    ├── Base/
    │   └── BaseApiTest.cs               # ⭐ [ClassInitialize] base
    │
    ├── Models/
    │   └── ChatModels.cs                # DTOs (ChatRequest, etc.)
    │
    ├── Helpers/
    │   └── TestDataFactory.cs           # Factory Pattern
    │
    ├── Fixtures/
    │   └── ConversationFixtures.cs      # Datos predefinidos
    │
    └── Tests/                           # ⭐ 6 clases de pruebas
        ├── HealthEndpointTest.cs        # GET / (3 tests)
        ├── VoicesEndpointTest.cs        # GET /voices (5 tests)
        ├── ChatEndpointTest.cs          # POST /chat básico (8 tests)
        ├── ChatModesTest.cs             # text/audio/both (7 tests)
        ├── TtsProvidersTest.cs          # edge/elevenlabs (6 tests)
        └── ConversationsEndpointTest.cs # CRUD (11 tests)
```

---

## 🧪 Casos de Prueba

| ID | Endpoint | Categoría | Total |
|----|----------|-----------|------:|
| TC-H01-03 | GET / | Health | 3 |
| TC-V01-05 | GET /voices | Voces | 5 |
| TC-C01-08 | POST /chat | Chat básico | 8 |
| TC-M01-07 | POST /chat | Modos respuesta | 7 |
| TC-T01-06 | POST /chat | Providers TTS | 6 |
| TC-CV01-11 | /conversations | CRUD | 11 |
| **TOTAL** | | | **40** |

---

## 🎯 Patrón Pacagroup aplicado

```csharp
// .NET Pacagroup pattern:                    // Equivalencia en este proyecto:

[TestClass]                                   // ✅ [TestClass]
public class UsersApplicationTest             // ✅ ChatEndpointTest : BaseApiTest

[ClassInitialize]                             // ✅ [AssemblyInitialize] en BaseApiTest
public static void ClassInit(TestContext ctx) // ✅ AssemblyInit configura DI + HttpClient

[TestMethod]                                  // ✅ [TestMethod]
public void Insert_Test()                     // ✅ Post_Chat_WithValidMessage_ShouldReturn...

ConfigurationBuilder + appsettings.json       // ✅ ConfigurationBuilder + appsettings.test.json
IServiceCollection (DI)                       // ✅ ServiceCollection + AddHttpClient

Assert.IsTrue(result.IsSuccess)               // ✅ FluentAssertions: result.Should().Be(...)
```

---

## ⚠️ Notas importantes

1. **El backend DEBE estar corriendo** en `http://localhost:3000`
   - Si no lo está, los tests devuelven `Inconclusive` en lugar de `Failed`

2. **Code Coverage en Visual Studio:**
   - **VS Enterprise**: Built-in coverage (mejor)
   - **VS Community/Pro**: Necesita `coverlet.collector` (ya incluido en .csproj)

3. **CI/CD:** Los reportes Cobertura/LCOV/OpenCover se generan en `TestResults/Coverage/`
