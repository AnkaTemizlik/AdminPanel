using DNA.Persistence.Contexts;
using DNA.Domain;
using DNA.Domain.Models;
using System;

namespace DNA.Persistence.Repositories {
    public abstract class BaseRepository {

        protected readonly IAppDbContext Context;

        public BaseRepository(IAppDbContext context) {
            Context = context;
        }

        protected string CleanCondition(string condition) {
            if (condition != null) {
                var check = condition.ToLower();
                if (check.Contains("update")
                    || check.Contains("delete")
                    || check.Contains("insert") || condition.ToUpper().Contains("INSERT")
                    || check.Contains("begin") || condition.ToUpper().Contains("BEGIN")
                    || check.Contains("trancate")
                    || check.Contains("commit") || condition.ToUpper().Contains("COMMIT")
                    || check.Contains(";")
                    )
                    
                    return " 1 = 0 ";
            }
            return condition;
        }
    }
}
