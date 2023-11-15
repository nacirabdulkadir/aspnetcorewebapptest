using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;

namespace AspNetCoreWebAppTest
{
    public class HomeController : Controller
    {

        private readonly KafkaSettings _kafkaSettings;

        public HomeController(IOptions<KafkaSettings> kafkaSettings)
        {
            _kafkaSettings = kafkaSettings.Value;
        }
        public IActionResult Index()
        {
            return View();
        }   

        public string ConsumeKafkaMessage()
        {
            StringBuilder sb = new StringBuilder();

            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers,
                GroupId = "test-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(_kafkaSettings.TopicName);
                try
                {
                    var result = consumer.Consume();
                    sb.Append(result.Message.Value);
                }
                catch (ConsumeException e)
                {
                    sb.Append("hata: " + e.Error.Reason);
                }
            }

            return sb.ToString();
        }
    }

    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
        public string TopicName { get; set; }
    }
}
