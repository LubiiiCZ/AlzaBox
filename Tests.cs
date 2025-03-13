namespace AlzaBox;

[TestFixture]
public class Tests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Assert.That(Identity.GetToken(), Is.True);
    }

    //K jakým AlzaBoxům má partner přístup? (id, name, address, ...)
    [Test]
    public void GetBoxesInfo()
    {
        List<string> parameters = [
            "name", "available", "address", "description",
            //"openingHours", "photos", "slots", "occupancy", "deliveryPin", "gps",
        ];

        ParcelLockers.GetBoxes(parameters);
        Assert.Pass();
    }

    //Jaké rezervace byly předvytvořené, do jakých AlzaBoxů, jaký čárový kód má balíček a v jakém jsou stavu?
    [Test]
    public void GetReservationsInfo()
    {
        ParcelLockers.GetReservations();
        Assert.Pass();
    }
}
