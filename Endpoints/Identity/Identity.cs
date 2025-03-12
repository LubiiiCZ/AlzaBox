namespace AlzaBox;

public static class Identity
{
    public static string Token { get; set; } = string.Empty;
    private readonly static string _endpoint = "identitymanagement";

    public static bool GetToken()
    {
        TestContext.Out.WriteLine("Getting token...");

        var client = new RestClient($"https://{_endpoint}.phx-test.alza.cz/connect/token");
        var request = new RestRequest("", Method.Post);

        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("grant_type", "password");
        request.AddParameter("scope", "konzole_access");
        request.AddParameter("client_id", "hiring7_client");
        request.AddParameter("client_secret", "Ww*5dIm&P13d0*b7");
        request.AddParameter("username", "Partner Hiring7");
        request.AddParameter("password", "1hj1T!yXw1cKXmVy");

        var response = client.Execute(request);

        if (response.Content != null)
        {
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(response.Content);
            Token = $"Bearer {tokenResponse?.AccessToken}";
            return true;
        }
        else
        {
            TestContext.Out.WriteLine("Failed to get token.");
            return false;
        }
    }
}
