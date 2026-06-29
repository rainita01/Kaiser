using Core_Layer.Profiles;
using Core_Layer.Repository.Address;
using Core_Layer.Repository.Cart;
using Core_Layer.Repository.Category;
using Core_Layer.Repository.Comment;
using Core_Layer.Repository.ContactUs;
using Core_Layer.Repository.Image;
using Core_Layer.Repository.Product;
using Core_Layer.Repository.User;
using Core_Layer.Repository.Visitors;
using Core_Layer.Services.ImageServices;
using Core_Layer.Services.Persian;
using Core_Layer.Services.Seed;
using Core_Layer.Services.TextServices;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
});

#endregion
builder.Services.AddIdentity<User, Role>(option =>
    {
        option.SignIn.RequireConfirmedAccount = false;
        option.SignIn.RequireConfirmedEmail = false;
        option.SignIn.RequireConfirmedPhoneNumber = false;
        option.User.RequireUniqueEmail = false;
        option.Password.RequireUppercase = false;
        option.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<IdentityErrorsToPersian>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
     
        policy.WithOrigins("http://localhost:3000")  
            .AllowAnyHeader()                      
            .AllowAnyMethod()                   
            .AllowCredentials();                   
    });
});
builder.Services.ConfigureApplicationCookie(options =>
{
    // تنظیمات کوکی
    options.Cookie.Name = "Kaiser.shop";
    options.Cookie.HttpOnly = false;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; 
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;

});

var app = builder.Build();
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

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath,"Uploads")),
    RequestPath = "/uploads"
});


// app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();
