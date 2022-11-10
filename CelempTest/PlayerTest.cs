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

    [TestMethod]
    public void Cmd_BuildMine()
    {
        Galaxy g = new();
        Planet pt = new();
        Player pr = new("Max");
        g.planets[235] = pt;
        pt.owner = 2;
        pr.InitPlayer(g, 2);
        pt.industry = 50;
        pt.indleft = 50;
        pt.ore[8] = 40;
        pt.ore[9] = 20;
        pt.mine[8] = 0;

        Command cmd = new("335b5m8", 2);
        pr.ProcessCommand(cmd);
        pr.OutputLog();

        Assert.IsTrue(pr.executed[0].Contains("Insufficient Ore 9"));

        // Should be constrained to building 4
        Assert.AreEqual(50 - 4 *10, pt.indleft);
        Assert.AreEqual(40 - 4 * 5, pt.ore[8]);
        Assert.AreEqual(20 - 4 * 5, pt.ore[9]);
        Assert.AreEqual(4, pt.mine[8]);
    }

    [TestMethod]
    public void Cmd_UnloadPDU()
    {
        Galaxy g = new();
        Planet pln = new();
        Player plr = new("Max");
        Ship s = new();

        g.planets[100] = pln;
        g.ships[0] = s;
        pln.pdu = 0;
        plr.InitPlayer(g, 1);

        s.cargo = 20;
        s.cargoleft = 0;
        s.carrying["PDU"] = 10;
        s.owner = 1;
        s.planet = 100;
        s.SetGalaxy(g);
        
        Command cmd = new("S100U10D", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(pln.pdu, 10);
        Assert.AreEqual(0, s.carrying["PDU"]);
        Assert.AreEqual(20, s.cargoleft);
    }

    [TestMethod]
    public void Cmd_BuildCargo()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new();
        Ship s = new();

        g.planets[1] = plan;
        g.ships[0] = s;
        plr.InitPlayer(g, 1);

        plan.owner = 1;
        plan.industry = 20;
        plan.indleft = 20;
        plan.ore[1] = 20;
        s.owner = 1;
        s.planet = 1;
        s.SetGalaxy(g);

        Command cmd = new("S100B10C", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(10, s.cargo);
        Assert.AreEqual(10, s.cargoleft);
        Assert.AreEqual(20 - 10, plan.indleft);
        Assert.AreEqual(20 - 10, plan.ore[1]);
    }

    [TestMethod]
    public void Cmd_BuildFighter()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new();
        Ship s = new();

        g.planets[1] = plan;
        g.ships[0] = s;
        plr.InitPlayer(g, 1);

        plan.owner = 1;
        plan.industry = 20;
        plan.indleft = 20;
        plan.ore[2] = 10;
        plan.ore[3] = 8;
        s.owner = 1;
        s.planet = 1;
        s.SetGalaxy(g);

        Command cmd = new("S100B10F", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.IsTrue(plr.executed[0].Contains("Insufficient Ore 3"));
        Assert.AreEqual(8, s.fighter);
        Assert.AreEqual(20 - (8*2), plan.indleft);
        Assert.AreEqual(10 - 8, plan.ore[2]);
        Assert.AreEqual(8 - 8, plan.ore[3]);
    }

    [TestMethod]
    public void Cmd_BuildTractor()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new();
        Ship s = new();

        g.planets[1] = plan;
        g.ships[0] = s;
        plr.InitPlayer(g, 1);

        plan.owner = 1;
        plan.industry = 20;
        plan.indleft = 20;
        plan.ore[7] = 15;
        s.owner = 1;
        s.planet = 1;
        s.SetGalaxy(g);

        Command cmd = new("S100B10T", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.IsTrue(plr.executed[0].Contains("Insufficient Ore 7"));

        Assert.AreEqual(7, s.tractor);
        Assert.AreEqual(20 - (7*2), plan.indleft);
        Assert.AreEqual(15 - (7*2), plan.ore[7]);
    }

    [TestMethod]
    public void Cmd_BuildShield()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new();
        Ship s = new();

        g.planets[1] = plan;
        g.ships[0] = s;
        plr.InitPlayer(g, 1);

        plan.owner = 1;
        plan.industry = 20;
        plan.indleft = 12;
        plan.ore[5] = 20;
        plan.ore[6] = 20;

        s.owner = 1;
        s.planet = 1;
        s.SetGalaxy(g);

        Command cmd = new("S100B10S", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.IsTrue(plr.executed[0].Contains("Insufficient Industry"));

        Assert.AreEqual(6, s.shield);
        Assert.AreEqual(12 - (6*2), plan.indleft);
        Assert.AreEqual(20 - 6, plan.ore[5]);
        Assert.AreEqual(20 - 6, plan.ore[6]);
    }

    [TestMethod]
    public void Cmd_UnloadAll()
    {
        Galaxy g = new();
        Planet pln = new();
        Player plr = new("Max");
        Ship s = new();

        g.planets[100] = pln;
        g.ships[0] = s;
        pln.ore[0] = 0;
        pln.ore[1] = 0;
        pln.ore[3] = 0;
        plr.InitPlayer(g, 1);

        s.cargo = 20;
        s.cargoleft = 5;
        s.carrying["0"] = 5;
        s.carrying["1"] = 5;
        s.carrying["3"] = 5;

        s.owner = 1;
        s.planet = 100;
        s.SetGalaxy(g);

        Command cmd = new("S100U", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(5, s.carrying["0"]);
        Assert.AreEqual(0, s.carrying["1"]);
        Assert.AreEqual(0, s.carrying["3"]);
        Assert.AreEqual(0, pln.ore[0]);
        Assert.AreEqual(5, pln.ore[1]);
        Assert.AreEqual(5, pln.ore[3]);
        Assert.AreEqual(20 - 5, s.cargoleft);
    }
}