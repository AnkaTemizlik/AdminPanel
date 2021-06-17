﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Contrib.Extensions {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : Attribute {
        public string Name { get; set; }
        public Type LookupType { get; set; }
        public bool DisplayExpr { get; set; }
        public bool Hidden { get; set; }
        public bool Color { get; set; }
        public bool Image { get; set; }
        public string Currency { get; set; }
    }
}