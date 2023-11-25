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
                GroupId = "test-consumer-group",
                BootstrapServers = _kafkaSettings.BootstrapServers,
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

            //using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            //{
            //    consumer.Subscribe(_kafkaSettings.TopicName);
            //    var cancellationToken = new CancellationTokenSource();

            //    try
            //    {
            //        while (!cancellationToken.IsCancellationRequested)
            //        {
            //            var consumeResult = consumer.Consume(cancellationToken.Token);
            //            sb.Append(consumeResult.Message.Value);
            //        }
            //    }
            //    catch (OperationCanceledException)
            //    {
            //        consumer.Close();
            //    }
            //}

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