using API.Models;
using API.Repositories;
using API.Repositories.Interfaces;
using API.Services;
using API.Services.Interfaces;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Program
    {
     public static void Main(string[] args)
  {
        var builder = WebApplication.CreateBuilder(args);

     // Add services to the container.
        builder.Services.AddControllers();
       
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
     {
         c.SwaggerDoc("v1", new OpenApiInfo
          {
         Title = "PDF Analysis API with Gemini 2.0",
        Version = "v1",
   Description = "API for analyzing PDF documents using Google Gemini 2.0 Flash API"
    });
            });

  // Configure GeminiSettings from appsettings.json
            builder.Services.Configure<GeminiSettings>(
                builder.Configuration.GetSection("GeminiSettings"));

  // Register HttpClient
   builder.Services.AddHttpClient();

  // Register repositories
        builder.Services.AddScoped<IPdfRepository, PdfRepository>();
            builder.Services.AddScoped<IGeminiRepository, GeminiRepository>();

 // Register services
    builder.Services.AddScoped<IPdfAnalysisService, PdfAnalysisService>();

            // Configure file upload limits
         builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
            {
     options.MultipartBodyLengthLimit = 20 * 1024 * 1024; // 20 MB
  });

            // Add CORS if needed
            builder.Services.AddCors(options =>
            {
           options.AddPolicy("AllowAll", policy =>
           {
        policy.AllowAnyOrigin()
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

            app.UseHttpsRedirection();

   app.UseCors("AllowAll");

  app.UseAuthorization();

         app.MapControllers();

            app.Run();
        }
    }
}
