﻿using System;
using Hangfire.Annotations;
using Hangfire.States;

namespace DNA.API.BackgroundServices.Hangfire {
    internal class CustomBackgroundJobStateChanger : IBackgroundJobStateChanger {
        private readonly IBackgroundJobStateChanger _inner;

        public CustomBackgroundJobStateChanger([NotNull] IBackgroundJobStateChanger inner) {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public IState ChangeState(StateChangeContext context) {
            Console.WriteLine($"ChangeState {context.BackgroundJobId} to {context.NewState}");
            return _inner.ChangeState(context);
        }
    }
}
