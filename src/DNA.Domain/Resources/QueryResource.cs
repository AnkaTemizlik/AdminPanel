using DNA.Domain.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.Domain.Resources {

    /// <summary>
    /// İstenilen veriyi sayfalanmış, sıralanmış veya filtrelenmiş şekilde alabilmek için ilgili alanları doldurun.
    /// </summary>
    public class QueryResource {

        /// <summary>
        /// Take değerine bağlı olarak, kaçıncı sayfanın getirileceğini belirtin. 0 girildiğinde ilk sayfayı verir.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Bir sayfada kaç kayıt olacağını belirleyin. en az 1 en çok 10000 girilebilir.
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// Gerekli ise tablodaki toplam kayıt sayısını almak için true girin.
        /// </summary>
        public bool RequireTotalCount { get; set; } = true;

        /// <summary>
        /// Belirli bir kolona göre sıralayarak sayfalandırma yapmak için örnekteki gibi değerler girin.
        /// <para><code>[{"selector":"UpdateTime","desc":false}]</code></para>
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// Veri içinde filtreleme yapmak için bu alanı doldurun.
        /// <para>Olası operatörler: <code><![CDATA["=", "<>", "<", ">", "<=", ">=", "between", "contains", "notcontains", "startswith", "endswith"]]></code></para>
        /// </summary>
        /// <example><code><![CDATA[[[["Logged","<","2020-12-07T00:00:00.000"],"or",["Logged",">=","2020-12-08T00:00:00.000"]],"and",["Level","=","Warning"]]]]> </code></example>
        public string Filter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="op"><![CDATA["=", "<>", "<", ">", "<=", ">=", "between", "contains", "notcontains", "startswith", "endswith"]]></param>
        /// <param name="value"></param>
        public void AddFilter(string field, string op, object value) {
            JArray array = string.IsNullOrWhiteSpace(Filter) ? new JArray() : JArray.Parse(Filter);

            JArray filter = new JArray {
                field,
                op,
                value
            };

            Filter = string.IsNullOrWhiteSpace(Filter)
                ? filter.ToString()
                : new JArray {
                        array,
                        "and",
                        filter
                    }.ToString();
        }

        // public string Group { get; set; }
    }
}
