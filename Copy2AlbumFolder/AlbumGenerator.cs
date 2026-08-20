namespace Rotherprivat.Copy2AlbumFolder
{
    internal class AlbumGenerator
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
                AlbumDirectory?.FullName,
                uiRecursive,
                filenameExample,
                Resources.Copy2AlbumFolder.ConfirmationPrompt
            );

            var input = In.ReadLine()?.ToLowerInvariant();

            return inputYes.Contains(input);
        }


        internal int Run()
        {
            if (SourceDirectory == null) return -1;
            if (AlbumDirectory == null) return -1;
            if (Pattern == null) return -1;

            if (!SourceDirectory.Exists) return -1;

            // Make sure AlbumDirectory exists
            if (!AlbumDirectory.Exists)
                AlbumDirectory.Create();

            CopySourceFolder(SourceDirectory);

            return 0;
        }

        #endregion

        #region private methodes
        private void CopySourceFolder(DirectoryInfo directory)
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

        private void CopySourceFile(FileInfo file)
        {
            var result = Metadata.GetDateTimeFromMeta(file, out var dateTime);
            switch (result)
            {
                case Metadata.MetaDataResult.Success:
                    var outputFile = GetUniqueOutputFilePath(file, dateTime!.Value.ToString(Pattern));
                    if (outputFile != null)
                    {
                        File.Copy(file.FullName, outputFile);
                        Out?.WriteLine($"{file.FullName}: {outputFile}");
                    }
                    break;
                case Metadata.MetaDataResult.ErrorNoMetadata:
                    Out?.WriteLine($"{file.FullName}: {Resources.Copy2AlbumFolder.ErrorNoMetadata}");
                    break;
                case Metadata.MetaDataResult.ErrorMissingDateTimeTag:
                    Out?.WriteLine($"{file.FullName}: {Resources.Copy2AlbumFolder.ErrorMissingDateTimeTag}");
                    break;
                case Metadata.MetaDataResult.ErrorProcessingMetaData:
                    Out?.WriteLine($"{file.FullName}: {Resources.Copy2AlbumFolder.ErrorProcessingMetaData}");
                    break;
                case Metadata.MetaDataResult.ErrorReadFile:
                    Out?.WriteLine($"{file.FullName}: {Resources.Copy2AlbumFolder.ErrorReadFile}");
                    break;
                default:
                    throw new InvalidOperationException("GetDateTimeFromMeta: Invalid result.");
            }
        }

        private string? GetUniqueOutputFilePath(FileInfo sourceFile, string outputBaseName)
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

            Err?.WriteLine($"{sourceFile.FullName}: {Resources.Copy2AlbumFolder.ErrorFileIndexExceeded}");

            return null;
        }
        #endregion

        #region properties
        public DirectoryInfo? SourceDirectory { get; set; }
        public DirectoryInfo? AlbumDirectory { get; set; }
        public string? Pattern { get; set; }
        public string? Postfix 
        {
            get => _Postfix; 
            set => _Postfix = value == null ? string.Empty : "_" + value;
        }
        public bool Recursive { get; set; }

        public TextReader? In { get; set; }
        public TextWriter? Out { get; set; }
        public TextWriter? Err { get; set; }


        #endregion

        private string _Postfix = string.Empty;
    }
}
