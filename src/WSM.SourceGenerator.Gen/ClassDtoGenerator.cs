// Ignore Spelling: Dto WSM

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json.Linq;
using WSM.SourceGenerator.Gen.Config;
using WSM.SourceGenerator.Gen.CsharpBuilder;
using WSM.SourceGenerator.Gen.CsharpBuilder.Keywords;
using WSM.SourceGenerator.Gen.Shared;

namespace WSM.SourceGenerator.Gen
{
    [Generator]
    public class ClassDtoGenerator : BaseSourceGenerator
    {
        public override void Execute(GeneratorExecutionContext context)
        {
            base.Execute(context);
            if (Config.Class != null)
            {
                var syntaxTrees = ManualLoad(Config.Sources);
                var classes = new CSBuilder();
                classes.args ??= new();
                foreach (var syntax in syntaxTrees)
                {
                    var node = syntax.GetRoot();
                    var nodes = node.DescendantNodes().Where(item => item.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration));
                    if (!nodes.Any(e => MatchAttribute(e, Config.Class.GenerateDtoAttribute)))
                        continue;
                    foreach (var item in nodes)
                    {
                        ClassDeclarationSyntax root = (ClassDeclarationSyntax)item;
                        if (!MatchAttribute(item, Config.Class.GenerateDtoAttribute))
                            continue;
                        var props = GetPropertiesList(root)
                            .Cast<PropertyDeclarationSyntax>()
                            .Where(e => !MatchAttribute(e, Config.Class.IgnoreDtoAttribute))
                            .ToList();
                        var className = $"{root.Identifier.ValueText}Dto";
                        var fileName = root.Identifier.ValueText + ".g.cs";
                        classes.args.Add(fileName);
                        var classBody = new CSBuilder();
                        AddConstructors(classBody, props, className);
                        AddFields(classBody, props, className);

                        var classPart = new ClassPatternPart(classBody, className, ClassTypeEnum.Partial);

                        classes.AddPattern(classPart);
                    }
                }
                var namespacePart = new NamespacePatternPart(Config.Namespace);
                if (Config.Class.OneGeneratorFile)
                {
                    classes.Insert(0, new DisableWarningsPatternPart());
                    classes.Insert(1, namespacePart);

                    AddSource(context, (Config.Class.OneGeneratorFileName ?? "Generator") + ".g.cs", classes.Build());
                }
                else
                {
                    var i = 0;
                    // This can't be for() cause Count = 1; And When (1 is Count) - 1 = 0
                    // So He Will Not In for()
                    // foreach() will make this work
                    foreach (var item in classes)
                    {
                        ICSBuilder fileBuilder = new CSBuilder();
                        fileBuilder.AddPattern(new DisableWarningsPatternPart());
                        fileBuilder.AddPattern(namespacePart);
                        fileBuilder.AddPattern(item);

                        AddSource(context, classes.args[i].ToString(), fileBuilder.Build());
                        i++;
                    }
                }
            }
        }

        private void AddFields(ICSBuilder classBody, List<PropertyDeclarationSyntax> props, string className)
        {
            foreach (var item in props)
            {
                var type = GetPropertyType(item.Type);
                var privateName = GetPrivateName(item.Identifier.ValueText);
                var propName = GetPropertyName(item.Identifier.ValueText);
                if (Config.Class.MakePrivateAndPublicFields)
                {
                    classBody.AddPattern(new FieldPatternPart(privateName, type, protection: CsharpBuilder.Enums.ProtectionTypeEnum.Private));
                    classBody.AddPattern(new PropertyPatternPart(propName, type, protection: CsharpBuilder.Enums.ProtectionTypeEnum.Public, getterValue: $" => {privateName}", setterValue: $" => {privateName} = value"));
                }
                else
                {
                    classBody.AddPattern(new PropertyPatternPart(propName, type, protection: CsharpBuilder.Enums.ProtectionTypeEnum.Public));
                }
            }
        }
        public void AddConstructors(ICSBuilder classBody, List<PropertyDeclarationSyntax> properties, string className)
        {
            if (Config.Class.GenerateEmptyConstructor)
            {
                classBody.AddPattern(new ConstructorPatternPart(className));
            }
            if (Config.Class.GenerateFieldsConstructor)
            {
                ICSBuilder setters = new CSBuilder();
                ICSBuilder @params = new CSBuilder();
                var propsAndIsOptional = properties
                    .Select(r => new { obj = r, optional = MatchAttribute(r, Config.Class.GenerateOptionalAttribute) })
                    .OrderBy(e => e.optional ? 1 : 0);

                foreach (var prop in propsAndIsOptional)
                {
                    setters.AddPattern(new IfPatternPart($"{GetFieldName(prop.obj.Identifier.ValueText)} is not null",
                        new CSBuilder()
                        {
                            new SetValuePatternPart(GetPrivateName(prop.obj.Identifier.ValueText), GetFieldName(prop.obj.Identifier.ValueText))
                        }),
                        prop.optional);

                    setters.AddPattern(new SetValuePatternPart(GetPrivateName(prop.obj.Identifier.ValueText), GetFieldName(prop.obj.Identifier.ValueText)), !prop.optional);
                    @params.AddPattern(new ParameterPatternPart(GetFieldName(prop.obj.Identifier.ValueText), GetPropertyType(prop.obj.Type, prop.optional), prop.optional ? "null" : string.Empty));
                }
                classBody.AddPattern(new ConstructorPatternPart(className, @params, setters));
            }
        }

        public override void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
        }
    }
}