using ERPBilling.Data;
using ERPBilling.Repository;
using ERPBilling.Repository.Interface;
using ERPBilling.Service;
using ERPBilling.Service.Interface;
using ERPBilling.Services;
using Microsoft.EntityFrameworkCore;
using QuestPDF;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Register EF Core with SQL Server using connection string from appsettings.json
builder.Services.AddDbContext<ERPBillingdbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Layer 3: Repositories ─────────────────────────────────────────────────
// AddScoped = one instance per HTTP request (correct for DB operations)
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

// ── Layer 2: Services ─────────────────────────────────────────────────────
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IDashboardService, ERPBilling.Services.DashboardService >();
builder.Services.AddScoped<IInvoicePdfService, InvoicePdfService>();
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// ── Build the app ─────────────────────────────────────────────────────────
var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// IMPORTANT: Serves CSS, JS, images from wwwroot folder
// Without this Bootstrap and all styling will not load
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// ── MVC Default Route ─────────────────────────────────────────────────────
// Goes to HomeController → Index action (Dashboard page)
// DO NOT add app.MapControllers() — that is for API only
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ── DB Connection Check ───────────────────────────────────────────────────
// Since you created DB manually via SQL script,
// db.Database.Migrate() is commented out intentionally.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ERPBillingdbContext>();
    // db.Database.Migrate(); // ← uncomment ONLY if using EF migrations
}

app.Run();
