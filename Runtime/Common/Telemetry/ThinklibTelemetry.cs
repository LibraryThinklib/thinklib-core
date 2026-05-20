// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Thinklib.Telemetry
{
    public static class ThinklibTelemetry
    {
        // Fonte da verdade vem do arquivo gerado pelo Editor: ThinklibVersion.cs
        public static string Package => ThinklibVersion.PackageName;
        public static string PackageVersion => ThinklibVersion.PackageVersion;

        static string _projectIdHash;
        static string _sessionId;

        static TelemetryQueue _queue;
        static bool _initialized;

        internal static void Initialize(TelemetryQueue queue, string projectIdHash, string sessionId)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _projectIdHash = string.IsNullOrWhiteSpace(projectIdHash) ? "unknown_project" : projectIdHash;
            _sessionId = string.IsNullOrWhiteSpace(sessionId) ? Guid.NewGuid().ToString("N") : sessionId;
            _initialized = true;
        }

        public static void Track(string eventName, string mechanic, string className, Dictionary<string, object> extra = null)
        {
            if (!_initialized) return;
            if (string.IsNullOrEmpty(eventName)) return;

            var e = new TelemetryEvent
            {
                eventName   = eventName,
                package     = Package,
                packageVersion = PackageVersion,
                mechanic    = mechanic ?? "",
                @class      = className ?? "",
                isEditor    = Application.isEditor,
                unityVersion= Application.unityVersion,
                plataform    = Application.platform.ToString(),
                projectIdHash = _projectIdHash,
                sessionId   = _sessionId,
                tsUnixMs    = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                extra       = extra
            };

            _queue.Enqueue(e);
        }
    }
}
