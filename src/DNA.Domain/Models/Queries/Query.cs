using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.Domain.Models {
    public class Query {
        public int Page { get; set; }
        //public int ItemsPerPage { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool RequireTotalCount { get; set; }
        public bool Paged { get; set; } = true;
        public List<Sort> Sort { get; set; } = new List<Sort>();
        public ConditionCollection Filter { get; set; }
        public string Group { get; set; }
        public int CurrentUserId { get; set; }
        public dynamic SqlParameters { get; set; }

        string SqlConditions;

        string SqlOrderBy;
        public string SqlCountQuery { get; private set; }

        public virtual void Load(Query query) {
            this.Page = query.Page;
            this.Paged = query.Paged;
            this.CurrentUserId = query.CurrentUserId;
            this.Filter = query.Filter;
            this.Group = query.Group;
            //this.ItemsPerPage = query.ItemsPerPage;
            this.RequireTotalCount = query.RequireTotalCount;
            this.Skip = query.Skip;
            this.Sort = query.Sort;
            this.SqlParameters = query.SqlParameters;
            this.Take = query.Take;
        }

        public virtual T Load<T>(Query query) where T : Query {
            this.Page = query.Page;
            this.Paged = query.Paged;
            this.CurrentUserId = query.CurrentUserId;
            this.Filter = query.Filter;
            this.Group = query.Group;
            //this.ItemsPerPage = query.ItemsPerPage;
            this.RequireTotalCount = query.RequireTotalCount;
            this.Skip = query.Skip;
            this.Sort = query.Sort;
            this.SqlParameters = query.SqlParameters;
            this.Take = query.Take;
            return (T)this;
        }

        public virtual void Prepare(Dictionary<string, object> extraParams = null, bool withDeclaretions = true) {
            if (Filter == null)
                Filter = new ConditionCollection();

            SqlConditions = withDeclaretions ? Filter.ToSqlQuery() : Filter.ToSqlQueryWithoutParams();

            var dictionary1 = (IDictionary<string, object>)Filter.Expando;

            if (extraParams != null) {
                foreach (var item in extraParams) {
                    dictionary1.Add(item);
                }
            }

            var expando = new ExpandoObject();
            foreach (var pair in dictionary1) {
                expando.TryAdd(pair.Key, pair.Value);
            }

            if (Paged && Take > 0 && withDeclaretions) {
                expando.TryAdd("Page", Page);
                expando.TryAdd("Take", Take);
            }
            if (CurrentUserId > 0)
                expando.TryAdd("CurrentUserId", CurrentUserId);

            SqlParameters = expando;

            if (Sort != null && Sort.Count > 0) {
                SqlOrderBy = string.Join(",", Sort.Where(_ => _.Selector != null).Select(_ => $"[{_.Selector}]{(_.Desc ? " DESC" : "")} "));
            }
            else
                SqlOrderBy ??= "1 DESC ";

            SqlOrderBy = SqlOrderBy.Replace('"', ' ').Replace(';', ' ').Replace("'", "").Replace("@", "");
        }

        public string GetSqlQuery(string sqlQuery, string orderBy = null, bool withDeclaretions = true, bool withCountQuery = true) {

            SqlOrderBy = orderBy;

            Prepare(null, withDeclaretions);

            var sql = $"SELECT * FROM ( {sqlQuery} ) AS sqlQuery" + Environment.NewLine;

            if (!string.IsNullOrWhiteSpace(SqlConditions))
                sql += "WHERE " + SqlConditions + Environment.NewLine;

            SqlCountQuery = "SELECT COUNT(0) AS [Count] FROM (" + sql + ") AS sqlQueryForCount" + Environment.NewLine;

            var sqlCount = RequireTotalCount && withCountQuery
                ? SqlCountQuery + Environment.NewLine
                : Environment.NewLine;

            sql += "ORDER BY " + (string.IsNullOrWhiteSpace(SqlOrderBy) ? $" {orderBy ?? "1 DESC "} " : SqlOrderBy);

            if (Paged && Take > 0) {
                sql += GetPagerSqlQuery(withDeclaretions) + Environment.NewLine;
            }
            if (RequireTotalCount) {
                sql += Environment.NewLine + sqlCount;
            }

            //System.Diagnostics.Debug.WriteLine(sql);

            return sql;
        }

        public string GetPagerSqlQuery(bool withDeclaretions = true) {
            string page = "Page";
            string take = "Take";
            return Paged && Take > 0
            ? (
                withDeclaretions
                    ? Environment.NewLine + @$" OFFSET ((@{page}) * @{take}) ROWS FETCH NEXT @{take} ROWS ONLY; "
                    : Environment.NewLine + @$" OFFSET ({Skip} * {Take}) ROWS FETCH NEXT {Take} ROWS ONLY;"
                )
            : string.Empty;
        }

    }

    public class Sort {
        public string Selector { get; set; }
        public bool Desc { get; set; }
    }
}
