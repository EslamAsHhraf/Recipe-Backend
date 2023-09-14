using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Authorization.Repository;
using Data_Access_layer.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Business_Access_Layer.Abstract;
using Business_Access_Layer.Concrete;
using Data_Access_layer.Model;
using Data_Access_layer.Repositories;
using Data_Access_layer.Interfaces;
using Microsoft.Extensions.FileProviders;
using Data_Access_layer.Repository;
using Business_Access_Layer.Authorization;
using Microsoft.AspNetCore.Builder;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserRepository<User>, UserRepository<User>>();
builder.Services.AddScoped<IRepository<Recipe>, Repository<Recipe>>();
builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
builder.Services.AddScoped<IRepository<RecipeIngredients>, Repository<RecipeIngredients>>();
builder.Services.AddScoped<IRepository<Rating>, Repository<Rating>>();
builder.Services.AddScoped<IRepository<Favourite>, Repository<Favourite>>();


//builder.Services.AddScoped<IRepository<Ingredient>, Repository<Ingredient>>();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
}) ;
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString,b=>b.MigrationsAssembly("Data_Access_layer")));

builder.Services.AddCors(options => options.AddPolicy(name: "RecipeOrigins",
            policy =>
            {
                policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            }
        ));
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddCookie(x =>
{
    x.Cookie.Name = "token";
    x.Cookie.SameSite = SameSiteMode.None;
    x.Cookie.SecurePolicy = CookieSecurePolicy.None; // Requires HTTPS



}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["token"];
            return Task.CompletedTask;
        }
    };

});

builder.Services.AddScoped<IAuthService, AuthManager>();
builder.Services.AddScoped<ICategory, CategoryServices>();
builder.Services.AddScoped<IRecipesServices, RecipesServices>();
builder.Services.AddScoped<IFileServices, FileServices>();

builder.Services.AddScoped<IRecipeIngredientsService, RecipeIngredientsService>();
builder.Services.AddScoped<IRecipes, RecipesRepository>();
builder.Services.AddScoped<IRecipesServices, RecipesServices>();
builder.Services.AddScoped<IRatingService, RatingServices>();
builder.Services.AddScoped<IFavouriteService, FavouriteService>();

var app = builder.Build();
app.Use((ctx, next) =>
{
    ctx.Response.Headers["Access-Control-Allow-Origin"] = "http://localhost:4200";
    return next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseWhen(context => ((context.Request.Method== "POST" || context.Request.Method == "DELETE" || context.Request.Method == "PUT" )
    && context.Request.Path != "/api/auth/register" )|| context.Request.Path == "/api/auth/me", applicationBuilder =>
{
    applicationBuilder.UseMiddleware<ApiKeyMiddleware>();
});
app.UseHttpsRedirection();
app.UseCors("RecipeOrigins");
app.UseAuthorization();
app.UseRouting(); // Enable routing

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    // Other endpoint configurations here
});
app.UseEndpoints(endpoints => // Configure endpoints
{
    endpoints.MapControllers(); // Map controllers as endpoints
});

app.Run();
