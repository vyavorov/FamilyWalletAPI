using FamilyWallet.Application.Repositories.Interfaces;
using FamilyWallet.Application.Repositories;
using FamilyWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FamilyWallet.Application.Services.Interfaces;
using FamilyWallet.Application.Services;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FamilyWallet.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine("Loaded Connection String: " + builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddDbContext<FamilyWalletDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ??
                      Environment.GetEnvironmentVariable("FAMILY_WALLET_CONNECTION")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IFamilyGroupRepository, FamilyGroupRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFamilyGroupService, FamilyGroupService>();
builder.Services.AddScoped<IMonthlyBudgetService, MonthlyBudgetService>();
builder.Services.AddScoped<IMonthlyBudgetSettingsRepository, MonthlyBudgetSettingsRepository>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy("_myAllowSpecificOrigins",
        builder => builder
            .WithOrigins("http://213.91.236.205:5096", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});


var jwtKey = builder.Configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("FAMILY_WALLET_JWT_KEY");

if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWT Key is missing! Add it to appsettings.json or set FAMILY_WALLET_JWT_KEY environment variable.");
}

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080); // enable HTTP
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MigrateDatabase();

app.Run();
