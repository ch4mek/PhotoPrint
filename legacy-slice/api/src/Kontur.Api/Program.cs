using System.Text;
using Kontur.Api.Data;
using Kontur.Api.Infrastructure;
using Kontur.Core.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

// --- DI: MySQL 8 (Pomelo), версия сервера определяется автоматически ---
var connectionString = cfg.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Не задана строка подключения ConnectionStrings:Default");
var serverVersion = ServerVersion.AutoDetect(connectionString);
builder.Services.AddDbContext<KonturDbContext>(o => o.UseMySql(connectionString, serverVersion));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<ICurrentUser, HttpCurrentUser>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<Kontur.Api.Modules.Analytics.KpiEngine>();
builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- JWT: роли и scope в claims, права проверяются на сервере (ОТ-02) ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.MapInboundClaims = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, ValidIssuer = cfg["Jwt:Issuer"],
            ValidateAudience = true, ValidAudience = cfg["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Secret"]!)),
            ValidateLifetime = true,
            RoleClaimType = "role",
            NameClaimType = "login",
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// --- Ошибки → RFC 7807 ProblemDetails (ОТ-05) ---
app.UseExceptionHandler(a => a.Run(async context =>
{
    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var code = ex is ApiException api ? api.StatusCode : 500;
    context.Response.StatusCode = code;
    context.Response.ContentType = "application/problem+json";
    await context.Response.WriteAsJsonAsync(new
    {
        type = "about:blank",
        title = ex?.Message ?? "Внутренняя ошибка",
        status = code,
        detail = ex?.Message,
        traceId = context.TraceIdentifier,
    });
}));

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok", utc = DateTime.UtcNow }));

// --- Создание схемы и сиды при старте (воспроизводимость, ТЗ §9.5) ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<KonturDbContext>();
    await db.Database.EnsureCreatedAsync();
    await Seeder.SeedAsync(db, app.Configuration, app.Logger);
}

app.Run();
