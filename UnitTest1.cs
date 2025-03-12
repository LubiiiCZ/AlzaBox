namespace AlzaBox;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        IdentityManagement.GetToken();
        Assert.Pass();
    }
}
