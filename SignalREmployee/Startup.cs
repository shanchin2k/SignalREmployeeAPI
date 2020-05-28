using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalREmployee.Data;

namespace SignalREmployee
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
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder => {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:4200");
            }));

            services.AddControllers(options => options.EnableEndpointRouting = false);

            services.AddSignalR();
            services.AddMvcCore();
            //services.AddMvc();//.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<IDocumentDBRepository<Employee>>(new DocumentDBRepository<Employee>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.  
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");
            
            app.UseRouting();
            app.UseMvc();
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
                endpoints.MapHub<BroadcastHub>("/notify");
            });

            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<BroadcastHub>("/notify");
            //});

            
        }
    }
}