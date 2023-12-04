using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using static Confluent.Kafka.ConfigPropertyNames;

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
        [HttpGet]
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
                consumer.Subscribe(_kafkaSettings.TopicJava);

                try
                {


                    while (true)
                    {
                        var result = consumer.Consume(TimeSpan.FromSeconds(3)); // 3 saniyelik zaman aşımı
                        if (result == null)
                        {
                            break; // Zaman aşımına ulaşıldığında döngüden çık.
                        }
                        sb.AppendLine($"Java: {result.Message.Value}");
                        consumer.Commit(result); // Her mesajı okuduktan sonra commit yap.
                    }

                    
                    consumer.Close();

                }
                catch (ConsumeException e)
                {
                    sb.AppendLine("Hata: " + e.Error.Reason);
                    return sb.ToString();
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
                    var result = await producer.ProduceAsync(_kafkaSettings.TopicAspNet, new Message<Null, string> { Value = message });
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
        public string TopicJava { get; set; }
        public string TopicAspNet { get; set; }
    }



}