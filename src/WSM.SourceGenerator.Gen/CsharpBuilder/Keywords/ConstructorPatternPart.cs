namespace SourceGenerator.CsharpBuilder.Keywords;
public class ConstructorPatternPart : ICSBuilderPart
{
    public ConstructorPatternPart(string name, ICSBuilder @params = null, ICSBuilder body = null, ProtectionTypeEnum protectionType = ProtectionTypeEnum.Public)
    {
        Name = name;
        Params = @params;
        Body = body;
        ProtectionType = protectionType;
    }

    public string Name { get; }
    public ICSBuilder Params { get; }
    public ICSBuilder Body { get; }
    public ProtectionTypeEnum ProtectionType { get; }

    public StringBuilder Build(StringBuilder builder)
    {
        builder.Append(ProtectionType.ToString().ToLower());
        builder.Append($" {Name}(");
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
