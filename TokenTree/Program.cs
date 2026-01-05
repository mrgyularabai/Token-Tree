using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Ozeki;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Linq;

namespace TokenTree
{
    class Program
    {
        // --------------------------------------------------------------------
        // Global state
        // --------------------------------------------------------------------
        private static string _selectedDirectory = "C:\\AIModels";
        private static List<string> _ggufFiles = new List<string>();
        private static OzGGUFFile _currentFile = null;
        private static OzAITokenizer _tokenizer = null;
        private static List<int> _lastTokens = new List<int>();

        // --------------------------------------------------------------------
        // Entry point
        // --------------------------------------------------------------------
        static void Main(string[] args)
        {
            // Load GGUF file list (if directory exists)
            if (Directory.Exists(_selectedDirectory))
                _ggufFiles = FindGGUFFiles(_selectedDirectory);

            while (true)
            {
                ShowMainMenu();
            }
        }

        // --------------------------------------------------------------------
        // UI helpers
        // --------------------------------------------------------------------
        static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                TokenTree – Console UI                    ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine($"║  Selected Directory: {_selectedDirectory,-56}║".Substring(0, 58) + " ║");
            Console.WriteLine($"║  GGUF Files Found: {_ggufFiles.Count,-56}║".Substring(0, 58) + " ║");
            Console.WriteLine($"║  Model Loaded: {(_currentFile?.FileName ?? "none"),-56}║".Substring(0, 58) + " ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  [1] Load AI Model                                       ║");
            Console.WriteLine("║  [2] Type Text                                           ║");
            Console.WriteLine("║  [3] Load Text File                                      ║");
            Console.WriteLine("║  [4] Save Tokens (CSV)                                   ║");
            Console.WriteLine("║  [5] Load Tokens & Detokenize                            ║");
            Console.WriteLine("║  [6] Download Webpage for Tokenization                   ║");
            Console.WriteLine("║  [0] Exit                                                ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.Write("\n  Enter your choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    LoadModel();
                    break;
                case "2":
                    TypeText();
                    break;
                case "3":
                    LoadTextFromFile();
                    break;
                case "4":
                    SaveTokens();
                    break;
                case "5":
                    LoadTokensAndDetokenize();
                    break;
                case "6":
                    DownloadAndProcessWebpage();
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    break;
            }
        }

        // --------------------------------------------------------------------
        // Directory / file helpers (copied from GGUFParser/Program.cs)
        // --------------------------------------------------------------------
        static void SelectDirectory()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                     Select Directory                     ║");
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
            _tokenizer = null;

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
            catch (Exception) { }
            return files;
        }

        // --------------------------------------------------------------------
        // Model loading
        // --------------------------------------------------------------------
        static void LoadModel()
        {
            // Allow user to change directory first
            Console.WriteLine("\nDo you want to change the model directory? (y/N)");
            var change = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(change) && change.Trim().ToLower().StartsWith("y"))
                SelectDirectory();

            if (_ggufFiles.Count == 0)
            {
                Console.WriteLine("\nNo GGUF files found. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 Load AI Model (Select File)              ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            for (int i = 0; i < _ggufFiles.Count; i++)
            {
                var rel = _ggufFiles[i].Replace(_selectedDirectory, "").TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                Console.WriteLine($"  [{i + 1,3}] {rel}");
            }
            Console.WriteLine("\n  [0] Cancel");
            Console.Write("\n  Enter the number of the file to load: ");

            var input = Console.ReadLine();
            if (input == "0")
                return;

            if (!int.TryParse(input, out int sel) || sel < 1 || sel > _ggufFiles.Count)
            {
                Console.WriteLine("\nInvalid selection. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var filePath = _ggufFiles[sel - 1];
            if (!OzGGUFFile.Create(filePath, out _currentFile, out string err))
            {
                Console.WriteLine($"\nFailed to load GGUF file: {err}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            // Create tokenizer based on model type
            if (!OzAITokenizer.CreateFromFile(_currentFile, out _tokenizer, out err))
            {
                Console.WriteLine($"\nFailed to create tokenizer: {err}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nModel and tokenizer loaded successfully!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        // --------------------------------------------------------------------
        // Text input helpers
        // --------------------------------------------------------------------
        static void TypeText()
        {
            if (!EnsureTokenizer())
                return;

            Console.Clear();
            Console.WriteLine("Enter your text (empty line to finish):");
            var lines = new List<string>();
            while (true)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;
                lines.Add(line);
            }

            var text = string.Join(Environment.NewLine, lines);
            TokenizeAndReport(text);
        }

        static void LoadTextFromFile()
        {
            if (!EnsureTokenizer())
                return;

            Console.Write("\nEnter path to text file: ");
            var path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                Console.WriteLine("\nFile not found. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var text = File.ReadAllText(path);
            TokenizeAndReport(text);
        }

        // --------------------------------------------------------------------
        // Tokenisation & reporting
        // --------------------------------------------------------------------
        static void TokenizeAndReport(string text)
        {
            var tokens = new List<int>();
            var sw = Stopwatch.StartNew();

            if (!_tokenizer.Tokenize(text, tokens, out string times, out string error, true))
            {
                Console.WriteLine($"\nTokenisation error: {error}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            sw.Stop();
            _lastTokens = tokens; // store for later saving/detokenising

            double elapsedMs = sw.Elapsed.TotalMilliseconds;
            double tokensPerSec = tokens.Count / (elapsedMs / 1000.0);

            Console.WriteLine("\n--- Tokenisation Report ---");
            Console.WriteLine($"Total tokens      : {tokens.Count}");
            Console.WriteLine($"Elapsed time (ms) : {elapsedMs:F2}");
            Console.WriteLine($"Tokens / second   : {tokensPerSec:F2}");

            // Show first 20 tokens
            Console.WriteLine("\nFirst 20 tokens (ID : Text):");
            int limit = Math.Min(20, tokens.Count);
            for (int i = 0; i < limit; i++)
            {
                int id = tokens[i];
                string tokenStr = _tokenizer.GetStringsRaw(new List<int> { id });
                Console.WriteLine($"  {id,5} : \"{tokenStr}\"");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        // --------------------------------------------------------------------
        // Save / Load token list
        // --------------------------------------------------------------------
        static void SaveTokens()
        {
            if (_lastTokens == null || _lastTokens.Count == 0)
            {
                Console.WriteLine("\nNo tokens to save. Perform a tokenisation first.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.Write("\nEnter path to save CSV (e.g., tokens.csv): ");
            var path = Console.ReadLine();

            try
            {
                File.WriteAllText(path, string.Join(",", _lastTokens));
                Console.WriteLine($"\nTokens saved to {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFailed to save tokens: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void LoadTokensAndDetokenize()
        {
            if (!EnsureTokenizer())
                return;

            Console.Write("\nEnter path to CSV token file: ");
            var path = Console.ReadLine();

            if (!File.Exists(path))
            {
                Console.WriteLine("\nFile not found.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            try
            {
                var csv = File.ReadAllText(path);
                var tokenStrings = csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var tokens = new List<int>();
                foreach (var s in tokenStrings)
                {
                    if (int.TryParse(s.Trim(), out int id))
                        tokens.Add(id);
                }

                string detokenized = _tokenizer.GetStringsRaw(tokens);
                Console.WriteLine("\n--- Detokenized Text ---");
                Console.WriteLine(detokenized);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError loading tokens: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        // --------------------------------------------------------------------
        // Helper validation
        // --------------------------------------------------------------------
        static bool EnsureTokenizer()
        {
            if (_tokenizer == null)
            {
                Console.WriteLine("\nNo model loaded. Load a model first (option 1).");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return false;
            }
            return true;
        }

        static void DownloadAndProcessWebpage()
        {
            if (!EnsureTokenizer())
                return;

            Console.Write("\nEnter URL: ");
            var url = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(url))
            {
                Console.WriteLine("\nInvalid URL. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            try
            {
                using var client = new HttpClient();
                var html = client.GetStringAsync(url).Result;

                // Extract the <body> content (including multiline)
                var bodyMatch = Regex.Match(html, "<body[^>]*>(.*?)</body>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                var body = bodyMatch.Success ? bodyMatch.Groups[1].Value : html;

                // Remove script and style sections
                body = Regex.Replace(body, "<script.*?>.*?</script>", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                body = Regex.Replace(body, "<style.*?>.*?</style>", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // Remove HTML comments (including multiline)
                body = Regex.Replace(body, "<!--.*?-->", " ", RegexOptions.Singleline);
                // Strip all remaining HTML tags (including multiline tags)
                var text = Regex.Replace(body, "<[^>]+>", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // Collapse whitespace and trim
                text = Regex.Replace(text, @"\s+", " ").Trim();

                // Take first 200 words
                var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var first200 = string.Join(" ", words.Take(200));

                Console.WriteLine("\n--- Extracted Text (first 200 words) ---");
                Console.WriteLine(first200);
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();

                TokenizeAndReport(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError downloading or processing: {ex.Message}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
