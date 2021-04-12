using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNA.Domain.Models {
    [Table("{TablePrefix}NOTIFICATION")]
    public class Notification : Model {
        [Column] public bool IsRead { get; set; }
        [Column] public int UserId { get; set; }
        [Column] public int NotificationType { get; set; }
        [Column] public string EntityKey { get; set; }
        [Column] public string Title { get; set; }
        [Column] public string Description { get; set; }
        [Column] public string Comment { get; set; }
        [Column] public string Url { get; set; }
        [Column] public string Target { get; set; } = "_self";
    }
}
