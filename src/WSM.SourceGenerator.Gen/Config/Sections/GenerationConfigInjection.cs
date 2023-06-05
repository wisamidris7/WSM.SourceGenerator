namespace SourceGenerator.Config;
public class GenerationConfigInjection
{
    public string StartAutoInjectionAttribute { get; set; } = "Auto";
    public string AutoTransientAttribute { get; set; } = "AutoTransient";
    public string AutoScopedAttribute { get; set; } = "AutoScoped";
    public string AutoSingletonAttribute { get; set; } = "AutoSingleton";
}
