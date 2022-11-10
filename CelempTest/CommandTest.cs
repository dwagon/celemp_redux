namespace CelempTest;
using Celemp;

[TestClass]
public class CommandTest
{
    [TestMethod]
    public void Test_Jump1()
    {
        Command cmd = new("S123J234", 1);
        Assert.AreEqual(134, cmd.numbers["jump1"]);
        Assert.AreEqual(CommandOrder.JUMP1, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["ship"]);
        Assert.AreEqual(1, cmd.plrNum);
    }

    [TestMethod]
    public void Test_Jump5()
    {
        Command cmd = new("S101J120J131J142J153J164", 2);
        Assert.AreEqual(20, cmd.numbers["jump1"]);
        Assert.AreEqual(31, cmd.numbers["jump2"]);
        Assert.AreEqual(42, cmd.numbers["jump3"]);
        Assert.AreEqual(53, cmd.numbers["jump4"]);
        Assert.AreEqual(64, cmd.numbers["jump5"]);
        Assert.AreEqual(CommandOrder.JUMP5, cmd.priority);
    }

    [TestMethod]
    public void Test_Scan()
    {
        Command cmd = new("SCAN123", 3);
        Assert.AreEqual(23, cmd.numbers["planet"]);
        Assert.AreEqual(CommandOrder.SCAN, cmd.priority);
    }

    [TestMethod]
    public void Test_ShipName()
    {
        Command cmd = new("S123=Blackguard", 5);
        Assert.AreEqual(23, cmd.numbers["ship"]);
        Assert.AreEqual("Blackguard", cmd.strings["name"]);
        Assert.AreEqual(CommandOrder.NAMESHIP, cmd.priority);
    }

    [TestMethod]
    public void Test_PlanetName()
    {
        Command cmd = new("259=Arakis", 5);
        Assert.AreEqual(159, cmd.numbers["planet"]);
        Assert.AreEqual("Arakis", cmd.strings["name"]);
        Assert.AreEqual(CommandOrder.NAMEPLAN, cmd.priority);
    }

    [TestMethod]
    public void Test_ShipGift()
    {
        Command cmd = new("S123GDavid", 5);
        Assert.AreEqual(23, cmd.numbers["ship"]);
        Assert.AreEqual("David", cmd.strings["recipient"]);
        Assert.AreEqual(CommandOrder.GIFTSHIP, cmd.priority);
    }

    [TestMethod]
    public void Test_PlanetGift()
    {
        Command cmd = new("259GMary", 5);
        Assert.AreEqual(159, cmd.numbers["planet"]);
        Assert.AreEqual("Mary", cmd.strings["recipient"]);
        Assert.AreEqual(CommandOrder.GIFTPLAN, cmd.priority);
    }

    [TestMethod]
    public void Test_LoadPDU()
    {
        Command cmd = new("S323L23D", 1);
        Assert.AreEqual(223, cmd.numbers["ship"]);
        Assert.AreEqual(CommandOrder.LOADPDU, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_UnloadPDU()
    {
        Command cmd = new("S323U23D", 1);
        Assert.AreEqual(223, cmd.numbers["ship"]);
        Assert.AreEqual(CommandOrder.UNLOADPDU, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_BuildMine()
    {
        Command cmd = new("235B5M8", 1);
        Assert.AreEqual(135, cmd.numbers["planet"]);
        Assert.AreEqual(5, cmd.numbers["amount"]);
        Assert.AreEqual(8, cmd.numbers["oretype"]);
        Assert.AreEqual(CommandOrder.BUILD_MINE, cmd.priority);
    }

    [TestMethod]
    public void Test_BuildCargo()
    {
        Command cmd = new("S340B10C", 1);
        Assert.AreEqual(240, cmd.numbers["ship"]);
        Assert.AreEqual(CommandOrder.BUILD_CARGO, cmd.priority);
        Assert.AreEqual(10, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_BuildFighter()
    {
        Command cmd = new("S140B1F", 1);
        Assert.AreEqual(40, cmd.numbers["ship"]);
        Assert.AreEqual(CommandOrder.BUILD_FIGHTER, cmd.priority);
        Assert.AreEqual(1, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_BuildTractor()
    {
        Command cmd = new("S341B100T", 1);
        Assert.AreEqual(241, cmd.numbers["ship"]);
        Assert.AreEqual(CommandOrder.BUILD_TRACTOR, cmd.priority);
        Assert.AreEqual(100, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_BuildShield()
    {
        Command cmd = new("S340B10S", 1);
        Assert.AreEqual(240, cmd.numbers["ship"]);
        Assert.AreEqual(CommandOrder.BUILD_SHIELD, cmd.priority);
        Assert.AreEqual(10, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_UnloadAll()
    {
        Command cmd = new("S101U", 1);
        Assert.AreEqual(CommandOrder.UNLOAD_ALL, cmd.priority);
        Assert.AreEqual(1, cmd.numbers["ship"]);
    }
}
