using Microsoft.AspNetCore.Mvc;
using WSM.SourceGenerator.Lib.Attributes;

namespace WSM.SourceGenerator.Demo.Api
{
    [AutoScoped]
    [ControllerService]
    public class TestService
    {
        [HttpGet("hello")]
        public string SayHello()
        {
            return "wsaim";
        }
        [HttpPost]
        public int Querysoin(Test test)
        {
            return test.Id * 9;
        }
    }
}
