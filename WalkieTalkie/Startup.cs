using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OxfordSpeechClient;
using TTSService;

namespace WalkieTalkie
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
            ConfigureSpeechRecognitionService(services);
            ConfigureTTSService(services);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

        }

        private void ConfigureSpeechRecognitionService(IServiceCollection services)
        {
            var options = new CustomSpeechServiceOptions();
            Configuration.GetSection("OxfordCredentials").Bind(options);

            services.AddSingleton<ISpeechRecognitionService>(new SpeechRecognitionService(options));
        }

        private void ConfigureTTSService(IServiceCollection services)
        {
            var options = new TTSServiceOptions();
            Configuration.GetSection("TTSConfiguration").Bind(options);

            services.AddSingleton<ITextToSpeechService>(new TextToSpeechService(options));
        }

    }
}
