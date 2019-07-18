using Microsoft.AspNetCore.Builder;

namespace AspNectCoreExamples.RouteBasedLocalization.Localization
{
    public class LocalizationPipeline
    {
        public void Configure(IApplicationBuilder app, RequestLocalizationOptions options)
        {
            app.UseRequestLocalization(options);
        }
    }
}
