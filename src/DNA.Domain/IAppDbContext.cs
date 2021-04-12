using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace DNA.Domain {
    public interface IAppDbContext : IDisposable {
        SqlConnection Connection { get; }
        SqlConnection NetsisConnection { get; }
        string SetTablePrefix(string query);
    }
}
