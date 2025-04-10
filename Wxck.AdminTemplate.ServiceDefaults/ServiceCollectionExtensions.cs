using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Wxck.AdminTemplate.Domain.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Wxck.AdminTemplate.ServiceDefaults {

    public static class ServiceCollectionExtensions {

        public static IServiceCollection AddRepositories(this IServiceCollection services) {
            var assemblies = LoadDllAssemblies("Infrastructure");
            foreach (var assembly in assemblies) {
                var typesWithAttribute = assembly.GetTypes()
                    .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetCustomAttribute<InjectableRepositoryAttribute>() != null);

                foreach (var implType in typesWithAttribute) {
                    var attr = implType.GetCustomAttribute<InjectableRepositoryAttribute>()!;
                    var serviceType = attr.ServiceType ?? implType.GetInterfaces().FirstOrDefault();

                    if (serviceType == null) continue;

                    services.AddSingleton(serviceType, implType);
                }
            }

            return services;
        }

        private static List<Assembly> LoadDllAssemblies(string dllFileName) {
            var loadedAssemblies = new List<Assembly>();
            var basePath = AppContext.BaseDirectory;

            var dllFiles = Directory.GetFiles(basePath, "*.dll")
                .Where(f => Path.GetFileName(f).Contains(dllFileName, StringComparison.OrdinalIgnoreCase));

            foreach (var dll in dllFiles) {
                try {
                    var assemblyName = AssemblyName.GetAssemblyName(dll);

                    if (AppDomain.CurrentDomain.GetAssemblies().All(a => a.GetName().Name != assemblyName.Name)) {
                        var assembly = Assembly.Load(assemblyName);
                        loadedAssemblies.Add(assembly);
                    }
                    else {
                        loadedAssemblies.Add(AppDomain.CurrentDomain
                            .GetAssemblies()
                            .First(a => a.GetName().Name == assemblyName.Name));
                    }
                }
                catch {
                    // 忽略加载失败
                }
            }

            return loadedAssemblies.Distinct().ToList();
        }

        public static IServiceCollection AddApplicationService(this IServiceCollection services) {
            var assemblies = LoadDllAssemblies("Application");
            foreach (var assembly in assemblies) {
                var typesWithAttribute = assembly.GetTypes()
                    .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetCustomAttribute<InjectableServiceAttribute>() != null);

                foreach (var implType in typesWithAttribute) {
                    var attr = implType.GetCustomAttribute<InjectableServiceAttribute>()!;
                    var serviceType = attr.ServiceType ?? implType.GetInterfaces().FirstOrDefault();

                    if (serviceType == null) continue;

                    services.AddSingleton(serviceType, implType);
                }
            }

            return services;
        }
    }
}