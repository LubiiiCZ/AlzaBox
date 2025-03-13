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
        ParcelLockers.GetBoxes();
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
