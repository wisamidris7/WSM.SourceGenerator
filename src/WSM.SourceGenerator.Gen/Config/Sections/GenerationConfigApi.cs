namespace SourceGenerator.Config;
public class GenerationConfigApi
{
    public string? ControllerAttributes { get; set; }
    public string? ControllerUsings { get; set; }
    public string ControllerInherent { get; set; } = "ControllerBase";
    public string ControllerName { get; set; } = "{name}Controller";
    public string ServiceAttribute { get; set; } = "ControllerService";
}
