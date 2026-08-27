# Copy2AlbumFolder

Create a photo album folder with unique, sortable file names from different cameras or people


## Using Copy2AlbumFolder

```CMD

Description:
  Merges picture folders to a common album folder and use the timestamp of the image- or video- metadata as file names,
  to get sortable and unique file names.

Usage:
  Copy2AlbumFolder [options]

Options:
  -s, --source <source> (REQUIRED)     Source folder, containing a part of the album.
  -a, --album <album> (REQUIRED)       Album folder, output folder.
  -p, --pattern <pattern>              Pattern for output file names. [default: yyyyMMdd_HHmmss]
  -x, --postfix <postfix>              Postfix of output filename.
  -r, --recursive                      Copys source files recursive.
  -y, --suppress-confirm-input, --yes  Suppresses the interactive input confirmation.
  -?, -h, --help                       Show help and usage information
  --version                            Show version information
```

## Example

