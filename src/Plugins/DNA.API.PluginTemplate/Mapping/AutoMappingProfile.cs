using AutoMapper;
using DNA.API.Models;
using DNA.API.PluginTemplate.Models;
using DNA.API.PluginTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.PluginTemplate.Mapping {
    public class AutoMappingProfile : Profile {
        public AutoMappingProfile() {
            CreateMap<InvoiceResource, Invoice>();
        }
    }
}

