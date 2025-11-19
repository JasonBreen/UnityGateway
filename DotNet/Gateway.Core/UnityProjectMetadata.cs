using System;
using System.IO;

namespace Gateway.DotNet
{
    /// <summary>
    /// Lightweight helpers for verifying the Unity project footprint during .NET CI jobs.
    /// </summary>
    public static class UnityProjectMetadata
    {
        private const string VersionKey = "m_EditorVersion";

        /// <summary>
        /// Parse a Unity ProjectVersion.txt string and return the editor version value.
        /// </summary>
        /// <param name="projectVersionContents">Raw text from ProjectSettings/ProjectVersion.txt.</param>
        /// <returns>The parsed Unity editor version.</returns>
        /// <exception cref="ArgumentException">Thrown when the input is null or whitespace.</exception>
        /// <exception cref="FormatException">Thrown when the expected key is missing.</exception>
        public static string ExtractEditorVersion(string projectVersionContents)
        {
            if (string.IsNullOrWhiteSpace(projectVersionContents))
            {
                throw new ArgumentException("ProjectVersion contents cannot be empty", nameof(projectVersionContents));
            }

            using var reader = new StringReader(projectVersionContents);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var separatorIndex = line.IndexOf(':');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = line.Substring(0, separatorIndex).Trim();
                if (!string.Equals(key, VersionKey, StringComparison.Ordinal))
                {
                    continue;
                }

                var value = line[(separatorIndex + 1)..].Trim();
                if (string.IsNullOrEmpty(value))
                {
                    throw new FormatException("m_EditorVersion value is missing");
                }

                return value;
            }

            throw new FormatException("Unable to find m_EditorVersion in ProjectVersion.txt contents");
        }

        /// <summary>
        /// Checks whether the supplied path resembles a Unity project by verifying
        /// the presence of the ProjectSettings and Packages manifest files.
        /// </summary>
        public static bool LooksLikeUnityProject(string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
            {
                return false;
            }

            var projectSettings = Path.Combine(rootPath, "ProjectSettings", "ProjectVersion.txt");
            var manifest = Path.Combine(rootPath, "Packages", "manifest.json");
            return File.Exists(projectSettings) && File.Exists(manifest);
        }
    }
}
