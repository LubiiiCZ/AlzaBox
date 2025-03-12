namespace AlzaBox;

public static class ParcelLockers
{
    private readonly static string _endpoint = "parcellockersconnector-test";

    public static void GetBoxes(List<string>? fields = null)
    {
        if (fields is null || fields.Count == 0) fields = ["name"];
        var parameters = string.Join("&", fields.Select(field => $"fields%5Bbox%5D={field}"));

        var client = new RestClient($"https://{_endpoint}.alza.cz/parcel-lockers/v2/boxes?{parameters}");
        var request = new RestRequest("", Method.Get);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", Identity.Token);

        var response = client.Execute(request);

        if (response.Content != null)
        {
            TestContext.Out.WriteLine(response.Content);
        }
        else
        {
            TestContext.Out.WriteLine("Failed to get boxes.");
        }
    }

    public static void GetBox()
    {
        var client = new RestClient($"https://{_endpoint}.alza.cz/parcel-lockers/v2/box?filter%5Bid%5D=7873");
        var request = new RestRequest("", Method.Get);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", Identity.Token);

        var response = client.Execute(request);

        if (response.Content != null)
        {
            TestContext.Out.WriteLine(response.Content);
        }
        else
        {
            TestContext.Out.WriteLine("Failed to get the box.");
        }
    }
}
