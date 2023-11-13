using AspNetCoreWebAppTest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // MVC i�in Controller ve View'lar� ekler.
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));

var app = builder.Build();

// Add services to the container.


// ... di�er yap�land�rmalar


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Hata y�netimi i�in MVC'de kullan�lan yol
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // MVC i�in varsay�lan route tan�m�

app.Run();
