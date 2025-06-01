using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddMvcOptions(options =>
    {
        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => $"'{x}' alanı için girilen '{y}' değeri geçersiz.");
        options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(x => $"'{x}' alanı gereklidir.");
        options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "Bir değer gereklidir.");
        options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => "İstek gövdesi boş olamaz.");
        options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(x => $"Girilen '{x}' değeri geçersiz.");
        options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => "Girilen değer geçersiz.");
        options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => "Alan sayı olmalıdır.");
        options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(x => $"'{x}' alanı için girilen değer geçersiz.");
        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => $"'{x}' değeri geçersiz.");
        options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x => $"'{x}' alanı sayı olmalıdır.");
        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => $"'{x}' alanı boş olamaz."); // This should cover "The plainText field is required."
    });

builder.Services.AddScoped<CryptoToolkit.Web.Services.AesService>();
builder.Services.AddScoped<CryptoToolkit.Web.Services.RsaService>();
builder.Services.AddScoped<CryptoToolkit.Web.Services.Sha256Service>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
