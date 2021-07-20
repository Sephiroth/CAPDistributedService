using CAPService.Impl;
using CAPService.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;

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

            // ע��DbContext
            string mysqlConnection = Configuration["MySqlConnection"];
            CAPService.Model.Cap_test_dbContext.ConnectionStr = mysqlConnection;
            services.AddDbContext<CAPService.Model.Cap_test_dbContext>(op =>
            {
                op.UseMySql(mysqlConnection, ServerVersion.Parse("8.0.21-mysql"));
            });
            
            var rabbitOption = CAPDistributedService.Configuration.RabbitMQConf.GetConf(Configuration, "RabbitMQ");
            services.AddCap(x =>
            {
                //�����ʹ�õ� EF �������ݲ���������Ҫ����������ã�
                //x.UseEntityFramework<CAPService.Model.Cap_test_dbContext>();
                x.UseMySql(mysqlConnection);

                //CAP֧�� RabbitMQ��Kafka��AzureServiceBus��AmazonSQS ����ΪMQ������ʹ��ѡ�����ã�
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
                //// ע��ڵ㵽 Consul
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
