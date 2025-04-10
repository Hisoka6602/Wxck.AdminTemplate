﻿namespace Wxck.AdminTemplate.Api.Warmup {

    /// <summary>
    /// 预热启动过滤器
    /// </summary>
    public class WarmupStartupFilter : IStartupFilter {
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceCollection _serviceCollection;

        public WarmupStartupFilter(IServiceProvider serviceProvider, IServiceCollection serviceCollection) {
            _serviceProvider = serviceProvider;
            _serviceCollection = serviceCollection;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) {
            return app => {
                // 预热逻辑
                WarmupMemoryCacheRepositories().GetAwaiter().GetResult();
                next(app);
            };
        }

        private async Task WarmupMemoryCacheRepositories() {
            foreach (var descriptor in _serviceCollection) {
                if (descriptor.ServiceType.Name.Contains("Repository")) {
                    try {
                        // 解析服务实例
                        var repository = _serviceProvider.GetRequiredService(descriptor.ServiceType);

                        // 获取 MemoryCacheData 方法
                        var method = repository.GetType().GetMethod("MemoryCacheData");

                        // 如果方法存在并返回一个 Task
                        if (method != null && method.Invoke(repository, null) is Task task) {
                            await task;
                        }
                    }
                    catch (Exception ex) {
                        NLog.LogManager.GetCurrentClassLogger().Error(
                            $"Error warming up repository {descriptor.ServiceType.FullName}: {ex.Message}");
                    }
                }
            }
        }
    }
}