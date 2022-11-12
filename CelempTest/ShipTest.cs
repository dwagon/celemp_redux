namespace CelempTest;
using Celemp;

[TestClass]
public class ShipTest
{
    [TestMethod]
    public void Test_Hull()
    {
        Ship empty = new();
        empty.number = 23;
        Assert.AreEqual(1, empty.CalcWeight());
        Assert.AreEqual(ShipType.Hull, empty.CalcType());
        Assert.AreEqual(true, empty.IsEmpty());
        Assert.AreEqual("S123", empty.DisplayNumber());
    }

    [TestMethod]
    public void Test_SmallShip()
    {
        Ship s = new();
        Galaxy g = new();
        g.turn = 99;    // Avoid amnesty
        g.earthAmnesty = 10;
        s.SetGalaxy(g);
        s.cargo = 10;
        s.fighter = 10;
        Assert.AreEqual(12, s.CalcWeight(), "CalcWeight - empty");
        s.LoadShip("0", 8);
        Assert.AreEqual(2, s.CargoLeft());
        Assert.AreEqual(8, s.carrying["0"]);
        Assert.AreEqual(16, s.CalcWeight(), "CalcWeight - Loaded");
        Assert.AreEqual(ShipType.SmallShip, s.CalcType());
        Assert.AreEqual(6, s.Shots(10));
        Assert.AreEqual(4, s.FuelRequired(2));
        Assert.AreEqual(true, s.UseFuel(2));
        Assert.AreEqual(4, s.carrying["0"], "Fuel level after use");
    }

    [TestMethod]
    public void Test_CargoScale()
    {
        Assert.AreEqual(1, Ship.CargoScale("1"));
        Assert.AreEqual(2, Ship.CargoScale("PDU"));
    }

    [TestMethod]
    public void Test_UnloadShip()
    {
        Ship s = new();
        s.cargo = 20;
        s.carrying["PDU"] = 10;

        Assert.AreEqual(10, s.UnloadShip("PDU", 20));
    }

    [TestMethod]
    public void CargoLeft()
    {
        Ship s = new();
        s.cargo = 20;
        Assert.AreEqual(20, s.CargoLeft());
        s.carrying["Industry"] = 1;
        Assert.AreEqual(10, s.CargoLeft());
        s.carrying["3"] = 3;
        Assert.AreEqual(7, s.CargoLeft());
    }

    [TestMethod]
    public void Test_FuelRequired()
    {
        Ship s = new();
        s.cargo = 20;

        Assert.AreEqual(1, s.FuelRequired(1));
        Assert.AreEqual(4, s.FuelRequired(2));
        Assert.AreEqual(9, s.FuelRequired(3));
        s.efficiency = 1;
        Assert.AreEqual(1, s.FuelRequired(1));
        Assert.AreEqual(2, s.FuelRequired(2));
        Assert.AreEqual(6, s.FuelRequired(3));
    }

    [TestMethod]
    public void Test_EffectiveEfficiency()
    {
        Ship s = new();
        s.cargo = 20;

        Assert.AreEqual(0, s.EffectiveEfficiency());

        s.cargo = 200;
        Assert.AreEqual(-1, s.EffectiveEfficiency());

        s.efficiency = 1;
        Assert.AreEqual(0, s.EffectiveEfficiency());
    }

    [TestMethod]
    public void Test_Shots() {
        Ship s = new();
        s.cargo = 20;
        s.carrying["0"] = 5;

        s.fighter = 20;
        Assert.AreEqual(16, s.Shots(20));

        s.fighter = 40;
        Assert.AreEqual(33, s.Shots(30));

        s.fighter = 300;
        Assert.AreEqual(61, s.Shots(30));
    }

    [TestMethod]
    public void Test_CalcWeight()
    {
        Ship s = new();
        s.cargo = 20;
        s.carrying["0"] = 10;
        s.fighter = 102;
        s.tractor = 10;
        s.shield = 20;
        int weight = s.CalcWeight();
        Assert.AreEqual(51, weight);
            
    }

    [TestMethod]
    public void Test_SufferDamage()
    {
        Ship s = new();
        Galaxy g = new();
        Player p1 = new();
        p1.InitPlayer(g, 1);
        g.players[1] = p1;

        s.owner = 1;
        s.fighter = 10;
        s.tractor = 10;
        s.cargo = 10;
        s.SetGalaxy(g);
        s.SufferShots(15);
        s.SufferDamage();
        Assert.AreEqual(0, s.fighter);
        Assert.AreEqual(5, s.tractor);
    }
}
