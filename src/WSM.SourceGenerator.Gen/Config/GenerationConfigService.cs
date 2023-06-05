using Newtonsoft.Json;

namespace SourceGenerator.Config;
public class GenerationConfigService
{
    public GenerationConfig GetConfig(GeneratorExecutionContext context)
    {
        try
        {
            var configFile = context.AdditionalFiles.FirstOrDefault(x => string.Equals(Path.GetFileName(x.Path), GeneratorUtilities.JsonFileName, StringComparison.OrdinalIgnoreCase));
            if (configFile == null)
                return new();
            var config = JsonConvert.DeserializeObject<GenerationConfig>(configFile.GetText().ToString());
            GeneratorUtilities.Config = config;
            return config;
        }
        catch (Exception)
        {
            return new();
        }
    }
}
