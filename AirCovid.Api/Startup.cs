using System;
using System.IO;
using System.Reflection;
using AirCovid.Api.Infra;
using AirCovid.Api.Services.Commands;
using AirCovid.Api.Services.Commands.Validation;
using AirCovid.Data;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace AirCovid.Api
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
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {

                    options.ClientErrorMapping[StatusCodes.Status400BadRequest].Link = "https://httpstatuses.com/400";
                    options.ClientErrorMapping[StatusCodes.Status404NotFound].Link = "https://httpstatuses.com/404";
                    options.ClientErrorMapping[StatusCodes.Status422UnprocessableEntity].Link = "https://httpstatuses.com/422";
                    options.ClientErrorMapping[StatusCodes.Status500InternalServerError].Link = "https://httpstatuses.com/500";
                });

            services.AddSwaggerGen(options =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                options.ExampleFilters();
            });
            services.AddSwaggerExamplesFromAssemblyOf<Startup>();

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddScoped<ICheckInRepository, CheckInRepository>();
            services.AddScoped<IFlightRepository, FlightRepository>();
            services.AddScoped<IPassengerRepository, PassengerRepository>();
            services.AddScoped<ICreateCheckInCommandValidator, CreateCheckInCommandValidator>();
            services.AddSingleton<ITimedAsyncLockFactory, TimedAsyncLockFactory>();

            services.AddProblemDetails();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();

            app.UseProblemDetails();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "AirCovid CheckIn Api"); });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}