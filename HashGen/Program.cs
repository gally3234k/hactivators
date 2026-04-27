using System;
using System.IO;

class Program {
    static void Main() {
        var file = @"C:\Users\Rohith\OneDrive\Desktop\Medical\PharmacyAPI\Data\AppDbContext.cs";
        var content = File.ReadAllText(file);
        content = content.Replace("BCrypt.Net.BCrypt.HashPassword(\"Admin@123\")", "\"" + BCrypt.Net.BCrypt.HashPassword("Admin@123") + "\"");
        content = content.Replace("BCrypt.Net.BCrypt.HashPassword(\"Pharma@123\")", "\"" + BCrypt.Net.BCrypt.HashPassword("Pharma@123") + "\"");
        content = content.Replace("BCrypt.Net.BCrypt.HashPassword(\"John@123\")", "\"" + BCrypt.Net.BCrypt.HashPassword("John@123") + "\"");
        File.WriteAllText(file, content);
    }
}
