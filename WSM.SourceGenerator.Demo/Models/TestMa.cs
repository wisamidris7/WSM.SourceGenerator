using System;

namespace WSM.SourceGenerator;

[GenerateDto]
public class TestMa
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? FirstName { get; set; }
    public string LastName { get; set; }
    public string HelloName { get; set; }
    [GenerateOptional]

    public string? MyName { get; set; }
    public int Age { get; set; }
}
