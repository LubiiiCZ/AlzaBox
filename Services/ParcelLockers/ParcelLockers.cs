namespace AlzaBox;

public static class ParcelLockers
{
    private readonly static string _service = "parcellockersconnector-test";

    public static void GetBoxes()
    {
        List<string> fields = [
            "name", "available", "address", "description",
            "openingHours", "photos", "slots", "occupancy", "deliveryPin", "gps",
        ];

        var parameters = string.Join("&", fields.Select(field => $"fields%5Bbox%5D={field}"));

        var client = new RestClient($"https://{_service}.alza.cz/parcel-lockers/v2/boxes?{parameters}");
        var request = new RestRequest("", Method.Get);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", Identity.Token);

        var response = client.Execute(request);

        if (response.Content != null)
        {
            ParseBoxes(response.Content);
        }
        else
        {
            TestContext.Progress.WriteLine("Failed to get boxes.");
        }

        Assert.That(response.IsSuccessful, Is.True);
    }

    private static void ParseBoxes(string response)
    {
        using JsonDocument doc = JsonDocument.Parse(response);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("data", out JsonElement dataElement) &&
            dataElement.TryGetProperty("boxes", out JsonElement boxesElement))
        {
            foreach (JsonElement box in boxesElement.EnumerateArray())
            {
                var boxId = box.GetProperty("id").GetInt32();
                var boxName = box.GetProperty("attributes").GetProperty("name").GetString() ?? "Unknown Name";
                var boxStreet = box.GetProperty("attributes").GetProperty("address").GetProperty("streetWithNumber").GetString() ?? "Unknown Street";
                var boxCity = box.GetProperty("attributes").GetProperty("address").GetProperty("city").GetString() ?? "Unknown City";
                var boxZip = box.GetProperty("attributes").GetProperty("address").GetProperty("zip").GetString() ?? "Unknown Zip";
                var boxCountry = box.GetProperty("attributes").GetProperty("address").GetProperty("countryShortCode").GetString() ?? "Unknown Country";

                TestContext.Progress.WriteLine($"Box ID: {boxId}");
                TestContext.Progress.WriteLine($"Name: {boxName}");
                TestContext.Progress.WriteLine($"Address: {boxStreet}, {boxCity}, {boxZip}, {boxCountry}");
                TestContext.Progress.WriteLine(new string('-', 60));
            }
        }
    }

    public static void GetReservations()
    {
        var client = new RestClient($"https://{_service}.alza.cz/parcel-lockers/v2/reservations");
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

    private static void ParseReservations(string response)
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

                TestContext.Progress.WriteLine($"Reservation ID: {reservationId}");
                TestContext.Progress.WriteLine($"Box ID: {boxId}");
                TestContext.Progress.WriteLine("Packages:");

                if (reservation.GetProperty("attributes").TryGetProperty("packages", out JsonElement packagesElement))
                {
                    foreach (JsonElement package in packagesElement.EnumerateArray())
                    {
                        string barcode = package.GetProperty("barcode").GetString() ?? "Unknown Barcode";
                        string packageState = package.GetProperty("packageState").GetString() ?? "Unknown Package State";
                        TestContext.Progress.WriteLine($"  - Barcode: {barcode} - State: {packageState}");
                    }
                }

                TestContext.Progress.WriteLine(new string('-', 60));
            }
        }
    }
}
