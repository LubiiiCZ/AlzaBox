namespace AlzaBox;

[TestFixture]
public class Tests
{
    //Získání tokenu pro IdentityManagement
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

    //Napsat testy, které zkusí vytvořit rezervaci a ověří, zda byla rezervace správně vytvořena nebo správně nevytvořena
    [Test]
    public void SetValidReservation()
    {
        var id = Utils.GetIdCounter();
        var ResId = $"DOHNAL_{id}";
        var BarcodeId = $"LD_{id}";

        var status = ParcelLockers.PostReservation(ResId, BarcodeId);
        Assert.That(status, Is.EqualTo("RESERVED"));
    }

    //Duplicitní rezervace - stejné ID rezervace a čárového kódu
    [Test]
    public void SetDuplicateReservation()
    {
        var id = Utils.GetIdCounter(false) - 1;
        var ResId = $"DOHNAL_{id}";
        var BarcodeId = $"LD_{id}";

        var status = ParcelLockers.PostReservation(ResId, BarcodeId, true);
        Assert.That(status, Is.EqualTo("CONFLICT"));
    }
}
