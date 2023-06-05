namespace SourceGenerator;
[Generator]
public class ServiceToControllerGenerator : BaseSourceGenerator
{
    public override void Execute(GeneratorExecutionContext context)
    {
        base.Execute(context);
        if (Config.Api == null) return;

        var classes = ManualLoad(context)
            .SelectMany(e => e.GetRoot().DescendantNodes())
            .Where(e => e.MatchAttributeNode(Config.Api.ServiceAttribute));
        foreach (var item in classes)
        {
            SyntaxNode serviceType;
            string serviceName;
            InitServices(item, out serviceType, out serviceName);
            string controllerName = IsInterfaceVaildation(item, serviceName);

            ICSBuilder controllerBuilder = new CSBuilder();
            BuildUsingsAndNamespace(item, controllerBuilder);
            var controllerBody = new CSBuilder();
            BuildInjection(serviceName, controllerName, controllerBody);
            BuildMethods(serviceType, controllerBody);
            if (!string.IsNullOrEmpty(Config.Api.ControllerAttributes))
                foreach (var attr in Config.Api.ControllerAttributes.Split(','))
                    controllerBuilder.AddPattern(new DynamicPatternPart(attr));

            controllerBuilder.AddPattern(new ClassPatternPart(controllerBody, controllerName, inherits: Config.Api.ControllerInherent));

            context.AddSource(controllerName.GetVaildFileName(), controllerBuilder);
        }
    }

    void BuildMethods(SyntaxNode serviceType, CSBuilder controllerBody)
    {
        foreach (SyntaxNode? methodType in serviceType.ChildNodes().Where(e => e.IsKind(SyntaxKind.MethodDeclaration)))
        {
            if (methodType is not MethodDeclarationSyntax method)
                continue;
            var attrubites = method.AttributeLists.SelectMany(e => e.Attributes);
            if (!attrubites.Any(e => e.Name.ToString().StartsWith("Http", StringComparison.OrdinalIgnoreCase)))
                continue;
            foreach (var attribute in method.AttributeLists.SelectMany(e => e.Attributes))
            {
                controllerBody.AddPattern(new AttributesPatternPart(attribute.ToString()));
            }
            var paramsBuilder = new CSBuilder();
            var paramsRequest = new List<string>();
            var @params = method.ParameterList;
            if (@params != null)
            {
                foreach (var param in @params.Parameters)
                {
                    paramsBuilder.AddPattern(new ParameterPatternPart(param.Identifier.ValueText, param.Type.GetPropertyType()));
                    paramsRequest.Add(param.Identifier.ValueText);
                }
            }
            controllerBody.AddPattern(new MethodPatternPart(method.Identifier.ValueText, new CSBuilder()
                        {
                            new ReturnValuePatternPart(new CSBuilder()
                            {
                                new DynamicPatternPart($"baseService.{method.Identifier.ValueText}({string.Join(",", paramsRequest)})")
                            })
                        }, param: paramsBuilder, returnValue: method.ReturnType.ToString()));
        }
    }
    void BuildInjection(string serviceName, string controllerName, CSBuilder controllerBody)
    {
        controllerBody.AddPattern(new FieldPatternPart("baseService", serviceName, protection: ProtectionTypeEnum.Private));
        controllerBody.AddPattern(new ConstructorPatternPart(controllerName,
            new CSBuilder()
            {
                        new ParameterPatternPart("baseService", serviceName)
            },
            new CSBuilder()
            {
                        new SetValuePatternPart("this.baseService", "baseService")
            }));
    }
    void BuildUsingsAndNamespace(SyntaxNode? item, ICSBuilder controllerBuilder)
    {
        var parent = (CompilationUnitSyntax)item.Parent.Parent;
        var parentNamespace = (FileScopedNamespaceDeclarationSyntax)item.Parent;
        var usings = parent.Usings.ToArray();
        controllerBuilder.AddPattern(new ByNodePatternPart(usings));
        var @namespace = Config.Namespace;
        if (!Config.Namespace.EndsWith(".Controllers", StringComparison.OrdinalIgnoreCase))
            @namespace = Config.Namespace + ".Controllers";
        controllerBuilder.AddPattern(new UsingPatternPart("Microsoft.AspNetCore.Mvc"));

        var usingSerivce = parentNamespace.Name.ToString();
        if (!@namespace.StartsWith(usingSerivce) && usingSerivce != "" && usingSerivce != null && usingSerivce != ".")
            controllerBuilder.AddPattern(new UsingPatternPart(usingSerivce));
        if (!string.IsNullOrEmpty(Config.Api.ControllerUsings))
            foreach (var @using in Config.Api.ControllerUsings.Split(','))
                controllerBuilder.AddPattern(new UsingPatternPart(@using));
        controllerBuilder.AddPattern(new NamespacePatternPart(@namespace));
    }
    string IsInterfaceVaildation(SyntaxNode? item, string serviceName)
    {
        var isInterface = item is InterfaceDeclarationSyntax;
        var controllerNameOne = serviceName.Replace("Service", string.Empty);

        var isInterfaceNaming = controllerNameOne.StartsWith("I");
        controllerNameOne = isInterfaceNaming ? controllerNameOne.Substring(1, controllerNameOne.Length - 1) : controllerNameOne;
        var controllerName = Config.Api.ControllerName
            .Replace("{name}", controllerNameOne)
            .Replace("{nameAll}", serviceName);
        return controllerName;
    }
    void InitServices(SyntaxNode? item, out SyntaxNode serviceType, out string serviceName)
    {
        serviceType = item;
        serviceName = string.Empty;
        if (serviceType is InterfaceDeclarationSyntax interfaceDeclarationSyntax)
            serviceName = interfaceDeclarationSyntax.Identifier.ValueText;
        else if (serviceType is ClassDeclarationSyntax classDeclarationSyntax)
            serviceName = classDeclarationSyntax.Identifier.ValueText;
    }

    public override void Initialize(GeneratorInitializationContext context)
    {
        //Debugger.Launch();
    }
}
