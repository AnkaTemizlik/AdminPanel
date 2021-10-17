using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using DNA.Domain;
using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Repositories;
using DNA.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNA.Persistence.Repositories {

    [Service(typeof(IProcessRepository), Lifetime.Transient)]
    public class ProcessRepository : BaseRepository {
        public ProcessRepository(IAppDbContext context) : base(context) { }
    }
}
