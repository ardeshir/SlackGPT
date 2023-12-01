// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.17.1

using System;
using System.Data;
using System.Net.Http;

using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.ApplicationInsights;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;

// using CDSAIServices.Services.OpenAIService;
using CDSAIServices.Dialogs.OpenAI;
using CDSAIServices.Services;
using CDSAIServices.Domain;
using CDSAIServices.Dialogs;
using Microsoft.Bot.Connector.Authentication;

namespace CDSAIServices
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient().AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.MaxDepth = HttpHelper.BotMessageSerializerSettings.MaxDepth;
            });

            services.AddHttpClient(nameof(OpenAIService), httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://<ADD YOUR FUNCTION NAME HERE>azurewebsites.net/api/");
                httpClient.Timeout = TimeSpan.FromSeconds(300);
                httpClient.DefaultRequestHeaders.Add("x-functions-key", "<ADD YOUR FUNCTION KEY HERE>");
            }).AddPolicyHandler(GetRetryPolicy());

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();


            services.AddSingleton<IStorage, MemoryStorage>();

            services.AddSingleton<UserState>();
            services.AddSingleton<ConversationState>();

            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IOpenAIService, OpenAIService>();

            //services.AddSingleton<MainDialog>();
            services.AddSingleton<OpenAIDialog>();

            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, Bots.CDSAIServices<OpenAIDialog>>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
