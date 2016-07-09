using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Hale.Core.Config
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AppConfig : IDisposable
    {
        /// <summary>
        /// Changes the app config path to param path.
        /// </summary>
        /// <param name="path">The new AppConfig Path.</param>
        /// <returns>The new path.</returns>
        public static AppConfig Change(string path)
        {
            return new ChangeAppConfig(path);
        }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public abstract void Dispose();

        private class ChangeAppConfig : AppConfig
        {
            private readonly string _oldConfig =
                AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();

            private bool _disposedValue;

            public ChangeAppConfig(string path)
            {
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", path);
                ResetConfigMechanism();
            }

            public override void Dispose()
            {
                if (!_disposedValue)
                {
                    AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", _oldConfig);
                    ResetConfigMechanism();


                    _disposedValue = true;
                }
                GC.SuppressFinalize(this);
            }

            private static void ResetConfigMechanism()
            {
                var initState = typeof(ConfigurationManager)
                    .GetField(
                        "s_initState",
                        BindingFlags.NonPublic | BindingFlags.Static
                    );
                var configSystem = typeof(ConfigurationManager)
                    .GetField(
                        "s_configSystem",
                        BindingFlags.NonPublic | BindingFlags.Static
                    );
                var current = typeof(ConfigurationManager)
                    .Assembly
                    .GetTypes()
                    .First(x => x.FullName ==
                                "System.Configuration.ClientConfigPaths")
                    .GetField(
                        "s_current",
                        BindingFlags.NonPublic | BindingFlags.Static
                    );

                initState?.SetValue(null, 0);
                configSystem?.SetValue(null, null);
                current?.SetValue(null, null);
            }
        }
    }
}
