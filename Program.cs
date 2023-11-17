using ParkingLot;
using ParkingLot.Configurations;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddTransient<IHourPriceProvider, HourPriceProvider>();
builder.Services.AddTransient<ILoggedUserAccessor, LoggedUserAccessor>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt => opt.CustomSchemaIds(x => x.FullName?.Replace("+", ".")));
builder.Services.AddFastEndpoints();

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

var app = builder.Build();

app.MapSwagger();
app.UseSwaggerUI();
app.UseFastEndpoints(c =>
{
    c.Errors.Configure();
});

app.Run();