using RandomVideo.Components;
using RandomVideo.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Ajouter un HttpClient avec une base URL définie dans la configuration
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["BaseUrl"]) });

// Ajouter le service IVideoDirectoryProvider
builder.Services.AddSingleton<IVideoDirectoryProvider, VideoDirectoryProvider>();

// Ajouter le service IVideoThumbnailGenerator
builder.Services.AddSingleton<IVideoThumbnailGenerator>(provider =>
    new VideoThumbnailGenerator(provider.GetRequiredService<ILogger<VideoThumbnailGenerator>>()));

// Ajouter le service IListVideo
builder.Services.AddSingleton<IListVideo>(provider =>
{
    var videoDirectoryProvider = provider.GetRequiredService<IVideoDirectoryProvider>();
    var logger = provider.GetRequiredService<ILogger<ListVideo>>();
    var thumbnailDirectory = builder.Configuration["PathThumbnailDirectory"];
    return new ListVideo(videoDirectoryProvider.GetVideoDirectory(), thumbnailDirectory, logger);
});

// Ajouter le service IVideoService
builder.Services.AddSingleton<IVideoService, VideoService>();

// Configure les services de journalisation
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddConsole(); // Ajoute le logger console

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