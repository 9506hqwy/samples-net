namespace Example.Test;

#pragma warning disable CA1515
[TestClass]
public class FuncTest
{
    [TestMethod]
    public void TestAdd()
    {
        Assert.AreEqual(3, Func.Add(2, 1));
    }

    [TestMethod]
    public void TestSub()
    {
        Assert.AreEqual(1, Func.Sub(2, 1));
    }
}
#pragma warning restore CA1515
