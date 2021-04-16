using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNA.Domain.Models.Pages {

    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Menu {
        public string to { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public string target { get; set; }
        public string color { get; set; }
        public bool? isDivider { get; set; }
        public bool? showOnAuth { get; set; }
        public string description { get; set; }
        public bool? isHeaderVisible { get; set; }
        public bool? areMenusVisible { get; set; }
        public bool? noLink { get; set; }
        public string[] roles { get; set; }
        public MenuCollection menus { get; set; } = new MenuCollection();
    }

    public class MenuCollection : List<Menu> {

    }
}
