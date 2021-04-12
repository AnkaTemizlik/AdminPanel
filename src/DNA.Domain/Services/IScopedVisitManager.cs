using DNA.Domain.Services.Communication;
using System;
using System.Threading.Tasks;

namespace DNA.Domain.Services {
    public interface IScopedVisitManager {

        event EventHandler<Response> ResponseHandler;
        string VisitorName { get; }
        Task DoWork<T>(T data);
    }
}
