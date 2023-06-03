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
using WSM.SourceGenerator.Gen.Config;
using WSM.SourceGenerator.Gen.CsharpBuilder;
using WSM.SourceGenerator.Gen.CsharpBuilder.Keywords;
using WSM.SourceGenerator.Gen.Shared;
using static System.Net.Mime.MediaTypeNames;

namespace WSM.SourceGenerator.Gen
{
    [Generator]
    public class ClassDtoGenerator : BaseSourceGenerator, ISourceGenerator
    {
        public override void Execute(GeneratorExecutionContext context)
        {
            base.Execute(context);
            // begin creating the source we'll inject into the users compilation
            var syntaxTrees = context.Compilation.SyntaxTrees.ToList();

            var classes = new CSBuilder();
            classes.args ??= new();
            foreach (var syntax in syntaxTrees)
            {
                var node = syntax.GetRoot();
                var nodes = node.DescendantNodes().Where(item => item.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration));
                if (!nodes.Any(e => MatchAttribute(e, Config.Class.GenerateDtoAttribute)))
                    continue;
                var fileName = Path.GetFileName(syntax.FilePath).Replace(".cs", ".g.cs");
                classes.args.Add(fileName);
                MultipleInOnePatternPart multiple = default;
                foreach (var item in nodes)
                {
                    ClassDeclarationSyntax root = (ClassDeclarationSyntax)item;
                    if (!MatchAttribute(item, Config.Class.GenerateDtoAttribute))
                        continue;
                    var props = GetPropertiesList(root, context.Compilation);
                    var className = $"{root.Identifier.ValueText}Dto";
                    var classBody = new CSBuilder();
                    AddConstructors(classBody, props, className);
                    AddFields(classBody, props, className);

                    var classPart = new ClassPatternPart(classBody, className, ClassTypeEnum.Partial);

                    if (nodes.Count() == 1)
                    {
                        classes.AddPattern(classPart);
                    }
                    else if (nodes.Count() > 1)
                    {
                        multiple ??= new(new CSBuilder());
                        multiple.Body.Add(classPart);
                    }
                }

                if (multiple != null)
                {
                    classes.AddPattern(multiple);
                }
            }
            var namespacePart = new NamespacePatternPart(Config.Class.Namespace);
            if (Config.Class.OneGeneratorFile)
            {
                classes.Insert(0, new DisableWarningsPatternPart());
                classes.Insert(1, namespacePart);
                context.AddSource((Config.Class.OneGeneratorFileName ?? "Generator") + ".g.cs", SourceText.From(classes.Build().ToString(), Encoding.UTF8));
            }
            else
            {
                for (int i = 0; i < classes.Count - 1; i++)
                {
                    var item = classes[i];
                    ICSBuilder fileBuilder = new CSBuilder();
                    fileBuilder.AddPattern(new DisableWarningsPatternPart());
                    fileBuilder.AddPattern(namespacePart);
                    fileBuilder.AddPattern(item);
                    var text = fileBuilder.Build().ToString();
                    context.AddSource(classes.args[i].ToString(), SourceText.From(text, Encoding.UTF8));
                }
            }
        }

        private void AddFields(ICSBuilder classBody, List<IPropertySymbol> props, string className)
        {
            foreach (var item in props)
            {
                var type = GetPropertyType(item);
                var privateName = GetPrivateName(item.Name);
                var propName = GetPropertyName(item.Name);
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
        public void AddConstructors(ICSBuilder classBody, List<IPropertySymbol> properties, string className)
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
                    .Select(r => new { obj = r, optional = MatchAttribute(r.GetAttributes(), Config.Class.GenerateOptionalAttribute) })
                    .OrderBy(e => e.optional ? 1 : 0);

                foreach (var prop in propsAndIsOptional)
                {
                    setters.AddPattern(new IfPatternPart($"{GetFieldName(prop.obj.Name)} is not null",
                        new CSBuilder()
                        {
                            new SetValuePatternPart(GetPrivateName(prop.obj.Name), GetFieldName(prop.obj.Name))
                        }),
                        prop.optional);

                    setters.AddPattern(new SetValuePatternPart(GetPrivateName(prop.obj.Name), GetFieldName(prop.obj.Name)), !prop.optional);
                    @params.AddPattern(new ParameterPatternPart(GetFieldName(prop.obj.Name), GetPropertyType(prop.obj, prop.optional), prop.optional ? "null" : string.Empty));
                }
                classBody.AddPattern(new ConstructorPatternPart(className, @params, setters));
            }
        }

        public override void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            //if (!Debugger.IsAttached)
            //{
            //    Debugger.Launch();
            //}
#endif
        }
    }
}