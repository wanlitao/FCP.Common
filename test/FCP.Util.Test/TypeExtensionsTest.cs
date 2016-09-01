using Xunit;

namespace FCP.Util.Test
{
    public class TypeExtensionsTest
    {
        [Fact]
        public void IsAssignableFromTest()
        {
            Assert.True(typeof(TestA).Is<ITest>());
            Assert.True(typeof(TestB).Is<ITest>());
            Assert.True(typeof(TestBGeneric<>).Is<ITest>());

            Assert.False(typeof(TestA).Is(typeof(ITestGeneric<>)));
            Assert.False(typeof(TestB).Is(typeof(ITestGeneric<>)));
            Assert.True(typeof(TestBGeneric<>).Is(typeof(ITestGeneric<>)));
            Assert.True(typeof(TestBGeneric<int>).Is(typeof(ITestGeneric<>)));

            Assert.False(typeof(TestBGeneric<>).Is<ITestGeneric<int>>());
            Assert.False(typeof(TestBGeneric<string>).Is<ITestGeneric<int>>());
            Assert.True(typeof(TestBGeneric<int>).Is<ITestGeneric<int>>());

            Assert.True(typeof(TestB).Is<TestA>());
            Assert.True(typeof(TestBGeneric<>).Is<TestB>());
            Assert.True(typeof(TestBGeneric<>).Is<TestA>());
            Assert.True(typeof(TestBGeneric<int>).Is(typeof(TestBGeneric<>)));

            Assert.False(typeof(ITestGeneric<>).Is<ITest>());
        }
    }

    #region type define
    internal class TestA : ITest
    {

    }

    internal class TestB : TestA
    {

    }

    internal class TestBGeneric<T> : TestB, ITestGeneric<T>
    {

    }

    internal interface ITest
    {

    }

    internal interface ITestGeneric<T>
    {

    }
    #endregion
}