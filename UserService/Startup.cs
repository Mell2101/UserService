using DBConnection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserService.Repositories;
using UserService.Services;

namespace UserService;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuring)
    {
        _configuration = configuring;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = _configuration.GetConnectionString("Connection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddScoped<IUserService, UserServiceImpl>();
        services.AddScoped<IUserRepository, UserRepositoryImpl>();
        services.AddScoped<IPasswordHasher, PasswordHasherImpl>();
        
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = false; 
            });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API",
                Version = "v1",
                Description = "API",
                Contact = new OpenApiContact
                {
                    Name = "Timofey",
                    Email = "tkagochkin@mail.ru",
                    Url = new Uri("http://localhost:5126/swagger/index.html")
                }
            });
        });

    }

    public void Configure(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/")
            {
                context.Response.Redirect("/swagger/index.html");
                return;
            }
            await next();
        });

        app.MapControllers();
    }
}
