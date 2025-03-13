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

        Assert.That(response.IsSuccessful, Is.True);
    }

    public static void GetReservations()
    {
        var client = new RestClient($"https://{_endpoint}.alza.cz/parcel-lockers/v2/reservations");
        var request = new RestRequest("", Method.Get);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", Identity.Token);

        var response = client.Execute(request);

        if (response.Content != null)
        {
            ParseReservations(response.Content);
        }
        else
        {
            TestContext.Progress.WriteLine("Failed to get reservations.");
        }

        Assert.That(response.IsSuccessful, Is.True);
    }

    public static void ParseReservations(string response)
    {
        using JsonDocument doc = JsonDocument.Parse(response);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("data", out JsonElement dataElement) &&
            dataElement.TryGetProperty("reservations", out JsonElement reservationsElement))
        {
            foreach (JsonElement reservation in reservationsElement.EnumerateArray())
            {
                string reservationId = reservation.GetProperty("id").GetString() ?? "Unknown ID";
                int boxId = reservation.GetProperty("relationships").GetProperty("box").GetProperty("id").GetInt32();

                TestContext.Out.WriteLine($"Reservation ID: {reservationId}");
                TestContext.Out.WriteLine($"Box ID: {boxId}");
                TestContext.Out.WriteLine("Packages:");

                if (reservation.GetProperty("attributes").TryGetProperty("packages", out JsonElement packagesElement))
                {
                    foreach (JsonElement package in packagesElement.EnumerateArray())
                    {
                        string barcode = package.GetProperty("barcode").GetString() ?? "Unknown Barcode";
                        string packageState = package.GetProperty("packageState").GetString() ?? "Unknown Package State";
                        TestContext.Out.WriteLine($"  - Barcode: {barcode} - State: {packageState}");
                    }
                }

                TestContext.Out.WriteLine(new string('-', 60));
            }
        }
    }
}
