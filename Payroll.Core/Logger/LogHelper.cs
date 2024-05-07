using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Payroll.Core.Logger
{
    public static partial class LogHelper
    {
        public static ILogger Logger { set; get; }

#pragma warning disable CS1998
        public static async ValueTask LoggerError<T>(string method, T input, Exception ex)
        {
            var mesBuilder = new StringBuilder();
            mesBuilder.AppendLine($"[{method}]");
            if (input != null)
                mesBuilder.AppendLine($"---[INPUT]: {JsonConvert.SerializeObject(input)}");
            mesBuilder.AppendLine($"---[MESSAGE]: {ex.Message}");
            mesBuilder.AppendLine($"---[STACKTRACE]: {ex.StackTrace}");

            Logger.LogError(mesBuilder.ToString());
        }
        public static async ValueTask LoggerCritical<T>(string method, T input, Exception ex)
        {
            var mesBuilder = new StringBuilder();
            mesBuilder.AppendLine($"[{method}]");
            if (input != null)
                mesBuilder.AppendLine($"---[INPUT]: {JsonConvert.SerializeObject(input)}");
            mesBuilder.AppendLine($"---[MESSAGE]: {ex.Message}");
            mesBuilder.AppendLine($"---[STACKTRACE]: {ex.StackTrace}");

            Logger.LogCritical(mesBuilder.ToString());
        }
        public static async ValueTask LoggerInfo<T>(string method, T input, string mes)
        {
            var mesBuilder = new StringBuilder();
            mesBuilder.AppendLine($"[{method}]");
            if (input != null)
                mesBuilder.AppendLine($"---[INPUT]: {JsonConvert.SerializeObject(input)}");
            mesBuilder.AppendLine($"---[MESSAGE]: {mes}");
            Logger.LogInformation(mesBuilder.ToString());
        }
#pragma warning restore CS1998
    }
}
