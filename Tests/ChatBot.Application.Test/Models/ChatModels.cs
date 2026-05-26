using System.Text.Json.Serialization;

namespace ChatBot.Application.Test.Models;

// ============================================================
// DTOs equivalentes a los objetos JSON del backend Express
// ============================================================

public class ChatRequest
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("mode")]
    public string Mode { get; set; } = "both";

    [JsonPropertyName("ttsProvider")]
    public string TtsProvider { get; set; } = "edge";

    [JsonPropertyName("conversationId")]
    public long? ConversationId { get; set; }
}

public class ChatResponse
{
    [JsonPropertyName("messages")]
    public List<MessageDto> Messages { get; set; } = new();

    [JsonPropertyName("conversationId")]
    public long? ConversationId { get; set; }
}

public class MessageDto
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("audio")]
    public string? Audio { get; set; }

    [JsonPropertyName("lipsync")]
    public object? Lipsync { get; set; }

    [JsonPropertyName("facialExpression")]
    public string FacialExpression { get; set; } = string.Empty;

    [JsonPropertyName("animation")]
    public string Animation { get; set; } = string.Empty;

    [JsonPropertyName("displayText")]
    public string? DisplayText { get; set; }

    [JsonPropertyName("fallbackUsed")]
    public string? FallbackUsed { get; set; }
}

public class ConversationDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public string? UpdatedAt { get; set; }
}

public class CreateConversationRequest
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

public class VoicesResponse
{
    [JsonPropertyName("active")]
    public string Active { get; set; } = string.Empty;

    [JsonPropertyName("available")]
    public List<VoiceDto> Available { get; set; } = new();
}

public class VoiceDto
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Locale")]
    public string Locale { get; set; } = string.Empty;

    [JsonPropertyName("ShortName")]
    public string ShortName { get; set; } = string.Empty;
}

public class StoredMessageDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("conversation_id")]
    public long ConversationId { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("animation")]
    public string? Animation { get; set; }

    [JsonPropertyName("facial_expression")]
    public string? FacialExpression { get; set; }
}

public class DeleteResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
}

public class ApiSettings
{
    public string BaseUrl { get; set; } = "http://localhost:3000";
    public int Timeout { get; set; } = 30000;
    public string DefaultUserId { get; set; } = "test-user-msi";
}
