using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace DNA.Domain.Extentions {
    public static class ServiceCollectionExtensions {

        public static void AddClassesWithServiceAttribute(this IServiceCollection serviceCollection,
            params string[] assemblyFilters) {
            var assemblies = GetAssemblies(assemblyFilters);
            serviceCollection.AddClassesWithServiceAttribute(assemblies);
        }

        public static void AddClassesWithServiceAttribute(this IServiceCollection serviceCollection, params Assembly[] assemblies) {

            var typesWithAttributes = assemblies
                .Where(assembly => !assembly.IsDynamic)
                .SelectMany(GetExportedTypes)
                .Where(type => !type.IsAbstract)
                .Select(type => new {
                    type.GetCustomAttribute<ServiceAttribute>()?.Lifetime,
                    ServiceType = type,
                    ImplementationType = type.GetCustomAttribute<ServiceAttribute>()?.ServiceType
                })
                .Where(t => t.Lifetime != null);

            foreach (var type in typesWithAttributes) {
                //var find = serviceCollection.FirstOrDefault(_ => _.ServiceType.FullName == type.ServiceType.FullName);
                //if (find != null)
                //    continue;
                if (type.ImplementationType == null) {
                    serviceCollection.Add(type.ServiceType, type.Lifetime.Value);
                }
                else {
                    serviceCollection.Add(type.ImplementationType, type.ServiceType, type.Lifetime.Value);
                }
            }
        }

        private static void Add(this IServiceCollection serviceCollection, Type type, Lifetime lifetime) {
            switch (lifetime) {
                case Lifetime.Scoped:
                    serviceCollection.AddScoped(type);
                    break;
                case Lifetime.Singleton:
                    serviceCollection.AddSingleton(type);
                    break;
                case Lifetime.Transient:
                    serviceCollection.AddTransient(type);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }
        }

        private static void Add(this IServiceCollection serviceCollection,
            Type serviceType,
            Type implementationType,
            Lifetime lifetime) {
            switch (lifetime) {
                case Lifetime.Scoped:
                    serviceCollection.AddScoped(serviceType, implementationType);
                    break;
                case Lifetime.Singleton:
                    serviceCollection.AddSingleton(serviceType, implementationType);
                    break;
                case Lifetime.Transient:
                    serviceCollection.AddTransient(serviceType, implementationType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }
        }

        private static Assembly[] GetAssemblies(IEnumerable<string> assemblyFilters) {
            var assemblies = new List<Assembly>();

            foreach (var assemblyFilter in assemblyFilters) {
                var domain = AppDomain.CurrentDomain.GetAssemblies();
                assemblies.AddRange(domain
                    .Where(assembly => IsWildcardMatch(assembly.GetName().Name, assemblyFilter))
                    .ToArray());
            }

            return assemblies.ToArray();
        }

        private static IEnumerable<Type> GetTypesImplementing(Type implementsType,
        IEnumerable<Assembly> assemblies,
        params string[] classFilter) {
            var types = GetTypesImplementing(implementsType, assemblies.ToArray());

            if (classFilter != null && classFilter.Any()) {
                types = types.Where(type => classFilter.Any(filter => IsWildcardMatch(type.FullName, filter)));
            }

            return types;
        }

        private static IEnumerable<Type> GetTypesImplementing(Type implementsType, params Assembly[] assemblies) {
            if (assemblies == null || assemblies.Length == 0) {
                return new Type[0];
            }

            var targetType = implementsType;

            return assemblies
                .Where(assembly => !assembly.IsDynamic)
                .SelectMany(GetExportedTypes)
                .Where(type => !type.IsAbstract
                  && !type.IsGenericTypeDefinition
                  && targetType.IsAssignableFrom(type))
                .ToArray();
        }

        public static IEnumerable<Type> GetExportedTypes(this IServiceCollection serviceCollection, Assembly assembly) {
            return GetExportedTypes(assembly);
        }

        private static IEnumerable<Type> GetExportedTypes(Assembly assembly) {
            try {
                return assembly.GetExportedTypes();
            }
            catch (NotSupportedException) {
                // A type load exception would typically happen on an Anonymously Hosted DynamicMethods
                // Assembly and it would be safe to skip this exception.
                return Type.EmptyTypes;
            }
            catch (FileLoadException) {
                // The assembly points to a not found assembly - ignore and continue
                return Type.EmptyTypes;
            }
            catch (ReflectionTypeLoadException ex) {
                // Return the types that could be loaded. Types can contain null values.
                return ex.Types.Where(type => type != null);
            }
            catch (Exception ex) {
                // Throw a more descriptive message containing the name of the assembly.
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "Unable to load types from assembly {0}. {1}", assembly.FullName, ex.Message), ex);
            }
        }

        private static bool IsWildcardMatch(string assemblyName, string wildcardStringToGetAssembly) {
            return assemblyName == wildcardStringToGetAssembly
                   || Regex.IsMatch(assemblyName, "^" + Regex.Escape(wildcardStringToGetAssembly)
                       .Replace("\\*", ".*") + "$", RegexOptions.IgnoreCase);
        }
    }
}
