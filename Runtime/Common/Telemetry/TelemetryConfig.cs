using UnityEngine;

namespace Thinklib.Telemetry
{
    public class TelemetryConfig : ScriptableObject
    {
        [Header("Endpoint")]
        public string apiBase = "http://localhost:8080";
        public string route = "/api/analytics/unity-logs";

        [Header("Envio")]
        public int flushIntervalSeconds = 10;
        public int maxBatchSize = 50;
    }
}
