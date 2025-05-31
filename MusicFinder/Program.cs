using SoundFingerprinting;
using SoundFingerprinting.Audio;
using SoundFingerprinting.Builder;
using SoundFingerprinting.Data;
using SoundFingerprinting.InMemory;
using System;
using System.IO;

namespace MusicFinder
{
    internal class Program
    {

        static void Main(string[] args)
        {
            // Pseudocode:
            // 1. Read arguments: args[0] = main file path, args[1] = samples folder
            // 2. If no arguments, ask the user
            // 3. Create a new InMemoryModelService (no more loading/saving)
            // 4. Index the samples
            // 5. For each sample, search for similarity in the main file
            // 6. Display results and log found files

            string mainFile = null;
            string folder = null;

            if (args.Length >= 2)
            {
                mainFile = args[0];
                folder = args[1];
            }
            else
            {
                Console.WriteLine("Main music (mixed) file path:");
                mainFile = Console.ReadLine();

                Console.WriteLine("Samples folder path:");
                folder = Console.ReadLine();
            }

            // Remove possible quotes
            if (!string.IsNullOrWhiteSpace(mainFile))
                mainFile = mainFile.Trim().Trim('"');
            if (!string.IsNullOrWhiteSpace(folder))
                folder = folder.Trim().Trim('"');

            // Check if main file and folder exist
            if (string.IsNullOrWhiteSpace(mainFile) || string.IsNullOrWhiteSpace(folder))
            {
                Console.WriteLine("Main file path or samples folder path is missing.");
                return;
            }
            if (!File.Exists(mainFile))
            {
                Console.WriteLine($"The specified main file does not exist: {mainFile}");
                return;
            }
            if (!Directory.Exists(folder))
            {
                Console.WriteLine($"The specified samples folder does not exist: {folder}");
                return;
            }

            var modelService = new InMemoryModelService();
            var audioService = new SoundFingerprintingAudioService();

            var sampleFiles = Directory.GetFiles(folder, "*.wav");
            int totalSteps = sampleFiles.Length;
            int filesFound = 0;
            int filesAnalyzed = 0;

            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "found_files.txt");
            using (var logWriter = new StreamWriter(logFilePath, false))
            {
                // Indexing samples
                Console.WriteLine("Starting sample indexing...");
                for (int i = 0; i < sampleFiles.Length; i++)
                {
                    var file = sampleFiles[i];
                    try
                    {
                        var track = new TrackInfo(file, file, "unknown");
                        var hashes = FingerprintCommandBuilder.Instance
                            .BuildFingerprintCommand()
                            .From(file)
                            .UsingServices(audioService)
                            .Hash()
                            .Result;
                        modelService.Insert(track, hashes);
                        int percent = (int)(((i + 1) * 100.0) / totalSteps);
                        Console.WriteLine($"[{percent}%] Sample {i + 1}/{sampleFiles.Length} indexed: {Path.GetFileName(file)}.");
                    }
                    catch (Exception ex)
                    {
                        int percent = (int)(((i + 1) * 100.0) / totalSteps);
                        Console.WriteLine($"[{percent}%] Error indexing file {file}: {ex.Message}, skipping.");
                        continue;
                    }
                }

                // Global similarity search
                Console.WriteLine("Starting similarity search...");
                var queryResult = QueryCommandBuilder.Instance
                    .BuildQueryCommand()
                    .From(mainFile)
                    .UsingServices(modelService, audioService)
                    .Query()
                    .Result;

                filesAnalyzed = sampleFiles.Length;

                foreach (var match in queryResult.ResultEntries)
                {
                    string foundFile = Path.GetFileName(match.TrackId);
                    Console.WriteLine($"Sample detected: {foundFile}");
                    logWriter.WriteLine(foundFile);
                    filesFound++;
                }

                Console.WriteLine($"Analysis complete. {filesFound} files found out of {filesAnalyzed} analyzed.");
            }
        }
    }
}