using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DNA.API.PluginTemplate {
    public class SqlQueries {

        static string GetByName(string name) => File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SqlQueries", name + ".sql"));

        #region Migration
        public static string Migration => GetByName("DNA.API.PluginTemplate.Migration");
        #endregion

        #region Module Queries
        public static string SelectInvoiceByCode => GetByName(nameof(SelectInvoiceByCode));

        #endregion
    }
}
