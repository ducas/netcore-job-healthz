using System;

namespace JobHealthz
{
    public class LastCheckpointHealthCheck : IHealthCheck
    {
        const string MessageFormat = "Last checkpoint: {0:s}Z. Elapsed time: {1:g}. Threshold {2:g}.";

        private readonly TimeSpan _tolerance;
        private DateTime _lastCheckpoint;

        public LastCheckpointHealthCheck(TimeSpan tolerance)
        {
            _tolerance = tolerance;
            _lastCheckpoint = DateTime.UtcNow;
        }

        public void Checkpoint()
        {
            _lastCheckpoint = DateTime.UtcNow;
        }

        public HealthzResult Check()
        {
            var elapsed = DateTime.UtcNow - _lastCheckpoint;
            var expired = elapsed > _tolerance;

            return new HealthzResult
            {
                Status = expired ? HealthzStatus.Fail : HealthzStatus.Ok,
                Message = string.Format(MessageFormat, _lastCheckpoint, elapsed, _tolerance)
            };
        }
    }
}