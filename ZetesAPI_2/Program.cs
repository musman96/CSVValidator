using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZetesAPI_2.Data;
using ZetesAPI_2.Services;

namespace ZetesAPI_2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // add database connection
            var connectionString = builder.Configuration.GetConnectionString("azure");
            builder.Services.AddDbContext<CsvvalidateContext>(x => x.UseSqlServer(connectionString));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // inject csv validator 
            builder.Services.AddTransient<ICSVValidator, CSVValidator>();
            builder.Services.AddTransient<ICsvResponsesDbRepository, CsvResponsesDBRepository>();
            builder.Services.AddScoped<CSVValidator>();
            builder.Services.AddScoped<CsvResponsesDBRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}