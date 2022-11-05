namespace CelempTest;
using Celemp;

[TestClass]
public class CommandTest
{
    [TestMethod]
    public void TestJump1()
    {
        Command cmd = new("S123J234", 1);
        Assert.AreEqual(134, cmd.numbers["jump1"]);
        Assert.AreEqual(CommandOrder.JUMP1, cmd.priority);
        Assert.AreEqual(23, cmd.numbers["ship"]);
        Assert.AreEqual(1, cmd.plrNum);
    }

    [TestMethod]
    public void TestJump5()
    {
        Command cmd = new("S101J120J131J142J153J164", 2);
        Assert.AreEqual(20, cmd.numbers["jump1"]);
        Assert.AreEqual(31, cmd.numbers["jump2"]);
        Assert.AreEqual(42, cmd.numbers["jump3"]);
        Assert.AreEqual(53, cmd.numbers["jump4"]);
        Assert.AreEqual(64, cmd.numbers["jump5"]);
        Assert.AreEqual(CommandOrder.JUMP5, cmd.priority);
    }
}
