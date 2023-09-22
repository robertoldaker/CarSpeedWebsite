using System.Drawing.Text;
using CarSpeedWebsite.Data;
using CarSpeedWebsite.Models;
using HaloSoft.DataAccess;
using Microsoft.OpenApi.Models;

public static class Program {
    private const int SCHEMA_VERSION=2;
    private const int SCRIPT_VERSION=1;
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
    
        // Add services to the container.
        builder.Services.AddControllersWithViews();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "Car Speed Monitor API",
                    Description = "Api to support Car Speed Monitor",
                    Version = "v1.0"
                }
             );
            //var exeName = System.AppDomain.CurrentDomain.FriendlyName;
            //var filePath = Path.Combine(System.AppContext.BaseDirectory, $"{exeName}.xml");
            //c.IncludeXmlComments(filePath);
        });
        // SignalR
        builder.Services.AddSignalR();

        var corsPolicyName = "allowAll";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyName,
                                  builder => {
                                      builder.WithOrigins("http://localhost:59226")
                                            .AllowCredentials()
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                  });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors(corsPolicyName);


        app.UseStaticFiles();
        app.UseRouting();


        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html");
        app.MapHub<NotificationHub>("/NotificationHub");


        #if DEBUG
                string host = "odin.local";
        #else
                string host = "localhost";
        #endif
        DataAccessBase.Initialise(new DbConnection(SCHEMA_VERSION, SCRIPT_VERSION)
        {
            Server = host,
            DatabaseName = "car_speed",
            Username = "car_speed",
            Password = "1234567890",
            DbProvider = DbProvider.PostgreSQL
        }, DataAccess.SchemaUpdated, DataAccess.StartupScript);

        app.Run();

    }

}

