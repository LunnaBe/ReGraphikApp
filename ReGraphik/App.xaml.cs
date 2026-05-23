using System.Windows;

namespace ReGraphik
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";

            // Força IE 11 no WebBrowser (necessário para Leaflet.js)
            Microsoft.Win32.Registry.SetValue(
                @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                appName,
                11001,
                Microsoft.Win32.RegistryValueKind.DWord);

            // Libera scripts em arquivos locais
            Microsoft.Win32.Registry.SetValue(
                @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BLOCK_LMZ_SCRIPT",
                appName,
                0,
                Microsoft.Win32.RegistryValueKind.DWord);
        }
    }
}