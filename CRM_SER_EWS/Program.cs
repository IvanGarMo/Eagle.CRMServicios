
using CRM_EWS.CRM.Helpers;
using CRM_EWS.CRM.Models.Equipos;
using EWS_Contextos.Ventas;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ConfiguracionContext, ConfiguracionContext>();
builder.Services.AddScoped<RegistroAceiteContext, RegistroAceiteContext>();
builder.Services.AddScoped<EquipoContext, EquipoContext>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ICatalogoVentas, CatalogoVentas>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliticaCORS", app =>
    {
        app.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();

    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("PoliticaCORS");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
