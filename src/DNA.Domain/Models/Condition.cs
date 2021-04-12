using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DNA.Domain.Models {

    public interface ILogic {
        List<ILogic> Conds { get; set; }
    }

    public class Condition : ILogic {

        public List<ILogic> Conds { get; set; } = new List<ILogic>();

        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public int Index { get; private set; }

        public static Condition From(JArray array, int i) {
            var c = new Condition {
                Field = array[0].ToString(),
                Operator = $"{array[1]}",
                Value = (array[2] as JValue).Value,
                Index = i
            };
            return c;
        }
    }

    public class Group : ILogic {
        public bool IsAnd { get; set; } = true;
        public List<ILogic> Conds { get; set; } = new List<ILogic>();

        public void Add(ILogic logic) {
            Conds.Add(logic);
        }
    }

    /// <summary>
    /// <![CDATA[Sample: [[["LogDate","<","2020-12-07T00:00:00.000"],"or",["LogDate",">=","2020-12-08T00:00:00.000"]],"and",["Level","=","Warning"]]]]>
    /// </summary>
    public class ConditionCollection {
        public Group Group { get; set; }
        public ExpandoObject Expando { get; set; } = new ExpandoObject();
        int _index = 0;
        public void Add(JArray array) {
            Group = new Group();
            Expando = new ExpandoObject();
            if (array[0] is JValue)
                ParseCond(array, Group);
            else {
                ParseGroup(array, this.Group);
            }
        }

        public void ParseCond(JArray array, Group g) {
            if (array[0] is JValue) {
                if (array[0].Value<string>() == "!")
                    return;
                var c = Condition.From(array, _index++);
                g.Add(c);
            }
        }

        public void ParseGroup(JArray array, Group g) {

            if (array[0] is JValue) {
                if (array[0].Value<string>() == "!")
                    return;

                var c = Condition.From(array as JArray, _index++);
                g.Add(c);
            }
            else
                foreach (var item in array) {
                    if (item is JValue) {
                        g.IsAnd = (item as JValue).Value<string>() == "and";
                    }
                    else if (item is JArray) {
                        if (item[0] is JValue) {
                            var c = Condition.From(item as JArray, _index++);
                            g.Add(c);
                        }
                        else {
                            var ng = new Group();
                            g.Add(ng);
                            ParseGroup(item as JArray, ng);
                        }
                    }
                }
        }

        public string ToSqlQuery() {
            if (Group == null)
                return "";
            return GetSqlString(Group);
        }
        
        public string ToSqlQueryWithoutParams() {
            if (Group == null)
                return "";
            return GetSqlStringWithoutParams(Group);
        }

        string GetSqlString(Group g) {

            return string.Join(g.IsAnd ? " AND " : " OR ", g.Conds.Select(_ => {
                if (_ is Group)
                    return "(" + GetSqlString(_ as Group) + ")";
                else {
                    var c = _ as Condition;
                    var paramName = $"p{c.Index}";

                    var condition = $"{c.Field} {c.Operator} @{paramName}";

                    switch (c.Operator) {
                        case "startswith":
                            condition = $"{c.Field} LIKE @{paramName}";
                            Expando.TryAdd(paramName, $"{c.Value}%");
                            break;

                        case "endswith":
                            condition = $"{c.Field} LIKE @{paramName}";
                            Expando.TryAdd(paramName, $"%{c.Value}");
                            break;

                        case "contains":
                            condition = $"{c.Field} LIKE @{paramName}";
                            Expando.TryAdd(paramName, $"%{c.Value}%");
                            break;

                        case "notcontains":
                            condition = $"{c.Field} NOT LIKE @{paramName}";
                            Expando.TryAdd(paramName, $"%{c.Value}%");
                            break;

                        default:
                            if (c.Value == null)
                                condition = $"{c.Field} IS {(c.Operator == "=" ? "" : "NOT")} NULL";
                            else {
                                condition = $"{c.Field} {c.Operator} @{paramName}";
                                Expando.TryAdd(paramName, c.Value);
                            }
                            break;
                    }

                    return condition;
                }
            }));
        }

        string GetSqlStringWithoutParams(Group g) {

            return string.Join(g.IsAnd ? " AND " : " OR ", g.Conds.Select(_ => {
                if (_ is Group)
                    return "(" + GetSqlStringWithoutParams(_ as Group) + ")";
                else {
                    var c = _ as Condition;
                    var paramName = $"p{c.Index}";

                    var condition = $"{c.Field} {c.Operator} @{paramName}";

                    switch (c.Operator) {
                        case "startswith":
                            condition = $"{c.Field} LIKE '{c.Value}%'";
                            break;

                        case "endswith":
                            condition = $"{c.Field} LIKE '%{c.Value}";
                            break;

                        case "contains":
                            condition = $"{c.Field} LIKE '%{c.Value}%'";
                            break;

                        case "notcontains":
                            condition = $"{c.Field} NOT LIKE '%{c.Value}%'";
                            break;

                        default:
                            if (c.Value == null)
                                condition = $"{c.Field} IS {(c.Operator == "=" ? "" : "NOT")} NULL";
                            else {
                                condition = $"{c.Field} {c.Operator} '{c.Value}'";
                            }
                            break;
                    }
                    return condition;
                }
            }));
        }
    }

    /*
    public class Condition {

        public string Field { get; set; }
        public OperatorTypes Operator { get; set; }
        public object Value { get; set; }
        public bool? IsOr { get; set; }
        public string Expression { get; set; }

        public static Condition From(JArray array) {
            var c = new Condition {
                Field = array[0].ToString(),
                Operator = GetOperator($"{array[1]}"),
                Value = (array[2] as JValue).Value
            };
            return c;
        }

        public object Parse(string param) {

            var val = Value;

            Expression = "";
            switch (Operator) {
                case OperatorTypes.Equals:
                    Expression = $"([{Field}] = {param})";
                    break;
                case OperatorTypes.Contains:
                    val = $"%{Value}%";
                    Expression = $"([{Field}] LIKE {param})";
                    break;
                case OperatorTypes.StartsWith:
                    val = $"{Value}%";
                    Expression = $"([{Field}] LIKE {param})";
                    break;
                case OperatorTypes.EndsWith:
                    val = $"%{Value}";
                    Expression = $"([{Field}] LIKE {param})";
                    break;
                case OperatorTypes.DoesNotEqual:
                    Expression = $"([{Field}] <> {param})";
                    break;
                case OperatorTypes.IsLessThan:
                    Expression = $"([{Field}] < '{param})";
                    break;
                case OperatorTypes.IsGreaterThan:
                    Expression = $"([{Field}] > {param})";
                    break;
                case OperatorTypes.IsLessThanOrEqualTo:
                    Expression = $"([{Field}] <= {param})";
                    break;
                case OperatorTypes.IsGreaterThanOrEqualTo:
                    Expression = $"([{Field}] >= {param})";
                    break;
                case OperatorTypes.IsBlank:
                    Expression = $"([{Field}] = '' OR [{Field}] IS NULL)";
                    break;
                case OperatorTypes.IsNotBlank:
                    Expression = $"([{Field}] <> '' AND [{Field}] IS NOT NULL)";
                    break;
                default:
                    Expression = $"([{Field}] = {param})";
                    break;
            }
            return val;
        }

        static OperatorTypes GetOperator(string o) {
            return o switch
            {
                "=" => OperatorTypes.Equals,
                "doesnotequal" => OperatorTypes.DoesNotEqual,
                "contains" => OperatorTypes.Contains,
                "doesnotcontain" => OperatorTypes.DoesNotContain,
                "startswith" => OperatorTypes.StartsWith,
                "endswith" => OperatorTypes.EndsWith,
                "lessthen" => OperatorTypes.IsLessThan,
                "greaterthan" => OperatorTypes.IsGreaterThan,
                "lessthanorequal" => OperatorTypes.IsLessThanOrEqualTo,

                _ => OperatorTypes.Equals,
            };
        }
    }
*/

    public enum OperatorTypes {
        Equals = 1,
        Contains = 2,
        DoesNotContain = 3,
        StartsWith = 4,
        EndsWith = 5,
        DoesNotEqual = 6,
        IsLessThan = 7,
        IsGreaterThan = 8,
        IsLessThanOrEqualTo = 9,
        IsGreaterThanOrEqualTo = 10,
        IsBlank = 11,
        IsNotBlank = 12,
        IsBetween = 13,
    }
}
