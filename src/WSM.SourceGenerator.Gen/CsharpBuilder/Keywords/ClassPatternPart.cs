﻿using System;
using System.Collections.Generic;
using System.Text;
using WSM.SourceGenerator.Gen.CsharpBuilder.Enums;

namespace WSM.SourceGenerator.Gen.CsharpBuilder.Keywords
{
    public class ClassPatternPart : ICSBuilderPart
    {
        private readonly ICSBuilder children;

        public ClassPatternPart(ICSBuilder children, string name, ClassTypeEnum type = ClassTypeEnum.Normal, ProtectionTypeEnum protectionType = ProtectionTypeEnum.Public, string? inherits = null)
        {
            this.children = children;
            Name = name;
            Type = type;
            ProtectionType = protectionType;
            Inherits = inherits;
        }

        public string Name { get; }
        public ClassTypeEnum Type { get; }
        public ProtectionTypeEnum ProtectionType { get; }
        public string? Inherits { get; }

        public StringBuilder Build(StringBuilder builder)
        {
            builder.Append(ProtectionType.ToString().ToLower());
            if (Type != ClassTypeEnum.Normal)
                builder.Append($" {Type.ToString().ToLower()}");
            builder.AppendLine($" class {Name}");
            if (!string.IsNullOrEmpty(Inherits))
                builder.AppendLine($" : {Inherits}");
            builder.AppendLine("{");
            builder.AppendLine(children.Build().ToString());
            builder.AppendLine("}");
            return builder;
        }
    }
    public enum ClassTypeEnum
    {
        Normal,
        Partial,
        Abstract
    }
}
