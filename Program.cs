using DaticianProj;
using DaticianProj.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container and configure the application.
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<KonsumeContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Configure the application and specify the Startup class type.
var app = builder.Build();
var env = app.Environment;

if (env.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = string.Empty;  // Sets the UI path to the application root
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "KONSUME v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("konsume");
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHndlingMiddleWare>();

app.MapControllers();

app.Run();

