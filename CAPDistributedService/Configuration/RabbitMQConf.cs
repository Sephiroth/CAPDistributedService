using System.Linq;

namespace CAPDistributedService.Configuration
{
    public class RabbitMQConf
    {
        public static DotNetCore.CAP.RabbitMQOptions GetConf(Microsoft.Extensions.Configuration.IConfiguration configuration, string key)
        {
            return new DotNetCore.CAP.RabbitMQOptions
            {
                HostName = configuration[$"{key}:HostName"],
                Port = System.Convert.ToUInt16(configuration[$"{key}:Port"] as string),
                VirtualHost = configuration[$"{key}:VirtualHost"],
                UserName = configuration[$"{key}:UserName"],
                Password = configuration[$"{key}:Password"],
                ExchangeName = configuration[$"{key}:ExchangeName"]
            };
        }
    }
}