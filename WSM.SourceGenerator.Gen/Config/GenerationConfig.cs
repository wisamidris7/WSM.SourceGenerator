using System;
using System.Collections.Generic;
using System.Text;

namespace WSM.SourceGenerator.Gen.Config
{
    public class GenerationConfig
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
        public bool MinGeneratorFile { get; set; }

    }
}
