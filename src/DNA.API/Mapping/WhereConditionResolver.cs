using AutoMapper;
using DNA.Domain.Models;
using DNA.Domain.Resources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Mapping {
    public class WhereConditionResolver : IValueResolver<QueryResource, Query, ConditionCollection> {
        //public WhereConditionResolver(IConfiguration config) {

        //}
        public ConditionCollection Resolve(QueryResource source, Query destination, ConditionCollection destMember, ResolutionContext context) {
            if (string.IsNullOrEmpty(source.Filter))
                source.Filter = string.Empty;
            destMember = new ConditionCollection();
            if (string.IsNullOrWhiteSpace(source.Filter))
                return destMember;
            destMember.Add(JArray.Parse(source.Filter));
            return destMember;
        }

    }
}
