using ChatBot.Application.Test.Models;

namespace ChatBot.Application.Test.Helpers;

/// <summary>
/// TestDataFactory — Factory Pattern para crear datos de prueba reutilizables
/// </summary>
public static class TestDataFactory
{
    public static ChatRequest CreateValidChatRequest(
        string message = "Hola, ¿cómo estás?",
        string mode = "both",
        string provider = "edge",
        long? conversationId = null)
    {
        return new ChatRequest
        {
            Message = message,
            Mode = mode,
            TtsProvider = provider,
            ConversationId = conversationId,
        };
    }

    public static ChatRequest CreateEmptyRequest()
    {
        return new ChatRequest
        {
            Message = null,
            Mode = "both",
            TtsProvider = "edge",
        };
    }

    public static ChatRequest CreateTextOnlyRequest(string message = "Test text mode")
    {
        return new ChatRequest
        {
            Message = message,
            Mode = "text",
            TtsProvider = "edge",
        };
    }

    public static ChatRequest CreateAudioOnlyRequest(string message = "Test audio mode")
    {
        return new ChatRequest
        {
            Message = message,
            Mode = "audio",
            TtsProvider = "edge",
        };
    }

    public static ChatRequest CreateElevenLabsRequest(string message = "Test elevenlabs")
    {
        return new ChatRequest
        {
            Message = message,
            Mode = "both",
            TtsProvider = "elevenlabs",
        };
    }

    public static CreateConversationRequest CreateConversationRequest(string? title = null)
    {
        return new CreateConversationRequest { Title = title };
    }

    public static string GenerateUniqueUserId()
    {
        return $"test-user-{Guid.NewGuid().ToString().Substring(0, 8)}";
    }

    public static string GenerateUniqueTitle()
    {
        return $"TEST_Title_{DateTime.UtcNow.Ticks}";
    }
}
