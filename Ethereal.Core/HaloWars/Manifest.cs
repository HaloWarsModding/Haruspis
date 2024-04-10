//-----------------------------------------------------------------------------
// File: Manifest.cs
// Description: Contains the Manifest class responsible for managing mod manifests in Halo Wars.
//    This class provides functionality to install and uninstall mod manifests.
//-----------------------------------------------------------------------------

using Ethereal.Core.Logging;
using System.IO;

namespace Ethereal.Core.HaloWars
{
    internal interface IManifestManager
    {
        /// <summary>
        /// Installs a mod by writing its path to the manifest file.a
        /// </summary>
        /// <param name="mod">The mod to be installed.</param>
        void Install(Mod mod);

        /// <summary>
        /// Uninstalls the mod by deleting the manifest file.
        /// </summary>
        void Uninstall();
    }

    public class Manifest : IManifestManager
    {
        private readonly string _path;
        private readonly LogWriter _writer;

        public Manifest(string path, LogWriter writer)
        {
            _path = path;
            _writer = writer;
        }

        public void Install(Mod mod)
        {
            _writer.Log(LogLevel.Information, "Installing manifest...");

            try
            {
                if (File.Exists(_path))
                {
                    _writer.Log(LogLevel.Warning, "Manifest file already exists. Overwriting...");
                    File.Delete(_path);
                }

                using (StreamWriter streamWriter = new(_path))
                {
                    streamWriter.Write(mod.Path);
                    streamWriter.Close();
                }

                _writer.Log(LogLevel.Information, "Manifest installed successfully.");
            }
            catch (Exception ex)
            {
                _writer.Log(LogLevel.Error, $"Error installing manifest: {ex.Message}");
                throw;
            }
        }

        public void Uninstall()
        {
            _writer.Log(LogLevel.Information, "Uninstalling manifest...");

            try
            {
                if (File.Exists(_path))
                {
                    File.Delete(_path);
                    _writer.Log(LogLevel.Information, "Manifest uninstalled successfully.");
                }
                else
                {
                    _writer.Log(LogLevel.Warning, "Manifest file not found. Nothing to uninstall.");
                }
            }
            catch (Exception ex)
            {
                _writer.Log(LogLevel.Error, $"Error uninstalling manifest: {ex.Message}");
                throw;
            }
        }
    }
}