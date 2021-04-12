using System;
using System.Collections.Generic;
using System.Text;

namespace DNA.Domain.Models.Queries {
    public class UserQuery : Query {
        public int UserId { get; set; }

        public override void Prepare(Dictionary<string, object> extraParams = null, bool withDeclaretions = true) {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            if (extraParams != null)
                foreach (var item in extraParams) {
                    parameters.Add(item.Key, item.Value);
                }

            if (!parameters.ContainsKey("UserId"))
                parameters.Add("UserId", UserId);

            base.Prepare(parameters, withDeclaretions);
        }
    }
}
