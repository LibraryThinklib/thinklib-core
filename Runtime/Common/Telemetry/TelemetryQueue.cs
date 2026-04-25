using System.Collections.Generic;

namespace Thinklib.Telemetry
{
    public class TelemetryQueue
    {
        readonly List<TelemetryEvent> _buffer = new();
        readonly object _lock = new();

        public int Count { get { lock (_lock) return _buffer.Count; } }

        public void Enqueue(TelemetryEvent ev)
        {
            if (ev == null) return;
            lock (_lock) _buffer.Add(ev);
        }

        public List<TelemetryEvent> Snapshot(int max)
        {
            lock (_lock)
            {
                int take = _buffer.Count < max ? _buffer.Count : max;
                if (take == 0) return new List<TelemetryEvent>(0);
                return new List<TelemetryEvent>(_buffer.GetRange(0, take));
            }
        }

        public void ClearFirstN(int n)
        {
            if (n <= 0) return;
            lock (_lock)
            {
                int rem = n > _buffer.Count ? _buffer.Count : n;
                if (rem > 0) _buffer.RemoveRange(0, rem);
            }
        }
    }
}
