using System;
class Program { 
    static void Main() { 
        Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("Admin@123")); 
        Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("Pharma@123")); 
        Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("John@123")); 
    } 
}
