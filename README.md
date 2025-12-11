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

---
