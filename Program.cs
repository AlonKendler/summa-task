using Resend;
using summa_task.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
// TODO: env variable is not wokring? 
builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken = "re_Bdq2kDgi_DwX7sPi783Smo2D8JWSbMmMN";
});
builder.Services.AddTransient<IResend, ResendClient>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<IEmailService, ResendEmailService>();
builder.Services.AddScoped<IImageProcessingService, GoogleVisionImageProcessingService>();
builder.Services.AddScoped<FormattingService>();

var googleCredentialsPath = builder.Configuration["GoogleCredentials"];
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.Combine(builder.Environment.ContentRootPath, googleCredentialsPath));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
