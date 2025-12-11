using System;
using System.IO;
using BLang;

class Program
{
    static void Main(string[] args)
    {
        // MODE 1: Drag and Drop
        if (args.Length > 0)
        {
            RunFile(args[0]);
        }
        // MODE 2: CLI
        else
        {
            RunCLI();
        }
    }

    static void RunCLI()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("========================================");
        Console.WriteLine("   BLang CLI V1.0-Beta-Build");
        Console.WriteLine("========================================");
        Console.ResetColor();
        Console.WriteLine("Type 'help' for commands.");

        while (true)
        {
            // GET CURRENT DIRECTORY FOR THE PROMPT
            string currentPath = Directory.GetCurrentDirectory();

            Console.ForegroundColor = ConsoleColor.Green;
            // Show path like: BLang [C:\Users\You\Desktop] >
            Console.Write($"\nBLang [{currentPath}] > ");
            Console.ResetColor();

            string input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input)) continue;

            // --- COMMANDS ---

            if (input == "exit" || input == "quit")
            {
                break;
            }
            else if (input == "clear" || input == "cls")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("   BLang CLI V1.0 Beta Build");
                Console.ResetColor();
            }
            else if (input == "help")
            {
                Console.WriteLine("Available Commands:");
                Console.WriteLine("  run <file.bl> : Executes a file");
                Console.WriteLine("  cd <folder>   : Change directory");
                Console.WriteLine("  dir           : List files in current folder");
                Console.WriteLine("  cls           : Clears screen");
                Console.WriteLine("  exit          : Closes BLang");
            }
            // NEW: CD COMMAND
            else if (input.StartsWith("cd "))
            {
                string newDir = input.Substring(3).Trim();
                // Remove quotes if user typed: cd "My Folder"
                newDir = newDir.Replace("\"", "");

                try
                {
                    Directory.SetCurrentDirectory(newDir);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
            // NEW: DIR / LS COMMAND
            else if (input == "dir" || input == "ls")
            {
                var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.bl");
                Console.WriteLine($"Found {files.Length} BLang files:");
                foreach (var file in files)
                {
                    Console.WriteLine("  - " + Path.GetFileName(file));
                }
            }
            // RUN COMMAND
            else if (input.StartsWith("run "))
            {
                string path = input.Substring(4).Trim();
                path = path.Replace("\"", "");
                Console.WriteLine("\n");
                RunFile(path);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Unknown command: '{input}'. Type 'help'.");
                Console.ResetColor();
            }
        }
    }

    static void RunFile(string filePath)
    {
        // Check if file exists (Relative OR Absolute)
        if (!File.Exists(filePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: File '{filePath}' not found in current directory.");
            Console.ResetColor();
            return;
        }

        try
        {
            string code = File.ReadAllText(filePath);
            var lexer = new Lexer(code);
            var tokens = lexer.Tokenize();
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            var interpreter = new Interpreter();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ResetColor();

            interpreter.Run(ast);
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"CRASH: {ex.Message}");
            Console.ResetColor();
        }
    }
}