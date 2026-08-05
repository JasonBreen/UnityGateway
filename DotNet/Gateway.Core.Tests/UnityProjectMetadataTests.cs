using System;
using Gateway.DotNet;
using Xunit;

namespace Gateway.Core.Tests
{
    public sealed class UnityProjectMetadataTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ExtractEditorVersion_ThrowsWhenEmpty(string? contents)
        {
            Assert.Throws<ArgumentException>(() => UnityProjectMetadata.ExtractEditorVersion(contents!));
        }

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
    }
}
