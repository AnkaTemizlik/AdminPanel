using AutoMapper;
using DNA.API.Models;
using PointmentApp.Models;
using PointmentApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Mapping {
    public class AutoMappingProfile : Profile {
        public AutoMappingProfile() {
            CreateMap<AppointmentResource, Appointment>();
        }
    }
}

