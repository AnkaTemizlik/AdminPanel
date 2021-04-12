using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using DNA.Domain.Models;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace DNA.Domain.Utils {

    public static class SqlTableScriptGenerator {
        public static void Generate(string path, List<ScreenModel> madels, string tablePrefix) {

            List<TableClass> tables = new List<TableClass>();
            StringBuilder builder = new StringBuilder();

            // Get Types in the assembly.
            foreach (var m in madels) {
                if (!m.Type.IsClass)
                    continue;
                TableClass tc = new TableClass(m);
                tables.Add(tc);
            }

            // Create SQL for each table
            foreach (TableClass table in tables) {
                builder.AppendLine(table.CreateTableScript());
                builder.AppendLine();
            }

            //TODO: Total Hacked way to find FK relationships! Too lazy to fix right now
            foreach (TableClass table in tables) {
                foreach (var field in table.Fields) {
                    foreach (TableClass t2 in tables) {
                        if (field.Value.Name == t2.Table.TableName) {
                            // We have a FK Relationship!
                            builder.AppendLine("GO");
                            builder.AppendLine("ALTER TABLE " + table.Table.TableName + " WITH NOCHECK");
                            builder.AppendLine("ADD CONSTRAINT FK_" + table.Table.TableName + "_" + field.Key + " FOREIGN KEY (" + field.Key + ") REFERENCES " + t2.Table.TableName + "(ID)");
                            builder.AppendLine("GO");
                        }
                    }
                }
            }

            File.WriteAllText(Path.Combine(path, "SqlQueries", "_CreateTableScripts.sql"), builder.ToString().Replace("{TablePrefix}", tablePrefix)); 
        }

        public static string SetTablePrefix(string query, IConfiguration _configuration) {
            var tablePrefix = _configuration["Config:Database:TablePrefix"] ?? "DNA_";
            return query.Replace("{TablePrefix}", tablePrefix);
        }
    }

    public class TableClass {
        public ScreenModel Table { get; private set; }

        Dictionary<Type, string> dataMapper;

        public Dictionary<string, PropertyInfo> Fields { get; private set; } = new Dictionary<string, PropertyInfo>();

        public TableClass(ScreenModel table) {
            Table = table;

            var props = Table.Type.GetProperties();
            foreach (PropertyInfo p in props.OrderBy(_ => _.DeclaringType.Name == Table.Type.Name ? 1 : 0)) {
                Fields.Add(p.Name, p);
            }

            dataMapper = new Dictionary<Type, string>();

            dataMapper.Add(typeof(string), "NVARCHAR(500)");

            dataMapper.Add(typeof(short), "INT NOT NULL DEFAULT(0)");
            dataMapper.Add(typeof(byte), "INT NOT NULL DEFAULT(0)");
            dataMapper.Add(typeof(int), "INT NOT NULL DEFAULT(0)");
            dataMapper.Add(typeof(long), "BIGINT NOT NULL DEFAULT(0)");
            dataMapper.Add(typeof(char), "CHAR(1) NOT NULL");
            dataMapper.Add(typeof(bool), "BIT NOT NULL DEFAULT(0)");
            dataMapper.Add(typeof(DateTime), "DATETIME NOT NULL DEFAULT(GETDATE())");
            dataMapper.Add(typeof(float), "FLOAT NOT NULL DEFAULT(0)");
            dataMapper.Add(typeof(double), "FLOAT NOT NULL DEFAULT(0)");
            dataMapper.Add(typeof(decimal), "DECIMAL(16,2) NOT NULL DEFAULT(0)");
            dataMapper.Add(typeof(Guid), "UNIQUEIDENTIFIER NOT NULL");

            dataMapper.Add(typeof(short?), "INT NULL");
            dataMapper.Add(typeof(byte?), "INT NULL");
            dataMapper.Add(typeof(int?), "INT NULL");
            dataMapper.Add(typeof(long?), "BIGINT NULL");
            dataMapper.Add(typeof(char?), "CHAR(1) NULL");
            dataMapper.Add(typeof(bool?), "BIT NULL");
            dataMapper.Add(typeof(DateTime?), "DATETIME NULL");
            dataMapper.Add(typeof(float?), "FLOAT NULL");
            dataMapper.Add(typeof(double?), "FLOAT NULL");
            dataMapper.Add(typeof(decimal?), "DECIMAL(16,2) NULL");
            dataMapper.Add(typeof(Guid?), "UNIQUEIDENTIFIER NULL");

        }

        public string CreateTableScript() {
            System.Text.StringBuilder script = new StringBuilder();
            string keyFieldName = "";

            //script.AppendLine("SET ANSI_NULLS ON");
            //script.AppendLine("SET QUOTED_IDENTIFIER ON");
            script.AppendLine($"IF NOT EXISTS(SELECT * FROM sys.tables WHERE UPPER(name COLLATE SQL_Latin1_General_CP1_CI_AS) = UPPER('{Table.TableName}' COLLATE SQL_Latin1_General_CP1_CI_AS) )");
            script.AppendLine($"CREATE TABLE [{Table.TableName}] ( ");

            List<string> columns = new List<string>();
            foreach (var field in Fields) {
                var p = field.Value;
                var attrs = p.GetCustomAttributes().ToList();
                var key = attrs.Find(_ => _.GetType() == typeof(Dapper.Contrib.Extensions.KeyAttribute));
                var jsonProperty = attrs.Find(_ => _.GetType() == typeof(JsonPropertyAttribute)) as JsonPropertyAttribute;
                var required = attrs.Find(_ => _.GetType() == typeof(RequiredAttribute)) as RequiredAttribute;

                // if (p.PropertyType.IsClass) { }
                // if (p.PropertyType.IsGenericType) { }

                //if (dataMapper.ContainsKey(p.PropertyType)) {
                //    if (key != null) {
                //        keyFieldName = p.Name;
                //        columns.Add($"\t [{p.Name}] {dataMapper[p.PropertyType]} {(Table.HasIdentityIncrement ? "IDENTITY(1, 1) " : "")}");
                //    }
                //    else {
                //       columns.Add($"\t [{p.Name}] {dataMapper[p.PropertyType]}");
                //    }
                //}
                //else {
                if (key != null) {
                    keyFieldName = p.Name;
                    columns.Add($"\t [{p.Name}] INT NOT NULL {(Table.HasIdentityIncrement ? "IDENTITY(1, 1) " : "")}");
                }
                else if (dataMapper.ContainsKey(p.PropertyType)) {
                    if (jsonProperty != null && jsonProperty.Required == Required.Always) {
                        columns.Add($"\t [{p.Name}] {dataMapper[p.PropertyType]}");
                    }
                    else if (required != null) {
                        columns.Add($"\t [{p.Name}] {dataMapper[p.PropertyType]}");
                    }
                    else if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                        columns.Add($"\t [{p.Name}] {dataMapper[p.PropertyType]}");
                    }
                    else if (!p.PropertyType.IsGenericType)
                        columns.Add($"\t [{p.Name}] {dataMapper[p.PropertyType]}");
                    else
                        throw new NotImplementedException($"{Table.TableName} {p.Name} {p.PropertyType}");
                }
                else if (p.PropertyType.IsEnum) {
                    columns.Add($"\t [{p.Name}] INT NOT NULL DEFAULT(0)");
                }
                else if (p.PropertyType.IsValueType) {
                    columns.Add($"\t [{p.Name}] INT NOT NULL");
                }
                else if (p.PropertyType.IsGenericType && (p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                    || p.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    || p.PropertyType.GetGenericTypeDefinition() == typeof(List<>))) {

                }
                else if (p.PropertyType.IsClass) {

                }
                else
                    throw new NotImplementedException($"{Table.TableName} {p.Name} {p.PropertyType}");

            }

            script.Append(string.Join(",\r\n", columns));

            if (!string.IsNullOrWhiteSpace(keyFieldName)) {
                script.AppendLine(",");
                script.AppendLine($"CONSTRAINT [PK_{Table.TableName}] PRIMARY KEY CLUSTERED ([{keyFieldName}] ASC)");
                script.AppendLine("WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");
                script.AppendLine(") ON [PRIMARY]");
            }
            else
                script.AppendLine(")");

            script.AppendLine(";");

            return script.ToString();
        }
    }
}
