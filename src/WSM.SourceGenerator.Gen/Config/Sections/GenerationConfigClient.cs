namespace SourceGenerator.Config;
public class GenerationConfigClient
{
    public string[]? JustThisControllers { get; set; } = null;
    public string[]? JustNotThisController { get; set; } = null;
    public string ClientName { get; set; } = "{name}Client";
    public string? ClientInterface { get; set; } = null;
    public string ControllersPath { get; set; }
    public bool GetFromWholeAppToFindController { get; set; } = true;
    public bool GetAlsoGeneratedServicesFromController { get; set; } = true;

    //public string ControllersUrl { get; set; } // Soon
}
