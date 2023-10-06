# RenameFinder

In Patch 6.5, SE renamed a lot of files in the FFXIV filesystem, particularly ones from VFX. This tool aims to use crowdsourced paths from [ResLogger2](https://rl2.perchbird.dev/) to find the new paths for these files.

## Usage

Create a text file like this:

```text
vfx/monster/m0012/texture/uv_ora1x.atex C:/Users/Julian/Downloads/test.atex
```

On each line, insert the old path on the left, and the location of the file's path on your computer.

Then, run the program, adjusting paths from below:

```shell
$ RenameFinder.exe "G:\Steam\steamapps\common\FINAL FANTASY XIV Online\game\sqpack" "D:\code\c#\ffxiv\RenameFinder\paths.txt" "D:\code\c#\ffxiv\RenameFinder\output.txt
```

The detected files will be saved in the output file, with the old path on the left, and the new path on the right.

## Notes

- This uses like a gig of RAM because I download the entire ResLogger2 hash database into memory. This was prototype code, sorry not sorry.
- Files are compared by being in the same folder and having the same contents (checked by size and then SHA256 hash). If the file had its contents changed or moved to a different folder, it won't be detected.
- Output paths may have hash stubs (start with `~`) when the path is not known. Install the ResLogger2 plugin to help find them!
