using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PointmentApp {
    public class SqlQueries {

        static string GetByName(string name) => File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SqlQueries", name + ".sql"));

        #region Migration
        public static string Migration => GetByName("PointmentApp.Migration");
        #endregion

        #region Module Queries
        public static string SelectSmsForPlaning => GetByName(nameof(SelectSmsForPlaning));
        public static string SelectSmsForCreation => GetByName(nameof(SelectSmsForCreation));
        public static string DeleteSms => GetByName(nameof(DeleteSms));
        public static string CheckOneDayAppointmentCount => GetByName(nameof(CheckOneDayAppointmentCount));

        #endregion
    }
}
