using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrderService.API.Extensions;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Repositories;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

namespace OrderService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((ctx, cfg) =>
        {
            cfg
                .ReadFrom.Configuration(ctx.Configuration);
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();

        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen(c =>
        {
            c.ExampleFilters();
        });
        builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

        builder.Services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });

        var app = builder.Build();

        app.UseGlobalExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}