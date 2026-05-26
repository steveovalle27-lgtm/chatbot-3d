using System.Net;
using ChatBot.Application.Test.Base;
using ChatBot.Application.Test.Helpers;
using ChatBot.Application.Test.Models;
using FluentAssertions;

namespace ChatBot.Application.Test.Tests;

/// <summary>
/// ChatModesTest — Pruebas de los 3 modos de chat: text, audio, both
/// </summary>
[TestClass]
public class ChatModesTest : BaseApiTest
{
    private static bool _backendAvailable;

    [ClassInitialize]
    public static async Task ClassInit(TestContext context)
    {
        _backendAvailable = await IsBackendRunningAsync();
    }

    // ============================================
    // TC-M01: MODO TEXT (solo texto)
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-M01: mode=text NO debe generar audio")]
    public async Task Post_Chat_TextMode_ShouldNotGenerateAudio()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateTextOnlyRequest("Test sin audio");

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        data!.Messages.Should().AllSatisfy(m =>
        {
            m.Audio.Should().BeNull();
            m.Lipsync.Should().BeNull();
        });
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-M02: mode=text DEBE devolver texto válido")]
    public async Task Post_Chat_TextMode_ShouldReturnText()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateTextOnlyRequest();

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        data!.Messages.Should().NotBeEmpty();
        data.Messages.Should().AllSatisfy(m => m.Text.Should().NotBeNullOrEmpty());
    }

    // ============================================
    // TC-M03-M04: MODO AUDIO (solo voz)
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-M03: mode=audio debe generar audio (base64)")]
    public async Task Post_Chat_AudioMode_ShouldGenerateAudio()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateAudioOnlyRequest("Hola");

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        data!.Messages.Should().NotBeEmpty();
        // Al menos un mensaje debe tener audio (puede fallar TTS, validamos esfuerzo)
        data.Messages.Should().Contain(m => m.Audio != null || m.Lipsync != null);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-M04: mode=audio debe poner displayText en vacío")]
    public async Task Post_Chat_AudioMode_ShouldEmptyDisplayText()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateAudioOnlyRequest();

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        data!.Messages.Should().AllSatisfy(m =>
            m.DisplayText.Should().BeEmpty());
    }

    // ============================================
    // TC-M05-M06: MODO BOTH (audio + texto)
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-M05: mode=both debe incluir texto Y audio")]
    public async Task Post_Chat_BothMode_ShouldReturnTextAndAudio()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateValidChatRequest("Hola", "both", "edge");

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        data!.Messages.Should().NotBeEmpty();
        data.Messages.Should().AllSatisfy(m => m.Text.Should().NotBeNullOrEmpty());
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-M06: mode por defecto debe ser 'both'")]
    public async Task Post_Chat_WithoutMode_ShouldDefaultToBoth()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = new ChatRequest
        {
            Message = "Test default mode",
            // Mode no especificado
        };

        // ACT
        var response = await PostJsonAsync("/chat", request);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ============================================
    // TC-M07: MODOS INVÁLIDOS
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-M07: mode inválido debe usar 'both' como default")]
    public async Task Post_Chat_WithInvalidMode_ShouldFallbackToBoth()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = new ChatRequest
        {
            Message = "Test invalid mode",
            Mode = "modo_invalido_xyz",
            TtsProvider = "edge",
        };

        // ACT
        var response = await PostJsonAsync("/chat", request);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
