using System.Net;
using ChatBot.Application.Test.Base;
using ChatBot.Application.Test.Models;
using FluentAssertions;

namespace ChatBot.Application.Test.Tests;

/// <summary>
/// VoicesEndpointTest — Pruebas del endpoint GET /voices
/// </summary>
[TestClass]
public class VoicesEndpointTest : BaseApiTest
{
    private static bool _backendAvailable;

    [ClassInitialize]
    public static async Task ClassInit(TestContext context)
    {
        _backendAvailable = await IsBackendRunningAsync();
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-V01: GET /voices debe devolver 200 OK")]
    public async Task Get_Voices_ShouldReturnOk()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");

        // ACT
        var response = await ApiClient.GetAsync("/voices");

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-V02: GET /voices debe incluir voz activa")]
    public async Task Get_Voices_ShouldIncludeActiveVoice()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");

        // ACT
        var response = await ApiClient.GetAsync("/voices");
        var voices = await ReadJsonAsync<VoicesResponse>(response);

        // ASSERT
        voices.Should().NotBeNull();
        voices!.Active.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-V03: La voz activa por defecto debe ser es-MX-DaliaNeural")]
    public async Task Get_Voices_DefaultActiveVoice_ShouldBeDaliaNeural()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");

        // ACT
        var response = await ApiClient.GetAsync("/voices");
        var voices = await ReadJsonAsync<VoicesResponse>(response);

        // ASSERT
        voices!.Active.Should().Be("es-MX-DaliaNeural");
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-V04: Las voces disponibles deben ser solo en español (locale es-*)")]
    public async Task Get_Voices_AvailableVoices_ShouldAllBeSpanish()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");

        // ACT
        var response = await ApiClient.GetAsync("/voices");
        var voices = await ReadJsonAsync<VoicesResponse>(response);

        // ASSERT
        voices.Should().NotBeNull();
        voices!.Available.Should().NotBeEmpty();
        voices.Available.Should().OnlyContain(v => v.Locale.StartsWith("es-"));
    }

    [TestMethod]
    [TestCategory("Integration")]
    [Description("TC-V05: Debe haber al menos una voz mexicana disponible")]
    public async Task Get_Voices_ShouldIncludeMexicanVoices()
    {
        // ARRANGE
        if (!_backendAvailable) Assert.Inconclusive("Backend no disponible");

        // ACT
        var response = await ApiClient.GetAsync("/voices");
        var voices = await ReadJsonAsync<VoicesResponse>(response);

        // ASSERT
        voices!.Available.Should().Contain(v => v.Locale == "es-MX");
    }
}
