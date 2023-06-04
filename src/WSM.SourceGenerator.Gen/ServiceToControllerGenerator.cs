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
                var classes = context.Compilation.SyntaxTrees
                    .SelectMany(e => e.GetRoot().DescendantNodes())
                    .Where(e => e.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration))
                    .Where(e => MatchAttribute(e, Config.Api.ServiceAttrubite));
                foreach (var item in classes)
                {
                    var serviceType = (ClassDeclarationSyntax)item;
                    var serviceName = serviceType.Identifier.ValueText;
                    var controllerName = Config.Api.ControllerName
                        .Replace("{name}", serviceName.Replace("Service", string.Empty))
                        .Replace("{nameAll}", serviceName);
                    ICSBuilder controllerBuilder = new CSBuilder();
                    var @namespace = Config.Namespace;
                    if (!Config.Namespace.EndsWith(".Controllers", StringComparison.OrdinalIgnoreCase))
                        @namespace = Config.Namespace + ".Controllers";
                    controllerBuilder.AddPattern(new UsingPatternPart("Microsoft.AspNetCore.Mvc"));
                    var classSymbal = context.Compilation
                        .GetSemanticModel(serviceType.SyntaxTree)
                        .GetDeclaredSymbol(serviceType);
                    var usingSerivce = classSymbal.ToString().Replace("." + classSymbal.Name, "");
                    if (!@namespace.StartsWith(usingSerivce) && usingSerivce != "" && usingSerivce != null && usingSerivce != ".")
                        controllerBuilder.AddPattern(new UsingPatternPart(usingSerivce));

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
                                var property = context.Compilation
                                    .GetSemanticModel(param.SyntaxTree)
                                    .GetDeclaredSymbol(param);
                                paramsBuilder.AddPattern(new ParameterPatternPart(param.Identifier.ValueText, GetPropertyType(property.Type)));
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
                    controllerBuilder.AddPattern(new AttributesPatternPart("ApiController"));
                    controllerBuilder.AddPattern(new AttributesPatternPart("Route", new CSBuilder()
                        {
                            new DynamicPatternPart("[controller]", true)
                        }));
                    controllerBuilder.AddPattern(new ClassPatternPart(controllerBody, controllerName, inherits: "ControllerBase"));

                    AddSource(context, controllerName + ".g.cs", controllerBuilder.Build());
                }
            }
        }

        public override void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
        }
    }
}
