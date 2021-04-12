using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DNA.Domain.Visitors {
    public class VisitorPool<T, TState> where TState : Enum {

        public event EventHandler<Exception> OnCanceled;
        protected IVisitManager<T, TState> Manager { get; set; }

        private List<Visitable<T, TState>> _Visitables = new List<Visitable<T, TState>>();

        public void Add(Visitable<T, TState> visitable) {
            _Visitables.Add(visitable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns>returns true if Canceled</returns>
        public async Task<bool> Accept(IVisitManager<T, TState> manager) {
            Manager = manager;
            foreach (var visitable in _Visitables) {
                await visitable.Accept(manager);
                if (visitable.Canceled) {
                    OnCanceled?.Invoke(visitable, visitable.Exception);
                    return true;
                }
            }
            return false;
        }
    }
}
