using Election.Abstractions;
using Election.Concrete;
using Election.Entity;
using Election.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:4200/", "http://localhost:5004", "https://localhost:4200").
AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(x => true)));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<FileSettings>(builder.Configuration.GetSection("FileSettings"));
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IElectionService, ElectionServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();

app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
