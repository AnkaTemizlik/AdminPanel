using System;

namespace DNA.Domain.Extentions {
    public enum Lifetime {
        Transient,
        Singleton,
        Scoped
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceAttribute : Attribute {
        public Lifetime Lifetime { get; set; }
        public Type ServiceType { get; set; }

        public ServiceAttribute(Type serviceType, Lifetime lifetime) {
            this.ServiceType = serviceType;
            this.Lifetime = lifetime;
        }
    }
}