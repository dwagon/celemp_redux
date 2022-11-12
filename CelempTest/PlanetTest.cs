namespace CelempTest;
using Celemp;

[TestClass]
public class PlanetTest
{
    [TestMethod]
    public void TestIncome()
    {
        Planet emptypln = new();
        Assert.AreEqual(20, emptypln.Income());

        Planet gotstuffpln = new();
        gotstuffpln.industry = 5;
        gotstuffpln.mine[4] = 2;
        Assert.AreEqual(20 + 5 * 5 + 2, gotstuffpln.Income());
    }

    [TestMethod]
    public void TestOwnership_NoShips()
    {
        Galaxy g = new();
        Planet p = new();
        p.owner = 1;
        p.number = 100;
        g.planets[100] = p;
        p.setGalaxy(g);
        g.ships = new();

        p.EndTurn();
        Assert.AreEqual(1, p.owner);
    }

    [TestMethod]
    public void TestOwnership_SingleShip()
    {
        Galaxy g = new();
        Planet p = new();
        Ship s1 = new();
        Player p1 = new();
        Player p2 = new();
        g.players[1] = p1;
        g.players[2] = p2;
        p1.InitPlayer(g, 1);
        p2.InitPlayer(g, 2);

        p.setGalaxy(g);
        p.owner = 1;
        p.number = 100;
        s1.planet = 100;
        s1.cargo = 5;
        s1.owner = 2;
        g.ships[0] = s1;

        p.EndTurn();
        Assert.AreEqual(2, p.owner);
    }

    [TestMethod]
    public void TestOwnership_DefendedPlanet()
    {
        Galaxy g = new();
        Planet p = new();
        Ship s1 = new();

        p.setGalaxy(g);
        p.pdu = 5;
        p.owner = 2;
        s1.owner = 3;
        p.EndTurn();
        Assert.AreEqual(2, p.owner);
    }

    [TestMethod]
    public void TestOwnership_MultiShip()
    {
        Galaxy g = new();
        Planet p = new();
        Ship s1 = new();
        Ship s2= new();

        p.setGalaxy(g);
        p.pdu = 0;
        p.owner = 1;
        s1.owner = 2;
        s2.owner = 3;
        s1.planet = 100;
        s2.planet = 100;
        g.ships[0] = s1;
        g.ships[1] = s2;
        p.EndTurn();
        Assert.AreEqual(1, p.owner, "Multiple ships");
    }
}