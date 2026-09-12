using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Doorstop
{
    public static class Entrypoint
    {
        private static string directory;
        private static bool started;
        private static readonly object logLock = new object();
        private static readonly string[] dependencies = {
            "OldMarket.MaterialCost", "0Harmony", "Mono.Cecil", "MonoMod.RuntimeDetour", "MonoMod.Utils"
        };

        public static void Start()
        {
            if (started) return;
            started = true;
            try
            {
                directory = Path.GetDirectoryName(typeof(Entrypoint).Assembly.Location);
                // Rotate only our log. No console, culture setters, game or save writes.
                string logPath = Path.Combine(directory, "loader.log");
                if (File.Exists(logPath)) File.Copy(logPath, logPath + ".previous", true);
                File.WriteAllText(logPath, "OldMarket standalone bootstrap 0.1.0\n");
                Log("Entry culture=" + CultureInfo.CurrentCulture.Name
                    + "; decimal=" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                string enabledPath = Path.Combine(directory, "enabled.txt");
                if (File.Exists(enabledPath) && File.ReadAllText(enabledPath).Trim().Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    Log("Cost module disabled; no resolver, Unity callbacks or Harmony loaded.");
                    return;
                }
                AppDomain.CurrentDomain.AssemblyResolve += Resolve;
                var assembly = Assembly.LoadFrom(Path.Combine(directory, "OldMarket.MaterialCost.dll"));
                assembly.GetType("OldMarket.MaterialCost.StandaloneEntry", true)
                    .GetMethod("Prepare", BindingFlags.Static | BindingFlags.Public)
                    .Invoke(null, new object[] { directory, (Action<string>)Log });
            }
            catch (Exception error)
            {
                AppDomain.CurrentDomain.AssemblyResolve -= Resolve;
                Log("Bootstrap failed: " + error);
            }
        }

        private static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            string name = new AssemblyName(args.Name).Name;
            if (Array.IndexOf(dependencies, name) < 0) return null;
            string file = Path.Combine(directory, name + ".dll");
            return File.Exists(file) ? Assembly.LoadFrom(file) : null;
        }

        private static void Log(string message)
        {
            try
            {
                lock (logLock)
                    File.AppendAllText(Path.Combine(directory, "loader.log"),
                        DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture) + " " + message + "\n");
            }
            catch { /* Logging failure must not interrupt the game. */ }
        }
    }
}
