using Microsoft.Extensions.Configuration;

namespace AlzaBox;

public static class Config
{
    private static IConfigurationRoot AppSettings { get; set; }
    public static string ClientId => AppSettings["client_id"] ?? "";
    public static string ClientSecret => AppSettings["client_secret"] ?? "";
    public static string Username => AppSettings["username"] ?? "";
    public static string Password => AppSettings["password"] ?? "";

    static Config()
    {
        AppSettings = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
}
