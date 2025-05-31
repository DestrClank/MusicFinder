# MusicFinder

MusicFinder is a command-line tool that detects which audio samples from a folder are present in a main (mixed) audio file. It uses the [SoundFingerprinting](https://github.com/AddictedCS/soundfingerprinting) library to generate audio fingerprints and perform similarity searches.

## Features

- **Indexes all `.wav` samples** in a specified folder.
- **Detects which samples are present** in a main audio file (also `.wav`).
- **Logs detected samples** to a text file (`found_files.txt`).
- **Progress and results** are displayed in the console.

## Requirements

- .NET Framework 4.7.2
- All audio files must be in `.wav` format.
- The [SoundFingerprinting](https://www.nuget.org/packages/SoundFingerprinting/) NuGet package.

## Usage

### Command Line

You can run the program with arguments:

```bash
MusicFinder.exe "path\to\main.wav" "path\to\samples_folder"
```

- The first argument is the path to the main (mixed) audio file.
- The second argument is the path to the folder containing the sample `.wav` files.

### Interactive Mode

If you run the program without arguments, it will prompt you to enter:

1. The path to the main (mixed) audio file.
2. The path to the folder containing the sample `.wav` files.

### Output

- The program will display progress for both indexing and detection.
- All detected samples will be listed in the console and saved to `found_files.txt` in the program's directory.

## Example

Suppose you have:

- A main file: `C:\Music\mix.wav`
- A folder with samples: `C:\Music\samples\` (containing `sample1.wav`, `sample2.wav`, ...)

Run:
```bash
MusicFinder.exe "C:\Music\mix.wav" "C:\Music\samples"
```


After completion, check `found_files.txt` for the list of detected samples.

## Notes

- Only `.wav` files in the samples folder are processed.
- The detection is based on audio fingerprinting; results may vary depending on the quality and length of the samples.
- The program does not modify your audio files.

## License

This project is provided as-is for educational and research purposes. See [SoundFingerprinting](https://github.com/AddictedCS/soundfingerprinting) for library licensing.