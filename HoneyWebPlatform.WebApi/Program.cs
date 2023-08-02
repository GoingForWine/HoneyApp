namespace HoneyWebPlatform.WebApi
{
    using Microsoft.EntityFrameworkCore;

    using Data;
    using Services.Data.Interfaces;
    using Web.Infrastructure.Extensions;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<HoneyWebPlatformDbContext>(opt =>
                opt.UseSqlServer(connectionString));

            builder.Services.AddApplicationServices(typeof(IHoneyService));

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(setup =>
            {
                setup.AddPolicy("HoneyWebPlatform", policyBuilder => //TODO CHECK THIS NAME
                {
                    policyBuilder
                        .WithOrigins("https://localhost:7130") //TODO FIX THIS LINK
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            
            app.MapControllers();

            app.UseCors("HoneyWebPlatform");

            app.Run();
        }
    }
}