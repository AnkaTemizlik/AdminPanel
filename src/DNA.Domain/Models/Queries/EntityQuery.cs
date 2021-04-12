using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace DNA.Domain.Models.Queries {
    public class EntityQuery : Query {
        
        public string Name { get; set; }

        public EntityQuery() {

        }
        
        public EntityQuery(Query query, string name) {
            Name = name;
            Load(query);
        }
    }
}
