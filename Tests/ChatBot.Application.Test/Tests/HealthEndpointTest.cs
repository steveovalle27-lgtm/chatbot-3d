using System.Net;
using ChatBot.Application.Test.Base;
using FluentAssertions;

namespace ChatBot.Application.Test.Tests;

/// <summary>
/// HealthEndpointTest — Pruebas del endpoint GET / (health check)
/// Equivalente a UsersApplicationTest.cs de Pacagroup
/// </summary>
[TestClass]
public class HealthEndpointTest : BaseApiTest
{
    private static bool _backendAvailable;

    [ClassInitialize]
    public static async Task ClassInit(TestContext context)
    {
        _backendAvailable = await IsBackendRunningAsync();
        if (!_backendAvailable)
        {
            Console.WriteLine("⚠️  Backend no está corriendo. Tests se marcarán como Inconclusive.");
        }
    }

    // ============================================
    // [TestMethod] - Casos de prueba siguiendo patrón AAA
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-H01: GET / debe devolver 200 OK cuando el servidor está corriendo")]
    public async Task Get_Root_WhenServerRunning_ShouldReturnOk()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");

        // ACT
        var response = await ApiClient.GetAsync("/");

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-H02: GET / debe devolver el mensaje de bienvenida")]
    public async Task Get_Root_ShouldReturnWelcomeMessage()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");

        // ACT
        var response = await ApiClient.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // ASSERT
        content.Should().Contain("Chatbot");
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-H03: GET / debe tener tiempo de respuesta < 2000ms")]
    public async Task Get_Root_ShouldRespondQuickly()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // ACT
        await ApiClient.GetAsync("/");
        stopwatch.Stop();

        // ASSERT
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000);
    }
}
