using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGenerator.Config
{
    public class GenerationConfig
    {
        public GenerationConfigClass? Class { get; set; }
        public GenerationConfigApi? Api { get; set; }
        public GenerationConfigInjection? Injection { get; set; }
        public GenerationConfigClient? Client { get; set; }
        public string[] Sources { get; set; }
        public string? Namespace { get; set; }
    }
}
