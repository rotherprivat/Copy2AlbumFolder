using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;

// see https://github.com/drewnoakes/metadata-extractor-dotnet

namespace Rotherprivat.Copy2AlbumFolder
{
    /// <summary>
    /// Helper class for reading DateTime from meta data
    /// </summary>
    internal static class Metadata
    {
        /// <summary>
        /// Results from meta data reader
        /// </summary>
        internal enum MetaDataResult
        {
            None,
            Success,
            ErrorMissingDateTimeTag,
            ErrorProcessingMetaData,
            ErrorReadFile
        }

        #region public methods

        /// <summary>
        /// Get DateTime from image (EXIF) - or video (QuickTime) meta data
        /// </summary>
        /// <param name="file">FileInfo</param>
        /// <param name="dateTime">DateTime in local time zone or null if no meta data present</param>
        /// <returns>Result</returns>
        internal static MetaDataResult GetDateTimeFromMetadata(FileInfo file, out DateTime? dateTime)
        {
            dateTime = null;
            try
            {
                var metadata = ImageMetadataReader.ReadMetadata(file.FullName);
                if (metadata == null)
                    return MetaDataResult.ErrorMissingDateTimeTag;

                if (TryGetDateTimeFromImage(metadata, out dateTime))
                    return MetaDataResult.Success;

                if (TryGetDateTimeFromVideo(metadata, out dateTime))
                    return MetaDataResult.Success; 

                return MetaDataResult.ErrorMissingDateTimeTag;
            }
            catch (ImageProcessingException)
            {
                return MetaDataResult.ErrorProcessingMetaData;
            }

            catch (IOException)
            {
                return MetaDataResult.ErrorReadFile;
            }

        }
        
        #endregion

        #region private mehtods
        private static bool TryGetDateTimeFromImage(IReadOnlyList<MetadataExtractor.Directory> metadata, out DateTime? dateTime)
        {
            dateTime = null;
            var exifDir = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            dateTime =  exifDir?.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);
            return dateTime != null;
        }

        private static bool TryGetDateTimeFromVideo(IReadOnlyList<MetadataExtractor.Directory> metadata, out DateTime? dateTime)
        {
            dateTime = null;
            var qtDirectory = metadata.OfType<QuickTimeMovieHeaderDirectory>().FirstOrDefault();
            dateTime = qtDirectory?.GetDateTime(QuickTimeMovieHeaderDirectory.TagCreated);
            if (dateTime == null)
                return false;

            // Calculate recording start time
            var duration = (TimeSpan?)qtDirectory?.GetObject(QuickTimeMovieHeaderDirectory.TagDuration);
            if (duration != null)
            {
                // Round duration to whole seconds before subtracting
                var roundedSeconds = Math.Round(duration.Value.TotalSeconds, MidpointRounding.AwayFromZero);
                dateTime -= TimeSpan.FromSeconds(roundedSeconds);
            }

            // QuickTime Timestamp is UTC
            dateTime = dateTime?.ToLocalTime();

            return true;
        }

        #endregion
    }
}
