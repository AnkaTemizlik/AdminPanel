using DNA.Domain.Extentions;
using DNA.Domain.Models.Pages;
using DNA.Domain.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Services {

    [Service(typeof(IMenuService), Lifetime.Singleton)]
    public class MenuService : IMenuService {

        private readonly Dictionary<string, Menu> _menus;

        public Dictionary<string, Menu> Root { get { return _menus; } }

        public MenuService() {
            _menus = new Dictionary<string, Menu>();
        }

        public void Load(JObject menus) {
            _menus.Clear();
            AddRoot("home", menus["home"].ToObject<Menu>());
            AddRoot("panel", menus["panel"].ToObject<Menu>());
            AddRoot("user", menus["user"].ToObject<Menu>());
            AddRoot("contact", menus["contact"].ToObject<Menu>());
            AddRoot("main", menus["main"].ToObject<Menu>());
            AddRoot("social", menus["social"].ToObject<Menu>());
            AddRoot("links", menus["links"].ToObject<Menu>());
            AddRoot("admin", menus["admin"].ToObject<Menu>());
            AddRoot("help", menus["help"].ToObject<Menu>());
        }

        public void Configure(IConfiguration configuration) {

            // Social Links
            var section = configuration.GetSection("Config:SocialMediaLinks");
            _menus["social"].menus.ForEach(menu => {
                if (string.IsNullOrWhiteSpace(section[menu.name ?? menu.label]))
                    menu.visible = false;
                else
                    menu.to = section[menu.name ?? menu.label] ?? "";
            });

            // 
        }

        public void AddRoot(string root, Menu menu) {
            _menus.Add(root, menu);
        }

    }
}
