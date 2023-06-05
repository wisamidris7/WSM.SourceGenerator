namespace SourceGenerator.CsharpBuilder.Keywords;
public class MethodPatternPart : ICSBuilderPart
{
    public MethodPatternPart(string name, ICSBuilder body, ICSBuilder? param = null, string returnValue = "void", ProtectionTypeEnum protectionType = ProtectionTypeEnum.Public, bool @static = false)
    {
        Body = body;
        Params = param;
        ReturnValue = returnValue;
        Name = name;
        ProtectionType = protectionType;
        Static = @static;
    }
    public ICSBuilder Body { get; }
    public ICSBuilder? Params { get; }
    public string ReturnValue { get; }
    public string Name { get; }
    public ProtectionTypeEnum ProtectionType { get; }
    public bool Static { get; }

    public StringBuilder Build(StringBuilder builder)
    {
        builder.Append(ProtectionType.ToString().ToLower());
        if (Static)
            builder.Append($" static ");
        builder.Append($" {ReturnValue} ");
        builder.Append($"{Name}(");
        if (Params != null)
        {
            var @params = Params.Select(e => e.Build(new()).ToString());
            builder.Append(string.Join(" ,", @params));
        }
        builder.Append(")");
        if (Body != null)
        {
            builder.AppendLine("\t\n{");
            builder.AppendLine(Body.Build().ToString());
            builder.AppendLine("\t}");
        }
        else
        {
            builder.Append(" { }\n");
        }
        return builder;
    }
}
