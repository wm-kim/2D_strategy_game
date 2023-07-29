using System;
using JetBrains.Annotations;
using UnityEditor;

namespace SingularityGroup.HotReload.Editor {
    internal static class HotReloadPrefs {
        private const string RemoteServerKey = "HotReloadWindow.RemoteServer";
        private const string RemoteServerHostKey = "HotReloadWindow.RemoteServerHost";
        private const string LicenseEmailKey = "HotReloadWindow.LicenseEmail";
        private const string RenderAuthLoginKey = "HotReloadWindow.RenderAuthLogin";
        private const string FirstLoginCachedKey = "HotReloadWindow.FirstLoginCachedKey";
        private const string ShowOnStartupKey = "HotReloadWindow.ShowOnStartup";
        private const string PasswordCachedKey = "HotReloadWindow.PasswordCached";
        private const string ExposeServerToLocalNetworkKey = "HotReloadWindow.ExposeServerToLocalNetwork";
        private const string ErrorHiddenCachedKey = "HotReloadWindow.ErrorHiddenCachedKey";
        private const string RefreshManuallyTipCachedKey = "HotReloadWindow.RefreshManuallyTipCachedKey";
        private const string ShowLoginCachedKey = "HotReloadWindow.ShowLoginCachedKey";
        private const string ConfigurationKey = "HotReloadWindow.Configuration";
        private const string ShowPromoCodesCachedKey = "HotReloadWindow.ShowPromoCodesCached";
        private const string ShowOnDeviceKey = "HotReloadWindow.ShowOnDevice";
        private const string ShowChangelogKey = "HotReloadWindow.ShowChangelog";
        private const string UnsupportedChangesKey = "HotReloadWindow.ShowUnsupportedChanges";
        private const string LoggedBurstHintKey = "HotReloadWindow.LoggedBurstHint";
        private const string ShouldDoAutoRefreshFixupKey = "HotReloadWindow.ShouldDoAutoRefreshFixup";
        private const string ActiveDaysKey = "HotReloadWindow.ActiveDays";
        private const string RateAppShownKey = "HotReloadWindow.RateAppShown";
        private const string PatchesCollapseKey = "HotReloadWindow.PatchesCollapse";
        private const string PatchesGroupAllKey = "HotReloadWindow.PatchesGroupAll";
        private const string LaunchOnEditorStartKey = "HotReloadWindow.LaunchOnEditorStart";
        private const string AutoRecompileUnsupportedChangesKey = "HotReloadWindow.AutoRecompileUnsupportedChanges";
        private const string AutoRecompileUnsupportedChangesImmediatelyKey = "HotReloadWindow.AutoRecompileUnsupportedChangesImmediately";
        private const string AutoRecompileUnsupportedChangesInPlayModeKey = "HotReloadWindow.AutoRecompileUnsupportedChangesInPlayMode";
        private const string AllowDisableUnityAutoRefreshKey = "HotReloadWindow.AllowDisableUnityAutoRefresh";
        private const string DefaultAutoRefreshKey = "HotReloadWindow.DefaultAutoRefresh";
        private const string DefaultAutoRefreshModeKey = "HotReloadWindow.DefaultAutoRefreshMode";
        private const string DefaultScriptCompilationKeyKey = "HotReloadWindow.DefaultScriptCompilationKey";
        private const string AppliedAutoRefreshKey = "HotReloadWindow.AppliedAutoRefresh";
        private const string AppliedScriptCompilationKey = "HotReloadWindow.AppliedScriptCompilation";
        private const string AllAssetChangesKey = "HotReloadWindow.AllAssetChanges";
        private const string DisableConsoleWindowKey = "HotReloadWindow.DisableConsoleWindow";

        public const string DontShowPromptForDownloadKey = "ServerDownloader.DontShowPromptForDownload";


        static string[] settingCacheKeys;
        [Obsolete]
        public static string[] SettingCacheKeys = settingCacheKeys ?? (settingCacheKeys = new[] {
            AllowHttpSettingCacheKey,
            AutoRefreshSettingCacheKey,
            ScriptCompilationSettingCacheKey,
            ProjectGenerationSettingCacheKey,
        });
        
        [Obsolete] public const string AllowHttpSettingCacheKey = "HotReloadWindow.AllowHttpSettingCacheKey";
        [Obsolete] public const string AutoRefreshSettingCacheKey = "HotReloadWindow.AutoRefreshSettingCacheKey";
        [Obsolete] public const string ScriptCompilationSettingCacheKey = "HotReloadWindow.ScriptCompilationSettingCacheKey";
        [Obsolete] public const string ProjectGenerationSettingCacheKey = "HotReloadWindow.ProjectGenerationSettingCacheKey";


        public static bool RemoteServer {
            get { return EditorPrefs.GetBool(RemoteServerKey, false); }
            set { EditorPrefs.SetBool(RemoteServerKey, value); }
        }
        
        public static bool DontShowPromptForDownload {
            get { return EditorPrefs.GetBool(DontShowPromptForDownloadKey, false); }
            set { EditorPrefs.SetBool(DontShowPromptForDownloadKey, value); }
        }

        public static string RemoteServerHost {
            get { return EditorPrefs.GetString(RemoteServerHostKey); }
            set { EditorPrefs.SetString(RemoteServerHostKey, value); }
        }

        public static string LicenseEmail {
            get { return EditorPrefs.GetString(LicenseEmailKey); }
            set { EditorPrefs.SetString(LicenseEmailKey, value); }
        }
        
        public static string LicensePassword {
            get { return EditorPrefs.GetString(PasswordCachedKey); }
            set { EditorPrefs.SetString(PasswordCachedKey, value); }
        }
        
        public static bool RenderAuthLogin { // false = render free trial
            get { return EditorPrefs.GetBool(RenderAuthLoginKey); }
            set { EditorPrefs.SetBool(RenderAuthLoginKey, value); }
        }
        
        public static bool FirstLogin {
            get { return EditorPrefs.GetBool(FirstLoginCachedKey, true); }
            set { EditorPrefs.SetBool(FirstLoginCachedKey, value); }
        }

        public static string ShowOnStartup { // WindowAutoOpen
            get { return EditorPrefs.GetString(ShowOnStartupKey); }
            set { EditorPrefs.SetString(ShowOnStartupKey, value); }
        }


        public static bool ErrorHidden {
            get { return EditorPrefs.GetBool(ErrorHiddenCachedKey); }
            set { EditorPrefs.SetBool(ErrorHiddenCachedKey, value); }
        }
        
        public static bool ShowLogin {
            get { return EditorPrefs.GetBool(ShowLoginCachedKey, true); }
            set { EditorPrefs.SetBool(ShowLoginCachedKey, value); }
        }

        public static bool Configuration {
            get { return EditorPrefs.GetBool(ConfigurationKey, true); }
            set { EditorPrefs.SetBool(ConfigurationKey, value); }
        }

        public static bool ShowPromoCodes {
            get { return EditorPrefs.GetBool(ShowPromoCodesCachedKey, true); }
            set { EditorPrefs.SetBool(ShowPromoCodesCachedKey, value); }
        }
        
        public static bool ShowOnDevice {
            get { return EditorPrefs.GetBool(ShowOnDeviceKey, true); }
            set { EditorPrefs.SetBool(ShowOnDeviceKey, value); }
        }
        
        public static bool ShowChangeLog {
            get { return EditorPrefs.GetBool(ShowChangelogKey, true); }
            set { EditorPrefs.SetBool(ShowChangelogKey, value); }
        }
        
        public static bool ShowUnsupportedChanges {
            get { return EditorPrefs.GetBool(UnsupportedChangesKey, true); }
            set { EditorPrefs.SetBool(UnsupportedChangesKey, value); }
        }
        
        [Obsolete]
        public static bool RefreshManuallyTip {
            get { return EditorPrefs.GetBool(RefreshManuallyTipCachedKey); }
            set { EditorPrefs.SetBool(RefreshManuallyTipCachedKey, value); }
        }
        
        public static bool LoggedBurstHint {
            get { return EditorPrefs.GetBool(LoggedBurstHintKey); }
            set { EditorPrefs.SetBool(LoggedBurstHintKey, value); }
        }
        
        [Obsolete]
        public static bool ShouldDoAutoRefreshFixup {
            get { return EditorPrefs.GetBool(ShouldDoAutoRefreshFixupKey, true); }
            set { EditorPrefs.SetBool(ShouldDoAutoRefreshFixupKey, value); }
        }
        
        public static string ActiveDays {
            get { return EditorPrefs.GetString(ActiveDaysKey, string.Empty); }
            set { EditorPrefs.SetString(ActiveDaysKey, value); }
        }
        
        public static bool RateAppShown {
            get { return EditorPrefs.GetBool(RateAppShownKey, false); }
            set { EditorPrefs.SetBool(RateAppShownKey, value); }
        }

        [Obsolete]
        public static bool PatchesGroupAll {
            get { return EditorPrefs.GetBool(PatchesGroupAllKey, false); }
            set { EditorPrefs.SetBool(PatchesGroupAllKey, value); }
        }

        [Obsolete]
        public static bool PatchesCollapse {
            get { return EditorPrefs.GetBool(PatchesCollapseKey, true); }
            set { EditorPrefs.SetBool(PatchesCollapseKey, value); }
        }

        public static ShowOnStartupEnum GetShowOnStartupEnum() {
            ShowOnStartupEnum showOnStartupEnum;
            if (Enum.TryParse(HotReloadPrefs.ShowOnStartup, true, out showOnStartupEnum)) {
                return showOnStartupEnum;
            }
            return ShowOnStartupEnum.Always;
        }
        
        public static bool ExposeServerToLocalNetwork {
            get { return EditorPrefs.GetBool(ExposeServerToLocalNetworkKey, false); }
            set { EditorPrefs.SetBool(ExposeServerToLocalNetworkKey, value); }
        }
        
        public static bool LaunchOnEditorStart {
            get { return EditorPrefs.GetBool(LaunchOnEditorStartKey, false); }
            set { EditorPrefs.SetBool(LaunchOnEditorStartKey, value); }
        }

        public static bool AutoRecompileUnsupportedChanges {
            get { return EditorPrefs.GetBool(AutoRecompileUnsupportedChangesKey, false); }
            set { EditorPrefs.SetBool(AutoRecompileUnsupportedChangesKey, value); }
        }
        
        public static bool AutoRecompileUnsupportedChangesImmediately {
            get { return EditorPrefs.GetBool(AutoRecompileUnsupportedChangesImmediatelyKey, false); }
            set { EditorPrefs.SetBool(AutoRecompileUnsupportedChangesImmediatelyKey, value); }
        }
        
        public static bool AutoRecompileUnsupportedChangesInPlayMode {
            get { return EditorPrefs.GetBool(AutoRecompileUnsupportedChangesInPlayModeKey, false); }
            set { EditorPrefs.SetBool(AutoRecompileUnsupportedChangesInPlayModeKey, value); }
        }

        public static bool AllowDisableUnityAutoRefresh {
            get { return EditorPrefs.GetBool(AllowDisableUnityAutoRefreshKey, false); }
            set { EditorPrefs.SetBool(AllowDisableUnityAutoRefreshKey, value); }
        }
        
        public static int DefaultAutoRefresh {
            get { return EditorPrefs.GetInt(DefaultAutoRefreshKey, -1); }
            set { EditorPrefs.SetInt(DefaultAutoRefreshKey, value); }
        }
        
        [UsedImplicitly]
        public static int DefaultAutoRefreshMode {
            get { return EditorPrefs.GetInt(DefaultAutoRefreshModeKey, -1); }
            set { EditorPrefs.SetInt(DefaultAutoRefreshModeKey, value); }
        }
        
        public static int DefaultScriptCompilation {
            get { return EditorPrefs.GetInt(DefaultScriptCompilationKeyKey, -1); }
            set { EditorPrefs.SetInt(DefaultScriptCompilationKeyKey, value); }
        }
        
        public static bool AppliedAutoRefresh {
            get { return EditorPrefs.GetBool(AppliedAutoRefreshKey); }
            set { EditorPrefs.SetBool(AppliedAutoRefreshKey, value); }
        }
        
        public static bool AppliedScriptCompilation {
            get { return EditorPrefs.GetBool(AppliedScriptCompilationKey); }
            set { EditorPrefs.SetBool(AppliedScriptCompilationKey, value); }
        }
        
        public static bool AllAssetChanges {
            get { return EditorPrefs.GetBool(AllAssetChangesKey, false); }
            set { EditorPrefs.SetBool(AllAssetChangesKey, value); }
        }
        
        public static bool DisableConsoleWindow {
            get { return EditorPrefs.GetBool(DisableConsoleWindowKey, false); }
            set { EditorPrefs.SetBool(DisableConsoleWindowKey, value); }
        }

        /// <summary>
        /// Prefs for storing temporary UI state of the Hot Reload EditorWindow.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Use this for things like the state of EditorGUILayout.Foldout to keep it collapsed or expanded.
        /// When Unity refreshes (compiles code), the EditorWindow is recreated, and C# field values are lost.<br/>
        /// </para>
        /// <para>
        /// Do not use this class for persistant options, like a checkbox thatthe user can click.<br/>
        /// We may later decide to clear these prefs when a project is closed.
        /// </para>
        /// </remarks>
        internal static class SessionPrefs {
            private const string FoldoutCheckBuildSupportKey = "HotReloadWindow.SessionPrefs.FoldoutCheckBuildSupportKey";
            private const string FoldoutQrCodeKey = "HotReloadWindow.SessionPrefs.FoldoutQrCodeKey";

            public static bool FoldoutCheckBuildSupport {
                get { return EditorPrefs.GetBool(FoldoutCheckBuildSupportKey, true); }
                set { EditorPrefs.SetBool(FoldoutCheckBuildSupportKey, value); }
            }
            
            public static bool FoldoutQrCode {
                // by default collapsed
                get { return EditorPrefs.GetBool(FoldoutQrCodeKey, false); }
                set { EditorPrefs.SetBool(FoldoutQrCodeKey, value); }
            }
        }
    }
}
