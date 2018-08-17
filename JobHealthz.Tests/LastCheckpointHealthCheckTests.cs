using System;
using System.Threading;
using Xunit;

namespace JobHealthz.Tests
{
    public class LastCheckpointHealthCheckTests
    {
        [Fact]
        public void ShouldReturnFail_WhenCheckpointOutsideThreshold()
        {
            var target = new LastCheckpointHealthCheck(TimeSpan.FromSeconds(1));

            target.Checkpoint();

            Thread.Sleep(2000);

            var result = target.Check();

            Assert.Equal(HealthzStatus.Fail, result.Status);
            Assert.Matches(@"Last checkpoint: [\d-:TZ]+. Elapsed time: [\d:\.]+. Threshold [\d:\.]+.", result.Message);
        }
        
        [Fact]
        public void ShouldReturnOk_WhenCheckpointWithinThreshold()
        {
            var target = new LastCheckpointHealthCheck(TimeSpan.FromSeconds(5));

            target.Checkpoint();

            Thread.Sleep(1000);

            var result = target.Check();

            Assert.Equal(HealthzStatus.Ok, result.Status);
            Assert.Matches(@"Last checkpoint: [\d-:TZ]+. Elapsed time: [\d:\.]+. Threshold [\d:\.]+.", result.Message);
        }
    }
}