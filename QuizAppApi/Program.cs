using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuizAppApi.Infrastructure.DBContext;
using QuizAppApi.Services.Authentication;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

// Add services to the container.

//authservice
builder.Services.AddScoped<IAuthServices, AuthServices>();
//DBContext
builder.Services.AddDbContext<DbInfo>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DbCon"]);
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standrad Authorization Header Using Bearer Scheme,Eg:\"bearer {token}\"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.OperationFilter<SecurityRequirementsOperationFilter>();
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.
    TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
        .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),

        ValidateIssuer = false,
        ValidateAudience = false,
    });

builder.Services.AddCors(options => {
    options.AddPolicy(name: "AngularClient",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                      });
    
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AngularClient");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
