using System.CommandLine;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Rotherprivat.Copy2AlbumFolder
{
    internal class Program
    {
        // source directory
        // album directory
        // datetime pattern
        // recursive
        static int Main(string[] args)
        {
#if Language_enUS
            string cultureCode =  "en-US";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureCode);
#endif
            
            RootCommand rootCommand = Copy2AlbumFolderCommand.BuildRootCommand();
            ParseResult parseResult = rootCommand.Parse(args);

            return parseResult.Invoke();
        }
    }
}
