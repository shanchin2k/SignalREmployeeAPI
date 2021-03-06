using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using SignalREmployee.APIHelper;
using SignalREmployee.Data;
using SignalREmployee.Hub;
using System;
using System.Collections.Generic;
using System.Globalization;

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
            IdentityModelEventSource.ShowPII = true;
            services.AddControllers(options => options.EnableEndpointRouting = false);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add valid issuer for OWIN-based login authority
            var AadAuthorityUrlSuffixSegment = "/" + Configuration["Authentication:AzureAdB2C:TenantId"] + "/" + "v2.0" + "/";
            Uri aadAuthorityUrl = new Uri(new Uri(Configuration["Authentication:AzureAdB2C:AadAuthorityUrl"]), AadAuthorityUrlSuffixSegment);
            var validIssuers = new List<string> { aadAuthorityUrl.ToString() };

            // Add valid issuer for b2clogin.com 
            //validIssuers.Add(string.Format(CultureInfo.InvariantCulture, ConfigHelper.AadB2CLoginAuthorityUrl.ToString(), ConfigHelper.TenantId));

            // Allow B2C SignIn policy based JWT authentication always
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             // Configure Default SignIn Authentication Scheme
             .AddJwtBearer(jwtOptions =>
             {
                 jwtOptions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                 {
                     // Accept only those tokens where the audience of the token is equal to the client ID of this application
                     ValidAudience = Configuration["Authentication:AzureAdB2C:ClientId"].ToString(), //this.appSettings.AAD.ClientId,

                     // Accept the default Sign-in Policy as defined
                     AuthenticationType = Configuration["Authentication:AzureAdB2C:SignUpSignInPolicyId"].ToString(),

                     // Accept the tokens only from valid issuers
                     ValidIssuers = validIssuers
                 };

                 // Set Authority and Audience to validate against JWT token
                 jwtOptions.Authority = $"" + string.Format(CultureInfo.InvariantCulture, Configuration["Authentication:AzureAdB2C:AadInstance"], Configuration["Authentication:AzureAdB2C:Tenant"], Configuration["Authentication:AzureAdB2C:SignUpSignInPolicyId"]);
                 jwtOptions.Audience = Configuration["Authentication:AzureAdB2C:ClientId"];

             });

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder => {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:4200", "http://192.168.1.239:4200");
            }));

            services.AddMvcCore(filter => filter.Filters.Add(new AuthenticationAttribute()));

            services.AddControllers(options => options.EnableEndpointRouting = false);

            services.AddSignalR();
            services.AddMvcCore();               

            services.AddSingleton<IDocumentDBRepository<Employee>>(new DocumentDBRepository<Employee>());
            //services.AddSingleton<INotificationBroadcaster, NotificationBroadcaster>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.  
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            
            
            app.UseAuthentication();
            
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseRouting();
            app.UseMvc();
            app.UseEndpoints(endpoints =>
            {                
                endpoints.MapHub<BroadcastHub>("/notify");
            });            


        }
    }
}