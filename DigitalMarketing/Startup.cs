using Infrastructure.Context;
using System.Text;
using Domain.Models;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DigitalMarketing;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = Configuration["DbContextSettings:DmDbConnection"];
        services.AddDbContext<DmDbContext>(opts => opts.UseNpgsql(connectionString));

        services.AddCors(options =>
        {
            options.AddPolicy("AllowsAll", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyOrigin().AllowAnyHeader();
            });
        });

        services.AddControllers();
        services.AddHttpContextAccessor();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(60);
        });

        RegisterServices(services);

        var appSettingsSection = Configuration.GetSection("AppSettings");
        services.Configure<AppSettings>(appSettingsSection);

        var appSettings = appSettingsSection.Get<AppSettings>();
        var key = Encoding.ASCII.GetBytes(appSettings!.Secret);

        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.RequireAuthenticatedSignIn = true;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true
                };
            });
        services.AddMvc(options => options.EnableEndpointRouting = false);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private void RegisterServices(IServiceCollection services)
    {
        DependencyContainer.RegisterServices(services);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseCors(options => options.AllowAnyOrigin());
        app.UseSession();
        app.UseRouting();
        app.Use(async (context, next) =>
        {
            var jwToken = context.Session.GetString("token");
            if (!string.IsNullOrEmpty(jwToken))
            {
                context.Request.Headers.Add("Authorization", $"Bearer {jwToken}");
            }

            await next();
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}