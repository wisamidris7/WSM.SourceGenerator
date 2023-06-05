using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WSM.SourceGenerator.Gen.CsharpBuilder;
using WSM.SourceGenerator.Gen.CsharpBuilder.Keywords;
using WSM.SourceGenerator.Gen.Shared;

namespace WSM.SourceGenerator.Gen
{
    [Generator]
    public class ServiceToControllerGenerator : BaseSourceGenerator
    {
        public override void Execute(GeneratorExecutionContext context)
        {
            base.Execute(context);
            if (Config.Api != null && Config.Namespace != null)
            {
                var classes = ManualLoad(Config.Sources)
                    .SelectMany(e => e.GetRoot().DescendantNodes())
                    .Where(e => e.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration) || e.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.InterfaceDeclaration))
                    .Where(e => MatchAttribute(e, Config.Api.ServiceAttribute));
                foreach (var item in classes)
                {
                    var serviceType = item;
                    var serviceName = string.Empty;
                    if (serviceType is InterfaceDeclarationSyntax interfaceDeclarationSyntax)
                        serviceName = interfaceDeclarationSyntax.Identifier.ValueText;
                    else if (serviceType is ClassDeclarationSyntax classDeclarationSyntax)
                        serviceName = classDeclarationSyntax.Identifier.ValueText;

                    var isInterface = item is InterfaceDeclarationSyntax;
                    var controllerNameOne = serviceName.Replace("Service", string.Empty);

                    var isInterfaceNaming = controllerNameOne.StartsWith("I");
                    controllerNameOne = isInterfaceNaming ? controllerNameOne.Substring(1, controllerNameOne.Length - 1) : controllerNameOne;
                    var controllerName = Config.Api.ControllerName
                        .Replace("{name}", controllerNameOne)
                        .Replace("{nameAll}", serviceName);

                    ICSBuilder controllerBuilder = new CSBuilder();
                    var parent = (CompilationUnitSyntax)(item.Parent.Parent);
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
                    var controllerBody = new CSBuilder();
                    controllerBody.AddPattern(new FieldPatternPart("baseService", serviceName, protection: CsharpBuilder.Enums.ProtectionTypeEnum.Private));
                    controllerBody.AddPattern(new ConstructorPatternPart(controllerName,
                        new CSBuilder()
                        {
                                new ParameterPatternPart("baseService", serviceName)
                        },
                        new CSBuilder()
                        {
                                new SetValuePatternPart("this.baseService", "baseService")
                        }));
                    foreach (var methodType in serviceType.ChildNodes().Where(e => e.IsKind(SyntaxKind.MethodDeclaration)))
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
                                paramsBuilder.AddPattern(new ParameterPatternPart(param.Identifier.ValueText, GetPropertyType(param.Type)));
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
                    if (!string.IsNullOrEmpty(Config.Api.ControllerAttributes))
                        foreach (var attr in Config.Api.ControllerAttributes.Split(','))
                            controllerBuilder.AddPattern(new DynamicPatternPart(attr));

                    controllerBuilder.AddPattern(new ClassPatternPart(controllerBody, controllerName, inherits: Config.Api.ControllerInherent));

                    AddSource(context, controllerName + ".g.cs", controllerBuilder.Build());
                }
            }
        }

        public override void Initialize(GeneratorInitializationContext context)
        {
            Debugger.Launch();
        }
    }
}
