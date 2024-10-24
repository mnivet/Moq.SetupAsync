using System;
using System.Threading.Tasks;
using Xunit;

namespace Moq.SetupAsync.Tests
{
    public class SetupAsyncFuncFixture
    {
        [Fact]
        public async Task SetupAsyncFuncCanBeAwaited()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.ExecFuncAsync());

            await mock.Object.ExecFuncAsync();

            mock.VerifyAll();
        }

        [Fact]
        public async Task SetupAsyncFuncWithOneArgCanBeAwaited()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.ExecFuncAsync(It.IsAny<string>()));

            await mock.Object.ExecFuncAsync("any string");

            mock.VerifyAll();
        }

        [Fact]
        public async Task SetupAsyncFuncIsVerifiable()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.ExecFuncAsync()).Verifiable();
            mock.Verify(x => x.ExecFuncAsync(), Times.Never);

            await mock.Object.ExecFuncAsync();

            mock.Verify(x => x.ExecFuncAsync(), Times.Once);
            mock.Verify();
        }

        [Fact]
        public async Task SetupAsyncFuncWithOneArgIsVerifiable()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.ExecFuncAsync(It.IsAny<string>())).Verifiable();
            mock.Verify(x => x.ExecFuncAsync("any string"), Times.Never);

            await mock.Object.ExecFuncAsync("any string");

            mock.Verify(x => x.ExecFuncAsync("any string"), Times.Once);
            mock.Verify();
        }

        [Fact]
        public void SetupAsyncFuncIsVerifiableAndErrorMessageCanBeSpecified()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.ExecFuncAsync()).Verifiable("custom fail message");

            var exception = Assert.Throws<MockException>(() => mock.Verify());
            Assert.Contains("custom fail message", exception.Message);
        }

        [Fact]
        public async Task SetupAsyncFuncReturnsNullByDefault()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.ExecFuncAsync());

            Assert.Null(await mock.Object.ExecFuncAsync());
        }

        [Fact]
        public async Task SetupAsyncFuncWithStaticReturns()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.ExecFuncAsync()).Returns("custom return");

            Assert.Equal("custom return", await mock.Object.ExecFuncAsync());
        }

        [Fact]
        public async Task SetupAsyncFuncWithComputedReturns()
        {
            var mock = new Mock<IFoo>();
            var isCalled = false;
            mock.SetupAsync(x => x.ExecFuncAsync()).Returns(() =>
            {
                isCalled = true;
                return "custom return";
            });

            Assert.Equal("custom return", await mock.Object.ExecFuncAsync());
            Assert.True(isCalled);
        }

        [Fact]
        public async Task SetupAsyncFuncWithOneArgWithReturnsThatDependsOnArg()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.ExecFuncAsync(It.IsAny<string>())).Returns<string>(s => "hello " + s);

            var result = await mock.Object.ExecFuncAsync("any string");

            Assert.Equal("hello any string", result);
        }

        [Fact]
        public async Task SetupAsyncFuncWithThrows()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.ExecFuncAsync()).Throws<ArgumentException>();
            var task = mock.Object.ExecFuncAsync();

            await Assert.ThrowsAsync<ArgumentException>(() => task);
        }

        public interface IFoo
        {
            Task<object> ExecFuncAsync();
            Task<object> ExecFuncAsync(string arg1);
        }
    }
}
