using Dapper.Contrib.Extensions;
using DNA.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Models {

    /// <summary>
    /// https://github.com/DevExpress-Examples/DataGrid---How-to-use-FileUploader-in-an-edit-form/blob/20.2.5%2B/React/src/App.js
    /// </summary>
    [Table("{TablePrefix}Document")]
    public class Document : Model {
        [Column] public int AppointmentId { get; set; }
        [Column] [Required] [StringLength(500)] public string Name { get; set; }
        [Column] [Required] public double Size { get; set; }
        [Column] public int? Width { get; set; }
        [Column] public int? Height { get; set; }
        [Column] public DateTime? LastModifiedDate { get; set; }
        [Column] [Required] [StringLength(16)] public string FileType { get; set; }
        [Column] [Required] [StringLength(1000)] public string Url { get; set; }
        [Column] [Required] [StringLength(1000)] public string ThumbnailUrl { get; set; }
    }
}
