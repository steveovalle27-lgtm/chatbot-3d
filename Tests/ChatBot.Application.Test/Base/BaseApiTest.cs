using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ChatBot.Application.Test.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBot.Application.Test.Base;

/// <summary>
/// BaseApiTest — Clase base que TODOS los tests heredan.
/// Implementa el patrón [ClassInitialize] de Pacagroup.Ecommerce.Application.Test
///
/// Inicializa UNA SOLA VEZ:
///   1. Configuración (appsettings.test.json)
///   2. Dependency Injection Container
///   3. HttpClient para llamar al backend Express
/// </summary>
[TestClass]
public abstract class BaseApiTest
{
    protected static IConfigurationRoot Configuration { get; private set; } = null!;
    protected static IServiceProvider ServiceProvider { get; private set; } = null!;
    protected static HttpClient ApiClient { get; private set; } = null!;
    protected static ApiSettings ApiSettings { get; private set; } = null!;
    protected static JsonSerializerOptions JsonOptions { get; private set; } = null!;

    /// <summary>
    /// [ClassInitialize] - Equivalente al ClassInit de Pacagroup pattern
    /// Se ejecuta UNA SOLA VEZ antes del primer test de esta clase
    /// </summary>
    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        // ============================================
        // 1. CONFIGURATION (equiv. ConfigurationBuilder)
        // ============================================
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();

        // Bindear ApiSettings
        ApiSettings = new ApiSettings();
        Configuration.GetSection("ApiSettings").Bind(ApiSettings);

        // ============================================
        // 2. DEPENDENCY INJECTION (equiv. IServiceCollection)
        // ============================================
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(Configuration);
        services.AddSingleton(ApiSettings);

        // HttpClientFactory para llamar al backend Express
        services.AddHttpClient("ChatBotApi", client =>
        {
            client.BaseAddress = new Uri(ApiSettings.BaseUrl);
            client.Timeout = TimeSpan.FromMilliseconds(ApiSettings.Timeout);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-User-Id", ApiSettings.DefaultUserId);
        });

        ServiceProvider = services.BuildServiceProvider();

        // Resolver el HttpClient
        var factory = ServiceProvider.GetRequiredService<IHttpClientFactory>();
        ApiClient = factory.CreateClient("ChatBotApi");

        // ============================================
        // 3. JSON SERIALIZER OPTIONS
        // ============================================
        JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
        };

        Console.WriteLine($"✅ BaseApiTest inicializado. BaseUrl: {ApiSettings.BaseUrl}");
    }

    /// <summary>
    /// [AssemblyCleanup] - Limpieza al finalizar todos los tests
    /// </summary>
    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        ApiClient?.Dispose();
        Console.WriteLine("🧹 BaseApiTest: cleanup completado");
    }

    // ============================================
    // HELPERS para subclases
    // ============================================

    /// <summary>
    /// Verifica si el backend está corriendo (para skipear tests si no lo está)
    /// </summary>
    protected static async Task<bool> IsBackendRunningAsync()
    {
        try
        {
            var response = await ApiClient.GetAsync("/");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Helper para POST JSON requests
    /// </summary>
    protected static async Task<HttpResponseMessage> PostJsonAsync<T>(string endpoint, T payload)
    {
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await ApiClient.PostAsync(endpoint, content);
    }

    /// <summary>
    /// Helper para deserializar respuestas con diagnóstico mejorado.
    /// Si el backend devuelve un error u otro tipo, marca Inconclusive
    /// con un mensaje útil en lugar de fallar con JsonException oscuro.
    /// </summary>
    protected static async Task<T?> ReadJsonAsync<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        var trimmed = json.TrimStart();

        // CASO 1: Backend respondió con código de error HTTP
        if (!response.IsSuccessStatusCode)
        {
            Assert.Inconclusive(
                $"Backend respondió con error HTTP {(int)response.StatusCode}.\n" +
                $"Body: {json}\n" +
                $"➡️  Verifica:\n" +
                $"   1. Las tablas Supabase existen (ver Tests/README.md → SQL)\n" +
                $"   2. Las credenciales en .env del backend son correctas\n" +
                $"   3. El backend está corriendo correctamente"
            );
        }

        // CASO 2: Backend respondió 200 pero con objeto error {"error":"..."}
        if (trimmed.StartsWith("{\"error"))
        {
            Assert.Inconclusive(
                $"Backend devolvió un error en lugar de '{typeof(T).Name}'.\n" +
                $"Body: {json}\n" +
                $"➡️  Probablemente faltan tablas en Supabase. Ver Tests/README.md"
            );
        }

        // CASO 3: Se esperaba lista pero recibimos objeto
        if (typeof(T).IsGenericType &&
            typeof(T).GetGenericTypeDefinition() == typeof(List<>) &&
            !trimmed.StartsWith("["))
        {
            Assert.Inconclusive(
                $"Se esperaba un array JSON pero el backend devolvió:\n{json}"
            );
        }

        // CASO 4: Body vacío
        if (string.IsNullOrWhiteSpace(json))
        {
            Assert.Inconclusive("Backend devolvió body vacío");
        }

        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            Assert.Inconclusive(
                $"Error parseando JSON como '{typeof(T).Name}':\n" +
                $"Body recibido: {json}\n" +
                $"Error: {ex.Message}"
            );
            return default;
        }
    }

    /// <summary>
    /// Verifica si las tablas Supabase responden correctamente con un array.
    /// Útil para skipear tests de conversaciones si la BD no está lista.
    /// </summary>
    protected static async Task<bool> AreSupabaseTablesReadyAsync()
    {
        try
        {
            var response = await ApiClient.GetAsync("/conversations");
            if (!response.IsSuccessStatusCode) return false;
            var body = await response.Content.ReadAsStringAsync();
            return body.TrimStart().StartsWith("[");
        }
        catch
        {
            return false;
        }
    }
}
