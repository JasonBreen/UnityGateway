using System;
using System.IO;
using Gateway.DotNet;
using Xunit;

namespace Gateway.Core.Tests
{
    public sealed class UnityProjectMetadataTests
    {
        [Fact]
        public void ExtractEditorVersion_ReadsFirstMatchingLine()
        {
            const string contents = "m_EditorVersionWithRevision: 6000.2.0f1\nm_EditorVersion: 6000.1.0f1";
            var version = UnityProjectMetadata.ExtractEditorVersion(contents);
            Assert.Equal("6000.1.0f1", version);
        }

        [Fact]
        public void ExtractEditorVersion_ThrowsWhenMissing()
        {
            const string contents = "m_SomeOtherKey: value";
            Assert.Throws<FormatException>(() => UnityProjectMetadata.ExtractEditorVersion(contents));
        }

        [Fact]
        public void LooksLikeUnityProject_FalseForEmptyPath()
        {
            Assert.False(UnityProjectMetadata.LooksLikeUnityProject(string.Empty));
        }

        [Fact]
        public void LooksLikeUnityProject_ReturnsTrueForValidProject()
        {
            var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                var projectSettingsDir = Path.Combine(tempRoot, "ProjectSettings");
                var packagesDir = Path.Combine(tempRoot, "Packages");
                Directory.CreateDirectory(projectSettingsDir);
                Directory.CreateDirectory(packagesDir);

                File.WriteAllText(Path.Combine(projectSettingsDir, "ProjectVersion.txt"), "dummy contents");
                File.WriteAllText(Path.Combine(packagesDir, "manifest.json"), "{}");

                Assert.True(UnityProjectMetadata.LooksLikeUnityProject(tempRoot));
            }
            finally
            {
                if (Directory.Exists(tempRoot))
                {
                    Directory.Delete(tempRoot, true);
                }
            }
        }
    }
}
