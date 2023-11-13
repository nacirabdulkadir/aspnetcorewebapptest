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
            var conf = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = _kafkaSettings.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            StringBuilder sb = new StringBuilder();
            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                var topicName = _kafkaSettings.TopicName;
                c.Subscribe(topicName);

                try
                {
                    var cr = c.Consume();
                    // Mesajı işle, örneğin: Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                    sb.Append(cr.Message);
                }
                catch (ConsumeException e)
                {
                    // Tüketim sırasında hata yönetimi
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
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
