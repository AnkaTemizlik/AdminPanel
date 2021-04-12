using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Extensions {
    public class SwaggerCustomSchemaNameGenerator : NJsonSchema.Generation.ISchemaNameGenerator {
        public string Generate(Type type) {
            return ConstructSchemaId(type);
        }

        public string ConstructSchemaId(Type type) {
            var typeName = type.Name;
            if (type.IsGenericType) {
                var genericArgs = string.Join(", ", type.GetGenericArguments().Select(ConstructSchemaId));

                int index = typeName.IndexOf('`');
                var typeNameWithoutGenericArity = index == -1 ? typeName : typeName.Substring(0, index);

                return $"{typeNameWithoutGenericArity}<{genericArgs}>";
            }
            return typeName;
        }
    }
}
