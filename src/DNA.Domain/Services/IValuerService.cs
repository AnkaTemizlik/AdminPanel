using DNA.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DNA.Domain.Services {
    public interface IValuerService {
        void SetCurrentModel(params object[] models);
        TValue GetByType<TValue>(string val);
        string Get(string source);
        string GetByObject(object model, string source);
    }
}
