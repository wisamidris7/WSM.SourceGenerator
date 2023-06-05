namespace SourceGenerator.Config;
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
