using DNA.Domain.Services.Communication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNA.Domain.Services {
    public interface ITranslationService {
        string T(string key);
        //Response T(Response response);
    }
}
