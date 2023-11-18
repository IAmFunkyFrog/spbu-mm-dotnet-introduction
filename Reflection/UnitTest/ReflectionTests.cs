using Reflection;

namespace UnitTest;

public class ReflectionTests
{
    private class TestClass1
    {
        public int Prop1 { get; set; }
    }

    private class TestSubclassForClass1 : TestClass1
    {
    }

    private class TestClass2
    {
        public TestClass1? Prop1 { get; set; }
        public TestSubclassForClass1? Prop2 { get; set; }
    }

    private class TestClass3
    {
        private TestClass1? Prop1 { get; set; }

        public void setProp1(TestClass1? val) {
            Prop1 = val;
        }
    }

    private class TestClass4
    {
        private static TestClass1? Prop1 { get; set; }

        public void setProp1(TestClass1? val) {
            Prop1 = val;
        }
    }

    [Test]
    public void SimpleAccessor()
    {
        var obj = new TestClass1();
        var accessor = ReflectionDelegateFactory.MakePropertyAccessor<TestClass1>("Prop1");

        int testValue = -1234;
        obj.Prop1 = testValue;

        Assert.That(testValue, Is.EqualTo(accessor.Invoke(obj)));
    }

    [Test]
    public void ChainAccessor()
    {
        var obj1 = new TestClass1();
        var obj2 = new TestClass2();
        var accessor = ReflectionDelegateFactory.MakePropertyAccessor<TestClass2, int>("Prop1.Prop1");

        int testValue = -1234;
        obj1.Prop1 = testValue;
        obj2.Prop1 = obj1;

        Assert.That(testValue, Is.EqualTo(accessor.Invoke(obj2)));
    }

    [Test]
    public void ChainAccessorWithSuperClassReturnValue()
    {
        var obj = new TestClass2();
        var subclass = new TestSubclassForClass1();
        var accessor = ReflectionDelegateFactory.MakePropertyAccessor<TestClass2, TestClass1>("Prop2");

        obj.Prop2 = subclass;

        Assert.That(subclass, Is.EqualTo(accessor.Invoke(obj)));
    }

    [Test]
    public void TypeCheckedChainAccessorFail()
    {
        var obj1 = new TestClass1();
        var obj2 = new TestClass2();
        try
        {
            var accessor = ReflectionDelegateFactory.MakePropertyAccessor<TestClass2, float>("Prop1.Prop1");
        } catch (Exception e) {
            Assert.IsTrue(e is ArgumentException);
            return;
        }
        Assert.IsTrue(false);
    }

    [Test]
    public void PrivateAccessorReturnsValue()
    {
        var obj1 = new TestClass1();
        var obj3 = new TestClass3();
        var accessor = ReflectionDelegateFactory.MakePropertyAccessor<TestClass3, int>("Prop1.Prop1");

        int testValue = -1234;
        obj1.Prop1 = testValue;
        obj3.setProp1(obj1);

        Assert.That(testValue, Is.EqualTo(accessor.Invoke(obj3)));
    }

    [Test]
    public void PrivateStaticAccessorReturnsValue()
    {
        var obj1 = new TestClass1();
        var obj4 = new TestClass4();
        var accessor = ReflectionDelegateFactory.MakePropertyAccessor<TestClass4, int>("Prop1.Prop1");

        int testValue = -1234;
        obj1.Prop1 = testValue;
        obj4.setProp1(obj1);

        Assert.That(testValue, Is.EqualTo(accessor.Invoke(obj4)));
    }
}