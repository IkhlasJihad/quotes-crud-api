using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using QuotesAPI.Services;
using QuotesAPI.Models;
using QuotesAPI.Data;
using QuotesAPI.AutoMapper;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.Configure<DBSettings>(
    builder.Configuration.GetSection("QuotesDBSettings"));
    /*
builder.Services.AddSingleton<QuotesDBContext>(sp =>
  sp.GetRequiredService<IOptions<DBSettings>>().Value);
*/
builder.Services.AddSingleton<IQuotesDBContext, QuotesDBContext>();
builder.Services.AddSingleton<IQuotesService, QuotesService>();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "QuotesAPI", 
            Version = "v1",
            Description = "An ASP.NET Core Web API for managing Goodreads most common Quotes." 
        });
    }
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuotesAPI v1");
    }
);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}"
        );
        endpoints.MapControllers();
    });
app.Run();
