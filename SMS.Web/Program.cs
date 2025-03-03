using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMS.AppCore.Interfaces;
using SMS.AppCore.MapProfiles;
using SMS.AppCore.Repositories;
using SMS.Domain.Entities;
using SMS.Infrastructure;
using SMS.Notification;
using SMS.Web.Hubs;
using SMS.Web.Services;
using Syncfusion.Licensing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Example setting
    });

var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(ConnectionString));

builder.Services.AddHangfire((sp, config) =>
{
    config.UseSqlServerStorage(ConnectionString);
});

builder.Services.AddHangfireServer();

//Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IProgressNotifier, ProgressNotifier>();

//Master
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IAssignTeacherClassSubjectRepository, AssignTeacherClassSubjectRepository>();

//Operation
builder.Services.AddScoped<IEnterMarksRepository, EnterMarksRepository>();
builder.Services.AddScoped<IStudentReportRepository, StudentReportRepository>();

//DTO Map
builder.Services.AddAutoMapper(new[] { typeof(Program), typeof(DTOProfileMap) });



//Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Register Syncfusion license
SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBMAY9C3t2XVhhQlJHfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hTH5RdEJiW39edHZRRmRV");
bool isValid = SyncfusionLicenseProvider.ValidateLicense(Platform.ASPNETCore);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapHub<StudentUploadHub>("/uploadProgressHub"); // Map SignalR Hub

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.UseHangfireDashboard("/jobs/hangfire", new DashboardOptions
{
    DashboardTitle = "SMS Background Jobs",
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = "admin",
            Pass = "admin123"
        }
    }
});

//Seed Roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Clerk", "Teacher" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

//Seed Admin User
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string email = "admin@sms.com";
    string password = "Admin@123";
    string fname = "SuperAdmin";
    string lname = "Admin";

    string temail = "teacher@sms.com";
    string tpassword = "Teacher@123";
    string tfname = "John";
    string tlname = "Doe";

    if (await userManager.FindByEmailAsync(temail) == null)
    {
        //var user = new ApplicationUser();
        //user.UserName = email;
        //user.Email = email;

        var teacher = new ApplicationUser
        {
            UserName = temail,
            Email = temail,
            FirstName = tfname,
            LastName = tlname,
        };




        //await userManager.CreateAsync(user, password);
        await userManager.CreateAsync(teacher, tpassword);

        await userManager.AddToRoleAsync(teacher, "Teacher");
    }

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = fname,
            LastName = lname,
        };


        await userManager.CreateAsync(user, password);

        await userManager.AddToRoleAsync(user, "Admin");
    }
}

app.Run();
