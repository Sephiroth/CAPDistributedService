using CAPService.Impl;
using CAPService.Interface;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CAPDistributedService
{
    public class Startup
    {
        public Startup()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient<IUserSubscriberService, UserSubscriberService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CAPDistributedService", Version = "v1" });
            });

            // 注入DbContext
            string mysqlConnection = Configuration["MySqlConnection"];
            CAPService.Model.Cap_test_dbContext.ConnectionStr = mysqlConnection;
            services.AddDbContextPool<CAPService.Model.Cap_test_dbContext>(op =>
            {
                op.UseMySql(mysqlConnection, ServerVersion.Parse("8.0.21-mysql"));
            }, 28);

            var rabbitOption = CAPDistributedService.Configuration.RabbitMQConf.GetConf(Configuration, "RabbitMQ");
            services.AddCap(x =>
            {
                //如果你使用的 EF 进行数据操作，你需要添加如下配置：
                x.UseEntityFramework<CAPService.Model.Cap_test_dbContext>();
                //CAP支持 RabbitMQ、Kafka、AzureServiceBus、AmazonSQS 等作为MQ，根据使用选择配置：
                x.UseRabbitMQ(options =>
                {
                    options.HostName = rabbitOption.HostName;
                    options.Port = rabbitOption.Port;
                    options.VirtualHost = rabbitOption.VirtualHost;
                    options.UserName = rabbitOption.UserName;
                    options.Password = rabbitOption.Password;
                    options.ExchangeName = rabbitOption.ExchangeName;
                });

                //x.UseDashboard();
                //// 注册节点到 Consul
                //x.UseDiscovery(d =>
                //{
                //    d.DiscoveryServerHostName = "localhost";
                //    d.DiscoveryServerPort = 8500;
                //    d.CurrentNodeHostName = "localhost";
                //    d.CurrentNodePort = 5800;
                //    d.NodeId = 1;
                //    d.NodeName = "CAP No.1 Node";
                //});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CAPDistributedService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            //app.UseCapDashboard();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
