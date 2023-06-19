using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Configuration;
using FitAplikacja.Infrastructure;
using FitAplikacja.Infrastructure.Repositories.Concrete;
using FitAplikacja.Infrastructure.Repositories.Abstract;
using AutoMapper;
using FluentValidation.AspNetCore;
using System.Reflection;
using FitAplikacjaAPI.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FitAplikacjaAPI.Services.Abstract;
using FitAplikacjaAPI.Services.Concrete;
using System.Threading.Tasks;
using FitAplikacja.Core.Models;
using System.IO;
using Microsoft.OpenApi.Models;
using FitAplikacjaAPI.Services.Requirements;
using Microsoft.AspNetCore.Authorization;
using FitAplikacjaAPI.Services.AuthorizationHandlers;
using FitAplikacja.Services.Abstract;
using FitAplikacja.Services.Concrete;
using FitAplikacja.Services.Settings;

namespace FitAplikacjaAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseLazyLoadingProxies()
                    .UseSqlServer(Environment.GetEnvironmentVariable("FitAplikacjaConnection"))
            );

            #region Authentication & Identity

            // token config
            services.Configure<JWTSettings>(Configuration.GetSection("JWT"));

            // identity config
            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 6;
                config.Password.RequireDigit = true;
                config.Password.RequireUppercase = false;
                config.Password.RequireNonAlphanumeric = false;
                config.SignIn.RequireConfirmedEmail = false;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<AppDbContext>();

            // authentication using JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,

                        ValidAudience = Configuration["JWT:Audience"],
                        ValidIssuer = Configuration["JWT:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey
                            (Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
                    };
                });

            // Facebook
            var fbAuthSettings = new FacebookAuthSettings();
            Configuration.Bind(nameof(FacebookAuthSettings), fbAuthSettings);
            services.AddSingleton(fbAuthSettings);

            services.AddHttpClient();
            services.AddSingleton<IFacebookAuthService, FacebookAuthService>();

            // Google
            var googleAuthSettings = new GoogleAuthSettings();
            Configuration.Bind(nameof(GoogleAuthSettings), googleAuthSettings);
            services.AddSingleton(googleAuthSettings);
            services.AddSingleton<IGoogleAuthService, GoogleAuthService>();

            // Login & Register Service
            services.AddScoped<IUserService, UserService>();

            #endregion

            #region Authorization & Policies

            services.AddAuthorization(options =>
            {
                // User access by route param
                options.AddPolicy("HasUserRouteAccess", policy =>
                    policy.Requirements.Add(new UserRouteRequirement()));

                // Resource ownership
                options.AddPolicy("HasOwnerAccess", policy =>
                    policy.Requirements.Add(new OwnershipRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, UserRouteAccessHandler>();
            services.AddSingleton<IAuthorizationHandler, OwnerAccessHandler>();

            #endregion


            #region Controllers

            services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
            }).AddFluentValidation(config =>
            {
                config.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                config.LocalizationEnabled = false;
            });

            #endregion

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            #region Swagger

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                { 
                    Title = Configuration.GetSection("APIName").Value,
                    Version = "v1"
                });

                // enable XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);

                // JWT auth
                var jwtScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Auth",
                    Description = "JWT Bearer Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                swagger.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtScheme, Array.Empty<string>() }
                });
            });

            #endregion

            #region Repositories

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAssignedProductRepository, AssignedProductRepository>();
            services.AddScoped<IExerciseRepository, ExerciseRepository>();
            services.AddScoped<IWorkoutRepository, WorkoutRepository>();

            #endregion

            services.AddScoped<IAssignedProductService, AssignedProductService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, AppDbContext dbContext)
        {
            dbContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(swagger =>
            {
                swagger.SwaggerEndpoint("/swagger/v1/swagger.json", Configuration.GetSection("APIName").Value);
            });

            CreateRoles(serviceProvider, new string[]{ "Admin" }).Wait();
        }

        /// <summary>
        /// Create selected roles if they don't exist
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider implementation instance</param>
        /// <param name="roles">Array of role names</param>
        /// <returns>Task</returns>
        private async Task CreateRoles(IServiceProvider serviceProvider, string[] roles)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            IdentityResult result;

            foreach(var roleName in roles)
            {
                var exists = await roleManager.RoleExistsAsync(roleName);

                // create role if it doesn't already exist
                if(!exists)
                {
                    result = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
