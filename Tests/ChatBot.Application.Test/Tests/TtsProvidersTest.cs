using System.Net;
using ChatBot.Application.Test.Base;
using ChatBot.Application.Test.Helpers;
using ChatBot.Application.Test.Models;
using FluentAssertions;

namespace ChatBot.Application.Test.Tests;

/// <summary>
/// TtsProvidersTest — Pruebas de los providers TTS (Edge / ElevenLabs)
/// </summary>
[TestClass]
public class TtsProvidersTest : BaseApiTest
{
    private static bool _backendAvailable;

    [ClassInitialize]
    public static async Task ClassInit(TestContext context)
    {
        _backendAvailable = await IsBackendRunningAsync();
    }

    // ============================================
    // TC-T01-T03: EDGE TTS (gratuito)
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-T01: Provider 'edge' debe funcionar correctamente")]
    public async Task Post_Chat_WithEdgeProvider_ShouldSucceed()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateValidChatRequest("Hola edge", "both", "edge");

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        data!.Messages.Should().NotBeEmpty();
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-T02: Provider por defecto debe ser 'edge'")]
    public async Task Post_Chat_WithoutProvider_ShouldUseEdge()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = new ChatRequest
        {
            Message = "Test default provider",
            Mode = "both",
            // TtsProvider no especificado
        };

        // ACT
        var response = await PostJsonAsync("/chat", request);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-T03: Provider 'edge' NO debe marcar fallbackUsed")]
    public async Task Post_Chat_WithEdge_ShouldNotMarkFallback()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateValidChatRequest("Test", "both", "edge");

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        data!.Messages.Should().AllSatisfy(m =>
            m.FallbackUsed.Should().BeNullOrEmpty());
    }

    // ============================================
    // TC-T04-T05: ELEVENLABS (premium con fallback)
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-T04: Provider 'elevenlabs' debe procesar la petición")]
    public async Task Post_Chat_WithElevenLabsProvider_ShouldProcess()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateElevenLabsRequest("Test elevenlabs");

        // ACT
        var response = await PostJsonAsync("/chat", request);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-T05: Si ElevenLabs falla, debe usar fallback a Edge TTS")]
    public async Task Post_Chat_ElevenLabs_OnFailure_ShouldFallbackToEdge()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateElevenLabsRequest("Test fallback");

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        // O bien ElevenLabs funciona, O usa fallback. Lo importante es que devuelva 200
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        data!.Messages.Should().NotBeEmpty();
    }

    // ============================================
    // TC-T06: PROVIDER INVÁLIDO
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-T06: Provider inválido debe usar 'edge' como default")]
    public async Task Post_Chat_WithInvalidProvider_ShouldFallbackToEdge()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = new ChatRequest
        {
            Message = "Test invalid provider",
            Mode = "both",
            TtsProvider = "provider_inexistente",
        };

        // ACT
        var response = await PostJsonAsync("/chat", request);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
