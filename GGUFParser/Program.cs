using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ozeki
{
    class Program
    {
        private static string _selectedDirectory = "C:\\AIModels";
        private static List<string> _ggufFiles = new List<string>();
        private static OzGGUFFile _currentFile = null;

        static void Main(string[] args)
        {
            if (Directory.Exists(_selectedDirectory))
            {
                _ggufFiles = FindGGUFFiles(_selectedDirectory);
            }

            while (true)
            {
                ShowMainMenu();
            }
        }

        static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    GGUF Parser - Main Menu               ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine($"║  Selected Directory: {_selectedDirectory,-56}║".Substring(0, 58) + " ║");
            Console.WriteLine($"║  GGUF Files Found: {_ggufFiles.Count,-56}║".Substring(0, 58) + " ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                          ║");
            Console.WriteLine("║  [1] Select Directory                                    ║");
            Console.WriteLine("║  [2] View GGUF Files                                     ║");
            Console.WriteLine("║  [3] Load and Inspect GGUF File                          ║");
            Console.WriteLine("║                                                          ║");
            Console.WriteLine("║  [0] Exit                                                ║");
            Console.WriteLine("║                                                          ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.Write("\n  Enter your choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SelectDirectory();
                    break;
                case "2":
                    ViewGGUFFiles();
                    break;
                case "3":
                    LoadAndInspectFile();
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
            }
        }

        static void SelectDirectory()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                   Select Directory                       ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  Enter the full path to a directory:");
            Console.Write("  > ");

            var path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("\n  No path entered. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            if (!Directory.Exists(path))
            {
                Console.WriteLine($"\n  ERROR: Directory does not exist: {path}");
                Console.WriteLine("  Press any key to continue...");
                Console.ReadKey();
                return;
            }

            _selectedDirectory = path;
            _ggufFiles = FindGGUFFiles(path);
            _currentFile = null;

            Console.WriteLine($"\n  Found {_ggufFiles.Count} GGUF file(s) in subdirectories.");
            Console.WriteLine("  Press any key to continue...");
            Console.ReadKey();
        }

        static List<string> FindGGUFFiles(string directory)
        {
            var files = new List<string>();
            try
            {
                foreach (var file in Directory.GetFiles(directory, "*.gguf", SearchOption.AllDirectories))
                {
                    files.Add(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error scanning directory: {ex.Message}");
            }
            return files;
        }

        static void ViewGGUFFiles()
        {
            if (string.IsNullOrEmpty(_selectedDirectory))
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                      GGUF Files                          ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine("  No directory selected. Please select a directory first.");
                Console.WriteLine("\n  Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      GGUF Files                          ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"  Directory: {_selectedDirectory}");
            Console.WriteLine();

            if (_ggufFiles.Count == 0)
            {
                Console.WriteLine("  No GGUF files found.");
            }
            else
            {
                for (int i = 0; i < _ggufFiles.Count; i++)
                {
                    var file = _ggufFiles[i];
                    var relativePath = file.Replace(_selectedDirectory, "").TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    Console.WriteLine($"  [{i + 1,3}] {relativePath}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("  Press any key to continue...");
            Console.ReadKey();
        }

        static void LoadAndInspectFile()
        {
            if (string.IsNullOrEmpty(_selectedDirectory))
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                 Load and Inspect File                    ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine("  No directory selected. Please select a directory first.");
                Console.WriteLine("\n  Press any key to continue...");
                Console.ReadKey();
                return;
            }

            if (_ggufFiles.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                 Load and Inspect File                    ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine("  No GGUF files found. Please select a directory with GGUF files.");
                Console.WriteLine("\n  Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 Load and Inspect File                    ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  Available GGUF files:");
            Console.WriteLine();

            for (int i = 0; i < _ggufFiles.Count; i++)
            {
                var file = _ggufFiles[i];
                var relativePath = file.Replace(_selectedDirectory, "").TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                Console.WriteLine($"  [{i + 1,3}] {relativePath}");
            }

            Console.WriteLine();
            Console.WriteLine("  [0] Cancel");
            Console.WriteLine();
            Console.Write("  Enter the number of the file to load: ");

            var input = Console.ReadLine();

            if (input == "0")
                return;

            if (!int.TryParse(input, out int selection) || selection < 1 || selection > _ggufFiles.Count)
            {
                Console.WriteLine("\n  Invalid selection. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var selectedFile = _ggufFiles[selection - 1];
            LoadAndDisplayFileInfo(selectedFile);
        }

        static void LoadAndDisplayFileInfo(string filePath)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    File Information                      ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"  Loading: {Path.GetFileName(filePath)}");
            Console.WriteLine();

            var error = "";
            if (!OzGGUFFile.Create(filePath, out _currentFile, out error))
            {
                Console.WriteLine($"  ERROR loading file: {error}");
                Console.WriteLine("\n  Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("  File loaded successfully!");
            Console.WriteLine();

            // Display key information
            DisplayFileInfo(_currentFile);

            Console.WriteLine();
            Console.WriteLine("  Press any key to continue...");
            Console.ReadKey();
        }

        static void DisplayFileInfo(OzGGUFFile file)
        {
            // File Info Section
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      File Info                           ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            var fileName = Path.GetFileName(file.FileName);
            Console.WriteLine($"║  File Name: {fileName,-56}║".Substring(0, 58) + " ║");
            Console.WriteLine($"║  Architecture: {file.Architecture,-56}║".Substring(0, 58) + " ║");
            Console.WriteLine($"║  Version: {(file.VersionNumber?.Value ?? 0),-56}║".Substring(0, 58) + " ║");
            Console.WriteLine($"║  Tensor Count: {(file.TensorCount?.Value ?? 0),-56}║".Substring(0, 58) + " ║");
            Console.WriteLine($"║  Metadata Count: {(file.MDCount?.Value ?? 0),-56}║".Substring(0, 58) + " ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

            Console.WriteLine();

            // Key Metadata Section
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                   Key Metadata                           ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");

            DisplayKeyMetadata(file);

            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

            Console.WriteLine();

            // Tensors Section
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      Tensors                             ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");

            if (file.Tensors != null && file.Tensors.Count > 0)
            {
                // Show first 10 tensors
                var displayCount = Math.Min(10, file.Tensors.Count);
                for (int i = 0; i < displayCount; i++)
                {
                    var tensor = file.Tensors[i];
                    var name = tensor.Name?.Value ?? "Unknown";
                    if (name.Length > 50) name = name.Substring(0, 47) + "...";
                    Console.WriteLine($"║  [{i + 1,2}] {name,-56}║".Substring(0, 58) + " ║");
                    Console.WriteLine($"║      Type: {tensor.Type?.ToString() ?? "Unknown",-56}║".Substring(0, 58) + " ║");
                    var dims = tensor.ElementCounts?.Select(e => e.Value).ToList() ?? new List<ulong>();
                    Console.WriteLine($"║      Dims: {string.Join(", ", dims),-56}║".Substring(0, 58) + " ║");
                }

                if (file.Tensors.Count > 10)
                {
                    var moreText = $"  ... and {file.Tensors.Count - 10} more tensors";
                    var line = $"║  ... and {file.Tensors.Count - 10} more tensors";
                    Console.WriteLine(line.PadRight(58) + " ║");
                }
            }
            else
            {
                Console.WriteLine("║  No tensors loaded.                                      ║");
            }

            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        }

        static void DisplayKeyMetadata(OzGGUFFile file)
        {
            if (file.MDs == null || file.MDs.Count == 0)
            {
                Console.WriteLine("║  No metadata available.                                  ║");
                return;
            }

            // Key metadata fields to display
            var keyFields = new Dictionary<string, string>
            {
                { "general.architecture", "Architecture" },
                { "general.name", "Model Name" },
                { "general.quantization_version", "Quantization Version" },
                { "general.file_type", "File Type" },
                { "general.size_label", "Size Label" },
                { "llama.context_length", "Context Length" },
                { "llama.embedding_length", "Embedding Length" },
                { "llama.block_count", "Block Count" },
                { "llama.feed_forward_length", "Feed Forward Length" },
                { "llama.attention.head_count", "Attention Heads" },
                { "llama.attention.head_count_kv", "KV Heads" },
                { "llama.attention.layer_norm_rms_eps", "RMS Norm Epsilon" },
                { "tokenizer.ggml.model", "Tokenizer Model" },
                { "tokenizer.ggml.tokens", "Token Count" },
                { "tokenizer.ggml.bos_token_id", "BOS Token ID" },
                { "tokenizer.ggml.eos_token_id", "EOS Token ID" },
            };

            foreach (var kvp in keyFields)
            {
                if (file.MDs.TryGetValue(Encoding.UTF8.GetBytes(kvp.Key), out var md))
                {
                    var value = GetMetadataValue(md);
                    if (!string.IsNullOrEmpty(value))
                    {
                        var label = kvp.Value;
                        if (label.Length > 20) label = label.Substring(0, 20);
                        var linePrefix = $"  {label}: ";
                        var valueStr = value ?? "";
                        var line = $"║  {label,-20}: {valueStr}";
                        Console.WriteLine(line.PadRight(58) + " ║");
                    }
                }
            }
        }

        static string GetMetadataValue(OzGGUF_MD md)
        {
            if (md?.MDValue == null)
                return "";

            try
            {
                switch (md.MDValue)
                {
                    case OzGGUF_String str:
                        var val = str.Value;
                        if (val.Length > 35) val = val.Substring(0, 32) + "...";
                        return val;
                    case OzGGUF_Int32 i32:
                        return i32.Value.ToString();
                    case OzGGUF_Int64 i64:
                        return i64.Value.ToString();
                    case OzGGUF_UInt32 ui32:
                        return ui32.Value.ToString();
                    case OzGGUF_UInt64 ui64:
                        return ui64.Value.ToString();
                    case OzGGUF_Float32 f32:
                        return f32.Value.ToString("G6");
                    case OzGGUF_Float64 f64:
                        return f64.Value.ToString("G6");
                    case OzGGUF_Bool b:
                        return b.Value.ToString();
                    default:
                        return md.MDValue.ToString();
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
