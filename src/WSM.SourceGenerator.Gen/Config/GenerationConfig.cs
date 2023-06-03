using System;
using System.Collections.Generic;
using System.Text;

namespace WSM.SourceGenerator.Gen.Config
{
    public class GenerationConfig
    {
        public GenerationConfigClass Class { get; set; }
        public GenerationConfigApiClient Client { get; set; }
    }
    public class GenerationConfigApiClient
    {
        public string ControllersPath { get; set; }
        public string MethodFormat { get; set; }
        public string ServiceFormat { get; set; }
        public string InterfaceFormat { get; set; }
    }
    public class GenerationConfigClass
    {
        public string? GenerateDtoAttribute { get; set; }
        public string? GenerateOptionalAttribute { get; set; }
        public string? Namespace { get; set; }
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
