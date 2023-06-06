using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace SourceGenerator;
[Generator]
public class ControllerToClientGenerator : BaseSourceGenerator
{
    public override void Execute(GeneratorExecutionContext context)
    {
        base.Execute(context);

        // Coming: Soon!
    }
    public override void Initialize(GeneratorInitializationContext context)
    {
        Debugger.Launch();
    }
}
