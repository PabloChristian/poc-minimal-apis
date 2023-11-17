using ParkingLot;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapSwagger();
app.UseSwaggerUI();
app.UseFastEndpoints(c =>
{
    c.Errors.Configure();
});

app.Run();