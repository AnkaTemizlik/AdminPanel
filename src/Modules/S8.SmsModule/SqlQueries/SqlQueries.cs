using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace S8.SmsModule {
    public class SqlQueries {

        static string GetByName(string name) => File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SqlQueries", name + ".sql"));

        #region Migration
        public static string Migration => GetByName("S8.SmsModule." + nameof(Migration));

        #endregion

        #region Module Queries

        public static string SelectProducts => GetByName(nameof(SelectProducts));

        #endregion

    }
}
