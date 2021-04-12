using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Contrib.Extensions {
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }

        public ColumnAttribute() {
            
        }

        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }
}