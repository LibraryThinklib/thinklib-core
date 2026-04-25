using System;
using System.Collections.Generic;
using System.Text;

namespace Thinklib.Telemetry
{
    [Serializable]
    public class TelemetryEvent
    {
        public string eventName;
        public string package;
        public string packageVersion;
        public string mechanic;
        public string @class;
        public bool isEditor;
        public string unityVersion;
        public string plataform;
        public string projectIdHash;
        public string sessionId;
        public long tsUnixMs;
        public Dictionary<string, object> extra;

        // JSON minimamente seguro para strings
        static string Esc(string s)
        {
            if (s == null) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        static void AppendValue(StringBuilder sb, object o)
        {
            if (o == null) { sb.Append("null"); return; }
            switch (o)
            {
                case bool b: sb.Append(b ? "true" : "false"); break;
                case byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal:
                    sb.Append(Convert.ToString(o, System.Globalization.CultureInfo.InvariantCulture)); break;
                default: sb.Append('"').Append(Esc(o.ToString())).Append('"'); break;
            }
        }

        // Serializa 1 evento para JSON em camelCase
        public string ToJson()
        {
            var sb = new StringBuilder(256);
            sb.Append('{');
            sb.Append("\"eventName\":\"").Append(Esc(eventName)).Append("\",");
            sb.Append("\"package\":\"").Append(Esc(package)).Append("\",");
            sb.Append("\"packageVersion\":\"").Append(Esc(packageVersion)).Append("\",");
            sb.Append("\"mechanic\":\"").Append(Esc(mechanic)).Append("\",");
            sb.Append("\"class\":\"").Append(Esc(@class)).Append("\",");
            sb.Append("\"isEditor\":").Append(isEditor ? "true" : "false").Append(',');
            sb.Append("\"unityVersion\":\"").Append(Esc(unityVersion)).Append("\",");
            sb.Append("\"plataform\":\"").Append(Esc(plataform)).Append("\",");
            sb.Append("\"projectIdHash\":\"").Append(Esc(projectIdHash)).Append("\",");
            sb.Append("\"sessionId\":\"").Append(Esc(sessionId)).Append("\",");
            sb.Append("\"tsUnixMs\":").Append(tsUnixMs).Append(',');
            sb.Append("\"extra\":");
            if (extra == null || extra.Count == 0)
            {
                sb.Append("null");
            }
            else
            {
                sb.Append('{');
                bool first = true;
                foreach (var kv in extra)
                {
                    if (!first) sb.Append(',');
                    first = false;
                    sb.Append('"').Append(Esc(kv.Key)).Append("\":");
                    AppendValue(sb, kv.Value);
                }
                sb.Append('}');
            }
            sb.Append('}');
            return sb.ToString();
        }
    }
}
