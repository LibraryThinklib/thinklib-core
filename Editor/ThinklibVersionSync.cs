#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;
using UPMInfo = UnityEditor.PackageManager.PackageInfo;


namespace Thinklib.Telemetry.Editor
{
    [InitializeOnLoad]
    public static class ThinklibVersionSync
    {
        // ID do pacote (veio do "name" no package.json)
        const string PackageName = "com.thinklib.core";
        const string OutputRelPath = "Runtime/Common/Telemetry/ThinklibVersion.cs";

        [System.Serializable] class Pkg { public string name; public string version; }

        static ThinklibVersionSync()
        {
            // dispara em todo domain reload
            TrySync(silent:true);
        }

        [MenuItem("Thinklib/Force Version Sync")]
        public static void ForceSyncMenu()
        {
            TrySync(silent:false);
        }

        static void TrySync(bool silent)
        {
            try
            {
                // Descobre o caminho resolvido do pacote via PackageInfo
                var pi = UPMInfo.FindForAssetPath($"Packages/{PackageName}");
                if (pi == null)
                {
                    if (!silent) Debug.LogWarning($"[Thinklib] PackageInfo not found for {PackageName}");
                    return;
                }

                var packageDir = pi.resolvedPath.Replace('\\','/');
                var packageJsonPath = Path.Combine(packageDir, "package.json").Replace('\\','/');
                var outputPath = Path.Combine(packageDir, OutputRelPath).Replace('\\','/');

                if (!File.Exists(packageJsonPath))
                {
                    if (!silent) Debug.LogWarning($"[Thinklib] package.json not found at {packageJsonPath}");
                    return;
                }

                var json = File.ReadAllText(packageJsonPath);
                var pkg  = JsonUtility.FromJson<Pkg>(json);
                if (pkg == null || string.IsNullOrEmpty(pkg.name) || string.IsNullOrEmpty(pkg.version))
                {
                    if (!silent) Debug.LogWarning($"[Thinklib] Could not parse package.json at {packageJsonPath}");
                    return;
                }

                var code =
$@"// AUTO-GERADO a partir de {packageJsonPath}. NÃO editar manualmente.
namespace Thinklib.Telemetry
{{
    internal static class ThinklibVersion
    {{
        public const string PackageName = ""{pkg.name}"";
        public const string PackageVersion = ""{pkg.version}"";
    }}
}}
";
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

                var write = true;
                if (File.Exists(outputPath))
                {
                    var current = File.ReadAllText(outputPath);
                    if (current == code) write = false;
                }
                if (write)
                {
                    File.WriteAllText(outputPath, code);
                    AssetDatabase.Refresh();
                    if (!silent) Debug.Log($"[Thinklib] Version sync OK → {pkg.name} v{pkg.version}\n→ {outputPath}");
                }
                else
                {
                    if (!silent) Debug.Log($"[Thinklib] Version already up-to-date → {pkg.name} v{pkg.version}");
                }
            }
            catch (System.Exception ex)
            {
                if (!silent) Debug.LogError($"[Thinklib] Version sync error: {ex.Message}");
            }
        }
    }
}
#endif
