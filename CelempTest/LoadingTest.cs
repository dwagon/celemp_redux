namespace CelempTest;
using Celemp;
using static Celemp.Constants;

[TestClass]
public class LoadingTest
{

    [TestMethod]
    public void Cmd_LoadPDU()
    {
        Galaxy g = new();
        Planet pt = new();
        Player plr = new("Max");
        Ship s = new();
        s.SetGalaxy(g);
        s.planet = 123;
        s.cargo = 10;
        s.owner = 2;
        s.carrying[cargo_pdu] = 0;
        g.ships[33] = s;
        g.planets[123] = pt;
        pt.pdu = 9;
        pt.owner = 2;
        plr.InitPlayer(g, 2);

        Command cmd = new("S133L6D", 2);
        plr.Cmd_LoadPDU(cmd);
        plr.OutputLog();

        // Should be constrained by available cargo
        Assert.AreEqual(4, pt.pdu);
        Assert.AreEqual(5, s.carrying[cargo_pdu]);
        Assert.AreEqual(0, s.CargoLeft());
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
        s.carrying[cargo_pdu] = 10;
        s.owner = 1;
        s.planet = 100;
        s.SetGalaxy(g);

        Command cmd = new("S100U10D", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(pln.pdu, 10);
        Assert.AreEqual(0, s.carrying[cargo_pdu]);
        Assert.AreEqual(20, s.CargoLeft());
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
        s.carrying[cargo_pdu] = 5;

        s.owner = 1;
        s.planet = 100;

        Command cmd = new("S100U4D", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(5 - 4, s.carrying[cargo_pdu]);
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
        s.carrying[cargo_mine] = 3;

        s.owner = 1;
        s.planet = 100;

        Command cmd = new("S100U2M1", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(3 - 2, s.carrying[cargo_mine]);
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
        s.carrying[cargo_industry] = 2;

        s.owner = 1;
        s.planet = 100;

        Command cmd = new("S100U1I", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(2 - 1, s.carrying[cargo_industry]);
        Assert.AreEqual(20 - (1 * 10), s.CargoLeft());
        Assert.AreEqual(1, pln.industry);
    }

    [TestMethod]
    public void Test_Cmd_TendIndustry()
    {
        Galaxy g = new();
        Planet pln = new();
        Player plr = new("Max");
        Ship s1 = new(g, 0);
        Ship s2 = new(g, 1);

        g.planets[100] = pln;
        plr.InitPlayer(g, 1);

        s1.cargo = 30;
        s1.carrying[cargo_industry] = 2;
        s1.owner = 1;
        s1.planet = 100;

        s2.cargo = 30;
        s2.carrying[cargo_industry] = 0;
        s2.owner = 1;
        s2.planet = 100;

        Command cmd = new("S100T1S101I", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(1, s1.carrying[cargo_industry]);
        Assert.AreEqual(1, s2.carrying[cargo_industry]);
    }

    [TestMethod]
    public void Test_Cmd_TendAll()
    {
        Galaxy g = new();
        Planet pln = new();
        Player plr = new("Max");
        Ship s1 = new(g, 0);
        Ship s2 = new(g, 1);

        g.planets[100] = pln;
        plr.InitPlayer(g, 1);

        s1.cargo = 30;
        s1.carrying["0"] = 5;
        s1.carrying["1"] = 10;
        s1.carrying["2"] = 10;
        s1.carrying["3"] = 5;
        s1.owner = 1;
        s1.planet = 100;

        s2.cargo = 30;
        s2.carrying["0"] = 0;
        s2.carrying["1"] = 0;
        s2.carrying["2"] = 0;
        s2.carrying["3"] = 0;
        s2.owner = 1;
        s2.planet = 100;

        Command cmd = new("S100TS101", 1);
        plr.ProcessCommand(cmd);
        plr.OutputLog();

        Assert.AreEqual(5, s1.carrying["0"]);
        Assert.AreEqual(0, s1.carrying["1"]);
        Assert.AreEqual(0, s1.carrying["2"]);
        Assert.AreEqual(0, s1.carrying["3"]);
        Assert.AreEqual(0, s2.carrying["0"]);
        Assert.AreEqual(10, s2.carrying["1"]);
        Assert.AreEqual(10, s2.carrying["2"]);
        Assert.AreEqual(5, s2.carrying["3"]);
    }
}

