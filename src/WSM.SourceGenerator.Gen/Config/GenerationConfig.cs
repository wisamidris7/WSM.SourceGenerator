using System;
using System.Collections.Generic;
using System.Text;

namespace WSM.SourceGenerator.Gen.Config
{
    public class GenerationConfig
    {
        public GenerationConfigClass Class { get; set; }
        public GenerationConfigApiClient Api { get; set; }
        public string? Namespace { get; set; } = "{name}Controller";
    }
    public class GenerationConfigApiClient
    {
        public string? ControllerName { get; set; } = "{name}Controller";
        public string? ServiceAttrubite { get; set; } = "ControllerService";
        public string? StartAutoInjectionAttrubite { get; set; } = "Auto";
        public string? AutoTransientAttrubite { get; set; } = "AutoTransient";
        public string? AutoScopedAttrubite { get; set; } = "AutoScoped";
        public string? AutoSingletonAttrubite { get; set; } = "AutoSingleton";
    }
    public class GenerationConfigClass
    {
        public string? GenerateDtoAttribute { get; set; }
        public string? GenerateOptionalAttribute { get; set; }
        public bool GenerateEmptyConstructor { get; set; }
        public bool GenerateFieldsConstructor { get; set; }
        public bool MakePrivateAndPublicFields { get; set; }
        public string? FieldNameFormat { get; set; }
        public string? PrivateFieldFormat { get; set; }
        public string? PublicFieldFormat { get; set; }
        public bool MakeWholeNamespace { get; set; }
        public bool NullableSupport { get; set; }
        public bool OneGeneratorFile { get; set; }
        public string? OneGeneratorFileName { get; set; }
    }
}
