using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WSM.SourceGenerator.Gen.Config
{
    public class GenerationConfigService
    {
        public GenerationConfig GetConfig(GeneratorExecutionContext context)
        {
            try
            {
                var configFile = context.AdditionalFiles.FirstOrDefault(x => string.Equals(Path.GetFileName(x.Path), "generatorConfig.json", StringComparison.OrdinalIgnoreCase));
                if (configFile == null)
                    return new();
                var config = JsonConvert.DeserializeObject<GenerationConfig>(configFile.GetText().ToString());
                return config;
            }
            catch (Exception)
            {
                return new();
            }
        }
    }
}
