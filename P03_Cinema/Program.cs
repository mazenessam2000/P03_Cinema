using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.Configure<FileSettings>(
    builder.Configuration.GetSection("FileSettings"));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFileService, FileService>();

//builder.Services.AddScoped<IRepository<Actor>, Repository<Actor>>();
//builder.Services.AddScoped<IRepository<MovieActor>, Repository<MovieActor>>();
//builder.Services.AddScoped<IRepository<Movie>, Repository<Movie>>();
//builder.Services.AddScoped<IRepository<MovieCategory>, Repository<MovieCategory>>();
//builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
//builder.Services.AddScoped<IRepository<CinemaMovie>, Repository<CinemaMovie>>();
//builder.Services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<IActorService, ActorService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICinemaService, CinemaService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IShowTimeService, ShowTimeService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
