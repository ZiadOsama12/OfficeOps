using AspNetCoreRateLimit;
using CompanyEmployees.Presentation.ActionFilters;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using NLog;
using Service.DataShaping;
using Shared.DataTransferObjects;
using System.Reflection.Metadata;
using Ultimate_WebAPI.Extensions;


namespace Ultimate_WebAPI;
public class Program
{
    public static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
        new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
        .Services.BuildServiceProvider()
        .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
        .OfType<NewtonsoftJsonPatchInputFormatter>().First();

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        LogManager.Setup().LoadConfigurationFromFile("nlog.config");

        builder.Services.ConfigureCors();// added
        builder.Services.ConfigureIISIntegration();// added
        builder.Services.ConfigureLoggerService(); // added
        builder.Services.ConfigureRepositoryManager(); // added
        builder.Services.ConfigureServiceManager(); // added
        builder.Services.ConfigureSqlContext(builder.Configuration);
        builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddCustomMediaTypes();
        builder.Services.ConfigureVersioning();

        builder.Services.ConfigureResponseCaching(); // Adding Cache Store
        builder.Services.ConfigureHttpCacheHeaders();

        builder.Services.AddMemoryCache(); // for rate limiting.
        builder.Services.ConfigureRateLimitingOptions();
        builder.Services.AddHttpContextAccessor();


        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        builder.Services.AddScoped<ValidationFilterAttribute>();
        builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();
        builder.Services.AddScoped<ValidateMediaTypeAttribute>();

        builder.Services.AddControllers(config =>
        {
            config.RespectBrowserAcceptHeader = true;
            config.ReturnHttpNotAcceptable = true;
            config.InputFormatters.Insert(0, GetJsonPatchInputFormatter()); //The default input formatters might not support JSON Patch, so we explicitly insert it at index 0, ensuring it takes priority.
            config.CacheProfiles.Add("120SecondsDuration", new CacheProfile
            {
                Duration = 120
            });

        }).AddXmlDataContractSerializerFormatters().AddCustomCSVFormatter()
        .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        var logger = app.Services.GetRequiredService<ILoggerManager>(); // added new
        app.ConfigureExceptionHandler(logger);
        if (app.Environment.IsProduction())
            app.UseHsts();

        //if (app.Environment.IsDevelopment()) // added old
        //    app.UseDeveloperExceptionPage();
        //else app.UseHsts();
        
  
        app.UseHttpsRedirection(); // middlewares

        app.UseStaticFiles();// add
        app.UseForwardedHeaders(new ForwardedHeadersOptions //add
        {
            ForwardedHeaders = ForwardedHeaders.All
        });

        app.UseIpRateLimiting();
        app.UseCors("CorsPolicy"); // added
        app.UseResponseCaching(); // for cache store
        app.UseHttpCacheHeaders(); // better method for caching


        app.UseAuthorization();

        app.Use(async (context, next) =>
        {
            Console.WriteLine($"Logic before executing the next delegate in the Use method");
            await next.Invoke();
            Console.WriteLine($"Logic after executing the next delegate in the Use method");
        });
        app.Map("/usingmapbranch", builder =>
        {
            builder.Use(async (context, next) =>
            {
                Console.WriteLine("Map branch logic in the Use method before the next delegate");
                await next.Invoke();
                Console.WriteLine("Map branch logic in the Use method after the next delegate");
            });
            builder.MapWhen(context => context.Request.Query.ContainsKey("testquerystring"), builder =>
            {
                builder.Run(async context =>
                {
                    Console.WriteLine(context.Request.Query.ToString());
                    await context.Response.WriteAsync("Hello from the MapWhen branch.");
                });
            });
            builder.Run(async context =>
            {
                Console.WriteLine($"Map branch response to the client in the Run method");
                //await context.Response.WriteAsync("Hello from the map branch.");
            });
        });

       

        //app.Run(async context => // will not be executed because of map and mapwhen
        //{
        //    Console.WriteLine($"Writing the response to the client in the Run method");
        //    await context.Response.WriteAsync("Hello from the middleware component.");
        //});
        app.MapControllers();

        app.Run();
    }
}
