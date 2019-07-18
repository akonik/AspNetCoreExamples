using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AspNectCoreExamples.RouteBasedLocalization.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNectCoreExamples.RouteBasedLocalization
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc(options =>
            {
                //Add localization pipeline filter.
                options.Filters.Add(new MiddlewareFilterAttribute(typeof(LocalizationPipeline)));
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                // Add support for localizing strings in data annotations (e.g. validation messages) via the
                // IStringLocalizer abstractions.
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider =
                        (type, factory) => factory.Create(typeof(SharedResource));
                }); 

            
            CultureInfo[] supportedCultures =
            {
                new CultureInfo("en"),
                new CultureInfo("ru"),
                new CultureInfo("uk")
            };

            //Register supported cultures.
            RequestLocalizationOptions requestLocalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(supportedCultures[0]),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            };
            requestLocalizationOptions.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                //Accepts culture from route.
                new RouteDataRequestCultureProvider() { Options = requestLocalizationOptions },

                //Accepts culture from headers.
                new AcceptLanguageHeaderRequestCultureProvider() { Options = requestLocalizationOptions}
            };

            services.AddSingleton(requestLocalizationOptions);

            // Add the localization services to the services container
            services.AddLocalization(options => options.ResourcesPath = "Resources");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                string defaultCulture = "ru";
                routes.MapMiddlewareRoute("{culture=" + defaultCulture + "}/{*mvcRoute}", subApp =>
                {
                    subApp.UseMvc(mvcRoutes =>
                    {
                        mvcRoutes.MapRoute(
                            name: "default_with_culture",
                            template: "{culture=" + defaultCulture + "}/{controller}/{action=Index}/{id?}");
                    });
                });
            });
        }
    }
}
