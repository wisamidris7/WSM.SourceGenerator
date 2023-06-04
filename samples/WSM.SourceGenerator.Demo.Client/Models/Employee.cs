
using System;
using WSM.SourceGenerator.Lib.Attributes;

namespace WSM.SourceGenerator;
[GenerateDto]
public class Employee1
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
[GenerateDto]
public class Employee2
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