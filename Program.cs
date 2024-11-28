using System.Text.Json.Serialization;
using api.Data;
using api.Interfaces;
using api.Repository;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using api.Models;
using api.Validations;
using System.ComponentModel;
using api.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using api.Dtos.Account;
using api.Service;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//controller + json for relation
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




//for database (DBContext)
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});




//controller + enum converter
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});




//Add Identity JWT (requiered)
builder.Services.AddIdentity<Appuser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

})
.AddEntityFrameworkStores<ApplicationDBContext>();




//Authentications JWT (requiered)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
});




//Scope
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<CommentInterface, CommentRepo>();

builder.Services.AddScoped<IValidator<Stock>, StockValidator>();
builder.Services.AddScoped<IValidator<Comment>, CommentValidator>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();

builder.Services.AddScoped<ITokenService, TokenService>();



//validation
builder.Services.AddValidatorsFromAssemblyContaining<StockValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CommentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


//for auth
app.UseAuthentication();
app.UseAuthorization();

//for controller
app.MapControllers();

app.Run();
