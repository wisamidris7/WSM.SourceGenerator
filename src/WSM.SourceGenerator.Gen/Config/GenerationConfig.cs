using System;
using System.Collections.Generic;
using System.Text;

namespace WSM.SourceGenerator.Gen.Config
{
    public class GenerationConfig
    {
        public GenerationConfigClass Class { get; set; }
        public GenerationConfigApiClient Api { get; set; }
        public string[] Sources { get; set; }
        public string? Namespace { get; set; }
    }
    public class GenerationConfigApiClient
    {
        public string? ControllerAttributes { get; set; }
        public string? ControllerUsings { get; set; }
        public string ControllerInherent { get; set; } = "ControllerBase";
        public string ControllerName { get; set; } = "{name}Controller";
        public string ServiceAttribute { get; set; } = "ControllerService";
        public string StartAutoInjectionAttribute { get; set; } = "Auto";
        public string AutoTransientAttribute { get; set; } = "AutoTransient";
        public string AutoScopedAttribute { get; set; } = "AutoScoped";
        public string AutoSingletonAttribute { get; set; } = "AutoSingleton";
    }
    public class GenerationConfigClass
    {
        public string IgnoreDtoAttribute { get; set; } = "IgnoreDto";
        public string GenerateDtoAttribute { get; set; } = "GenerateDto";
        public string GenerateOptionalAttribute { get; set; } = "GenerateOptional";
        public bool GenerateEmptyConstructor { get; set; } = true;
        public bool GenerateFieldsConstructor { get; set; } = true;
        public bool MakePrivateAndPublicFields { get; set; } = true;
        public string FieldNameFormat { get; set; } = "{lowertxt}";
        public string PrivateFieldFormat { get; set; } = "_{lowertxt}";
        public string PublicFieldFormat { get; set; } = "{txt}";
        public bool MakeWholeNamespace { get; set; } = true;
        public bool NullableSupport { get; set; } = true;
        public bool OneGeneratorFile { get; set; }
        public string? OneGeneratorFileName { get; set; }
    }
}
