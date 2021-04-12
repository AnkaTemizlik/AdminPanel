using DNA.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNA.Domain.Visitors {

    public abstract class Visitable<T, TState> where TState : Enum {
        public bool Canceled { get; private set; }
        public Alert Exception { get; private set; }
        public virtual TState InputState { get; set; }
        public virtual TState OutputState { get; set; }
        public IVisitManager<T, TState> Manager { get; private set; }
        public Visitable() {

        }

        public Visitable(TState outputState) {
            OutputState = outputState;
        }

        public Visitable(TState inputState, TState outputState) {
            InputState = inputState;
            OutputState = outputState;
        }

        public async Task Accept(IVisitManager<T, TState> manager) {
            Manager = manager;
            Exception = null;
            Canceled = false;
            await manager.VisitAsync(this);
        }

        public virtual void Cancel() {
            Canceled = true;
        }

        public virtual void Cancel(Alert ex) {
            Canceled = true;
            Exception = ex;
        }

        [Obsolete("parametre vermeden çalışması sağlandı. direct DoAsync() çağırılabilir. Visitorlerden temizlense yeterli.")]
        public async virtual Task DoAsync(IVisitManager<T, TState> manager) { await DoAsync(); }

        public async Task DoImplAsync() {
            await DoAsync();
        }

        public async virtual Task DoAsync() { await Task.CompletedTask; }
    }
}
