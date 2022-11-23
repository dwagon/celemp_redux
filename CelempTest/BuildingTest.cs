namespace CelempTest;
using Celemp;
using static Celemp.Constants;

[TestClass]
public class BuildingTest
{
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
    public void Cmd_BuildCargo()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new();
        Ship s = g.InitShip();

        g.planets[1] = plan;
        plr.InitPlayer(g, 1);

        plan.owner = 1;
        plan.industry = 20;
        plan.ind_left = 20;
        plan.ore[1] = 20;
        s.owner = 1;
        s.planet = 1;

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
        Ship s = g.InitShip();

        g.planets[1] = plan;
        plr.InitPlayer(g, 1);

        plan.owner = 1;
        plan.industry = 20;
        plan.ind_left = 20;
        plan.ore[2] = 10;
        plan.ore[3] = 8;
        s.owner = 1;
        s.planet = 1;

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
        Ship s = g.InitShip();

        g.planets[1] = plan;
        plr.InitPlayer(g, 1);

        plan.owner = 1;
        plan.industry = 20;
        plan.ind_left = 12;
        plan.ore[5] = 20;
        plan.ore[6] = 20;

        s.owner = 1;
        s.planet = 1;

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
    public void Cmd_Planet_Build_Hyperdrive()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new("Max");
        Ship s = g.InitShip();

        int fighter = 1;
        int cargo = 2;
        int tractor = 3;
        int shield = 4;

        g.players[1] = plr;
        g.planets[100] = plan;

        plr.InitPlayer(g, 1);
        plan.number = 100;
        plan.owner = 1;
        plan.industry = 80;
        plan.ore[1] = 30;
        plan.ore[2] = 30;
        plan.ore[3] = 30;
        plan.ore[4] = 30;
        plan.ore[5] = 30;
        plan.ore[6] = 30;
        plan.ore[7] = 30;
        plan.setGalaxy(g);
        plan.InitialiseTurn();

        Command cmd = new($"200B1H{fighter}/{cargo}/{tractor}/{shield}", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(80 - 40 - fighter * 2 - cargo - tractor * 2 - shield * 2, plan.ind_left);
        Assert.AreEqual(30 - cargo, plan.ore[1]);
        Assert.AreEqual(30 - fighter, plan.ore[2]);
        Assert.AreEqual(30 - fighter, plan.ore[3]);
        Assert.AreEqual(30 - 10, plan.ore[4]);
        Assert.AreEqual(30 - 10 - shield, plan.ore[5]);
        Assert.AreEqual(30 - 10 - shield, plan.ore[6]);
        Assert.AreEqual(30 - 10 - tractor * 2, plan.ore[7]);

        Assert.AreEqual(1, plan.ShipsOrbitting().Count);

        foreach (var ship in plan.ShipsOrbitting())
        {
            Assert.AreEqual(fighter, ship.fighter);
            Assert.AreEqual(cargo, ship.cargo);
            Assert.AreEqual(tractor, ship.tractor);
            Assert.AreEqual(shield, ship.shield);
            Assert.AreEqual(1, ship.number);
        }
    }

    [TestMethod]
    public void Cmd_Unbuild_Cargo()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new();
        Ship s = g.InitShip();

        g.planets[1] = plan;
        plr.InitPlayer(g, 1);

        plan.owner = 1;
        plan.industry = 20;
        plan.ind_left = 20;
        plan.ore[1] = 20;
        s.owner = 1;
        s.planet = 1;
        s.cargo = 20;

        Command cmd = new("S100Z10C", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(20 - 10, s.cargo);
        Assert.AreEqual(20 - 10, plan.ind_left);
        Assert.AreEqual(20 + 10 / 2, plan.ore[1]);
    }
}

