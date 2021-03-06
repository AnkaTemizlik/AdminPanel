using DNA.Domain.Exceptions;
using DNA.Domain.Models;

namespace PointmentApp.Exceptions {
    public class PluginAlertCodes : AlertCodes {
        public static KeyValue DoWorkInfo = new KeyValue(nameof(DoWorkInfo));
        public static KeyValue DoWorkException = new KeyValue(951, nameof(DoWorkException));
    }
}
