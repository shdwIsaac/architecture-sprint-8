using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using sprint8;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddControllersAsServices();
IdentityModelEventSource.ShowPII = true;
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder => {
        policyBuilder.AllowAnyOrigin();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowAnyHeader();
    });
});

builder.Services.AddScoped<IClaimsTransformation, AddRolesClaimsTransformation>();



builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://keycloak:8080/realms/reports-realm";
        options.Audience = "reports-frontend";
        options.RequireHttpsMetadata = false;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:8080/realms/reports-realm",  // Ensure this matches the issuer in the token
            ValidateAudience = false,
            ValidAudience = "reports-frontend",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role
        };
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authHeader))
                {
                    Console.WriteLine($"Authorization Header: {authHeader}");
                }
                else
                {
                    Console.WriteLine("Authorization header is missing");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                if (context.SecurityToken is JwtSecurityToken token)
                {
                    Console.WriteLine($"Token received: {token.RawData}");
                    // Check the azp claim
                    if (token?.Claims.FirstOrDefault(c => c.Type == "azp")?.Value != "reports-frontend")
                    {
                        context.Fail("Invalid audience (azp) claim.");
                    }
                }
                else
                {
                    Console.WriteLine("Token is NULL");
                }
                return Task.CompletedTask;
            }
        };
        
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();