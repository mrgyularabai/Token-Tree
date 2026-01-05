using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Ozeki;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;


//BenchmarkRunner.Run<SmallTextInputBenchmark>();
BenchmarkRunner.Run<LargeTextInputBenchmark>();

public class SmallTextInputBenchmark
{
    // Target sizes in ASCII bytes for smaller files
    [Params(
        128,   // 0.125 KB
        256,   // 0.25 KB
        512,   // 0.5 KB
        1024,  // 1 KB
        2048   // 2 KB
    )]
    public int TargetSizeBytes;

    // Number of calls per benchmark invocation
    [Params(10000)]
    public int Iterations;

    private string[] _preparedInputs;
    private int _index;
    private static OzGGUFFile _currentFile;
    private static OzAITokenizer _tokenizer;

    [GlobalSetup]
    public void GlobalSetup()
    {
        InitializeTokenizer();
        _preparedInputs = PrepareInputs(TargetSizeBytes, isLarge: false);
        _index = 0;
    }

    [Benchmark]
    public void Tokenization()
    {
        for (int i = 0; i < Iterations; i++)
        {
            var input = _preparedInputs[_index++ % _preparedInputs.Length];
            FunctionUnderTest(input);
        }
    }

    private static void InitializeTokenizer()
    {
        if (_tokenizer != null) return;

        var filePath = @"C:\AIModels\Llama-3.2-1B-Instruct-f16.gguf";
        if (!OzGGUFFile.Create(filePath, out _currentFile, out string err))
        {
            throw new InvalidOperationException($"Failed to load GGUF file: {err}");
        }

        if (!OzAITokenizer.CreateFromFile(_currentFile, out _tokenizer, out err))
        {
            throw new InvalidOperationException($"Failed to create tokenizer: {err}");
        }
    }

    private static string[] PrepareInputs(int targetSize, bool isLarge)
    {
        var files = Directory.GetFiles(@"C:\_Projects\_2026-01-02_TokenizerPaper\archive\All", "*.txt");
        if (files.Length == 0)
            throw new InvalidOperationException("No input files found.");

        // Read and sort files by size (ascending order)
        var fileData = files
            .Select(f => new { Path = f, Size = new FileInfo(f).Length })
            .OrderBy(x => x.Size)
            .ToArray();

        var inputs = new List<string>(fileData.Length);

        foreach (var file in fileData)
        {
            // Read file once
            string text = File.ReadAllText(file.Path);

            // Normalize newlines
            text = text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");

            // Convert to ASCII bytes
            byte[] asciiBytes = Encoding.ASCII.GetBytes(text);

            // Only include files that have enough content for the target size
            if (asciiBytes.Length < targetSize)
                continue;

            // Truncate to exactly targetSize bytes
            var truncated = new byte[targetSize];
            Array.Copy(asciiBytes, truncated, targetSize);

            // Convert back to string
            inputs.Add(Encoding.ASCII.GetString(truncated));
        }

        if (inputs.Count == 0)
            throw new InvalidOperationException($"No files found with sufficient content for target size {targetSize} bytes.");

        return inputs.ToArray();
    }

    private static void FunctionUnderTest(string text)
    {
        var tokens = new List<int>();
        _tokenizer.Tokenize(text, tokens, out string times, out string error, true);
    }
}

public class LargeTextInputBenchmark
{
    // Target sizes in ASCII bytes for larger files
    [Params(
        //1024,          // 1 KB
        //1024 * 2,      // 2 KB
        //1024 * 4,      // 4 KB
        1024 * 8      // 8 KB
        //1024 * 16,     // 16 KB
        //1024 * 32,     // 32 KB
        //1024 * 64,     // 64 KB
        //1024 * 128,    // 128 KB
        //1024 * 256,    // 256 KB
        //1024 * 512,    // 512 KB
        //1024 * 1024,   // 1 MB
        //1024 * 1024 * 2,   // 2 MB
        //1024 * 1024 * 4    // 4 MB
    )]
    public int TargetSizeBytes;

    // Number of calls per benchmark invocation
    [Params(10000)]
    public int Iterations;
    public int realIter;

    private string[] _preparedInputs;
    private int _index;
    private static OzGGUFFile _currentFile;
    private static OzAITokenizer _tokenizer;

    [GlobalSetup]
    public void GlobalSetup()
    {
        InitializeTokenizer();
        _preparedInputs = PrepareInputs(TargetSizeBytes, isLarge: true);
        _index = 0;
        realIter = Iterations;
        if (TargetSizeBytes > 1024 * 4)
        {
            realIter = 1000;
        }
        if (TargetSizeBytes > 1024 * 64)
        {
            realIter = 100;
        }
        if (TargetSizeBytes > 1024 * 512)
        {
            realIter = 10;
        }
    }

    [Benchmark]
    public void Tokenization()
    {
        for (int i = 0; i < realIter; i++)
        {
            var input = _preparedInputs[_index++ % _preparedInputs.Length];
            FunctionUnderTest(input);
        }
    }

    private static void InitializeTokenizer()
    {
        if (_tokenizer != null) return;

        var filePath = @"C:\AIModels\Llama-3.2-1B-Instruct-f16.gguf";
        if (!OzGGUFFile.Create(filePath, out _currentFile, out string err))
        {
            throw new InvalidOperationException($"Failed to load GGUF file: {err}");
        }

        if (!OzAITokenizer.CreateFromFile(_currentFile, out _tokenizer, out err))
        {
            throw new InvalidOperationException($"Failed to create tokenizer: {err}");
        }
    }

    private static string[] PrepareInputs(int targetSize, bool isLarge)
    {
        var files = Directory.GetFiles(@"C:\_Projects\_2026-01-02_TokenizerPaper\archive\All", "*.txt");
        if (files.Length == 0)
            throw new InvalidOperationException("No input files found.");

        // Read and sort files by size
        var fileData = files
            .Select(f => new { Path = f, Size = new FileInfo(f).Length })
            .OrderBy(x => x.Size)
            .ToArray();

        // Filter files based on category
        var inputs = new List<string>(fileData.Length);

        foreach (var file in fileData)
        {
            // Read file once
            string text = File.ReadAllText(file.Path);

            // Normalize newlines
            text = text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");

            // Convert to ASCII bytes
            byte[] asciiBytes = Encoding.ASCII.GetBytes(text);

            // Only truncate if file has enough content
            if (asciiBytes.Length < targetSize)
                continue;

            // Truncate to target size
            var truncated = new byte[targetSize];
            Array.Copy(asciiBytes, truncated, targetSize);

            // Convert back to string
            inputs.Add(Encoding.ASCII.GetString(truncated));
        }

        if (inputs.Count == 0)
            throw new InvalidOperationException($"No files found with sufficient content for target size {targetSize} bytes in {(isLarge ? "large" : "small")} category.");

        return inputs.ToArray();
    }

    private static void FunctionUnderTest(string text)
    {
        var tokens = new List<int>();
        _tokenizer.Tokenize(text, tokens, out string times, out string error, true);
    }
}