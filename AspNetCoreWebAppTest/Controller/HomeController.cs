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
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(_kafkaSettings.TopicName);

                try
                {
                    //// Belirli bir sayıda mesajı oku veya belirli bir süre boyunca mesajları oku
                    //for (int i = 0; i < consumer.; i++) // Örneğin, en fazla 100 mesaj oku
                    //{
                        var result = consumer.Consume();
                        
                        if (result == null)
                            return sb.ToString();

                        sb.AppendLine($"Partition: {result.Partition} Mesaj: { result.Message.Value}");
                        consumer.Commit();
                    //}
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