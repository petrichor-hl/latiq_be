using LaTiQ.Core.Identity;
using LaTiQ.Core.ServiceContracts;
using LaTiQ.Core.Services;
using LaTiQ.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    //Authorization policy
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
 .AddXmlSerializerFormatters();

builder.Services.AddTransient<IJwtService, JwtService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    // options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
    .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

/*
 * Cần check lại điểm này:
 * [AllowAnonymous] ở Controller thì tất cả các Action sẽ AllowAnonymous, 
 * dù cho có gán [Authorize] ở đầu Action 
 * => Khá không hợp lý
 * ------------------------------------------------------------------------------------------------------
 * Mọi Controller mặc định đã được gán [Authorize] ở class
 * Muốn một action trong (authorized) controller được truy cập anonymously
 * thì chỉ cần gán [AllowAnonymous] ở đầu action 
 * (VD: TestController)
 */
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
 .AddJwtBearer(options => {
     options.TokenValidationParameters = new TokenValidationParameters()
     {
         ValidateAudience = true,
         ValidAudience = builder.Configuration["Jwt:Audience"],
         ValidateIssuer = true,
         ValidIssuer = builder.Configuration["Jwt:Issuer"],
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
     };
     options.Events = new JwtBearerEvents
     {
         OnTokenValidated = async context =>
         {
             // Console.WriteLine("OnTokenValidated");
             // Có thể dùng cách này:
             // IEnumerable<Claim>? claims = context.Principal?.Claims;
             // Claim? emailClaim = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email);

             string? emailClaim = context.Principal.FindFirstValue(ClaimTypes.Email);
             if (emailClaim == null)
             {
                 context.Fail("Token does not contain an Email claim.");
             }

             var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

             ApplicationUser? user = await userManager.FindByEmailAsync(emailClaim);
             string? tokenVersionClaim = context.Principal.FindFirstValue("tokenVersion");

             if (user == null || !int.TryParse(tokenVersionClaim, out int intTokenVersion) || intTokenVersion != user.TokenVersion)
             {
                 context.Fail("Invalid Access Token.");
             }
         },
         OnChallenge = context =>
         {
             // Customize the response if token validation fails
             if (!context.Handled)
             {
                 context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                 context.Response.ContentType = "application/json";

                 var result = JsonSerializer.Serialize(new
                 {
                     error = "invalid_token",
                     error_description = "User authentication failed"
                 });

                 return context.Response.WriteAsync(result);
             }
             return Task.CompletedTask;
         }
     };
 });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
