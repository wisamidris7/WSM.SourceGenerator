
using System;

namespace WSM.SourceGenerator;
[GenerateDto]
public class Employee
{
    public Guid Id { get; set; }
    [GenerateOptional]
    public string? Name { get; set; }
    public string? FirstName { get; set; }
    public string LastName { get; set; }
    public string HelloName { get; set; }
    public string MyName { get; set; }
    public int Age { get; set; }
}
