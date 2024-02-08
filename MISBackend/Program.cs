
using Serilog.Events;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using MISBackend.Migrations;
using MISBackend.Middleware;
using Microsoft.AspNetCore.Identity;
using MISBackend.DAL;
using MISBackend.BLL.Services;

namespace MISBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Set the culture for the entire application
            var cultureInfo = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            var builder = WebApplication.CreateBuilder(args);

            // Inisialisasi Serilog
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.RollingFile("logs/logfile_{Date}.txt")
                .CreateLogger();

            // Register Serilog
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);


            // Inisialisasi Configuration
            var configuration = builder.Configuration;

            // Ambil nilai dari konfigurasi
            var conString = configuration.GetConnectionString("DefaultConnection");
            var jwtSettings = configuration.GetSection("JwtSettings");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var secretKey = jwtSettings["SecretKey"];

            // Add services to the container
            builder.Services.AddDbContext<MISDbContext>(options =>
                    options.UseSqlServer(conString));

            // Konfigurasi otentikasi Bearer
            var tokenLifetimeManager = new Middleware.JwtTokenLifetimeManager();
            builder.Services.AddSingleton<Middleware.ITokenLifetimeManager>(tokenLifetimeManager);
            builder.Services.AddScoped<JwtTokenLifetimeManager>(); // Or use another appropriate lifetime scope


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; // Set true jika menggunakan HTTPS
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer, // Ganti dengan issuer yang sesuai
                        ValidAudience = audience, // Ganti dengan audience yang sesuai
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? "Th3RealW@rld!@@1989")) // Ganti dengan kunci rahasia yang sesuai
                    };
                });
            builder.Services.AddAuthorization();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            // Konfigurasi Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MIS.Back", Version = "v1" });
                // Konfigurasi filter untuk Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT token in the input box below.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Daftarkan DatabaseSeeder
            builder.Services.AddScoped<MISDbContextSeed>();

            // Daftarkan Hosted Service
            builder.Services.AddHostedService<DatabaseSeederHostedService>();

            //Menambahkan Service / BLL
            builder.Services.AddScoped<RepMisBack>();

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MIS.Back v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Middleware otentikasi Bearer
            app.UseAuthentication();

            app.MapControllers();

            app.Run();

            // Tutup dan flush log saat aplikasi berakhir
            Log.CloseAndFlush();
        }
    }
}