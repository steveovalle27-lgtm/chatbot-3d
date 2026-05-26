using System.Net;
using ChatBot.Application.Test.Base;
using ChatBot.Application.Test.Helpers;
using ChatBot.Application.Test.Models;
using FluentAssertions;

namespace ChatBot.Application.Test.Tests;

/// <summary>
/// ConversationsEndpointTest — Pruebas CRUD de conversaciones
/// Endpoints: GET, POST, DELETE /conversations
/// </summary>
[TestClass]
public class ConversationsEndpointTest : BaseApiTest
{
    private static bool _backendAvailable;
    private static bool _tablesReady;
    private static long? _createdConversationId;

    [ClassInitialize]
    public static async Task ClassInit(TestContext context)
    {
        _backendAvailable = await IsBackendRunningAsync();
        _tablesReady = _backendAvailable && await AreSupabaseTablesReadyAsync();

        if (_backendAvailable && !_tablesReady)
        {
            Console.WriteLine("⚠️  Supabase tables no responden. Ver Tests/README.md → SQL");
        }
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        // Cleanup: borrar conversaciones creadas en los tests
        if (_backendAvailable && _createdConversationId.HasValue)
        {
            try
            {
                await ApiClient.DeleteAsync($"/conversations/{_createdConversationId}");
            }
            catch { /* ignored */ }
        }
    }

    // ============================================
    // TC-CV01-CV03: LISTAR CONVERSACIONES
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-CV01: GET /conversations debe devolver 200 OK")]
    public async Task Get_Conversations_ShouldReturnOk()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");

        // ACT
        var response = await ApiClient.GetAsync("/conversations");

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-CV02: GET /conversations debe devolver un array")]
    public async Task Get_Conversations_ShouldReturnArray()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");

        // ACT
        var response = await ApiClient.GetAsync("/conversations");
        var conversations = await ReadJsonAsync<List<ConversationDto>>(response);

        // ASSERT
        conversations.Should().NotBeNull();
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-CV03: Conversaciones deben estar ordenadas por updated_at DESC")]
    public async Task Get_Conversations_ShouldBeOrderedByUpdatedAtDesc()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");

        // ACT
        var response = await ApiClient.GetAsync("/conversations");
        var conversations = await ReadJsonAsync<List<ConversationDto>>(response);

        // ASSERT
        if (conversations is { Count: > 1 })
        {
            for (int i = 0; i < conversations.Count - 1; i++)
            {
                var current = DateTime.Parse(conversations[i].UpdatedAt ?? "1900-01-01");
                var next = DateTime.Parse(conversations[i + 1].UpdatedAt ?? "1900-01-01");
                current.Should().BeOnOrAfter(next);
            }
        }
    }

    // ============================================
    // TC-CV04-CV07: CREAR CONVERSACIÓN
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-CV04: POST /conversations con título debe crear conversación")]
    public async Task Post_Conversation_WithTitle_ShouldCreateConversation()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var title = TestDataFactory.GenerateUniqueTitle();
        var request = TestDataFactory.CreateConversationRequest(title);

        // ACT
        var response = await PostJsonAsync("/conversations", request);
        var conversation = await ReadJsonAsync<ConversationDto>(response);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        conversation.Should().NotBeNull();
        conversation!.Title.Should().Be(title);
        conversation.Id.Should().BeGreaterThan(0);

        // Guardar para cleanup
        _createdConversationId = conversation.Id;
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-CV05: POST /conversations sin título debe usar 'Nueva conversación'")]
    public async Task Post_Conversation_WithoutTitle_ShouldUseDefault()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = new CreateConversationRequest { Title = null };

        // ACT
        var response = await PostJsonAsync("/conversations", request);
        var conversation = await ReadJsonAsync<ConversationDto>(response);

        // ASSERT
        conversation!.Title.Should().Be("Nueva conversación");

        // Cleanup inmediato
        await ApiClient.DeleteAsync($"/conversations/{conversation.Id}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-CV06: POST /conversations debe asignar created_at y updated_at")]
    public async Task Post_Conversation_ShouldHaveTimestamps()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateConversationRequest("Test timestamps");

        // ACT
        var response = await PostJsonAsync("/conversations", request);
        var conversation = await ReadJsonAsync<ConversationDto>(response);

        // ASSERT
        conversation!.CreatedAt.Should().NotBeNullOrEmpty();
        conversation.UpdatedAt.Should().NotBeNullOrEmpty();

        // Cleanup
        await ApiClient.DeleteAsync($"/conversations/{conversation.Id}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-CV07: POST /conversations debe asignar user_id desde X-User-Id header")]
    public async Task Post_Conversation_ShouldAssignUserIdFromHeader()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var request = TestDataFactory.CreateConversationRequest("Test user_id");

        // ACT
        var response = await PostJsonAsync("/conversations", request);
        var conversation = await ReadJsonAsync<ConversationDto>(response);

        // ASSERT
        conversation!.UserId.Should().Be(ApiSettings.DefaultUserId);

        // Cleanup
        await ApiClient.DeleteAsync($"/conversations/{conversation.Id}");
    }

    // ============================================
    // TC-CV08-CV09: MENSAJES DE CONVERSACIÓN
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-CV08: GET /conversations/:id/messages debe devolver array")]
    public async Task Get_ConversationMessages_ShouldReturnArray()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        // Crear una conversación primero
        var createResponse = await PostJsonAsync("/conversations",
            TestDataFactory.CreateConversationRequest("Test messages"));
        var conv = await ReadJsonAsync<ConversationDto>(createResponse);

        // ACT
        var response = await ApiClient.GetAsync($"/conversations/{conv!.Id}/messages");
        var messages = await ReadJsonAsync<List<StoredMessageDto>>(response);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        messages.Should().NotBeNull();

        // Cleanup
        await ApiClient.DeleteAsync($"/conversations/{conv.Id}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-CV09: Después de chat con conversationId, debe haber 2 mensajes")]
    public async Task Chat_WithConversationId_ShouldSaveMessages()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        // 1. Crear conversación
        var createResponse = await PostJsonAsync("/conversations",
            TestDataFactory.CreateConversationRequest("Test save messages"));
        var conv = await ReadJsonAsync<ConversationDto>(createResponse);

        // 2. Enviar mensaje al chat con ese conversationId
        var chatRequest = TestDataFactory.CreateTextOnlyRequest("Hola conversación");
        chatRequest.ConversationId = conv!.Id;
        await PostJsonAsync("/chat", chatRequest);

        // ACT
        var messagesResponse = await ApiClient.GetAsync($"/conversations/{conv.Id}/messages");
        var messages = await ReadJsonAsync<List<StoredMessageDto>>(messagesResponse);

        // ASSERT
        messages.Should().NotBeNull();
        messages!.Should().HaveCountGreaterThanOrEqualTo(2); // user + assistant
        messages.Should().Contain(m => m.Role == "user");
        messages.Should().Contain(m => m.Role == "assistant");

        // Cleanup
        await ApiClient.DeleteAsync($"/conversations/{conv.Id}");
    }

    // ============================================
    // TC-CV10-CV11: BORRAR CONVERSACIÓN
    // ============================================

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-CV10: DELETE /conversations/:id debe devolver success: true")]
    public async Task Delete_Conversation_ShouldReturnSuccess()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var createResponse = await PostJsonAsync("/conversations",
            TestDataFactory.CreateConversationRequest("Test delete"));
        var conv = await ReadJsonAsync<ConversationDto>(createResponse);

        // ACT
        var deleteResponse = await ApiClient.DeleteAsync($"/conversations/{conv!.Id}");
        var result = await ReadJsonAsync<DeleteResponse>(deleteResponse);

        // ASSERT
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        result!.Success.Should().BeTrue();
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-CV11: Conversación borrada NO debe aparecer en GET /conversations")]
    public async Task Delete_Conversation_ShouldRemoveFromList()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");
        var title = $"TEST_DEL_{Guid.NewGuid()}";
        var createResponse = await PostJsonAsync("/conversations",
            TestDataFactory.CreateConversationRequest(title));
        var conv = await ReadJsonAsync<ConversationDto>(createResponse);

        // ACT
        await ApiClient.DeleteAsync($"/conversations/{conv!.Id}");

        // ASSERT
        var listResponse = await ApiClient.GetAsync("/conversations");
        var list = await ReadJsonAsync<List<ConversationDto>>(listResponse);
        list.Should().NotContain(c => c.Id == conv.Id);
    }
}
