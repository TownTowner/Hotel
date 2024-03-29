using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Hotel.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hotel.Configuration;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using Prometheus;

namespace Hotel
{
    public class Startup
    {
        private const string HealthCheckRoute = "/health";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                jwt =>
                {
                    var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);
                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true, //这将使用我们在 appsettings 中添加的 secret 来验证 JWT token 的第三部分，并验证 JWT token 是由我们生成的
                        IssuerSigningKey = new SymmetricSecurityKey(key), //将密钥添加到我们的 JWT 加密算法中
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        RequireExpirationTime = false
                    };
                });
            //options => Configuration.Bind("JwtSettings", options)
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
            //    options => Configuration.Bind("CookieSettings", options))

            services.AddCors(option => option.AddPolicy("cors", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(new[] { "http://localhost:3000" })));

            services.AddDbContext<HotelDbContext>(options => options.UseInMemoryDatabase("Hilton"));
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddScoped(sp =>
            {
                var dbContext = sp.GetRequiredService<HotelDbContext>();
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var userId = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                return new UserService(dbContext, userId);
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddHealthChecks();

            services.AddHotelConsulServiceRegistration(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //env.IsProduction(); //是否生产环境, ASPNETCORE_ENVIRONMENT 环境变量值是否是 Production
            //env.IsEnvironment("ok");//ASPNETCORE_ENVIRONMENT 环境变量值是否是ok
            //env.IsStaging();//是否测试环境

            //是否开发环境, ASPNETCORE_ENVIRONMENT 环境变量值是否是 Development
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseCors("cors");

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            //prometheus
            app.UseHttpMetrics()
                .UseMetricServer();
            //prometheus self-counter defination
            app.UseMetricPathCounter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapMetrics();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHealthChecks(HealthCheckRoute);
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
