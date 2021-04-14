using Dapper.Contrib.Extensions;
using DNA.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Models {
    /// <summary>
    /// https://www.nationsonline.org/oneworld/country_code_list.htm
    /// </summary>
    [Table("{TablePrefix}Country")]
    public class Country : Model {
       [Column] public bool IsActive { get; set; }
       [Column] [Required] [StringLength(2)] public string Alpha2 { get; set; }
       [Column] [StringLength(3)] public string Alpha3 { get; set; }
       [Column] [StringLength(16)] public string UNCode { get; set; }
       [Column] [StringLength(8)] public string CallingCode { get; set; }
       [Column] [Required] [StringLength(50)] public string Name { get; set; }
       [Computed] [StringLength(1000)] public string Image { get { return $"https://www.countryflags.io/{Alpha2.ToLower()}/shiny/48.png"; } }
    }
}
