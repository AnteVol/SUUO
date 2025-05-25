using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SUUO_DZ3.Data;
using SUUO_DZ3.Models.Validation;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        if (!builder.Environment.EnvironmentName.Equals("Testing"))
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStringAnte")));
        }
        
        builder.Services.AddValidatorsFromAssemblyContaining<KonobarValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<KuharValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<StavkaNarudbeValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<NarudzbaValidator>();

        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddFluentValidationClientsideAdapters();
        
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; 
            });


        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (db.Database.IsSqlServer())
            {
                db.Database.Migrate();
            }
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Narudzbe}/{id?}");

        app.Run();
    }
}