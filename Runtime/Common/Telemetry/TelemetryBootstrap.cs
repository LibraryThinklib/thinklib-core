// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.IO;
using UnityEngine;

namespace Thinklib.Telemetry
{
    public static class TelemetryBootstrap
    {
        const string ConfigResourceName = "ThinklibTelemetryConfig";
        const string ProjectGuidFile = "thinklib_project_guid.txt";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Boot()
        {
            var cfg = Resources.Load<TelemetryConfig>(ConfigResourceName);
            if (cfg == null)
            {
                // Fallback for development only, so it doesn't break
                cfg = ScriptableObject.CreateInstance<TelemetryConfig>();
            }

            var go = new GameObject("ThinklibTelemetry");
            UnityEngine.Object.DontDestroyOnLoad(go);
            var runtime = go.AddComponent<TelemetryRuntime>();

            var queue = new TelemetryQueue();
            var projectId = LoadOrCreateProjectGuid();
            var sessionId = Guid.NewGuid().ToString("N");

            runtime.Init(cfg, queue, projectId, sessionId);
        }

        static string LoadOrCreateProjectGuid()
        {
            try
            {
                var path = Path.Combine(Application.persistentDataPath, ProjectGuidFile);
                if (File.Exists(path))
                {
                    var txt = File.ReadAllText(path).Trim();
                    if (!string.IsNullOrEmpty(txt)) return txt;
                }
                var guid = Guid.NewGuid().ToString("N");
                File.WriteAllText(path, guid);
                return guid;
            }
            catch { return Guid.NewGuid().ToString("N"); }
        }
    }
}
