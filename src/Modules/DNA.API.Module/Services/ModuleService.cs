using DNA.Domain.Extentions;
using DNA.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Module.Services {

    public interface IModuleService {

    }

    [Service(typeof(IModuleService), Lifetime.Scoped)]
    public class ModuleService : IModuleService {

        private readonly ILogger<ModuleService> _logger;
        private readonly IProcessService _processService;

        public ModuleService(ILogger<ModuleService> logger, IProcessService processService) {
            _logger = logger;
            _processService = processService;
        }
    }
}
