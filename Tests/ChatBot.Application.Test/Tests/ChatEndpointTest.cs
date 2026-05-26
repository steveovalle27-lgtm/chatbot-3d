using System.Net;
using ChatBot.Application.Test.Base;
using ChatBot.Application.Test.Helpers;
using ChatBot.Application.Test.Models;
using FluentAssertions;

namespace ChatBot.Application.Test.Tests;

/// <summary>
/// ChatEndpointTest — Pruebas del endpoint POST /chat
/// EQUIVALENTE A UsersApplicationTest.cs (Pacagroup pattern)
/// Patrón: AAA (Arrange-Act-Assert)
/// </summary>
[TestClass]
public class ChatEndpointTest : BaseApiTest
{
    private static bool _backendAvailable;

    [ClassInitialize]
    public static async Task ClassInit(TestContext context)
    {
        _backendAvailable = await IsBackendRunningAsync();
        Console.WriteLine($"📡 Backend disponible: {_backendAvailable}");
    }

    [TestInitialize]
    public void TestInit()
    {
        // [TestInitialize] - Se ejecuta ANTES de cada test
        Console.WriteLine($"🧪 Ejecutando test...");
    }

    // ============================================
    // TC-C01-C05: ESCENARIOS BÁSICOS
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-C01: POST /chat sin mensaje debe devolver bienvenida")]
    public async Task Post_Chat_WithEmptyMessage_ShouldReturnWelcome()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateEmptyRequest();

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        data.Should().NotBeNull();
        data!.Messages.Should().HaveCountGreaterThan(0);
        data.Messages[0].Text.Should().Contain("Hola");
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-C02: POST /chat con mensaje válido debe devolver respuesta del LLM")]
    public async Task Post_Chat_WithValidMessage_ShouldReturnLLMResponse()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateValidChatRequest("Hola, ¿cómo estás?");

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        data!.Messages.Should().NotBeEmpty();
        data.Messages.Should().AllSatisfy(m =>
        {
            m.Text.Should().NotBeNullOrEmpty();
            m.FacialExpression.Should().NotBeNullOrEmpty();
            m.Animation.Should().NotBeNullOrEmpty();
        });
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-C03: La respuesta debe tener máximo 3 mensajes según el system prompt")]
    public async Task Post_Chat_Response_ShouldHaveMaxThreeMessages()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateValidChatRequest("Test");

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        data!.Messages.Count.Should().BeLessThanOrEqualTo(3);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-C04: Las expresiones faciales deben ser válidas")]
    public async Task Post_Chat_Response_FacialExpressions_ShouldBeValid()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var validExpressions = new[] { "smile", "sad", "angry", "surprised", "funnyFace", "default" };
        var request = TestDataFactory.CreateValidChatRequest("Cuéntame algo");

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        data!.Messages.Should().AllSatisfy(m =>
            validExpressions.Should().Contain(m.FacialExpression));
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-C05: Las animaciones deben ser válidas")]
    public async Task Post_Chat_Response_Animations_ShouldBeValid()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var validAnimations = new[]
        {
            "Talking_0", "Talking_1", "Talking_2",
            "Crying", "Laughing", "Rumba", "Idle", "Terrified", "Angry",
        };
        var request = TestDataFactory.CreateValidChatRequest("Saluda");

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        data!.Messages.Should().AllSatisfy(m =>
            validAnimations.Should().Contain(m.Animation));
    }

    // ============================================
    // TC-C06-C08: CARACTERES ESPECIALES Y CASOS EDGE
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-C06: POST /chat con caracteres UTF-8 (ñ, áéíóú) debe funcionar")]
    public async Task Post_Chat_WithSpecialUtf8Chars_ShouldHandleCorrectly()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateValidChatRequest("¿Año, ñoño, áéíóú?");

        // ACT
        var response = await PostJsonAsync("/chat", request);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var data = await ReadJsonAsync<ChatResponse>(response);
        data!.Messages.Should().NotBeEmpty();
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-C07: POST /chat con mensaje largo debe funcionar")]
    public async Task Post_Chat_WithLongMessage_ShouldHandleCorrectly()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var longMessage = string.Join(" ",
            Enumerable.Repeat("Cuéntame una historia detallada y larga.", 20));
        var request = TestDataFactory.CreateValidChatRequest(longMessage);

        // ACT
        var response = await PostJsonAsync("/chat", request);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-C08: POST /chat con mensaje único 'Hola' debe devolver saludo")]
    public async Task Post_Chat_WithSimpleGreeting_ShouldReturnGreeting()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateValidChatRequest("Hola");

        // ACT
        var response = await PostJsonAsync("/chat", request);
        var data = await ReadJsonAsync<ChatResponse>(response);

        // ASSERT
        data!.Messages.Should().NotBeEmpty();
        data.Messages[0].Text.Should().NotBeNullOrEmpty();
    }
}
