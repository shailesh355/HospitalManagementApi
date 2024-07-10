using BaseClass;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Cors Policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
        });
});
#endregion

#region Authentication
Utilities util = new Utilities();
ReturnClass.ReturnBool rbKey = util.GetAppSettings("AppSettings", "Secret");
var key = rbKey.status ? Encoding.ASCII.GetBytes(rbKey.message) : Encoding.ASCII.GetBytes("");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
#endregion

#region Swagger
rbKey = util.GetAppSettings("Project", "Name");
var projectName = rbKey.status ? rbKey.message : "Not defined";

rbKey = util.GetAppSettings("Project", "Version");
var projectVersion = rbKey.status ? rbKey.message : "Not Set";
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = projectVersion,
        Title = projectName
    });
    options.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                Scheme = "Bearer",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
            }
        ); ;
    options.AddSecurityRequirement(
           new OpenApiSecurityRequirement
           {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },Scheme = JwtBearerDefaults.AuthenticationScheme,
                        Name = "Bearer",
                         In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
           }
       );
    #region Swagger XMl Documentation  
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    #endregion
});
#endregion

// Add services to the container.

//builder.Services.AddControllers();
//builder.Services.AddControllers().AddNewtonsoftJson(options =>
//options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
//);

//builder.Services.AddControllers();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // Disable the default model validation
    options.SuppressModelStateInvalidFilter = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region IP Forwarding Settings
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
#endregion

var app = builder.Build();
//// Configure the HTTP request pipeline.
////if (app.Environment.IsDevelopment())
////{
////    app.UseSwagger();
////    app.UseSwaggerUI();
////}
//////else
//////{
//////    app.UseHttpsRedirection();
//////}
//app.UseSwagger();
//app.UseSwaggerUI();
//app.UseForwardedHeaders();
////
//app.UseCors();
////app.UseRouting();
//app.UseAuthorization();
//app.UseMiddleware<AuthorizationMiddleware>();
//app.MapControllers();
//app.Run();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
