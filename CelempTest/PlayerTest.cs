namespace CelempTest;
using Celemp;
using static Celemp.Constants;


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

    [TestMethod]
    public void Cmd_EngageShip()
    {
        Galaxy g = new();
        Planet p1 = new();
        Planet p2 = new();
        Player plr = new("Max");
        Player vic = new("Min");
        Ship s_atk = new(g, 23);
        Ship s_vic = new(g, 24);

        g.players[1] = plr;
        g.players[2] = vic;
        g.planets[0] = p1;
        g.planets[1] = p2;

        p1.link[0] = 1;
        p1.setGalaxy(g);
        p2.setGalaxy(g);

        plr.InitPlayer(g, 1);
        vic.InitPlayer(g, 2);

        s_atk.tractor = 20;
        s_atk.cargo = 5;
        s_atk.planet = 0;
        s_atk.owner = 1;
        s_atk.carrying["0"] = 5;
        s_atk.InitialiseTurn();

        s_vic.cargo = 5;
        s_vic.owner = 2;
        s_vic.planet = 0;
        s_vic.InitialiseTurn();

        Command cmd = new("S123ES124", 1);
        plr.ProcessCommand(cmd);

        plr.OutputLog();
        vic.OutputLog();

        Assert.AreEqual(true, s_vic.IsEngaged());
        Assert.AreEqual(true, s_atk.IsEngaging());

        cmd = new("S123J101", 1);
        plr.ProcessCommand(cmd);

        plr.OutputLog();
        vic.OutputLog();

        Assert.AreEqual(1, s_vic.planet);
        Assert.AreEqual(1, s_atk.planet);
    }

    [TestMethod]
    public void Cmd_ContractCargo()
    {
        Galaxy g = new();
        Planet earth = new();
        Player plr = new();
        Player neut = new("Neutral");

        Ship s = new();

        g.planets[1] = earth;
        g.ships[0] = s;
        plr.InitPlayer(g, 1);
        plr.earthCredit = 100;
        neut.InitPlayer(g, 0);

        earth.owner = 0;
        earth.industry = 20;
        earth.ind_left = 20;
        earth.earth = true;
        s.owner = 1;
        s.cargo = 10;
        s.carrying["1"] = 10;
        s.planet = 1;
        s.SetGalaxy(g);

        Command cmd = new("S100B10C3", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Command resolv = new("ENDCONTRACTING", 0, true);
        neut.ProcessCommand(resolv);

        Assert.AreEqual(10 + 10, s.cargo);
        Assert.AreEqual(0, s.carrying["1"]);
        Assert.AreEqual(20 - 10, earth.ind_left);
        Assert.AreEqual(100 - (10 * 3), plr.earthCredit);
    }

    [TestMethod]
    public void Cmd_Planet_Name()
    {
        Galaxy g = new();
        Planet plan = new();
        Player plr = new("Max");

        g.players[1] = plr;
        g.planets[100] = plan;

        plr.InitPlayer(g, 1);
        plan.number = 100;
        plan.owner = 1;
        plan.setGalaxy(g);

        plan.InitialiseTurn();

        Command cmd = new("200=Testing", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual("Testing", plan.name);
    }
}