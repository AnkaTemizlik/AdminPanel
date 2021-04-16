using DNA.Domain.Models.Pages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNA.Domain.Services {
    public interface IMenuService {
        Dictionary<string, Menu> Root { get; }
        void AddRoot(string root, Menu menu);
        void Configure(IConfiguration configuration);
        void Load(JObject menus);
    }
}
