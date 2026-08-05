using System;
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
        public void ExtractEditorVersion_ThrowsWhenValueIsEmpty()
        {
            const string contents = "m_EditorVersion:  ";
            var exception = Assert.Throws<FormatException>(() => UnityProjectMetadata.ExtractEditorVersion(contents));
            Assert.Equal("m_EditorVersion value is missing", exception.Message);
        }

        [Fact]
        public void LooksLikeUnityProject_FalseForEmptyPath()
        {
            Assert.False(UnityProjectMetadata.LooksLikeUnityProject(string.Empty));
        }
    }
}
