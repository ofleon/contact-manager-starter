using ContactManager.Hubs;
using ContactManagerStarter.Extensions;
//using ContactManagerStarter.Extensions;
using ContactManagerStarter.Provider.Domain.Repositories;
using ContactManagerStarter.Provider.Infrastructure.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });


// Add services to the container.
builder.Services.AddContactManagerDb(builder.Configuration);
builder.Services.AddScoped<IContactManagerRepository, ContactManagerRepository>();

builder.Services.AddSignalR();

// add logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders(); // optional (clear providers already added)
    logging.AddConsole();
    logging.AddFile("ContactLogs/ContactManagerLog-{Date}.txt");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//using (var scope = app.Services.CreateScope())
//{
//    var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
//    dataContext.Database.Migrate();
//}

//app.EnsureContactDbIsCreated();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ContactHub>("/contacthub");

app.Run();
