using Microsoft.EntityFrameworkCore;
using Users.Core.Repositories;
using Users.Infrastructure.Database;
using Users.Infrastructure.Repositories;
using Users.Infrastructure.Exceptions;
using Users.Application.Handlers.Commands;
using Users.Application.Handlers.Queries;
using MediatR;
using Users.Core.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Users;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddHttpClient();
builder.Services.AddSwaggerConfiguration();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.Audience = "backend-client";
        options.Authority = "http://localhost:8080/realms/GoTicket";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:8080/realms/GoTicket",
            ValidateAudience = true,
            ValidAudience = "backend-client",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                logger.LogError(context.Exception, "Error en autenticación JWT");
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is ClaimsIdentity claimsIdentity)
                {
                    var resourceAccess = context.Principal.FindFirst("resource_access")?.Value;
                    if (!string.IsNullOrEmpty(resourceAccess))
                    {
                        var resourceAccessJson = System.Text.Json.JsonDocument.Parse(resourceAccess);
                        if (resourceAccessJson.RootElement.TryGetProperty("backend-client", out var backendClientElement) &&
                            backendClientElement.TryGetProperty("roles", out var rolesElement))
                        {
                            foreach (var role in rolesElement.EnumerateArray())
                            {
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()));
                            }
                        }
                    }
                }
                return Task.CompletedTask;
            }
        };
    });



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSwagger", builder =>
    {
        builder.AllowAnyOrigin() // Para pruebas locales
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnectionUser")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IKeycloakRepository, KeycloakRepository>();
//builder.Services.AddScoped<IActivityRepository, ActivityRepository>();

builder.Services.AddMediatR(typeof(CreateUserCommandHandler).Assembly);
//builder.Services.AddMediatR(typeof(DeleteUserCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(UpdateUserCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(ForgotPasswordCommandHandler).Assembly);
//builder.Services.AddMediatR(typeof(LoginUserCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(GetUserByIdQueryHandler).Assembly);
builder.Services.AddMediatR(typeof(GetAllUsersQueryHandler).Assembly);

/*

builder.Services.AddMediatR(typeof(GetActivityByDateQueryHandler).Assembly);
builder.Services.AddMediatR(typeof(GetActivityByTypeQueryHandler).Assembly);
builder.Services.AddMediatR(typeof(GetUserActivityQueryHandler).Assembly);

*/

builder.Services.AddTransient<IUserDbContext, UserDbContext>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Prueba Api");
    });
    app.UseCors("AllowSwagger");
}

app.UseHttpsRedirection();


app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization(); 
app.MapControllers();

app.Run();