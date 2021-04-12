using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace DNA.API.Infrastructure {
    public static class Errors {

        public static ModelStateDictionary AddErrorToModelState(string code, string description, ModelStateDictionary modelState) {
            modelState.TryAddModelError(code, description);
            return modelState;
        }
        public static ModelStateDictionary AddErrorToModelState(Exception ex, ModelStateDictionary modelState) {
            modelState.TryAddModelError(ex.GetType().ToString(), ex.Message);
            return modelState;
        }
    }
}
