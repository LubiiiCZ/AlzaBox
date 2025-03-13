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
        var id = GetIdCounter();
        var ResId = $"DOHNAL_{id}";
        var BarcodeId = $"LD_{id}";
        ParcelLockers.PostReservation(ResId, BarcodeId);
        Assert.Pass();
    }

    private static int GetIdCounter()
    {
        string filePath = "persistId.txt";
        int counter = 1;

        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            if (int.TryParse(content, out int lastValue))
            {
                counter = lastValue + 1;
            }
        }

        File.WriteAllText(filePath, counter.ToString());

        return counter;
    }
}
