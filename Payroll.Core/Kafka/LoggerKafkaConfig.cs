using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Core.Kafka
{
    public class LoggerKafkaConfig
    {
        public string Name { get; set; }
        public int BatchSizeLimit { get; set; }
        public int Period { get; set; }
        public string Brokers { get; set; }
        public string Topic { get; set; }
    }
}
