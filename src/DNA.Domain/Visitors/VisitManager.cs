using DNA.Domain.Services;
using DNA.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DNA.Domain.Exceptions;

namespace DNA.Domain.Visitors {

    public interface IVisitManager<T, TState> where TState : Enum {
        VisitorPool<T, TState> Pool { get; }
        T DataStore { get; }
        IConfiguration Configuration { get; }
        ILogger<VisitManager<T, TState>> Logger { get; }
        IProcessService ProcessService { get; }
        IEmailService EmailService { get; }
        IValuerService Valuer { get; }
        TState GetModelState();
        Task VisitAsync(Visitable<T, TState> visitable);
    }

    public abstract class VisitManager<T, TState> : IVisitManager<T, TState> where TState : Enum, IConvertible {
        public VisitorPool<T, TState> Pool { get; } = new VisitorPool<T, TState>();
        public T DataStore { get; set; }
        public IConfiguration Configuration { get; }
        public ILogger<VisitManager<T, TState>> Logger { get; }
        public IProcessService ProcessService { get; }
        public IEmailService EmailService { get; }
        public IValuerService Valuer { get; }

        public VisitManager(
            IConfiguration configuration,
            ILogger<VisitManager<T, TState>> logger,
            IProcessService processService,
            IEmailService emailService,
            IValuerService valuerService
            ) {

            Configuration = configuration;
            Logger = logger;
            ProcessService = processService;
            EmailService = emailService;
            Valuer = valuerService;
        }

        public abstract TState GetModelState();

        public async Task VisitAsync(Visitable<T, TState> visitable) {
            var isInputStateNone = visitable.InputState.ToInt32(null) == 0;
            var modelState = GetModelState();
            if (isInputStateNone || visitable.InputState.HasFlag(modelState)) {
                await visitable.DoImplAsync();
            }
            else {
                Logger.LogInformation(AlertCodes.VisitorAlreadyCompleted, ("Name", visitable.GetType().Name));
            }
        }
    }
}
