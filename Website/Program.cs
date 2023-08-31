using System.Drawing.Text;
using CarSpeedWebsite.Data;
using HaloSoft.DataAccess;

public static class Program {
    private const int SCHEMA_VERSION=2;
    private const int SCRIPT_VERSION=1;
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllersWithViews();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseStaticFiles();
        app.UseRouting();


        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html");

        #if DEBUG
                string host = "localhost";
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

