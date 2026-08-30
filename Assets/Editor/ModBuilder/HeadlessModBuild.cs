#nullable enable
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BAModTemplate.Editor
{
    /// <summary>
    /// CLI-обёртка для сборки мода без GUI (два прогона, потому что импорт DLL
    /// вызывает перекомпиляцию и перезагрузку домена):
    /// <code>
    /// Unity -batchmode -projectPath sdk \
    ///   -executeMethod BAModTemplate.Editor.HeadlessModBuild.ImportGameDlls -quit
    /// Unity -batchmode -projectPath sdk \
    ///   -executeMethod BAModTemplate.Editor.HeadlessModBuild.BuildMod
    /// </code>
    /// Путь установки игры — env <c>BA_INSTALL_PATH</c> (папка, содержащая
    /// <c>Big Ambitions_Data</c>), id мода — <c>BA_MOD_ID</c> (по умолчанию
    /// NYTaxi), <c>BA_INSTALL_AFTER_BUILD=1</c> — скопировать в ModsLocal.
    /// BuildMod не использует -quit: выходит сам с кодом 0/1 по завершении job.
    /// </summary>
    public static class HeadlessModBuild
    {
        public static void ImportGameDlls()
        {
            var installPath =
                Environment.GetEnvironmentVariable("BA_INSTALL_PATH") ?? string.Empty;
            if (!SteamInstallLocator.IsValidBigAmbitionsInstall(installPath))
            {
                Debug.LogError(
                    $"[HeadlessModBuild] BA_INSTALL_PATH invalid: '{installPath}'");
                EditorApplication.Exit(1);
                return;
            }
            GameDllImporter.SetConfiguredInstallPath(installPath);
            GameDllImporter.Import(installPath);
            Debug.Log("[HeadlessModBuild] Game DLLs imported");
        }

        public static void BuildMod()
        {
            var modId = Environment.GetEnvironmentVariable("BA_MOD_ID") ?? "NYTaxi";
            if (!GameDllImporter.AllDllsPresent())
            {
                Debug.LogError(
                    "[HeadlessModBuild] Game DLLs are not imported; run ImportGameDlls first");
                EditorApplication.Exit(1);
                return;
            }
            var mod = ModDiscovery.DiscoverAll()
                .FirstOrDefault(m => m.Manifest != null && m.Manifest.ModId == modId);
            if (mod == null)
            {
                Debug.LogError($"[HeadlessModBuild] Mod '{modId}' not found");
                EditorApplication.Exit(1);
                return;
            }
            var install =
                Environment.GetEnvironmentVariable("BA_INSTALL_AFTER_BUILD") == "1";
            var job = ModPackager.Enqueue(mod, installAfterBuild: install);
            EditorApplication.update += WaitForJob;
            return;

            void WaitForJob()
            {
                if (!job.IsTerminal)
                    return;
                EditorApplication.update -= WaitForJob;
                foreach (var line in job.Log)
                    Debug.Log("[HeadlessModBuild] " + line);
                foreach (var message in job.CompilerMessages)
                    Debug.Log($"[HeadlessModBuild] compiler: {message.message}");
                if (job.State == BuildState.Failed)
                {
                    Debug.LogError($"[HeadlessModBuild] Build failed: {job.StatusText}");
                    EditorApplication.Exit(1);
                    return;
                }
                Debug.Log(
                    $"[HeadlessModBuild] Build done: {job.OutputDirectoryAbsolute}");
                EditorApplication.Exit(0);
            }
        }
    }
}
