namespace CelempTest;
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
        Assert.AreEqual(0, s.CargoLeft());
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
        pt.ind_left = 50;
        pt.ore[8] = 40;
        pt.ore[9] = 20;
        pt.mine[8] = 0;

        Command cmd = new("335b5m8", 2);
        pr.ProcessCommand(cmd);
        pr.OutputLog();

        Assert.IsTrue(pr.messages[0].Contains("Insufficient Ore 9"));

        // Should be constrained to building 4
        Assert.AreEqual(50 - 4 * 10, pt.ind_left);
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
        s.carrying["PDU"] = 10;
        s.owner = 1;
        s.planet = 100;
        s.SetGalaxy(g);

        Command cmd = new("S100U10D", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(pln.pdu, 10);
        Assert.AreEqual(0, s.carrying["PDU"]);
        Assert.AreEqual(20, s.CargoLeft());
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
        plan.ind_left = 20;
        plan.ore[1] = 20;
        s.owner = 1;
        s.planet = 1;
        s.SetGalaxy(g);

        Command cmd = new("S100B10C", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(10, s.cargo);
        Assert.AreEqual(10, s.CargoLeft());
        Assert.AreEqual(20 - 10, plan.ind_left);
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
        plan.ind_left = 20;
        plan.ore[2] = 10;
        plan.ore[3] = 8;
        s.owner = 1;
        s.planet = 1;
        s.SetGalaxy(g);

        Command cmd = new("S100B10F", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.IsTrue(plr.messages[0].Contains("Insufficient Ore 3"));
        Assert.AreEqual(8, s.fighter);
        Assert.AreEqual(20 - (8 * 2), plan.ind_left);
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
        plan.ind_left = 20;
        plan.ore[7] = 15;
        s.owner = 1;
        s.planet = 1;
        s.SetGalaxy(g);

        Command cmd = new("S100B10T", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.IsTrue(plr.messages[0].Contains("Insufficient Ore 7"));

        Assert.AreEqual(7, s.tractor);
        Assert.AreEqual(20 - (7 * 2), plan.ind_left);
        Assert.AreEqual(15 - (7 * 2), plan.ore[7]);
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
        plan.ind_left = 12;
        plan.ore[5] = 20;
        plan.ore[6] = 20;

        s.owner = 1;
        s.planet = 1;
        s.SetGalaxy(g);

        Command cmd = new("S100B10S", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.IsTrue(plr.messages[0].Contains("Insufficient Industry"));

        Assert.AreEqual(6, s.shield);
        Assert.AreEqual(12 - (6 * 2), plan.ind_left);
        Assert.AreEqual(20 - 6, plan.ore[5]);
        Assert.AreEqual(20 - 6, plan.ore[6]);
    }

    [TestMethod]
    public void Cmd_UnloadAll()
    {
        Galaxy g = new();
        Planet pln = new();
        Player plr = new("Max");
        Ship s = new(g, 0);

        g.planets[100] = pln;
        pln.ore[0] = 0;
        pln.ore[1] = 0;
        pln.ore[3] = 0;
        plr.InitPlayer(g, 1);

        s.cargo = 20;
        s.carrying["0"] = 5;
        s.carrying["1"] = 5;
        s.carrying["3"] = 5;

        s.owner = 1;
        s.planet = 100;

        Command cmd = new("S100U", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(5, s.carrying["0"]);
        Assert.AreEqual(0, s.carrying["1"]);
        Assert.AreEqual(0, s.carrying["3"]);
        Assert.AreEqual(0, pln.ore[0]);
        Assert.AreEqual(5, pln.ore[1]);
        Assert.AreEqual(5, pln.ore[3]);
        Assert.AreEqual(20 - 5, s.CargoLeft());
    }

    [TestMethod]
    public void Test_Cmd_UnloadPDU()
    {
        Galaxy g = new();
        Planet pln = new();
        Player plr = new("Max");
        Ship s = new(g, 0);

        g.planets[100] = pln;
        pln.pdu = 0;
        plr.InitPlayer(g, 1);

        s.cargo = 20;
        s.carrying["PDU"] = 5;

        s.owner = 1;
        s.planet = 100;

        Command cmd = new("S100U4D", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(5 - 4, s.carrying["PDU"]);
        Assert.AreEqual(20 - (1 * 2), s.CargoLeft());
        Assert.AreEqual(4, pln.pdu);
    }

    [TestMethod]
    public void Test_Cmd_UnloadMine()
    {
        Galaxy g = new();
        Planet pln = new();
        Player plr = new("Max");
        Ship s = new(g, 0);

        g.planets[100] = pln;
        pln.mine[1] = 0;
        plr.InitPlayer(g, 1);

        s.cargo = 80;
        s.carrying["Mine"] = 3;

        s.owner = 1;
        s.planet = 100;

        Command cmd = new("S100U2M1", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(3 - 2, s.carrying["Mine"]);
        Assert.AreEqual(80 - (1 * 20), s.CargoLeft());
        Assert.AreEqual(2, pln.mine[1]);
    }

    [TestMethod]
    public void Test_Cmd_UnloadIndustry()
    {
        Galaxy g = new();
        Planet pln = new();
        Player plr = new("Max");
        Ship s = new(g, 0);

        g.planets[100] = pln;
        pln.industry = 0;
        plr.InitPlayer(g, 1);

        s.cargo = 20;
        s.carrying["Industry"] = 2;

        s.owner = 1;
        s.planet = 100;

        Command cmd = new("S100U1I", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(2 - 1, s.carrying["Industry"]);
        Assert.AreEqual(20 - (1 * 10), s.CargoLeft());
        Assert.AreEqual(1, pln.industry);
    }

    [TestMethod]
    public void Cmd_Ship_Attack_PDU()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new("Max");
        Player vic = new("Min");
        Ship s = new(g, 23);

        g.players[1] = plr;
        g.players[2] = vic;
        g.planets[100] = plan;

        plr.InitPlayer(g, 1);
        vic.InitPlayer(g, 2);
        plan.pdu = 20;
        plan.number = 100;
        plan.owner = 2;
        plan.setGalaxy(g);

        s.fighter = 20;
        s.planet = 100;
        s.owner = 1;
        s.InitialiseTurn();

        Command cmd = new("S123A10D", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(10, s.ShotsLeft());
        Assert.AreEqual(20 - 7, plan.pdu);
    }


    [TestMethod]
    public void Cmd_Ship_Attack_Industry()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new("Max");
        Player vic = new("Min");
        Ship s = new(g, 23);

        g.players[1] = plr;
        g.players[2] = vic;
        g.planets[100] = plan;

        plr.InitPlayer(g, 1);
        vic.InitPlayer(g, 2);
        plan.industry = 20;
        plan.number = 100;
        plan.owner = 2;
        plan.setGalaxy(g);

        s.fighter = 20;
        s.planet = 100;
        s.owner = 1;
        s.InitialiseTurn();

        Command cmd = new("S123AI", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(0, s.ShotsLeft());
        Assert.AreEqual(20 - 4, plan.industry);
    }

    [TestMethod]
    public void Cmd_Ship_Attack_Mine()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new("Max");
        Player vic = new("Min");
        Ship s = new(g, 23);

        g.players[1] = plr;
        g.players[2] = vic;
        g.planets[100] = plan;

        plr.InitPlayer(g, 1);
        vic.InitPlayer(g, 2);
        plan.mine[3] = 2;
        plan.ore[3] = 100;
        plan.number = 100;
        plan.owner = 2;
        plan.setGalaxy(g);

        s.fighter = 20;
        s.planet = 100;
        s.owner = 1;
        s.InitialiseTurn();

        Command cmd = new("S123AM3", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(0, s.ShotsLeft());
        Assert.AreEqual(2 - 2, plan.mine[3]);
        Assert.AreEqual(100 - 25, plan.ore[3]);
    }


    [TestMethod]
    public void Cmd_Ship_Attack_Ore()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new("Max");
        Player vic = new("Min");
        Ship s = new(g, 23);

        g.players[1] = plr;
        g.players[2] = vic;
        g.planets[100] = plan;

        plr.InitPlayer(g, 1);
        vic.InitPlayer(g, 2);
        plan.ore[3] = 100;
        plan.number = 100;
        plan.owner = 2;
        plan.setGalaxy(g);

        s.fighter = 20;
        s.planet = 100;
        s.owner = 1;
        s.InitialiseTurn();

        Command cmd = new("S123AR3", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(0, s.ShotsLeft());
        Assert.AreEqual(100 - 45, plan.ore[3]);
    }

    [TestMethod]
    public void Cmd_Ship_Attack_Ship()
    {
        Galaxy g = new();
        Planet p1 = new();
        Player plr = new("Max");
        Player neut = new("Neutral");
        Player vic = new("Min");
        Ship s_atk = new(g, 23);
        Ship s_vic = new(g, 24);

        g.players[1] = plr;
        g.players[2] = vic;
        g.planets[100] = p1;

        neut.InitPlayer(g, 0);
        plr.InitPlayer(g, 1);
        vic.InitPlayer(g, 2);

        s_atk.fighter = 20;
        s_atk.planet = 100;
        s_atk.owner = 1;
        s_atk.InitialiseTurn();

        s_vic.shield = 10;
        s_vic.cargo = 50;
        s_vic.owner = 2;
        s_vic.planet = 100;
        s_vic.InitialiseTurn();

        Command cmd = new("S123AS124", 1);
        plr.ProcessCommand(cmd);

        Command resolv = new("RESOLVEATTACK", 0, true);
        neut.ProcessCommand(resolv);

        plr.OutputLog();
        vic.OutputLog();

        Assert.AreEqual(0, s_atk.ShotsLeft());
        Assert.AreEqual(0, s_vic.shield);
        Assert.AreEqual(50 - 17, s_vic.cargo);
    }

    [TestMethod]
    public void Cmd_Ship_Attack_Spacemine()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new("Max");
        Player vic = new("Min");
        Ship s = new(g, 23);

        g.players[1] = plr;
        g.players[2] = vic;
        g.planets[100] = plan;

        plr.InitPlayer(g, 1);
        vic.InitPlayer(g, 2);
        plan.deployed = 100;
        plan.number = 100;
        plan.owner = 2;
        plan.setGalaxy(g);

        s.fighter = 20;
        s.planet = 100;
        s.owner = 1;
        s.InitialiseTurn();

        Command cmd = new("S123ASM", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(0, s.ShotsLeft());
        Assert.AreEqual(100 - 45, plan.deployed);
    }

    [TestMethod]
    public void Cmd_Planet_Attack_Ship()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new("Max");
        Player vic = new("Min");
        Ship s = new(g, 23);

        g.players[1] = plr;
        g.players[2] = vic;
        g.planets[100] = plan;

        plr.InitPlayer(g, 1);
        vic.InitPlayer(g, 2);
        plan.pdu = 100;
        plan.number = 100;
        plan.owner = 1;
        plan.setGalaxy(g);

        s.fighter = 20;
        s.planet = 100;
        s.owner = 1;
        s.InitialiseTurn();
        plan.InitialiseTurn();

        Command cmd = new("200A23S123", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(100 - 23, plan.PduLeft());
    }

    [TestMethod]
    public void Cmd_Planet_Build_Industry()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new("Max");

        g.players[1] = plr;
        g.planets[100] = plan;

        plr.InitPlayer(g, 1);
        plan.number = 100;
        plan.owner = 1;
        plan.industry = 50;
        plan.ore[8] = 30;
        plan.ore[9] = 30;
        plan.setGalaxy(g);
        plan.InitialiseTurn();

        Command cmd = new("200B10I", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(50 + 5, plan.industry);
        Assert.AreEqual(0, plan.ind_left);
        Assert.AreEqual(30 - (5 * 5), plan.ore[8]);
        Assert.AreEqual(30 - (5 * 5), plan.ore[9]);
    }

    [TestMethod]
    public void Cmd_Planet_Build_PDU()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new("Max");

        g.players[1] = plr;
        g.planets[100] = plan;

        plr.InitPlayer(g, 1);
        plan.number = 100;
        plan.owner = 1;
        plan.industry = 50;
        plan.ore[4] = 30;
        plan.pdu = 0;
        plan.setGalaxy(g);
        plan.InitialiseTurn();

        Command cmd = new("200B10D", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(50 - 10, plan.ind_left);
        Assert.AreEqual(30 - (10 * 1), plan.ore[4]);
        Assert.AreEqual(10, plan.pdu);
    }

    [TestMethod]
    public void Cmd_Planet_Build_Spacemines()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new("Max");

        g.players[1] = plr;
        g.planets[100] = plan;

        plr.InitPlayer(g, 1);
        plan.number = 100;
        plan.owner = 1;
        plan.industry = 50;
        plan.ore[2] = 30;
        plan.pdu = 0;
        plan.setGalaxy(g);
        plan.InitialiseTurn();

        Command cmd = new("200B10S2", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(50 - 10, plan.ind_left);
        Assert.AreEqual(30 - (10 * 1), plan.ore[2]);
        Assert.AreEqual(10, plan.spacemines);
    }

    [TestMethod]
    public void Cmd_SellOre()
    {
        Galaxy g = new();
        Player plr = new("Betty");
        Ship s = new(g, 1);
        Planet plan = new();
        g.players[1] = plr;
        g.earth_price[1] = 10;
        g.planets[99] = plan;
        s.carrying["1"] = 20;
        plr.InitPlayer(g, 1);
        plr.earthCredit = 0;

        s.owner = 1;
        s.planet = 99;
        plan.earth = true;
        plan.setGalaxy(g);
        plan.ore[1] = 0;

        Command cmd = new("S101X10R1", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(10 * 10 * 2 / 3, plr.earthCredit);
        Assert.AreEqual(10, s.carrying["1"]);
        Assert.AreEqual(10, plan.ore[1]);
    }

    [TestMethod]
    public void Cmd_BuyOre()
    {
        Galaxy g = new();
        Player plr = new("Betty");
        Ship s = new(g, 1);
        Planet plan = new();
        g.players[1] = plr;
        g.earth_price[1] = 10;
        g.planets[99] = plan;
        s.carrying["1"] = 0;
        plr.InitPlayer(g, 1);
        plr.earthCredit = 100;

        s.owner = 1;
        s.planet = 99;
        s.cargo = 20;
        plan.earth = true;
        plan.setGalaxy(g);
        plan.ore[1] = 10;

        Command cmd = new("S101P10R1", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(100 - 10 * 10, plr.earthCredit);
        Assert.AreEqual(10, s.carrying["1"]);
        Assert.AreEqual(0, plan.ore[1]);
    }

    [TestMethod]
    public void Cmd_SetStandingOrder()
    {
        Galaxy g = new();
        Player plr = new("Betty");
        Ship s = new(g, 1);
        g.players[1] = plr;
        plr.InitPlayer(g, 1);

        s.owner = 1;

        Command cmd = new("OS101P10R1", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual("S101P10R1", s.stndord);
    }
}