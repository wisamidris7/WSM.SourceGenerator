using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using WSM.SourceGenerator.Gen.Config;

namespace WSM.SourceGenerator.Gen
{
    [Generator]
    public class ClassDtoGenerator : ISourceGenerator
    {
        public const string DisibleWarningStart = """
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
        public const string DisibleWarningEnd = "";
        public GenerationConfig Config = new();
        public void Execute(GeneratorExecutionContext context)
        {
            Config = new GenerationConfigService().GetConfig(context);
            // begin creating the source we'll inject into the users compilation
            var syntaxTrees = context.Compilation.SyntaxTrees
                .Where(e => e.GetRoot().DescendantNodes().Any(e => e.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration)))
                .Where(e => ((ClassDeclarationSyntax)e.GetRoot().DescendantNodes().First(e => e.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration))).AttributeLists.SelectMany(e => e.Attributes)
                    .Any(e => e.Name.ToString().Replace("Attribute", "") == Config.GenerateDtoAttribute.Replace("Attribute", "")))
                .ToList();
            var res = "";
            foreach (var syntax in syntaxTrees)
            {
                ClassDeclarationSyntax root = GetClassBySyntax(syntax);
                var props = GetPropertiesList(root, context.Compilation);
                var @class = $$"""
                    // Created By Wisam Idris
                    public partial class {{root.Identifier.ValueText}}Dto
                    { {{(Config.GenerateEmptyConstructor ? GetEmptyConstructor(syntax, "Dto") : "")}} {{(Config.GenerateFieldsConstructor ? GetConstructor(props, syntax, "Dto") : "")}}
                {{(Config.MakePrivateAndPublicFields ? GetPrivateProperties(props) : GetProperties(props))}} {{(Config.MakePrivateAndPublicFields ? GetPublicProperties(props) : "")}}
                    }
                """;
                if (!Config.MinGeneratorFile)
                {
                    var text = $$"""
                        {{DisibleWarningStart}}
                        namespace {{Config.Namespace}}
                        {
                            {{@class}}
                        }
                        {{DisibleWarningEnd}}
                        """;
                    context.AddSource(Path.GetFileName(syntax.FilePath).Replace(".cs", "Dto.g.cs"), SourceText.From(text, Encoding.UTF8));
                }
                else
                {
                    res += @class;
                }
            }
            if (Config.MinGeneratorFile)
            {
                var text = $$"""
                        {{DisibleWarningStart}}
                        namespace {{Config.Namespace}}
                        {
                            {{res}}
                        }
                        {{DisibleWarningEnd}}
                        """;
                context.AddSource("AllClassesGenerated.g.cs", SourceText.From(text, Encoding.UTF8));
            }
        }
        public ClassDeclarationSyntax GetClassBySyntax(SyntaxTree syntax)
        {
            return (ClassDeclarationSyntax)syntax.GetRoot().DescendantNodes().First(e => e.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration));
        }
        public string GetConstructor(List<IPropertySymbol> properties, SyntaxTree node, string additional = "")
        {
            var innerBuilder = new StringBuilder();
            var propsByIsOptional = properties.Select(r => new { obj = r, optional = r.GetAttributes().Any(e => e.AttributeClass.Name.ToString().Replace("Attribute", "") == Config.GenerateOptionalAttribute.Replace("Attribute", "")) }).OrderBy(e => e.optional ? 1 : 0).ToList();
            foreach (var prop in propsByIsOptional)
            {
                var item = prop.obj;
                if (prop.optional)
                {
                    innerBuilder.AppendLine($"\t\t\tif ({GetFieldName(item.Name)} is not null) {GetPrivateName(item.Name)} = {GetFieldName(item.Name)} ?? default; // <-- just to make complier understand");
                }
                else
                {
                    innerBuilder.AppendLine($"\t\t\t{GetPrivateName(item.Name)} = {GetFieldName(item.Name)};");
                }
            }
            var paramsBuilder = new StringBuilder();
            foreach (var prop in propsByIsOptional.Take(propsByIsOptional.Count - 1))
            {
                paramsBuilder.Append($"{GetPropertyType(prop.obj, prop.optional)} {GetFieldName(prop.obj.Name)}{(prop.optional ? "=null" : "")}, ");
            }
            var lastProp = propsByIsOptional.LastOrDefault();
            if (propsByIsOptional.Count >= 2 && lastProp != null)
                paramsBuilder.Append($"{GetPropertyType(lastProp.obj)} {GetFieldName(lastProp.obj.Name)}{(lastProp.optional ? "=null" : "")}");
            return $$"""
                
                public {{GetClassName(node)}}{{additional}}({{paramsBuilder}}) 
                        { 
                {{innerBuilder}}
                        }
                """;
        }
        public bool MatchAttribute(SyntaxNode node, string name)
        {
            if (node is TypeDeclarationSyntax typeDeclaration)
            {
                return typeDeclaration.AttributeLists.SelectMany(e => e.Attributes).Any(e => string.Equals(e.Name.ToString(), name, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }
        public string GetEmptyConstructor(SyntaxTree node, string additional = "")
        {
            return $"\npublic {GetClassName(node)}{additional}() {{ }}";
        }
        public string GetClassName(SyntaxTree node)
        {
            return GetClassBySyntax(node).Identifier.ValueText;
        }
        public string GetPrivateProperties(List<IPropertySymbol> properties)
        {
            var fieldsBuilder = new StringBuilder();

            foreach (var item in properties)
            {
                fieldsBuilder.AppendLine($"\t\tprivate {GetPropertyType(item)} {GetPrivateName(item.Name)};");
            }
            return fieldsBuilder.ToString();
        }
        public string GetPublicProperties(List<IPropertySymbol> properties)
        {
            var propsBuilder = new StringBuilder();

            foreach (var item in properties)
            {
                propsBuilder.AppendLine($"\t\tpublic {GetPropertyType(item)} {GetPropertyName(item.Name)} {{ get => {GetPrivateName(item.Name)}; set => {GetPrivateName(item.Name)} = value; }}");
            }
            return propsBuilder.ToString();
        }
        public string GetProperties(List<IPropertySymbol> properties)
        {
            var propsBuilder = new StringBuilder();

            foreach (var item in properties)
            {
                propsBuilder.AppendLine($"\t\tpublic {GetPropertyType(item)} {GetPropertyName(item.Name)} {{ get; set; }}");
            }
            return "\n" + propsBuilder.ToString();
        }
        public List<IPropertySymbol> GetPropertiesList(ClassDeclarationSyntax classDeclaration, Compilation compilation)
        {
            return classDeclaration.Members.Select(e =>
            {
                var property = compilation
                    .GetSemanticModel(e.SyntaxTree)
                    .GetDeclaredSymbol(e);
                return (IPropertySymbol)property;
            }).ToList();
        }
        public string GetPrivateName(string prop)
        {
            return GetFormat(prop, Config.PrivateFieldFormat);
        }
        public string GetFieldName(string prop)
        {
            return GetFormat(prop, Config.FieldNameFormat);
        }
        public string GetPropertyName(string prop)
        {
            return GetFormat(prop, Config.PublicFieldFormat);
        }
        public string GetFormat(string prop, string format)
        {
            return format.Replace("{lowertxt}", prop.ToLower())
                .Replace("{uppertxt}", prop.ToUpper())
                .Replace("{txt}", prop);
        }
        public string GetPropertyType(IPropertySymbol propertySymbol, bool optional = false)
        {
            var propertyType = propertySymbol.Type;
            var propType = (Config.MakeWholeNamespace ? propertyType.ContainingNamespace.Name + "." : "") + propertyType.Name;

            if (optional)
                propType = Config.NullableSupport ? $"{propType}? = null" : $"{propType} = null";
            else if (Config.NullableSupport && propertyType.NullableAnnotation == NullableAnnotation.Annotated)
                propType = $"{propType}?";

            return propType;
        }
        public void Initialize(GeneratorInitializationContext context)
        {

        }
    }
}