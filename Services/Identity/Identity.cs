namespace AlzaBox;

public static class Identity
{
    public static string Token { get; set; } = string.Empty;
    private readonly static string _service = "identitymanagement";

    public static bool GetToken()
    {
        var client = new RestClient($"https://{_service}.phx-test.alza.cz/connect/token");
        var request = new RestRequest("", Method.Post);

        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("grant_type", "password");
        request.AddParameter("scope", "konzole_access");
        request.AddParameter("client_id", Config.ClientId);
        request.AddParameter("client_secret", Config.ClientSecret);
        request.AddParameter("username", Config.Username);
        request.AddParameter("password", Config.Password);

        var response = client.Execute(request);

        if (response.Content != null)
        {
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(response.Content);
            Token = $"Bearer {tokenResponse?.AccessToken}";
            return true;
        }
        else
        {
            TestContext.Progress.WriteLine("Failed to get token.");
            return false;
        }
    }
}
