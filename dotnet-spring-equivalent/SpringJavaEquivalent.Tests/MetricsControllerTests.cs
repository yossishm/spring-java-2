namespace SpringJavaEquivalent.Tests
{
    using Microsoft.AspNetCore.Mvc;
    using SpringJavaEquivalent.Controllers;
    using Xunit;

    public class MetricsControllerTests
    {
        private readonly MetricsController controller;

        public MetricsControllerTests()
        {
            this.controller = new MetricsController();
        }

        [Fact]
        public async Task TestMetrics_ShouldReturnString()
        {
            // Act
            var result = await this.controller.TestMetrics();

            // Assert
            Assert.IsType<string>(result);
            Assert.Equal("Metrics test completed", result);
        }

        [Fact]
        public void IncrementCounter_ShouldReturnString()
        {
            // Act
            var result = this.controller.IncrementCounter(5);

            // Assert
            Assert.IsType<string>(result);
            Assert.Equal("Counter incremented by 5", result);
        }

        [Fact]
        public async Task SlowEndpoint_ShouldReturnString()
        {
            // Act
            var result = await this.controller.SlowEndpoint();

            // Assert
            Assert.IsType<string>(result);
            Assert.Equal("Slow operation completed", result);
        }
    }
}