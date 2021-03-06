using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Api
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
            });

            services.AddAuthentication("Bearer").AddJwtBearer("Bearer",
                options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.Audience = "weatherapi";
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("delete-access", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "weather.delete");
                });

                options.AddPolicy("write-access", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "weather.write", "weather.delete");
                });

                options.AddPolicy("read-access", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "weather.read", "weather.write", "weather.delete");
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("https://localhost:5015")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("default");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
