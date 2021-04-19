using Dapper.Contrib.Extensions;
using DNA.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Models {

    [Table("{TablePrefix}Service")]
    public class Service : Model {
        [Column(DisplayExpr = true)] [Required] [StringLength(500)] public string Name { get; set; }
        [Column] [StringLength(int.MaxValue)] public string Description { get; set; }
    }
}
