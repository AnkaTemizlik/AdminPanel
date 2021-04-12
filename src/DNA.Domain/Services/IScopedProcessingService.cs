using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DNA.Domain.Services {
    public interface IScopedProcessingService {
        string ProcessName { get; }
        Task DoWork(CancellationToken stoppingToken);
        Task Init();
    }
}
