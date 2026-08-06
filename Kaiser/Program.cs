using Core_Layer.Middlewares;
using Core_Layer.Profiles;
using Core_Layer.Repository.Address;
using Core_Layer.Repository.Cart;
using Core_Layer.Repository.Category;
using Core_Layer.Repository.Comment;
using Core_Layer.Repository.ContactUs;
using Core_Layer.Repository.Image;
using Core_Layer.Repository.Order;
using Core_Layer.Repository.Payment;
using Core_Layer.Repository.Product;
using Core_Layer.Repository.User;
using Core_Layer.Repository.Visitors;
using Core_Layer.Services.Api;
using Core_Layer.Services.CheckOut;
using Core_Layer.Services.Ghasedak;
using Core_Layer.Services.ImageServices;
using Core_Layer.Services.Persian;
using Core_Layer.Services.Seed;
using Core_Layer.Services.TextServices;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Serilog.Sinks.MSSqlServer;


var builder = WebApplication.CreateBuilder(args);

#region Logger
Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine("SERILOG ERROR: " + msg));
var columnOptions = new ColumnOptions();
columnOptions.Store.Remove(StandardColumn.Properties); // skip XML blob column, we'll use structured columns instead
columnOptions.Store.Add(StandardColumn.LogEvent);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});

#endregion

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Context>(e =>
{
    e.UseSqlServer(builder.Configuration.GetConnectionString("KaiserShop"), s => s.MigrationsAssembly(nameof(Kaiser)));
});

#region Services

builder.Services.AddScoped<ImageServices>();
builder.Services.AddScoped<TextServices>();
builder.Services.AddScoped<ICheckOutServices, CheckoutService>();
builder.Services.Configure<GhasedakOption>(builder.Configuration.GetSection("Ghasedak"));
builder.Services.AddScoped<ISmsServices, GhasedakSmsService>();

#endregion
#region Repositories

builder.Services.AddScoped<IImageRepo, ImageRepo>();
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IViewsRepo, ViewRepo>();
builder.Services.AddScoped<IAddressRepo, AddressRepo>();
builder.Services.AddScoped<ICartRepo, CartRepo>();
builder.Services.AddScoped<IContactUsRepo, ContactUsRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<ICommentRepo, CommentRepo>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IPaymentRepo,PaymentRepo>();
#endregion

#region Mapper

builder.Services.AddAutoMapper(mapp =>
{
    mapp.AddProfile<ImageProfile>();
    mapp.AddProfile<CategoryProfile>();
    mapp.AddProfile<ProductProfile>();
    mapp.AddProfile<AddressProfile>();
    mapp.AddProfile<ContactUsProfile>();
    mapp.AddProfile<CommentProfile>();
    mapp.AddProfile<CartAndSnapShotProfile>();
    mapp.AddProfile<OrderProfile>();
    mapp.AddProfile<PaymentProfile>();
});

#endregion

builder.Services.AddHttpClient<IZarinPalServices, ZarinPalServices>(client =>
{
    var uri = builder.Configuration["Payment"];
    client.BaseAddress = new Uri(uri!);
});

builder.Services.AddIdentity<User, Role>(option =>
    {
        option.SignIn.RequireConfirmedAccount = false;
        option.SignIn.RequireConfirmedEmail = false;
        option.SignIn.RequireConfirmedPhoneNumber = false;
        option.User.RequireUniqueEmail = false;
        option.Password.RequireUppercase = false;
        option.Password.RequireNonAlphanumeric = false;
        option.Password.RequiredLength = 10;
    })
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<IdentityErrorsToPersian>();

var frontendUrl =
    builder.Configuration["Cors:FrontendUrl"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(frontendUrl!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("login", limiter =>
    {
        limiter.PermitLimit = 5;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
    });


    options.AddFixedWindowLimiter("api", limiter =>
    {
        limiter.PermitLimit = 200;
        limiter.Window = TimeSpan.FromMinutes(1);
    });


    options.RejectionStatusCode = 429;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "Kaiser.shop";
    options.Cookie.HttpOnly = true;

    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseForwardedHeaders();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}
app.UseCors("AllowFrontend");
if (app.Environment.IsDevelopment()) 
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else 
{
    app.UseHsts();
}
var uploadPath = Path.Combine(
    app.Environment.ContentRootPath,
    "Uploads");

if (!Directory.Exists(uploadPath))
{
    Directory.CreateDirectory(uploadPath);
}
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath,"Uploads")),
    RequestPath = "/uploads"
});

app.UseRateLimiter();
// app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
