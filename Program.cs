using RandomVideo.Components;
using RandomVideo.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Ajouter les services nécessaires.
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["BaseUrl"]) });
builder.Services.AddSingleton<IVideoThumbnailGenerator>(provider =>
    new VideoThumbnailGenerator());
builder.Services.AddSingleton<IListVideo>(provider =>
    new ListVideo(builder.Configuration["PathVideoDirectory"],builder.Configuration["PathThumbnailDirectory"] ));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapControllers();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();