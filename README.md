# Copy2AlbumFolder

**Copy2AlbumFolder** is a free command-line utility for consolidating image and video files from multiple source folders into a common album directory.

The application copies files from a specified source folder to the selected album folder and can optionally process source folders recursively. 
To make the resulting files easy to sort and identify, Copy2AlbumFolder uses the timestamp available in the image or video metadata to generate 
unique and sortable output filenames.

## Filename generation

By default, files are renamed using the following format:

```text
{timestamp}_{two-digit numeric suffix}{user given postfix}.{file extension}
```

The timestamp format is configurable and defaults to:

```text
yyyyMMdd_HHmmss
```

For example:

```text
20260910_121530_00.jpg
20260910_121531_00.jpg
20260910_121531_01.jpg
20260910_121532_00.mp4
```

The original file extension is retained. An optional user-defined postfix can be added to the generated filename.
The postfix is limited to 5 characters 'a'-'z', 'A'-'Z', '0'-'9', '-' and '_'.

For example postfix "EOS":

```text
20260910_121530_00_EOS.jpg
20260910_121531_00_EOS.jpg
20260910_121531_01_EOS.jpg
20260910_121532_00_EOS.mp4
```

### Filename collisions

Copy2AlbumFolder attempts to prevent filename collisions by adding a two-digit numeric suffix to the generated filename. 
If a filename already exists in the destination album folder, the suffix is incremented until an available filename is found.

For example:

```text
20260910_121530_00.jpg
20260910_121530_01.jpg
20260910_121530_02.jpg
20260910_121530_03.jpg
```

This allows multiple files with the same metadata timestamp to be stored in the same album without overwriting an existing file.

If the available two-digit suffix range is exhausted and no unique filename can be generated, the affected file will **not be copied**. 
The collision is reported to the user so that it can be identified and handled separately.

## Command-line interface

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

**Please note**: Changing the timestamp pattern can result in inappropriate file names, which may, for example, 
make generating unique names or sorting impossible.

## Example

## Disclaimer

**Copy2AlbumFolder is provided free of charge and without any warranty or guarantee.**

This application is intended to assist with copying image and video files from one or more source folders into a common album folder. Depending on the configured options, files may be copied recursively and renamed using timestamps obtained from image or video metadata.

By using Copy2AlbumFolder, you acknowledge and agree to the following:

* **Use at your own risk.** The authors and contributors of Copy2AlbumFolder are not responsible for any loss, corruption, modification, overwriting, or unintended alteration of files or data resulting from the use of this application.
* **Keep backups.** You are solely responsible for maintaining appropriate backups of your original photos, videos, and other important data before using this application.
* **Verify the result.** You should verify that all expected files have been copied correctly and that the resulting album contains the data you intended to copy before deleting or modifying the original source files.
* **Metadata may be incomplete or incorrect.** Image and video files may contain missing, inaccurate, or inconsistent date/time metadata. Consequently, generated filenames may not always represent the actual date or time at which a file was created or captured.
* **Filename collisions are handled automatically, but cannot be guaranteed to be resolved in every case.** Copy2AlbumFolder attempts to avoid collisions by adding and incrementing a two-digit numeric suffix to generated filenames. If no unique filename can be generated within the available suffix range, the affected file will not be copied and the event will be reported. Users should therefore review the application's output and reported errors after copying
* **File compatibility is not guaranteed.** The application may encounter files, formats, metadata structures, permissions, or filesystem conditions that it cannot process correctly.
* **No guarantee of availability or continued development.** As the application is provided free of charge, there is no guarantee regarding availability, maintenance, updates, support, or future compatibility.
* **You are responsible for your configuration.** Options such as recursive copying, filename patterns, postfixes, and suppression of confirmation prompts can affect the resulting files. Make sure you understand the selected options before running the application, especially when using the `--yes` option.
* **No liability.** To the maximum extent permitted by applicable law, the authors and contributors shall not be liable for any direct, indirect, incidental, consequential, or other damages arising from or related to the use of, or inability to use, Copy2AlbumFolder.

**Important:** Do not rely on Copy2AlbumFolder as a backup solution. Always retain independent backups of valuable photos, videos, and other data.

If you discover unexpected behavior, please report it through the project's designated issue tracker or support channel, preferably including the command used and relevant error information. Do not include private or sensitive personal data in bug reports.

**Use of Copy2AlbumFolder constitutes acceptance of this disclaimer.**


(contains AI-Generated content)