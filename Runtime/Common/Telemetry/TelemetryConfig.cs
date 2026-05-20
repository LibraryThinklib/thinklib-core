// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

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
