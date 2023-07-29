using System;
using System.Collections.Generic;
using System.Linq;
using SingularityGroup.HotReload.DTO;

namespace SingularityGroup.HotReload.Editor {
    internal static class EditorIndicationState {
        internal enum IndicationStatus {  
            Stopped,
            Login,
            Stopping,
            Installing,
            Starting,
            Idle,
            Unsupported,
            Patching,
            Loading,
            Compiling,
            CompileErrors,
            ActivationFailed
        }
        
        private static readonly Dictionary<IndicationStatus, string> IndicationIcon = new Dictionary<IndicationStatus, string> {
            // grey icon:
            { IndicationStatus.Stopped, "winbtn_mac_inact@2x" },
            { IndicationStatus.Login, "winbtn_mac_inact@2x" },
            // green icon:
            { IndicationStatus.Idle, "winbtn_mac_max@2x" },
            // orange icon:
            { IndicationStatus.Unsupported, "d_winbtn_mac_min@2x" },
            // spinner:
            { IndicationStatus.Stopping, Spinner.SpinnerIconPath },
            { IndicationStatus.Starting, Spinner.SpinnerIconPath },
            { IndicationStatus.Patching, Spinner.SpinnerIconPath },
            { IndicationStatus.Loading, Spinner.SpinnerIconPath },
            { IndicationStatus.Compiling, Spinner.SpinnerIconPath },
            { IndicationStatus.Installing, Spinner.SpinnerIconPath },
            // red icon:
            { IndicationStatus.CompileErrors, "d_winbtn_mac_close@2x" },
            { IndicationStatus.ActivationFailed, "d_winbtn_mac_close@2x" },
        };
        
        private static readonly IndicationStatus[] SpinnerIndications = IndicationIcon
            .Where(kvp => kvp.Value == Spinner.SpinnerIconPath)
            .Select(kvp => kvp.Key)
            .ToArray();
        
        private static readonly Dictionary<IndicationStatus, string> IndicationText = new Dictionary<IndicationStatus, string> {
            { IndicationStatus.Stopping, "Stopping Hot Reload" },
            { IndicationStatus.Stopped, "Run Hot Reload" },
            { IndicationStatus.Installing, "Installing" },
            { IndicationStatus.Starting, "Starting Hot Reload" },
            { IndicationStatus.Idle, "All patches applied" },
            { IndicationStatus.Unsupported, "Latest patch failed" },
            { IndicationStatus.Patching, "Applying patch" },
            { IndicationStatus.Compiling, "Compiling" },
            { IndicationStatus.CompileErrors, "Scripts have compile errors" },
            { IndicationStatus.ActivationFailed, "Activation failed" },
            { IndicationStatus.Loading, "Loading" },
        };
        
        private const int MinSpinnerDuration = 200;
        private static DateTime spinnerStartedAt;
        private static IndicationStatus latestStatus;
        private static bool SpinnerCompletedMinDuration => DateTime.UtcNow - spinnerStartedAt > TimeSpan.FromMilliseconds(MinSpinnerDuration);
        private static IndicationStatus GetIndicationStatus() {
            var status = GetIndicationStatusCore();
            var newStatusIsSpinner = SpinnerIndications.Contains(status);
            var latestStatusIsSpinner = SpinnerIndications.Contains(latestStatus);
            if (status == latestStatus) {
                return status;
            } else if (latestStatusIsSpinner) {
                if (newStatusIsSpinner) {
                    return status;
                } else if (SpinnerCompletedMinDuration) {
                    latestStatus = status;
                    return status;
                } else {
                    return latestStatus;
                }
            } else if (newStatusIsSpinner) {
                spinnerStartedAt = DateTime.UtcNow;
                latestStatus = status;
                return status;    
            } else {
                spinnerStartedAt = DateTime.UtcNow;
                latestStatus = IndicationStatus.Loading;
                return status;
            }
        }
        
        private static IndicationStatus GetIndicationStatusCore() {
            if (EditorCodePatcher.RenderFirstLogin)
                return IndicationStatus.Login;
            if (EditorCodePatcher.DownloadRequired && EditorCodePatcher.DownloadStarted || EditorCodePatcher.RequestingDownloadAndRun && !EditorCodePatcher.Starting && !EditorCodePatcher.Stopping)
                return IndicationStatus.Installing;
            if (EditorCodePatcher.Stopping)
                return IndicationStatus.Stopping;
            if (EditorCodePatcher.Compiling && !EditorCodePatcher.Stopping && !EditorCodePatcher.Starting && EditorCodePatcher.Running)
                return IndicationStatus.Compiling;
            if (EditorCodePatcher.Starting && !EditorCodePatcher.Stopping)
                return IndicationStatus.Starting;
            if (!EditorCodePatcher.Running)
                return IndicationStatus.Stopped;
            if (EditorCodePatcher.Status?.isLicensed != true && EditorCodePatcher.Status?.isFree != true && EditorCodePatcher.Status?.freeSessionFinished == true)
                return IndicationStatus.ActivationFailed;
            if (EditorCodePatcher.compileError)
                return IndicationStatus.CompileErrors;
            
            // fallback on patch status
            if (EditorCodePatcher.Started || EditorCodePatcher.Running) {
                switch (EditorCodePatcher.patchStatus) {
                    case PatchStatus.Idle:        return IndicationStatus.Idle;
                    case PatchStatus.Patching:    return IndicationStatus.Patching;
                    case PatchStatus.Unsupported: return IndicationStatus.Unsupported;
                    case PatchStatus.Compiling:   return IndicationStatus.Compiling;
                    case PatchStatus.None:
                    default:                      return IndicationStatus.Idle;
                }
            }
            // default
            return IndicationStatus.Stopped;
        }
        
        internal static IndicationStatus CurrentIndicationStatus => GetIndicationStatus();
        internal static bool SpinnerActive => SpinnerIndications.Contains(CurrentIndicationStatus);
        internal static string IndicationIconPath => IndicationIcon[CurrentIndicationStatus];
        internal static string IndicationStatusText {
            get {
                var indicationStatus = CurrentIndicationStatus;
                string txt;
                if (EditorCodePatcher.RenderFirstLogin) {
                    txt = HotReloadPrefs.RenderAuthLogin ? "Login" : "Start Free Trial";
                } else if (indicationStatus == IndicationStatus.Starting && EditorCodePatcher.StartupProgress != null) {
                    txt = EditorCodePatcher.StartupProgress.Item2;
                } else if (EditorCodePatcher.Failures.Count > 0 && indicationStatus == IndicationStatus.Idle) {
                    txt = "Latest patch applied";
                } else if (!EditorCodePatcher.Compiling && !EditorCodePatcher.firstPatchAttempted && !EditorCodePatcher.compileError && indicationStatus == IndicationStatus.Idle) {
                    txt = "Waiting for code changes";
                } else if (!IndicationText.TryGetValue(indicationStatus, out txt)) {
                    Log.Warning($"Indication text not found for status {indicationStatus}");
                } else {
                    txt = IndicationText[indicationStatus];
                }
                return txt;
            }
        }
    }
}
