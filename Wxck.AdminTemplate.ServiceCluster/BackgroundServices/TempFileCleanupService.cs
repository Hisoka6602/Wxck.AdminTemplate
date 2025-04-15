using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wxck.AdminTemplate.Domain.Attributes;

namespace Wxck.AdminTemplate.ServiceCluster.BackgroundServices {

    [HostedService]
    public class TempFileCleanupService : BackgroundService {
        private readonly ILogger<TempFileCleanupService> _logger;
        private readonly string _tempFolderPath;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1); // 每小时清理一次
        private readonly TimeSpan _fileLifetime = TimeSpan.FromHours(6);   // 文件存活时间

        public TempFileCleanupService(IWebHostEnvironment env, ILogger<TempFileCleanupService> logger) {
            _logger = logger;
            _tempFolderPath = Path.Combine(env.WebRootPath, "Temp"); // 指向 wwwroot/Temp
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                try {
                    if (Directory.Exists(_tempFolderPath)) {
                        var files = Directory.GetFiles(_tempFolderPath);
                        foreach (var file in files) {
                            try {
                                var fileInfo = new FileInfo(file);
                                if (DateTime.UtcNow - fileInfo.LastWriteTimeUtc > _fileLifetime) {
                                    fileInfo.Delete();
                                }
                            }
                            catch (Exception ex) {
                                _logger.LogWarning(ex, $"删除文件失败: {file}");
                            }
                        }
                    }
                    else {
                        Directory.CreateDirectory(_tempFolderPath);
                    }
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "清理临时文件错误");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }
    }
}