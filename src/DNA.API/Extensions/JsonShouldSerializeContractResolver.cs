using DNA.Domain.Extentions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DNA.API.Extensions {
    public class JsonShouldSerializeContractResolver : DefaultContractResolver {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            var restrictedRoleAttribute = member.DeclaringType.GetCustomAttribute<JsonRestrictedAttribute>();
            if (restrictedRoleAttribute != null) {
                var restrictedAttribute = member.GetCustomAttribute<JsonRestrictedRoleAttribute>();
                if (restrictedAttribute != null) {
                    property.ShouldSerialize = x => {
                        var user = (new Microsoft.AspNetCore.Http.HttpContextAccessor().HttpContext).User;
                        return !restrictedAttribute.IsRestricted(user);
                    };
                }
            }
            return property;
        }
    }
}
