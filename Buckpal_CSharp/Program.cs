using Buckpal_CSharp.Adapters.Out.Persistence;
using Buckpal_CSharp.Application.Ports.In;
using Buckpal_CSharp.Application.Ports.Out;
using Buckpal_CSharp.Domain.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BuckpalDbContext>(options =>
    options.UseInMemoryDatabase("BuckpalDb"));

builder.Services.AddScoped<ISendMoneyUseCase, SendMoneyService>();
builder.Services.AddScoped<ILoadAccountPort, AccountPersistenceAdapter>();
builder.Services.AddScoped<IUpdateAccountStatePort, AccountPersistenceAdapter>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BuckpalDbContext>();
    context.Accounts.Add(new AccountJpaEntity { Id = 1 });
    context.Accounts.Add(new AccountJpaEntity { Id = 2 });
    
    context.Activities.Add(new ActivityJpaEntity 
    { 
        OwnerAccountId = 1,
        TargetAccountId = 1,
        SourceAccountId = 2,
        Amount = 1000m,
        Timestamp = DateTime.Now.AddDays(-20)
    });
    context.SaveChanges();
}

app.Run();

// 讓測試專案的 WebApplicationFactory<Program> 能夠存取此類別
public partial class Program { }
