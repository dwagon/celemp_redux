﻿namespace CelempTest;
using Celemp;

[TestClass]
public class PlayerTest
{
    [TestMethod]
    public void Cmd_GiftShip()
    {
        Ship s = new();
        Galaxy g = new();
        s.owner = 1;
        g.ships[23] = s;
        Command cmd = new("S123GJohn", 1);
        s.number = 23;
        Player p1 = new("Doner");
        p1.InitPlayer(g, 1);
        Player p2 = new("John");
        p2.InitPlayer(g, 2);
        g.players[1] = p1;
        g.players[2] = p2;

        p1.Cmd_GiftShip(cmd);
        Assert.AreEqual(2, s.owner);
    }

    [TestMethod]
    public void Cmd_GiftPlanet_OK()
    {
        Planet p = new();
        Galaxy g = new();
        p.owner = 1;
        g.planets[23] = p;
        p.number = 23;
        Player p1 = new("Doner");
        p1.InitPlayer(g, 1);
        Player p2 = new("John");
        p2.InitPlayer(g, 2);
        g.players[1] = p1;
        g.players[2] = p2;

        Command cmd = new("123GJohn", 1);
        p1.Cmd_GiftPlan(cmd);
        Assert.AreEqual(2, p.owner);
    }

    [TestMethod]
    public void Cmd_GiftPlanet_Fail()
    {
        Planet p = new();
        Galaxy g = new();
        p.owner = 1;
        g.planets[23] = p;
        p.number = 23;
        Player p1 = new("Doner");
        p1.InitPlayer(g, 1);
        Player p2 = new("John");
        p2.InitPlayer(g, 2);
        g.players[1] = p1;
        g.players[2] = p2;

        Command cmd = new("123GBadName", 1);
        p1.Cmd_GiftPlan(cmd);
        Assert.AreEqual(1, p.owner);
    }

    [TestMethod]
    public void Cmd_LoadPDU()
    {
        Galaxy g = new();
        Planet pt = new();
        Player pr = new("Max");
        Ship s = new();
        s.SetGalaxy(g);
        s.planet = 123;
        s.cargo = 10;
        s.owner = 2;
        s.number = 33;
        s.cargoleft = 10;
        g.ships[33] = s;
        g.planets[123] = pt;
        pt.pdu = 9;
        pt.owner = 2;
        pr.InitPlayer(g, 2);

        Command cmd = new("S133L6D", 2);
        pr.Cmd_LoadPDU(cmd);
      
        // Should be constrained by available cargo
        Assert.AreEqual(4, pt.pdu);
        Assert.AreEqual(5, s.carrying["PDU"]);
        Assert.AreEqual(0, s.cargoleft);
    }
}