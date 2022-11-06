namespace CelempTest;
using Celemp;

[TestClass]
public class GalaxyTest
{
    [TestMethod]
    public void Test_GuessPlayerName()
    {
        Galaxy g = new();
        Player p0 = new("Neutral");
        Player p1 = new("Alpha");
        Player p2 = new("Beta");
        Player p3 = new("Gamma");

        g.players[0] = p0;
        p1.InitPlayer(g, 1);
        g.players[1] = p1;
        p2.InitPlayer(g, 2);
        g.players[2] = p2;
        p3.InitPlayer(g, 3);
        g.players[3] = p3;

        Assert.AreEqual(1, g.GuessPlayerName("Alpha"));
        Assert.AreEqual(-1, g.GuessPlayerName("Foo"));
    }

}