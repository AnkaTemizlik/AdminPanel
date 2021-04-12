using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Domain.Models {
    public class NotificationTypes : List<IdValue> {

        public void AddDefaults() {
            if (!ContainsId(1))
                Add(new IdValue(1, "FatalError"));
        }

        public bool ContainsId(int id) {
            return this.Any(_ => _.Id == id);
        }

        public void AddRange() {

        }
    }
}
