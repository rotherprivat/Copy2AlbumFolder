using System;
using System.IO;
using Rotherprivat.Copy2AlbumFolder;
using Xunit;

namespace Copy2AlbumFolder.Tests
{
    public class MetadataTests
    {
        [Fact]
        public void GetDateTimeFromMetadata_ReturnsCorrectDateTime_FromImage()
        {
            // Arrange
            var testFilePath = Path.Combine(AppContext.BaseDirectory, "TestData", "SourceFolder", "20250924_173020.jpg");
            var result = Metadata.GetDateTimeFromMetadata(new FileInfo(testFilePath), out DateTime? dateTime);
            // Assert
            Assert.Equal(Metadata.MetaDataResult.Success, result);
            Assert.NotNull(dateTime);
            Assert.Equal(new DateTime(2025, 9, 24, 17, 30, 20), dateTime.Value);
        }

        [Fact]
        public void GetDateTimeFromMetadata_ReturnsCorrectDateTime_FromVideo()
        {
            // Arrange
            var testFilePath = Path.Combine(AppContext.BaseDirectory, "TestData", "SourceFolder", "20260822_160735.mp4");
            var result = Metadata.GetDateTimeFromMetadata(new FileInfo(testFilePath), out DateTime? dateTime);
            // Assert
            Assert.Equal(Metadata.MetaDataResult.Success, result);
            Assert.NotNull(dateTime);
            Assert.Equal(new DateTime(2026, 8, 22, 16, 28, 42), dateTime.Value);
        }

        [Fact]
        public void GetDateTimeFromMetadata_ErrorProcessingMetaData()
        {
            // Arrange
            var testFilePath = Path.Combine(AppContext.BaseDirectory, "TestData", "SourceFolder", "nometadata.txt");
            var result = Metadata.GetDateTimeFromMetadata(new FileInfo(testFilePath), out DateTime? dateTime);
            // Assert
            Assert.Equal(Metadata.MetaDataResult.ErrorProcessingMetaData, result);
            Assert.Null(dateTime);
        }

        [Fact]
        public void GetDateTimeFromMetadata_ErrorReadFile()
        {
            // Arrange
            var testFilePath = Path.Combine(AppContext.BaseDirectory, "TestData", "SourceFolder", "noFile.txt");
            var result = Metadata.GetDateTimeFromMetadata(new FileInfo(testFilePath), out DateTime? dateTime);
            // Assert
            Assert.Equal(Metadata.MetaDataResult.ErrorReadFile, result);
            Assert.Null(dateTime);
        }

        [Fact]
        public void GetDateTimeFromMetadata_ErrorNoMetadata()
        {
            // Arrange
            var testFilePath = Path.Combine(AppContext.BaseDirectory, "TestData", "SourceFolder", "noMetadata.jpg");
            var result = Metadata.GetDateTimeFromMetadata(new FileInfo(testFilePath), out DateTime? dateTime);
            // Assert
            Assert.Equal(Metadata.MetaDataResult.ErrorMissingDateTimeTag, result);
            Assert.Null(dateTime);
        }

    }
}
