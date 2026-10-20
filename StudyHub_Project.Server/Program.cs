using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using StudyHub_Project.Server.Authentication;
using StudyHub_Project.Server.Data;
using StudyHub_Project.Server.Data.Repositories;
using StudyHub_Project.Server.Middleware;
using StudyHub_Project.Server.Models;
using StudyHub_Project.Server.Services;
using System.Text.Json;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(
                JsonNamingPolicy.CamelCase));
    });


builder.Services.Configure<ApiBehaviorOptions>(
    options =>
    {
        options.InvalidModelStateResponseFactory =
            context =>
            {
                var message =
                    context.ModelState
                        .Values
                        .SelectMany(value =>
                            value.Errors)
                        .Select(error =>
                            error.ErrorMessage)
                        .FirstOrDefault();


                if (string.IsNullOrWhiteSpace(message))
                {
                    message =
                        "Некорректные данные.";
                }


                return new BadRequestObjectResult(
                    new
                    {
                        message
                    });
            };
    });



builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
            "Frontend",
            policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:5173",
                        "http://localhost:5174",
                        "http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });

builder.Services.AddSingleton<
    InMemoryDataStore>();



builder.Services.AddScoped<
    IUserRepository,
    InMemoryUserRepository>();

builder.Services.AddScoped<
    ITaskRepository,
    InMemoryTaskRepository>();

builder.Services.AddScoped<
    INoteRepository,
    InMemoryNoteRepository>();

builder.Services.AddScoped<
    IFocusSessionRepository,
    InMemoryFocusSessionRepository>();

builder.Services.AddScoped<
    ICampfireRepository,
    InMemoryCampfireRepository>();



builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<AccountService>();

builder.Services.AddScoped<TaskService>();

builder.Services.AddScoped<NoteService>();

builder.Services.AddScoped<FocusService>();

builder.Services.AddScoped<CampfireService>();


builder.Services.AddScoped<
    IPasswordHasher<UserAccount>,
    PasswordHasher<UserAccount>>();


builder.Services
    .AddAuthentication(
        options =>
        {
            options.DefaultAuthenticateScheme =
                "Bearer";

            options.DefaultChallengeScheme =
                "Bearer";
        })
    .AddScheme<
        AuthenticationSchemeOptions,
        InMemoryBearerAuthenticationHandler>(
        "Bearer",
        options =>
        {
        });


builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc(
            "v1",
            new OpenApiInfo
            {
                Title = "StudyHub API",

                Version = "v1",

                Description =
                    "Backend API приложения StudyHub"
            });

        options.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Name =
                    "Authorization",

                Type =
                    SecuritySchemeType.Http,

                Scheme =
                    "bearer",

                BearerFormat =
                    "Opaque token",

                In =
                    ParameterLocation.Header,

                Description =
                    "Введите Bearer token, полученный после регистрации или входа. Вставляйте только сам token, без слова Bearer."
            });

        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference =
                            new OpenApiReference
                            {
                                Type =
                                    ReferenceType.SecurityScheme,

                                Id =
                                    "Bearer"
                            }
                    },

                    Array.Empty<string>()
                }
            });
    });


var app =
    builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();


    app.UseSwaggerUI(
        options =>
        {
            options.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "StudyHub API v1");

            options.DocumentTitle =
                "StudyHub Swagger";
        });
}

app.UseMiddleware<
    ApiExceptionMiddleware>();

app.UseCors(
    "Frontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.Run();