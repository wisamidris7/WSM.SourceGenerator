using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using WSM.SourceGenerator.Gen.Config;
using WSM.SourceGenerator.Gen.Shared.Builder;
using WSM.SourceGenerator.Gen.Shared.Provider;

namespace WSM.SourceGenerator.Gen.Shared
{
    public abstract class BaseSourceGenerator : ISourceGenerator
    {
        public virtual void Build()
        {
            var builder = CreateBuilder();
            Provider = BuildGenerator(builder).Build();
        }
        public virtual IGeneratorBaseProvider Provider { get; set; }
        public virtual IGeneratorBaseBuilder CreateBuilder() => new GeneratorBaseBuilder();
        public virtual IGeneratorBaseBuilder BuildGenerator(IGeneratorBaseBuilder generator) => generator;

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
        public GenerationConfig Config;
        public virtual void Execute(GeneratorExecutionContext context)
        {
            Config = new GenerationConfigService().GetConfig(context);
        }
        public bool MatchAttribute(SyntaxNode node, string name)
        {
            if (node is ClassDeclarationSyntax typeDeclaration)
            {
                return typeDeclaration.AttributeLists.SelectMany(e => e.Attributes).Any(e => string.Equals(e.Name.ToString().Replace("Attribute", ""), name.Replace("Attribute", ""), StringComparison.OrdinalIgnoreCase));
            }
            if (node is PropertyDeclarationSyntax propDeclaration)
            {
                return propDeclaration.AttributeLists.SelectMany(e => e.Attributes).Any(e => string.Equals(e.Name.ToString().Replace("Attribute", ""), name.Replace("Attribute", ""), StringComparison.OrdinalIgnoreCase));
            }
            throw new UnauthorizedAccessException(node.SyntaxTree.FilePath);
            return false;
        }
        public bool MatchAttribute(ImmutableArray<AttributeData> attributes, string name)
        {
            return attributes.Any(e => string.Equals(e.AttributeClass.ToString().Replace("Attribute", ""), name.Replace("Attribute", ""), StringComparison.OrdinalIgnoreCase));
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
            return GetFormat(prop, Config.Class.PrivateFieldFormat);
        }
        public string GetFieldName(string prop)
        {
            return GetFormat(prop, Config.Class.FieldNameFormat);
        }
        public string GetPropertyName(string prop)
        {
            return GetFormat(prop, Config.Class.PublicFieldFormat);
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
            var propType = (Config.Class.MakeWholeNamespace ? propertyType.ContainingNamespace.Name + "." : "") + propertyType.Name;

            if (optional)
                propType = Config.Class.NullableSupport ? $"{propType}? = null" : $"{propType} = null";
            else if (Config.Class.NullableSupport && propertyType.NullableAnnotation == NullableAnnotation.Annotated)
                propType = $"{propType}?";

            return propType;
        }
        public string GetPropertyType(ITypeSymbol property, bool optional = false)
        {
            var propType = property.ContainingNamespace.Name + "." + property.Name;
            return propType;
        }
        public void AddSource(GeneratorExecutionContext context, string fileName ,string text)
        {
            context.AddSource(fileName, SourceText.From(text, Encoding.UTF32));
        }
        public void AddSource(GeneratorExecutionContext context, string fileName, StringBuilder builder)
        {
            AddSource(context, fileName, builder.ToString());
        }
        public abstract void Initialize(GeneratorInitializationContext context);
    }
}
