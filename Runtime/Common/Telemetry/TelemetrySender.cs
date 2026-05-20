// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Thinklib.Telemetry
{
    public static class TelemetrySender
    {
        // Coloque false para silenciar logs. Ou defina THINKLIB_TELEMETRY_LOG em Scripting Define Symbols.
        const bool LOG = true;

        public static IEnumerator PostBatch(TelemetryConfig cfg, List<TelemetryEvent> items, Action<bool, long> onDone)
        {
            if (cfg == null || items == null || items.Count == 0)
            {
                if (LOG) D("Skip send: cfg null? {0} | items null? {1} | count {2}", cfg == null, items == null, items?.Count ?? 0);
                onDone?.Invoke(true, 0);
                yield break;
            }

            var url = cfg.apiBase.TrimEnd('/') + cfg.route;
            var body = BuildPayload(items);
            var data = Encoding.UTF8.GetBytes(body);

            if (LOG)
            {
                // Mostra só o início do payload para não poluir o console
                var preview = body.Length > 400 ? body.Substring(0, 400) + " …" : body;
                D("POST {0} | events={1} | bytes={2}\nPayload preview: {3}", url, items.Count, data.Length, preview);
            }

            using var req = new UnityWebRequest(url, "POST");
            req.uploadHandler = new UploadHandlerRaw(data);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            yield return req.SendWebRequest();
            var duration = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start;

            bool ok = req.result == UnityWebRequest.Result.Success && req.responseCode < 300;

            if (LOG)
            {
                D("Response code={0} | result={1} | ok={2} | {3}ms", req.responseCode, req.result, ok, duration);
                if (!ok)
                {
                    D("Error: {0}", req.error);
                    var text = req.downloadHandler != null ? req.downloadHandler.text : null;
                    if (!string.IsNullOrEmpty(text)) D("Body: {0}", text);
                }
            }

            onDone?.Invoke(ok, duration);
        }

        static string BuildPayload(List<TelemetryEvent> items)
        {
            var sb = new StringBuilder(512 + items.Count * 128);
            sb.Append("{\"items\":[");
            for (int i = 0; i < items.Count; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append(items[i].ToJson());
            }
            sb.Append("]}");
            return sb.ToString();
        }

        [System.Diagnostics.Conditional("THINKLIB_TELEMETRY_LOG")]
        static void D_conditional(string fmt, params object[] args) => Debug.Log("[Thinklib][Telemetry] " + string.Format(fmt, args));

        static void D(string fmt, params object[] args)
        {
#if THINKLIB_TELEMETRY_LOG
            D_conditional(fmt, args);
#else
            if (LOG) Debug.Log("[Thinklib][Telemetry] " + string.Format(fmt, args));
#endif
        }
    }
}
