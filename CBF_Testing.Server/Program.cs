using CBF_Testing.Application.Data.Commands;
using CBF_Testing.Application.MappingProfiles;
using CBF_Testing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddDbContext<CBFTestingDbContext>(options =>
        options.UseSqlite("Data Source=Anime.db"));

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(AllMappingProfiles).Assembly);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(LoadAllData).Assembly));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
