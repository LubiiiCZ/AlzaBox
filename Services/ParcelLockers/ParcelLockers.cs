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

    public static string PostReservation(string resId, string barcodeId, bool duplicate = false)
    {
        TestContext.Progress.WriteLine($"Creating reservation for: {resId} - {barcodeId}");

        var client = new RestClient($"https://{_service}.alza.cz/parcel-lockers/v2/reservation");
        var request = new RestRequest("", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", Identity.Token);
        request.AddJsonBody(CreateReservationBody(resId, barcodeId));

        var response = client.Execute(request);

        if (!duplicate)
        {
            Assert.That(response.IsSuccessful, Is.True);

            if (response.Content != null)
            {
                var status = ParseReservationStatus(response.Content);
                TestContext.Progress.WriteLine($"Reservation status: {status}");
                return status;
            }
            else
            {
                TestContext.Progress.WriteLine("Failed to set reservation.");
                return "FAILED";
            }
        }
        else
        {
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Conflict));

            if (response.Content != null)
            {
                var status = ParseDuplicateReservationStatus(response.Content);
                TestContext.Progress.WriteLine($"Reservation status: {status}");
                return status;
            }
            else
            {
                TestContext.Progress.WriteLine("Failed to set reservation.");
                return "FAILED";
            }
        }
    }

    private static string CreateReservationBody(string resId, string barcodeId)
    {
        DateTime startDate = DateTime.UtcNow;
        DateTime expirationDate = startDate.AddDays(3);

        var body = new
        {
            data = new
            {
                reservation = new
                {
                    id = resId,
                    attributes = new
                    {
                        startReservationDate = startDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        expirationDate = expirationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        packages = new[]
                        {
                            new
                            {
                                depth = 3.5,
                                height = 2.8,
                                width = 4.2,
                                barcode = barcodeId,
                                requirements = new
                                {
                                    maxSlotSize = "M"
                                }
                            }
                        },
                        type = "TIME"
                    },
                    relationships = new
                    {
                        box = new
                        {
                            id = 7873
                        }
                    }
                }
            }
        };

        return JsonSerializer.Serialize(body, new JsonSerializerOptions { WriteIndented = false });
    }

    private static string ParseReservationStatus(string response)
    {
        using JsonDocument doc = JsonDocument.Parse(response);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("data", out JsonElement dataElement))
        {
            return dataElement.GetProperty("attributes").GetProperty("status").GetString() ?? "Unknown Status";
        }

        return "Unknown Status";
    }

    private static string ParseDuplicateReservationStatus(string response)
    {
        using JsonDocument doc = JsonDocument.Parse(response);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty("errors", out JsonElement errorsElement))
        {
            return errorsElement[0].GetProperty("extensions").GetProperty("code").GetString() ?? "Unknown Status";
        }

        return "Unknown Status";
    }
}
