// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thinklib.Telemetry
{
    [DefaultExecutionOrder(-10000)]
    public class TelemetryRuntime : MonoBehaviour
    {
        [SerializeField] TelemetryConfig config;

        TelemetryQueue _queue;
        float _elapsed;
        bool _sending;

        public void Init(TelemetryConfig cfg, TelemetryQueue queue, string projectIdHash, string sessionId)
        {
            config = cfg;
            _queue = queue;
            ThinklibTelemetry.Initialize(_queue, projectIdHash, sessionId);
        }

        void Update()
        {
            if (config == null || _queue == null) return;

            _elapsed += Time.unscaledDeltaTime;
            if (_elapsed < Mathf.Max(1, config.flushIntervalSeconds)) return;
            _elapsed = 0f;

            if (_sending) return;
            if (_queue.Count == 0) return;

            var batch = _queue.Snapshot(Mathf.Max(1, config.maxBatchSize));
            if (batch.Count == 0) return;

            _sending = true;
            StartCoroutine(Send(batch));
        }

        IEnumerator Send(List<TelemetryEvent> batch)
        {
            bool ok = false;
            long dur = 0;
            yield return TelemetrySender.PostBatch(config, batch, (success, durationMs) =>
            {
                ok = success;
                dur = durationMs;
            });

            if (ok)
            {
                _queue.ClearFirstN(batch.Count);
            }
            _sending = false;
        }

        void OnApplicationQuit()
        {
            if (config == null || _queue == null) return;
            if (_queue.Count == 0) return;

            var batch = _queue.Snapshot(Mathf.Max(1, config.maxBatchSize));
            if (batch.Count == 0) return;

#if UNITY_WEBGL
            // WebGL doesn't allow blocking; leave it for the next load
#else
            var cr = TelemetrySender.PostBatch(config, batch, (ok, _) =>
            {
                if (ok) _queue.ClearFirstN(batch.Count);
            });
            var e = cr;
            while (e.MoveNext()) { } // final synchronous attempt on quit (native platforms)
#endif
        }
    }
}
