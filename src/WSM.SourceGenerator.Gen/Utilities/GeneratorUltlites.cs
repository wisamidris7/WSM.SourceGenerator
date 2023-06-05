namespace SourceGenerator.Utilities;
static class GeneratorUtilities
{
    public static string Remove(this string value, string txt)
    {
        return value.Replace(txt, string.Empty);
    }
    public static bool EqualsAttribute(this string a1, string a2)
    {
        var att1 = a1.GetAttributeName();
        var att2 = a2.GetAttributeName();

        return att1.Equals(att2);
    }
    public static bool EqualsAttribute(this AttributeSyntax a1, string a2)
    {
        var att1 = a1.GetAttributeName();
        var att2 = a2.GetAttributeName();

        return att1.Equals(att2);
    }
    public static bool MatchAttributeNode(this SyntaxNode node, string name)
    {
        if (node.GetType().GetProperty(nameof(TypeDeclarationSyntax.AttributeLists)) is { } attributeListsProp && attributeListsProp.GetValue(node) is SyntaxList<AttributeListSyntax> attributeLists)
            return attributeLists.SelectMany(e => e.Attributes).Any(e => e.EqualsAttribute(name));
        return false;
    }
    public static List<MemberDeclarationSyntax> GetPropertiesList(this ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.Members.ToList();
    }
    public static string GetPrivateName(this string prop)
    {
        return GetFormat(prop, Config.Class.PrivateFieldFormat);
    }
    public static string GetFieldName(this string prop)
    {
        return GetFormat(prop, Config.Class.FieldNameFormat);
    }
    public static string GetPropertyName(this string prop)
    {
        return GetFormat(prop, Config.Class.PublicFieldFormat);
    }
    public static string GetFormat(this string prop, string format)
    {
        return format.Replace("{lowertxt}", prop.ToLower())
            .Replace("{uppertxt}", prop.ToUpper())
            .Replace("{txt}", prop);
    }
    public static string GetPropertyType(this TypeSyntax typeSyntax, bool optional = false)
    {
        var propType = typeSyntax.ToString();
        return propType;
    }
    public static void AddSource(this GeneratorExecutionContext context, string fileName, string text)
    {
        context.AddSource(fileName, SourceText.From(text, Encoding.UTF32));
    }
    public static void AddSource(this GeneratorExecutionContext context, string fileName, StringBuilder builder)
    {
        AddSource(context, fileName, builder.ToString());
    }
    public static void AddSource(this GeneratorExecutionContext context, string fileName, ICSBuilder builder)
    {
        AddSource(context, fileName, builder.Build());
    }
    public static string GetAttributeName(this AttributeSyntax value)
    {
        var name = value.Name.ToString();
        return name.GetAttributeName();
    }
    public static string GetAttributeName(this string value)
    {
        return value.EndsWith(AttributeName) ? value : value + AttributeName;
    }
    public static string GetVaildFileName(this string fileName)
    {
        if (fileName.EndsWith(".g.cs"))
            return fileName;
        else if (fileName.EndsWith(".cs"))
            return fileName.Substring(0, fileName.Length - 4) + ".g.cs";
        return fileName + ".g.cs";
    }
    public const string DisableStart = """
            #nullable enable
            #pragma warning disable 108
            #pragma warning disable 114
            #pragma warning disable 472
            #pragma warning disable 1573
            #pragma warning disable 1591
            #pragma warning disable 8073
            #pragma warning disable 3016
            #pragma warning disable 8600
            #pragma warning disable 8603
            #pragma warning disable 8618
            #pragma warning disable 8604
            """;
    public const string DisableEnd = "";
    public const string AttributeName = "Attribute";
    public const string JsonFileName = "generatorConfig.json";
    public static GenerationConfig Config;
}
