using System.CommandLine;
using System.Text.RegularExpressions;

namespace Rotherprivat.Copy2AlbumFolder
{
    internal static partial class Copy2AlbumFolderCommand
    {
        internal static RootCommand BuildRootCommand()
        {
            // --source
            Option<DirectoryInfo> sourceDirectoryOption = new("--source", "-s")
            {
                Description = Resources.Copy2AlbumFolder.SourceDirectoryOption,
                Required = true
            };

            sourceDirectoryOption.Validators.Add(result =>
            {
                var folder = result.GetValue(sourceDirectoryOption);
                if (folder == null || !folder.Exists)
                    result.AddError($"{result.Option.Name}: {Resources.Copy2AlbumFolder.ErrorSourceDirectoryOptionNoDirectory}");
            });

            // --album
            Option<DirectoryInfo> albumDirectoryOption = new("--album", "-a")
            {
                Description = Resources.Copy2AlbumFolder.AlbumDirectoryOption,
                Required = true
            };

            // --pattern
            Option<string> patternOption = new("--pattern", "-p")
            {
                Description = Resources.Copy2AlbumFolder.PatternOption,
                DefaultValueFactory = parseResult => "yyyyMMdd_HHmmss",
            };

            patternOption.Validators.Add(result =>
            {
                var pattern = result.GetValue(patternOption);
                try
                {
                    var fileNamePart = DateTime.Now.ToString(pattern);
                    if (!IsValidFileName(fileNamePart))
                        result.AddError($"{result.Option.Name}: {pattern} {Resources.Copy2AlbumFolder.ErrorInvalidFileNames}");
                }
                catch (FormatException e)
                {
                    result.AddError($"{result.Option.Name}: {e.Message}\n{Resources.Copy2AlbumFolder.ErrorPatternOptionSeeLink}: https://learn.microsoft.com/de-de/dotnet/api/system.datetime.tostring?view=net-8.0#system-datetime-tostring(system-string)");
                }
            });

            // --postfix
            Option<string> postfixOption = new("--postfix", "-x")
            {
                Description = Resources.Copy2AlbumFolder.PostfixOption
            };

            patternOption.Validators.Add(result =>
            {
                var postfix = result.GetValue(postfixOption);
                if (postfix == null)
                    return;

                if (!_PostfixValidator.IsMatch(postfix))
                    result.AddError($"{result.Option.Name}: {Resources.Copy2AlbumFolder.ErrorPostfixOptionAllowedChars}");
            });

            // --recursive
            Option<bool> recursiveOption = new("--recursive", "-r")
            {
                Description = Resources.Copy2AlbumFolder.RecursiveOption
            };

            // --suppress-confirm-input
            Option<bool> suppressConfirmInputOption = new("--suppress-confirm-input", "--yes", "-y")
            {
                Description = Resources.Copy2AlbumFolder.SuppressConfirmInputOption
            };

            RootCommand rootCommand =
            [
                sourceDirectoryOption,
                albumDirectoryOption,
                patternOption,
                postfixOption,
                recursiveOption,
                suppressConfirmInputOption
            ];

            rootCommand.Description = Resources.Copy2AlbumFolder.AppDescription;

            rootCommand.SetAction(parseResult =>
            {
                var albumGenerator = new AlbumGenerator()
                {
                    SourceDirectory = parseResult.GetValue(sourceDirectoryOption),
                    AlbumDirectory = parseResult.GetValue(albumDirectoryOption),
                    Pattern = parseResult.GetValue(patternOption),
                    Postfix = parseResult.GetValue(postfixOption),
                    Recursive = parseResult.GetValue(recursiveOption),
                    In = Console.In,
                    Out = Console.Out,
                    Err = Console.Error
                };


                var confirmInput = !parseResult.GetValue(suppressConfirmInputOption);
                if (confirmInput)
                {
                    if (!albumGenerator.ConfirmInput())
                    {
                        Console.Error.WriteLine(Resources.Copy2AlbumFolder.Aborted);
                        return -1;
                    }

                }

                return albumGenerator.Run();
            });

            return rootCommand;
        }

        private static bool IsValidFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            return !fileName.Any(c => Path.GetInvalidFileNameChars().Contains(c));
        }

        private static readonly Regex _PostfixValidator = PostfixValidatorRegex();

        [GeneratedRegex(@"^[A-Za-z0-9\-_]{1,5}$")]
        private static partial Regex PostfixValidatorRegex();
    }
}