using ChatBot.Application.Test.Models;

namespace ChatBot.Application.Test.Fixtures;

/// <summary>
/// Fixtures con datos predefinidos para escenarios de prueba
/// </summary>
public static class ConversationFixtures
{
    public static readonly List<ConversationDto> SampleConversations = new()
    {
        new ConversationDto
        {
            Id = 1,
            UserId = "test-user-msi",
            Title = "Primera conversación",
            CreatedAt = "2024-01-01T00:00:00Z",
            UpdatedAt = "2024-01-01T01:00:00Z",
        },
        new ConversationDto
        {
            Id = 2,
            UserId = "test-user-msi",
            Title = "Segunda conversación",
            CreatedAt = "2024-01-02T00:00:00Z",
            UpdatedAt = "2024-01-02T01:00:00Z",
        },
    };

    public static readonly string[] ValidFacialExpressions =
    {
        "smile", "sad", "angry", "surprised", "funnyFace", "default",
    };

    public static readonly string[] ValidAnimations =
    {
        "Talking_0", "Talking_1", "Talking_2",
        "Crying", "Laughing", "Rumba", "Idle", "Terrified", "Angry",
    };

    public static readonly string[] ValidModes = { "text", "audio", "both" };
    public static readonly string[] ValidProviders = { "edge", "elevenlabs" };
}
