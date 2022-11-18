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
        Assert.AreEqual(CommandOrder.NAME_SHIP, cmd.priority);
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
        Assert.AreEqual(CommandOrder.LOAD_PDU, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_UnloadPDU()
    {
        Command cmd = new("S323U23D", 1);
        Assert.AreEqual(223, cmd.numbers["ship"]);
        Assert.AreEqual(CommandOrder.UNLOAD_PDU, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["amount"]);
    }

    [TestMethod]

    [ExpectedException(typeof(CommandParseException))]
    public void Test_Load_Fail()
    {
        Command cmd = new("S323L270", 1);
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
    [ExpectedException(typeof(CommandParseException))]
    public void Test_BuildMine_Fail()
    {
        Command cmd = new("235B5M", 1);
        Assert.AreEqual(135, cmd.numbers["planet"]);
        Assert.AreEqual(5, cmd.numbers["amount"]);
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

    [TestMethod]
    public void Test_ExtractAmount()
    {
        int val;
        int offset;
        (val, offset) = Command.ExtractAmount("S123A23D", 5);
        Assert.AreEqual(23, val);
        Assert.AreEqual(2, offset);

        (val, offset) = Command.ExtractAmount("S123AD", 5);
        Assert.AreEqual(-1, val);
        Assert.AreEqual(0, offset);
    }

    [TestMethod]
    public void Test_ShipAttackShip()
    {
        Command cmd = new("S123AS345", 1);
        Assert.AreEqual(CommandOrder.ATTACK_SHIP, cmd.priority);
        Assert.AreEqual(-1, cmd.numbers["amount"]);
        Assert.AreEqual(245, cmd.numbers["victim"]);

        cmd = new("S123A23S345", 1);
        Assert.AreEqual(CommandOrder.ATTACK_SHIP, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["amount"]);
        Assert.AreEqual(245, cmd.numbers["victim"]);
    }

    [TestMethod]
    public void Test_ShipAttackPDU()
    {
        Command cmd = new("S123AD", 1);
        Assert.AreEqual(CommandOrder.ATTACK_PDU, cmd.priority);
        Assert.AreEqual(-1, cmd.numbers["amount"]);

        cmd = new("S123A23D", 1);
        Assert.AreEqual(CommandOrder.ATTACK_PDU, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_ShipAttackIndustry()
    {
        Command cmd = new("S123AI", 1);
        Assert.AreEqual(CommandOrder.ATTACK_IND, cmd.priority);
        Assert.AreEqual(-1, cmd.numbers["amount"]);

        cmd = new("S123A2I", 1);
        Assert.AreEqual(CommandOrder.ATTACK_IND, cmd.priority);
        Assert.AreEqual(2, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_ShipAttackMine()
    {
        Command cmd = new("S123AM2", 1);
        Assert.AreEqual(CommandOrder.ATTACK_MINE, cmd.priority);
        Assert.AreEqual(-1, cmd.numbers["amount"]);
        Assert.AreEqual(2, cmd.numbers["oretype"]);

        cmd = new("S123A200M9", 1);
        Assert.AreEqual(CommandOrder.ATTACK_MINE, cmd.priority);
        Assert.AreEqual(200, cmd.numbers["amount"]);
        Assert.AreEqual(9, cmd.numbers["oretype"]);
    }

    [TestMethod]
    public void Test_ShipAttackSpacemines()
    {
        Command cmd = new("S123ASM", 1);
        Assert.AreEqual(CommandOrder.ATTACK_SPCM, cmd.priority);
        Assert.AreEqual(-1, cmd.numbers["amount"]);

        cmd = new("S123A20SM", 1);
        Assert.AreEqual(CommandOrder.ATTACK_SPCM, cmd.priority);
        Assert.AreEqual(20, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_ShipAttackOre()
    {
        Command cmd = new("S123AR5", 1);
        Assert.AreEqual(CommandOrder.ATTACK_ORE, cmd.priority);
        Assert.AreEqual(-1, cmd.numbers["amount"]);
        Assert.AreEqual(5, cmd.numbers["oretype"]);

        cmd = new("S123A20R3", 1);
        Assert.AreEqual(CommandOrder.ATTACK_ORE, cmd.priority);
        Assert.AreEqual(20, cmd.numbers["amount"]);
        Assert.AreEqual(3, cmd.numbers["oretype"]);
    }

    [TestMethod]
    public void Test_UnloadMine()
    {
        Command cmd = new("S123UM3", 1);
        Assert.AreEqual(CommandOrder.UNLOAD_MINE, cmd.priority);
        Assert.AreEqual(-1, cmd.numbers["amount"]);
        Assert.AreEqual(3, cmd.numbers["oretype"]);

        cmd = new("S123U20M4", 1);
        Assert.AreEqual(CommandOrder.UNLOAD_MINE, cmd.priority);
        Assert.AreEqual(20, cmd.numbers["amount"]);
        Assert.AreEqual(4, cmd.numbers["oretype"]);
    }

    [TestMethod]
    public void Test_UnloadIndustry()
    {
        Command cmd = new("S123UI", 1);
        Assert.AreEqual(CommandOrder.UNLOAD_IND, cmd.priority);
        Assert.AreEqual(-1, cmd.numbers["amount"]);

        cmd = new("S123U3I", 1);
        Assert.AreEqual(CommandOrder.UNLOAD_IND, cmd.priority);
        Assert.AreEqual(3, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_UnloadOre()
    {
        Command cmd = new("S123UR5", 1);
        Assert.AreEqual(CommandOrder.UNLOAD_ORE, cmd.priority);
        Assert.AreEqual(-1, cmd.numbers["amount"]);
        Assert.AreEqual(5, cmd.numbers["oretype"]);

        cmd = new("S123U3R6", 1);
        Assert.AreEqual(CommandOrder.UNLOAD_ORE, cmd.priority);
        Assert.AreEqual(3, cmd.numbers["amount"]);
        Assert.AreEqual(6, cmd.numbers["oretype"]);
    }

    [TestMethod]
    public void Test_UnloadSpacemine()
    {
        Command cmd = new("S123US", 1);
        Assert.AreEqual(CommandOrder.UNLOAD_SPCM, cmd.priority);
        Assert.AreEqual(-1, cmd.numbers["amount"]);

        cmd = new("S123U3S", 1);
        Assert.AreEqual(CommandOrder.UNLOAD_SPCM, cmd.priority);
        Assert.AreEqual(3, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_Planet_Attack_Ship()
    {
        Command cmd = new("123AS234", 1);
        Assert.AreEqual(CommandOrder.PLANET_ATTACK_SHIP, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["planet"]);
        Assert.AreEqual(-1, cmd.numbers["amount"]);
        Assert.AreEqual(134, cmd.numbers["victim"]);

        cmd = new("123A23S234", 1);
        Assert.AreEqual(CommandOrder.PLANET_ATTACK_SHIP, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["amount"]);
        Assert.AreEqual(23, cmd.numbers["planet"]);
        Assert.AreEqual(134, cmd.numbers["victim"]);
    }

    [TestMethod]
    public void Test_Planet_BuildIndustry()
    {
        Command cmd = new("123B10I", 1);
        Assert.AreEqual(CommandOrder.BUILD_IND, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["planet"]);
        Assert.AreEqual(10, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_Planet_BuildPDU()
    {
        Command cmd = new("123B10D", 1);
        Assert.AreEqual(CommandOrder.BUILD_PDU, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["planet"]);
        Assert.AreEqual(10, cmd.numbers["amount"]);
    }

    [TestMethod]
    public void Test_Planet_BuildSpacemine()
    {
        Command cmd = new("123B10S3", 1);
        Assert.AreEqual(CommandOrder.BUILD_SPACEMINE, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["planet"]);
        Assert.AreEqual(10, cmd.numbers["amount"]);
        Assert.AreEqual(3, cmd.numbers["oretype"]);
    }

    [TestMethod]
    public void Test_Sell_All()
    {
        Command cmd = new("S123X", 1);
        Assert.AreEqual(CommandOrder.SELL_ALL, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["ship"]);
    }

    [TestMethod]
    public void Test_Sell_Ore()
    {
        Command cmd = new("S234X5R9", 1);
        Assert.AreEqual(CommandOrder.SELL_ORE, cmd.priority);
        Assert.AreEqual(134, cmd.numbers["ship"]);
        Assert.AreEqual(5, cmd.numbers["amount"]);
        Assert.AreEqual(9, cmd.numbers["oretype"]);
    }

    [TestMethod]
    public void Test_Purchase_Ore()
    {
        Command cmd = new("S234P5R9", 1);
        Assert.AreEqual(CommandOrder.BUY_ORE, cmd.priority);
        Assert.AreEqual(134, cmd.numbers["ship"]);
        Assert.AreEqual(5, cmd.numbers["amount"]);
        Assert.AreEqual(9, cmd.numbers["oretype"]);
    }

    [TestMethod]
    public void Test_SetStandingOrder()
    {
        Command cmd = new("OS234P5R9", 1);
        Assert.AreEqual(CommandOrder.STANDING_ORDER, cmd.priority);
        Assert.AreEqual("S234P5R9", cmd.strings["command"]);
        Assert.AreEqual("setship", cmd.strings["order"]);
    }

    [TestMethod]
    public void Test_ClearStandingOrder()
    {
        Command cmd = new("X234", 1);
        Assert.AreEqual(CommandOrder.STANDING_ORDER, cmd.priority);
        Assert.AreEqual(134, cmd.numbers["planet"]);
        Assert.AreEqual("clearplanet", cmd.strings["order"]);
    }

    [TestMethod]
    public void Test_EngageTractor()
    {
        Command cmd = new("S123ES234", 1);
        Assert.AreEqual(CommandOrder.ENGAGE_TRACTOR, cmd.priority);
        Assert.AreEqual(134, cmd.numbers["victim"]);
        Assert.AreEqual(23, cmd.numbers["ship"]);
    }

    [TestMethod]

    public void Test_Contract_Cargo()
    {
        Command cmd = new("S340B10C1", 1);
        Assert.AreEqual(240, cmd.numbers["ship"]);
        Assert.AreEqual(CommandOrder.CONTRACT_CARGO, cmd.priority);
        Assert.AreEqual(10, cmd.numbers["amount"]);
        Assert.AreEqual(1, cmd.numbers["bid"]);
    }

    [TestMethod]
    public void Test_Contract_Fighter()
    {
        Command cmd = new("S140B1F2", 1);
        Assert.AreEqual(40, cmd.numbers["ship"]);
        Assert.AreEqual(CommandOrder.CONTRACT_FIGHTER, cmd.priority);
        Assert.AreEqual(1, cmd.numbers["amount"]);
        Assert.AreEqual(2, cmd.numbers["bid"]);
    }

    [TestMethod]
    public void Test_Contract_Tractor()
    {
        Command cmd = new("S341B100T3", 1);
        Assert.AreEqual(241, cmd.numbers["ship"]);
        Assert.AreEqual(CommandOrder.CONTRACT_TRACTOR, cmd.priority);
        Assert.AreEqual(100, cmd.numbers["amount"]);
        Assert.AreEqual(3, cmd.numbers["bid"]);
    }

    [TestMethod]
    public void Test_Contract_Shield()
    {
        Command cmd = new("S340B10S4", 1);
        Assert.AreEqual(240, cmd.numbers["ship"]);
        Assert.AreEqual(CommandOrder.CONTRACT_SHIELD, cmd.priority);
        Assert.AreEqual(10, cmd.numbers["amount"]);
        Assert.AreEqual(4, cmd.numbers["bid"]);
    }
}
