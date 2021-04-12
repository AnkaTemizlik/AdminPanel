using System;
using System.Collections.Generic;
using System.Text;

namespace DNA.Domain.Models.Pages {
    public class Menu {
        public string to { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool isHeaderVisible { get; set; }
        public bool areMenusVisible { get; set; }
        public bool noLink { get; set; }
        public List<Menu> menus { get; set; }
    }
}
