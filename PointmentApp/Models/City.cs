using Dapper.Contrib.Extensions;
using DNA.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Models {

    [Table("{TablePrefix}City")]
    public class City : Model {

        [Column(LookupType = typeof(Country))]
        [Required]
        public int CountryId { get; set; }

        [Column(DisplayExpr = true)] [StringLength(3)] [Required] public string Code { get; set; }
        [Column(DisplayExpr = true)] [StringLength(50)] [Required] public string Name { get; set; }
    }
}
