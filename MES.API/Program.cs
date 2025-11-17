using Microsoft.EntityFrameworkCore;
using MES.Business.Services;
using MES.Data;
using Serilog;
using MES.Business.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MES.Data.Interfaces;
using MES.Data.Repositories;
using MES.Shared.Translators;

namespace MES.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Добавление сервисов
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // 2. JWT Authentication 
            var jwtSettings = builder.Configuration.GetSection("Jwt"); 
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!); 

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("OperatorOnly", policy =>
                    policy.RequireRole("Operator"));
                options.AddPolicy("TechnologistOnly", policy =>
                    policy.RequireRole("Technologist"));
                options.AddPolicy("ManagerOnly", policy =>
                    policy.RequireRole("Manager"));
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));
            });

            // 3. PostgreSQL
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 4. Регистрация репозиториев 
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            // builder.Services.AddScoped<IProductionPlanRepository, ProductionPlanRepository>();

            // 5. Регистрация сервисов 
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IProductionPlanService, ProductionPlanService>();

            // 6. Регистрация трансляторов 
            builder.Services.AddScoped<OrderDTOTranslator>();
            builder.Services.AddScoped<UserDTOTranslator>();

            // 7. Redis 
            /*
            builder.Services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]));
            */

            // 8. MinIO/S3 
            /*
            builder.Services.AddSingleton<IFileStorage>(new S3Storage(
                builder.Configuration["S3:Endpoint"],
                builder.Configuration["S3:AccessKey"],
                builder.Configuration["S3:SecretKey"]));
            */

            // 9. OPC UA (если нужно) 
            /*
            builder.Services.AddSingleton<IOpcUaService, OpcUaService>();
            */

            // 10. Логи
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            var app = builder.Build();

            // Middleware pipeline 
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Authentication ДОЛЖНО быть перед Authorization!
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}