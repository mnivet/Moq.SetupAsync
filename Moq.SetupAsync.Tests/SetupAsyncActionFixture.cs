using System;
using System.Threading.Tasks;
using Xunit;

namespace Moq.SetupAsync.Tests
{
    public class SetupAsyncActionFixture
    {
        [Fact]
        public async Task SetupAsyncActionCanBeAwaited()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.DoActionAsync());

            await mock.Object.DoActionAsync();

            mock.VerifyAll();
        }

        [Fact]
        public async Task SetupAsyncActionWithOneArgCanBeAwaited()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.DoActionAsync(It.IsAny<string>()));

            await mock.Object.DoActionAsync("any string");

            mock.VerifyAll();
        }

        [Fact]
        public async Task SetupAsyncActionIsVerifiable()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.DoActionAsync()).Verifiable();
            mock.Verify(x => x.DoActionAsync(), Times.Never);

            await mock.Object.DoActionAsync();

            mock.Verify(x => x.DoActionAsync(), Times.Once);
            mock.Verify();
        }

        [Fact]
        public async Task SetupAsyncActionWithOneArgIsVerifiable()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.DoActionAsync(It.IsAny<string>())).Verifiable();
            mock.Verify(x => x.DoActionAsync("any string"), Times.Never);

            await mock.Object.DoActionAsync("any string");

            mock.Verify(x => x.DoActionAsync("any string"), Times.Once);
            mock.Verify();
        }

        [Fact]
        public void SetupAsyncActionIsVerifiableAndErrorMessageCanBeSpecified()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.DoActionAsync()).Verifiable("custom fail message");

            var exception = Assert.Throws<MockException>(() => mock.Verify());
            Assert.Contains("custom fail message", exception.Message);
        }

        [Fact]
        public async Task SetupAsyncActionWithCallback()
        {
            var mock = new Mock<IFoo>();
            var isCalled = false;
            mock.SetupAsync(x => x.DoActionAsync()).Callback(() => isCalled = true);

            await mock.Object.DoActionAsync();

            Assert.True(isCalled);
        }

        [Fact]
        public async Task SetupAsyncActionWithOneArgWithCallback()
        {
            var mock = new Mock<IFoo>();
            var isCalled = false;
            mock.SetupAsync(x => x.DoActionAsync(It.IsAny<string>())).Callback<string>(s => isCalled = s == "any string");

            await mock.Object.DoActionAsync("any string");

            Assert.True(isCalled);
        }

        [Fact]
        public async Task SetupAsyncActionWithThrows()
        {
            var mock = new Mock<IFoo>();
            mock.SetupAsync(x => x.DoActionAsync()).Throws<ArgumentException>();
            var task = mock.Object.DoActionAsync();

            await Assert.ThrowsAsync<ArgumentException>(() => task);
        }

        public interface IFoo
        {
            Task DoActionAsync();
            Task DoActionAsync(string arg1);
        }
    }
}
