﻿namespace CelempTest;
using Celemp;

[TestClass]
public class ShipTest
{
    [TestMethod]
    public void TestHull()
    {
        Ship empty = new();
        empty.number = 23;
        Assert.AreEqual(1, empty.CalcWeight());
        Assert.AreEqual(ShipType.Hull, empty.CalcType());
        Assert.AreEqual(true, empty.IsEmpty());
        Assert.AreEqual("123", empty.DisplayNumber());
    }

    [TestMethod]
    public void TestSmallShip()
    {
        Ship s = new();
        Galaxy g = new();
        g.turn = 99;    // Avoid amnesty
        g.earthAmnesty = 10;
        s.SetGalaxy(g);
        s.cargo = 10;
        s.cargoleft = 10;
        s.fighter = 10;
        Assert.AreEqual(12, s.CalcWeight(), "CalcWeight - empty");
        s.LoadShip("Ore 0", 8);
        Assert.AreEqual(2, s.cargoleft, "foo");
        Assert.AreEqual(8, s.carrying["Ore 0"]);
        Assert.AreEqual(16, s.CalcWeight(), "CalcWeight - Loaded");
        Assert.AreEqual(ShipType.SmallShip, s.CalcType());
        Assert.AreEqual(6, s.Shots(10));
        Assert.AreEqual(4, s.FuelRequired(2));
        Assert.AreEqual(true, s.UseFuel(2));
        Assert.AreEqual(4, s.carrying["Ore 0"], "Fuel level after use");
    }
}