using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AutoMapper;
using Scm.Data;
using Scm.Domain;
using Scm.Data.Repositories;
using Scm.Infrastructure.Authentication;
using Scm.Infrastructure.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Scm.Service;

namespace Scm
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                 c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v0.1" });
                 c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    { 
                    In = ParameterLocation.Header, 
                    Description = "Please insert JWT with Bearer into field", 
                    Name = "Authorization", Type = SecuritySchemeType.ApiKey 
                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    },
                                    Scheme = "oauth2",
                                    Name = "Bearer",
                                    In = ParameterLocation.Header,
                                },
                                new List<string>()
                            }
                        });
            });
            #endregion
            //Database configuration
            services.AddDbContext<ScmContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            //Identity configuration
             services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<ScmContext>()
                .AddDefaultTokenProviders();

            JwtSettings settings = GetJwtSettings();
            
            #region JWT Authentication

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(cfg => {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(settings.DaysToExpiration)
                };
            });

            #endregion

            #region Identity Options
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });
            #endregion

            services.AddHttpContextAccessor();

            services.AddCors();
            //Automapper
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();

            #region services registration
            services.AddSingleton(settings);
            //Each request uses the same instance
            services.AddSingleton(mapper);
            //Each request creates a new instance
            services.AddScoped<EmpleadoRepository>();
            services.AddScoped<RegistroValeRepository>();
            services.AddScoped<RegistroValeService>();
            services.AddScoped<ValeRepository>();
            services.AddScoped<ValeService>();
            services.AddScoped<RetencionRepository>();
            services.AddScoped<CuentaRepository>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Swagger
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            #endregion
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            
            app.UseCors(options => options.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

          public JwtSettings GetJwtSettings()
        {
            JwtSettings settings = new JwtSettings();
            settings.Audience = Configuration["JwtSettings:audience"];
            settings.Issuer = Configuration["JwtSettings:issuer"];
            settings.Key = Configuration["JwtSettings:key"];
            settings.DaysToExpiration = Convert.ToInt32(Configuration["JwtSettings:expireDays"]);
            return settings;
        }
    }
}
