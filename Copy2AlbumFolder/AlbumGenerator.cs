using System.Security;

namespace Rotherprivat.Copy2AlbumFolder
{
    internal sealed class AlbumGenerator : IDisposable
    {
        #region constructors
        internal AlbumGenerator() { }
        #endregion

        #region Internal methodes

        internal bool ConfirmInput()
        {
            if (Out==null)
                throw new InvalidOperationException("Out stream can not be null.");

            if (In == null)
                throw new InvalidOperationException("In stream can not be null.");

            var uiRecursive = Recursive ? Resources.Copy2AlbumFolder.Yes : Resources.Copy2AlbumFolder.No;
            var filenameExample = $"{DateTime.Now.ToString(Pattern)}_00{Postfix}.jpg";
            string[] inputYes = Resources.Copy2AlbumFolder.ConfirmationAnswer.Split(',');

            Out.Write(Resources.Copy2AlbumFolder.ConfirmationText,
                SourceDirectory?.FullName,
                GetFsNameInfoString(AlbumDirectory),
                GetFsNameInfoString(LogFile) ?? "---",
                uiRecursive,
                filenameExample,
                Resources.Copy2AlbumFolder.ConfirmationPrompt
            );

            var input = In.ReadLine()?.ToLowerInvariant();

            return inputYes.Contains(input);
        }

        internal static string? GetFsNameInfoString(FileSystemInfo? fsInfo) 
        {
            if (fsInfo == null) return null;
            if (!fsInfo.Exists) return fsInfo.FullName + $" ({Resources.Copy2AlbumFolder.New})";
            return fsInfo.FullName + $" ({Resources.Copy2AlbumFolder.Exists})";
        }

        internal int Run()
        {
            if (SourceDirectory == null) return -1;
            if (AlbumDirectory == null) return -1;
            if (Pattern == null) return -1;

            if (!SourceDirectory.Exists) return -1;

            // Open LogFile if specified
            if (!ConditionallyCreateLogFile())
                return -1;

            // Make sure AlbumDirectory exists
            if (!ConditionallyCreateAlbumDirectory())
                return -1;

            CopySourceFolder(SourceDirectory);

            return 0;
        }

        internal bool ConditionallyCreateAlbumDirectory()
        {
            if (AlbumDirectory == null)
                throw new InvalidOperationException("AlbumDirectory can not be null");

            if (TryFileSystemAction(() => AlbumDirectory.Create()))
                return true;

            // Failed
            Err?.WriteLine(Resources.Copy2AlbumFolder.ErrorCreateAlbumDirectory, AlbumDirectory.FullName);
            return false;
        }

        internal bool ConditionallyCreateLogFile()
        {
            if (LogFile == null) return true;
            if (_LogWriter != null) return true;
            if (TryFileSystemAction(() =>
            {
                LogFile.Directory?.Create();
                _LogWriter = new StreamWriter(LogFile.FullName, true);
            })) return true;

            // Failed
            try { _LogWriter?.Dispose(); } catch { }
            _LogWriter = null;
            Err?.WriteLine(Resources.Copy2AlbumFolder.ErrorAccessLogFile, LogFile.FullName);
            return false;
        }

        internal void CopySourceFolder(DirectoryInfo directory)
        {
            if (Recursive)
            {
                foreach (var subdirectory in directory.GetDirectories())
                {
                    CopySourceFolder(subdirectory);
                }
            }

            foreach (var file in directory.GetFiles("*.*"))
            {
                CopySourceFile(file);
            }
        }

        internal void CopySourceFile(FileInfo file)
        {
            var result = Metadata.GetDateTimeFromMetadata(file, out var dateTime);
            switch (result)
            {
                case Metadata.MetaDataResult.Success:
                    var outputFile = GetUniqueOutputFilePath(file, dateTime!.Value.ToString(Pattern));
                    if (outputFile != null)
                    {
                        // Copy the file to the album directory
                        if (TryFileSystemAction(() => File.Copy(file.FullName, outputFile)))
                        {
                            Log?.WriteLine($"{file.FullName}: {outputFile}");
                        }
                        else
                        {
                            Err?.WriteLine(Resources.Copy2AlbumFolder.ErrorCopyFile, file.FullName);
                        }
                    }
                    break;
                case Metadata.MetaDataResult.ErrorMissingDateTimeTag:
                    Err?.WriteLine($"{file.FullName}: {Resources.Copy2AlbumFolder.ErrorMissingDateTimeTag}");
                    break;
                case Metadata.MetaDataResult.ErrorProcessingMetaData:
                    Err?.WriteLine($"{file.FullName}: {Resources.Copy2AlbumFolder.ErrorProcessingMetaData}");
                    break;
                case Metadata.MetaDataResult.ErrorReadFile:
                    Err?.WriteLine($"{file.FullName}: {Resources.Copy2AlbumFolder.ErrorReadFile}");
                    break;
                default:
                    throw new InvalidOperationException("GetDateTimeFromMeta: Invalid result.");
            }
        }

        internal string? GetUniqueOutputFilePath(FileInfo sourceFile, string outputBaseName)
        {
            if (AlbumDirectory == null) 
                throw new InvalidOperationException("AlbumDirectory can not be null");

            var extension = sourceFile.Extension;
            for (int i = 0; i < 100; i++)
            {
                var filename = $"{outputBaseName}_{i:D2}{Postfix}{extension}";
                var fullPath = Path.Combine(AlbumDirectory.FullName, filename);
                if (!File.Exists(fullPath))
                    return fullPath;
            }

            Err?.WriteLine($"{sourceFile.FullName} {outputBaseName}_xx: {Resources.Copy2AlbumFolder.ErrorFileIndexExceeded}");

            return null;
        }
        #endregion

        #region Private methods
        private static bool TryFileSystemAction(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception e) when
                (e is IOException ||
                 e is SecurityException)
            {
                return false;
            }
        }
        #endregion

        #region properties
        internal DirectoryInfo? SourceDirectory { get; set; }
        internal DirectoryInfo? AlbumDirectory { get; set; }
        internal FileInfo? LogFile { get; set; }
        internal string? Pattern { get; set; }
        internal string? Postfix 
        {
            get => _Postfix; 
            set => _Postfix = value == null ? string.Empty : "_" + value;
        }
        internal bool Recursive { get; set; }

        internal TextReader? In { get; set; }
        internal TextWriter? Out { get; set; }
        internal TextWriter? Err { get; set; }
        internal TextWriter? Log => _LogWriter ?? Out;
        #endregion

        private string _Postfix = string.Empty;
        private TextWriter? _LogWriter = null;

        public void Dispose()
        {
            // Dispose any owned disposable resources
            _LogWriter?.Dispose();
            _LogWriter = null;

            GC.SuppressFinalize(this);
        }
    }
}
