namespace SourceGenerator;
[Generator]
public class ClassDtoGenerator : BaseSourceGenerator
{
    public override void Execute(GeneratorExecutionContext context)
    {
        base.Execute(context);
        if (Config.Class == null) return;

        var syntaxTrees = ManualLoad(context);
        var classes = new CSBuilder();
        classes.args ??= new();
        foreach (var syntax in syntaxTrees)
        {
            var node = syntax.GetRoot();
            var nodes = node.DescendantNodes().Where(item => item.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration));
            if (!nodes.Any(e => e.MatchAttributeNode(Config.Class.GenerateDtoAttribute)))
                continue;
            foreach (var item in nodes)
            {
                var root = (ClassDeclarationSyntax)item;
                if (!item.MatchAttributeNode(Config.Class.GenerateDtoAttribute))
                    continue;
                var props = root.GetPropertiesList()
                    .Cast<PropertyDeclarationSyntax>()
                    .Where(e => !e.MatchAttributeNode(Config.Class.IgnoreDtoAttribute))
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

            context.AddSource((Config.Class.OneGeneratorFileName ?? "Generator") + ".g.cs", classes.Build());
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

                context.AddSource(classes.args[i].ToString(), fileBuilder.Build());
                i++;
            }
        }
    }

    private void AddFields(ICSBuilder classBody, List<PropertyDeclarationSyntax> props, string className)
    {
        foreach (var item in props)
        {
            var type = item.Type.GetPropertyType();
            var privateName = item.Identifier.ValueText.GetPrivateName();
            var propName = item.Identifier.ValueText.GetPropertyName();
            if (Config.Class.MakePrivateAndPublicFields)
            {
                classBody.AddPattern(new FieldPatternPart(privateName, type, protection: ProtectionTypeEnum.Private));
                classBody.AddPattern(new PropertyPatternPart(propName, type, protection: ProtectionTypeEnum.Public, getterValue: $" => {privateName}", setterValue: $" => {privateName} = value"));
            }
            else
            {
                classBody.AddPattern(new PropertyPatternPart(propName, type, protection: ProtectionTypeEnum.Public));
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
                .Select(r => new { obj = r, optional = r.MatchAttributeNode(Config.Class.GenerateOptionalAttribute) })
                .OrderBy(e => e.optional ? 1 : 0);

            foreach (var prop in propsAndIsOptional)
            {
                setters.AddPattern(new IfPatternPart($"{prop.obj.Identifier.ValueText.GetFieldName()} is not null",
                    new CSBuilder()
                    {
                            new SetValuePatternPart(prop.obj.Identifier.ValueText.GetPrivateName(), prop.obj.Identifier.ValueText.GetFieldName())
                    }),
                    prop.optional);

                setters.AddPattern(new SetValuePatternPart(prop.obj.Identifier.ValueText.GetPrivateName(), prop.obj.Identifier.ValueText.GetFieldName()), !prop.optional);
                @params.AddPattern(new ParameterPatternPart(prop.obj.Identifier.ValueText.GetFieldName(), prop.obj.Type.GetPropertyType(prop.optional), prop.optional ? "null" : string.Empty));
            }
            classBody.AddPattern(new ConstructorPatternPart(className, @params, setters));
        }
    }

    public override void Initialize(GeneratorInitializationContext context)
    {
        //Debugger.Launch();
    }
}
