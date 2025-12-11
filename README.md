# BLang — A Custom Programming Language & CLI Environment

BLang (BossLang) is a lightweight, expressive scripting language designed for learning, automation, and experimentation.  
It includes:

- A custom lexer, parser, and interpreter  
- User-defined functions  
- Variables & types  
- Control flow (`if`, `else`, `while`, `for`)  
- Custom operators (`+` for math, `++` for string concat)  
- A fully working CLI tool  
- Directory navigation & command execution

BLang is written entirely in **C#** and is designed to be simple, readable, and fun to experiment with.

---

## ✨ Features

### 🧠 **Custom Syntax**
BLang supports a C-like syntax with additional flavor:

```c
func addNums(int a, int b) {
    return a + b;
}

int x = 10;
int y = 5;

if (x > y) {
    print_out("X is bigger");
} else {
    print_out("Y is bigger");
}

int result = addNums(x, y);
print_out("Math result: " ++ result);

for(int i = 0; i < 3; i = i + 1) {
    print_out("Looping: " ++ i);
}```

⚙️ Types Supported

int

string

🔢 Custom Operators
Operator	Purpose
+	Integer addition
++	String concatenation
🧵 Control Flow

if / else

while

for loops
(With full expression parsing)

🧩 Functions

Supports:

parameters

return values

calling functions inside expressions

💻 BLang CLI

A fully interactive command-line environment:

BLang CLI V1.0-Beta-Build
Type 'help' for commands.

Supports:

Command	Description
run <file>	Executes a .bl script
cd <path>	Changes current directory
help	Shows all available commands
exit	Closes the CLI

Example:

BLang [C:\MyFolder] > run script.bl

📁 Project Structure
/BLang
 ├── Lexer/
 ├── Parser/
 ├── Interpreter/
 ├── CLI/
 └── Program.cs

Each component is built manually (no parser generators), giving full control over syntax and behavior.

🚀 Running BLang

Clone the project:

git clone https://github.com/yourusername/BLang.git


Navigate into the project:

cd BLang


Build:

dotnet build


Run the CLI:

dotnet run


Run a BLang script inside the CLI:

run myscript.bl

📌 Example Output
X is bigger
Math result: 15
Looping: 0
Looping: 1
Looping: 2

🛠 Future Plans

Arrays / lists

Error system with line numbers

Standard library (math, file I/O, strings)

A REPL (boss> interactive mode)

Module system (import)

Compiler backend (possibly LLVM or C output)

Packaging system

Embeddable scripting for game engines

🏗 Goal

BLang is a personal experiment that evolved into a real scripting language.
Its purpose is to:

help explore compiler & language design

serve as a scripting engine for tools or games

evolve into a clean, fast, modern lightweight language

🧑‍💻 Author

Created by [Your Name], 2025
A passion project exploring compilers, scripting languages, and system design.

⭐ Contribute

Feel free to fork, submit issues, or open pull requests!
Ideas, improvements, and contributions are always welcome.

📜 License

MIT License — free to use, modify, and distribute.
