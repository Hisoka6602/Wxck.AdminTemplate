using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Wxck.AdminTemplate.Domain.Attributes;
using Wxck.AdminTemplate.CrossCutting.Utils;
using Wxck.AdminTemplate.Domain.Repositories.Logs;

namespace Wxck.AdminTemplate.ServiceCluster.BackgroundServices {

    [HostedService]
    public class LogsCleanupService : Microsoft.Extensions.Hosting.BackgroundService {
        private readonly IOperationLogRepository _operationLogRepository;
        private readonly ILogger<LogsCleanupService> _logger;
        private DateTime _lastDeleteTime = DateTime.MinValue;

        public LogsCleanupService(IOperationLogRepository operationLogRepository,
            ILogger<LogsCleanupService> logger) {
            _operationLogRepository = operationLogRepository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                if (DateTime.Now.Subtract(_lastDeleteTime).TotalHours > 10) {
                    var addMonths = DateTime.Now.AddMonths(-3);

                    var deleteOperationLogs = _operationLogRepository.DeleteLogsBeforeDate(addMonths, stoppingToken);

                    var results = await Task.WhenAll(deleteOperationLogs);

                    if (!results[0].Key) {
                        _logger.LogError($"删除日志失败:{results[0].Value}");
                    }

                    _lastDeleteTime = DateTime.Now;
                }
                //删除本地日志文件
                var logsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                if (Directory.Exists(logsFolderPath)) {
                    // 匹配日期命名的.log文件
                    var regex = new Regex(@"^\d{4}-\d{2}-\d{2}\.log$");
                    // 调用递归方法来处理logs文件夹及其所有子文件夹
                    var logFiles = FileUtils.GetLogFiles(logsFolderPath, regex);
                    foreach (var file in from file in logFiles
                                         let creationTime = File.GetCreationTime(file)
                                         let difference =
                                             DateTime.Now - creationTime
                                         where difference.TotalDays > 3
                                         select file) {
                        File.Delete(file);
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}