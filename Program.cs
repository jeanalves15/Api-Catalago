using Catalogo_Api.Context;
using Catalogo_Api.DTOs.Mappings;
using Catalogo_Api.Models;
using Catalogo_Api.Repositories;
using Catalogo_Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            options.JsonSerializerOptions
            .ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Bearer JWT",

            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference =new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string [] {}
                }
            });
        });

        var mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<appDbContext>(options =>
                            options.UseMySql(mySqlConnection,
                            ServerVersion.AutoDetect(mySqlConnection)));

        var SecretKey = builder.Configuration["JWT:SecretKey"]
                           ?? throw new ArgumentException("Invalid secrety key!!");
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(0),
                ValidAudience = builder.Configuration["JWT:ValidAudience"],
                ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(
                                  Encoding.UTF8.GetBytes(SecretKey))

            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Adimin"));

            options.AddPolicy("SuperAdminOnly", policy =>
                                policy.RequireRole("Admin").RequireClaim("id", "Deus"));

            options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));

            options.AddPolicy("ExclusiveOnly",
                                policy => policy.RequireAssertion(context =>
                                context.User.HasClaim(claim =>claim.Type =="id" &&
                                                      claim.Value == "Deus") ||
                                                      context.User.IsInRole("SuperAdmin")));
        });

        builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddAutoMapper(typeof(ProdutoDTOmappingProfile));
        builder.Services.AddScoped<ItolkenService, TolkenService>();


        //builder.Services.AddAuthentication("Bearer").AddJwtBearer();
        builder.Services.AddAuthorization();
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>().
            AddEntityFrameworkStores<appDbContext>().
            AddDefaultTokenProviders();

       

        var app = builder.Build();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}