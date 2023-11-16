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

            var conf = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = _kafkaSettings.BootstrapServers,
                // Otomatik offset commit'i devre dışı bırakmak için:
                EnableAutoCommit = false,
                // Eğer son tüketilmemiş mesajlardan başlamak isterseniz:
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                c.Subscribe(_kafkaSettings.TopicName);

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true;
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            sb.Append($"'{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                            
                        }
                        catch (ConsumeException e)
                        {
                            sb.Append($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Consumer'ı kapatmak için:
                    c.Close();
                }
            }

            return sb.ToString();
        }


        [HttpPost]
  
        public async Task<IActionResult> ProducerKafkaMessage(string message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var result = await producer.ProduceAsync(_kafkaSettings.TopicName, new Message<Null, string> { Value = message });
                    return Ok($"Mesaj gönderildi: '{result.Value}' Topic: '{result.Topic}', Partition: '{result.Partition}', Offset: '{result.Offset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    return BadRequest($"Mesaj gönderilirken hata oluştu: {e.Error.Reason}");
                }
            }


        }


    }

    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
        public string TopicName { get; set; }
    }

}