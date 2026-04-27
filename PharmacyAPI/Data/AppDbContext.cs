using Microsoft.EntityFrameworkCore;
using PharmacyAPI.Models;

namespace PharmacyAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique email for users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // One-to-one: Order -> Payment
            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.OrderId)
                .IsUnique();

            // Cascade delete for OrderItems
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Medicine)
                .WithMany(m => m.OrderItems)
                .HasForeignKey(oi => oi.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> Orders: no cascade
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> Prescriptions: no cascade
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.User)
                .WithMany(u => u.Prescriptions)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order -> Prescription: optional
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Prescription)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.PrescriptionId)
                .OnDelete(DeleteBehavior.SetNull);

            // Payment -> Order: cascade
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== SEED DATA =====

            // Admin user (password: Admin@123)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin User",
                    Email = "admin@pharmacy.com",
                    PasswordHash = "$2a$11$edR8FoCTXi0E7SB3Je32Ze9A6DXfQCPxZipT2hxFVuIcGNftigMMa", // Admin@123
                    Role = "Admin",
                    Phone = "9876543210",
                    Address = "123 Admin Street, Healthcare City",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    Name = "Pharmacist User",
                    Email = "pharmacist@pharmacy.com",
                    PasswordHash = "$2a$11$0E5s7kw4UaiNeZ0mm0SWnOP4gsc82GGq.6PA6gvWZ.UjzIuvcCARS", // Pharma@123
                    Role = "Pharmacist",
                    Phone = "9876543211",
                    Address = "456 Pharma Lane, Healthcare City",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 3,
                    Name = "John Customer",
                    Email = "john@example.com",
                    PasswordHash = "$2a$11$Mj0XmcxisGbHWG8OShMhzuQPNzZ6ZWr1wKeHdw.nii2.CNFkBStnq", // John@123
                    Role = "Customer",
                    Phone = "9876543212",
                    Address = "789 Customer Road, Wellness Town",
                    CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed medicines (20 medicines across categories)
            modelBuilder.Entity<Medicine>().HasData(
                // Pain Relief
                new Medicine { Id = 1, Name = "Paracetamol 500mg", Description = "Effective pain reliever and fever reducer. Suitable for headaches, muscle aches, and cold symptoms.", Category = "Pain Relief", Price = 35.00m, StockQuantity = 500, ImageUrl = "/images/medicines/paracetamol.jpg", RequiresPrescription = false, Dosage = "1-2 tablets every 4-6 hours", Packaging = "Strip of 10 tablets", Manufacturer = "Sun Pharma", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Medicine { Id = 2, Name = "Ibuprofen 400mg", Description = "Non-steroidal anti-inflammatory drug for pain, fever, and inflammation.", Category = "Pain Relief", Price = 45.00m, StockQuantity = 350, ImageUrl = "/images/medicines/ibuprofen.jpg", RequiresPrescription = false, Dosage = "1 tablet every 6-8 hours", Packaging = "Strip of 10 tablets", Manufacturer = "Cipla", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Medicine { Id = 3, Name = "Diclofenac 50mg", Description = "Prescription-grade anti-inflammatory for moderate to severe pain relief.", Category = "Pain Relief", Price = 65.00m, StockQuantity = 200, ImageUrl = "/images/medicines/diclofenac.jpg", RequiresPrescription = true, Dosage = "1 tablet twice daily", Packaging = "Strip of 10 tablets", Manufacturer = "Novartis", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

                // Antibiotics
                new Medicine { Id = 4, Name = "Amoxicillin 500mg", Description = "Broad-spectrum antibiotic for bacterial infections including throat, ear, and urinary infections.", Category = "Antibiotics", Price = 120.00m, StockQuantity = 150, ImageUrl = "/images/medicines/amoxicillin.jpg", RequiresPrescription = true, Dosage = "1 capsule every 8 hours", Packaging = "Strip of 10 capsules", Manufacturer = "Dr. Reddy's", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Medicine { Id = 5, Name = "Azithromycin 250mg", Description = "Macrolide antibiotic for respiratory, skin, and ear infections.", Category = "Antibiotics", Price = 95.00m, StockQuantity = 180, ImageUrl = "/images/medicines/azithromycin.jpg", RequiresPrescription = true, Dosage = "1 tablet daily for 3-5 days", Packaging = "Strip of 6 tablets", Manufacturer = "Zydus", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Medicine { Id = 6, Name = "Ciprofloxacin 500mg", Description = "Fluoroquinolone antibiotic for urinary tract and respiratory infections.", Category = "Antibiotics", Price = 85.00m, StockQuantity = 120, ImageUrl = "/images/medicines/ciprofloxacin.jpg", RequiresPrescription = true, Dosage = "1 tablet every 12 hours", Packaging = "Strip of 10 tablets", Manufacturer = "Lupin", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

                // Vitamins & Supplements
                new Medicine { Id = 7, Name = "Vitamin C 1000mg", Description = "High-potency vitamin C supplement for immunity support and antioxidant protection.", Category = "Vitamins & Supplements", Price = 250.00m, StockQuantity = 400, ImageUrl = "/images/medicines/vitaminc.jpg", RequiresPrescription = false, Dosage = "1 tablet daily", Packaging = "Bottle of 60 tablets", Manufacturer = "HealthKart", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Medicine { Id = 8, Name = "Vitamin D3 60000 IU", Description = "Weekly vitamin D3 supplement for bone health and calcium absorption.", Category = "Vitamins & Supplements", Price = 180.00m, StockQuantity = 300, ImageUrl = "/images/medicines/vitamind.jpg", RequiresPrescription = false, Dosage = "1 capsule weekly", Packaging = "Strip of 8 capsules", Manufacturer = "Sun Pharma", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Medicine { Id = 9, Name = "Multivitamin Complex", Description = "Complete daily multivitamin with minerals for overall health and wellness.", Category = "Vitamins & Supplements", Price = 350.00m, StockQuantity = 250, ImageUrl = "/images/medicines/multivitamin.jpg", RequiresPrescription = false, Dosage = "1 tablet daily after meals", Packaging = "Bottle of 90 tablets", Manufacturer = "Centrum", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Medicine { Id = 10, Name = "Omega-3 Fish Oil", Description = "Premium fish oil capsules rich in EPA and DHA for heart and brain health.", Category = "Vitamins & Supplements", Price = 450.00m, StockQuantity = 200, ImageUrl = "/images/medicines/omega3.jpg", RequiresPrescription = false, Dosage = "1 capsule twice daily", Packaging = "Bottle of 60 softgels", Manufacturer = "HealthVit", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

                // Digestive Health
                new Medicine { Id = 11, Name = "Omeprazole 20mg", Description = "Proton pump inhibitor for acid reflux, heartburn, and gastric ulcers.", Category = "Digestive Health", Price = 75.00m, StockQuantity = 300, ImageUrl = "/images/medicines/omeprazole.jpg", RequiresPrescription = false, Dosage = "1 capsule before breakfast", Packaging = "Strip of 15 capsules", Manufacturer = "Cipla", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Medicine { Id = 12, Name = "Domperidone 10mg", Description = "Anti-nausea medication for relief of nausea, vomiting, and bloating.", Category = "Digestive Health", Price = 55.00m, StockQuantity = 250, ImageUrl = "/images/medicines/domperidone.jpg", RequiresPrescription = false, Dosage = "1 tablet before meals", Packaging = "Strip of 10 tablets", Manufacturer = "Torrent Pharma", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

                // Allergy & Cold
                new Medicine { Id = 13, Name = "Cetirizine 10mg", Description = "Antihistamine for allergic rhinitis, hay fever, and skin allergies.", Category = "Allergy & Cold", Price = 25.00m, StockQuantity = 600, ImageUrl = "/images/medicines/cetirizine.jpg", RequiresPrescription = false, Dosage = "1 tablet daily", Packaging = "Strip of 10 tablets", Manufacturer = "Dr. Reddy's", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Medicine { Id = 14, Name = "Montelukast 10mg", Description = "Leukotriene receptor antagonist for asthma and allergic rhinitis.", Category = "Allergy & Cold", Price = 140.00m, StockQuantity = 8, ImageUrl = "/images/medicines/montelukast.jpg", RequiresPrescription = true, Dosage = "1 tablet daily at bedtime", Packaging = "Strip of 10 tablets", Manufacturer = "Sun Pharma", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Medicine { Id = 15, Name = "Cough Syrup (Dextromethorphan)", Description = "Effective cough suppressant for dry, tickly cough relief.", Category = "Allergy & Cold", Price = 85.00m, StockQuantity = 180, ImageUrl = "/images/medicines/coughsyrup.jpg", RequiresPrescription = false, Dosage = "10ml every 4-6 hours", Packaging = "Bottle of 100ml", Manufacturer = "Dabur", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

                // Skin Care
                new Medicine { Id = 16, Name = "Clotrimazole Cream 1%", Description = "Antifungal cream for athlete's foot, ringworm, and fungal skin infections.", Category = "Skin Care", Price = 95.00m, StockQuantity = 150, ImageUrl = "/images/medicines/clotrimazole.jpg", RequiresPrescription = false, Dosage = "Apply twice daily", Packaging = "Tube of 20g", Manufacturer = "Glenmark", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Medicine { Id = 17, Name = "Betamethasone Cream 0.1%", Description = "Topical corticosteroid for eczema, dermatitis, and inflammatory skin conditions.", Category = "Skin Care", Price = 110.00m, StockQuantity = 5, ImageUrl = "/images/medicines/betamethasone.jpg", RequiresPrescription = true, Dosage = "Apply thin layer twice daily", Packaging = "Tube of 15g", Manufacturer = "GSK", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

                // Diabetes
                new Medicine { Id = 18, Name = "Metformin 500mg", Description = "First-line medication for type 2 diabetes mellitus management.", Category = "Diabetes", Price = 45.00m, StockQuantity = 400, ImageUrl = "/images/medicines/metformin.jpg", RequiresPrescription = true, Dosage = "1 tablet twice daily with meals", Packaging = "Strip of 10 tablets", Manufacturer = "USV", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

                // Heart & BP
                new Medicine { Id = 19, Name = "Amlodipine 5mg", Description = "Calcium channel blocker for high blood pressure and chest pain (angina).", Category = "Heart & BP", Price = 55.00m, StockQuantity = 3, ImageUrl = "/images/medicines/amlodipine.jpg", RequiresPrescription = true, Dosage = "1 tablet daily", Packaging = "Strip of 10 tablets", Manufacturer = "Pfizer", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Medicine { Id = 20, Name = "Aspirin 75mg", Description = "Low-dose aspirin for cardiovascular protection and blood thinning.", Category = "Heart & BP", Price = 30.00m, StockQuantity = 500, ImageUrl = "/images/medicines/aspirin.jpg", RequiresPrescription = false, Dosage = "1 tablet daily", Packaging = "Strip of 14 tablets", Manufacturer = "Bayer", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
