namespace AspNetCoreExamples.Services
{
    public class AppSettings
    {
        public string Secret { get; set; }

        public int TokenLifeTimeInMunutes { get; set; } = 60;
    }
}
