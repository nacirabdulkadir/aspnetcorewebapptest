using AspNetCoreWebAppTest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // MVC için Controller ve View'larý ekler.
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));

var app = builder.Build();

// Add services to the container.


// ... diðer yapýlandýrmalar


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Hata yönetimi için MVC'de kullanýlan yol
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // MVC için varsayýlan route tanýmý

app.Run();
