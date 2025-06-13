using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml; // Make sure this using is present
using StocksApi.Middleware;
using SupplyChain.DatabaseContext;
using SupplyChain.DatabaseContext;
using SupplyChain.IRepoContracts;
using SupplyChain.IServiceContracts;
using SupplyChain.RepoContracts;
using SupplyChain.ServiceContracts;
using SupplyChain.Services;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// ✔️ For EPPlus 8+
//ExcelPackage.License = new OfficeOpenXml.LicenseProvider.NonCommercialLicense();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});



builder.Services.AddTransient<IJwtService, JwtService>();
// 🔹 Add Controllers with JSON format support
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new ProducesAttribute("application/json"));
    opt.Filters.Add(new ConsumesAttribute("application/json"));
}).AddXmlSerializerFormatters().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
{
    opt.Password.RequiredLength = 5;
    opt.Password.RequireNonAlphanumeric = false;

    opt.Password.RequireUppercase = false;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireDigit = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddUserStore<UserStore<ApplicationUser, ApplicationRole,ApplicationDbContext, Guid>>()
.AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidAudience = configuration["Jwt:Audience"],
        ValidateIssuer = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "User"));

});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // Redis server URL
    options.InstanceName = "SampleInstance_"; // Optional prefix for keys
});

// 🔹 Add Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("con")));
builder.Services.AddControllers();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICartService,CartService>();   
builder.Services.AddScoped<ICartRepository,CartRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
    {
        policyBuilder.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>())
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Auth Bearer Scheme",
        Name = "Authorisation",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }

    };
    c.AddSecurityDefinition("Bearer", securitySchema);
    var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        securitySchema,new[] {"Bearer"}
                    }
                };
    c.AddSecurityRequirement(securityRequirement);
}); ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseCors("AllowSpecificOrigins"); // 🔹 Use Named CORS Policy

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
