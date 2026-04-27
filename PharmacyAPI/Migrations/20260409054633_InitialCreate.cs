using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PharmacyAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequiresPrescription = table.Column<bool>(type: "bit", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Packaging = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PrescriptionId = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ShippingAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Medicines_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Medicines",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "Dosage", "ImageUrl", "IsAvailable", "Manufacturer", "Name", "Packaging", "Price", "RequiresPrescription", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "Pain Relief", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Effective pain reliever and fever reducer. Suitable for headaches, muscle aches, and cold symptoms.", "1-2 tablets every 4-6 hours", "/images/medicines/paracetamol.jpg", true, "Sun Pharma", "Paracetamol 500mg", "Strip of 10 tablets", 35.00m, false, 500 },
                    { 2, "Pain Relief", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Non-steroidal anti-inflammatory drug for pain, fever, and inflammation.", "1 tablet every 6-8 hours", "/images/medicines/ibuprofen.jpg", true, "Cipla", "Ibuprofen 400mg", "Strip of 10 tablets", 45.00m, false, 350 },
                    { 3, "Pain Relief", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Prescription-grade anti-inflammatory for moderate to severe pain relief.", "1 tablet twice daily", "/images/medicines/diclofenac.jpg", true, "Novartis", "Diclofenac 50mg", "Strip of 10 tablets", 65.00m, true, 200 },
                    { 4, "Antibiotics", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Broad-spectrum antibiotic for bacterial infections including throat, ear, and urinary infections.", "1 capsule every 8 hours", "/images/medicines/amoxicillin.jpg", true, "Dr. Reddy's", "Amoxicillin 500mg", "Strip of 10 capsules", 120.00m, true, 150 },
                    { 5, "Antibiotics", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Macrolide antibiotic for respiratory, skin, and ear infections.", "1 tablet daily for 3-5 days", "/images/medicines/azithromycin.jpg", true, "Zydus", "Azithromycin 250mg", "Strip of 6 tablets", 95.00m, true, 180 },
                    { 6, "Antibiotics", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fluoroquinolone antibiotic for urinary tract and respiratory infections.", "1 tablet every 12 hours", "/images/medicines/ciprofloxacin.jpg", true, "Lupin", "Ciprofloxacin 500mg", "Strip of 10 tablets", 85.00m, true, 120 },
                    { 7, "Vitamins & Supplements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "High-potency vitamin C supplement for immunity support and antioxidant protection.", "1 tablet daily", "/images/medicines/vitaminc.jpg", true, "HealthKart", "Vitamin C 1000mg", "Bottle of 60 tablets", 250.00m, false, 400 },
                    { 8, "Vitamins & Supplements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Weekly vitamin D3 supplement for bone health and calcium absorption.", "1 capsule weekly", "/images/medicines/vitamind.jpg", true, "Sun Pharma", "Vitamin D3 60000 IU", "Strip of 8 capsules", 180.00m, false, 300 },
                    { 9, "Vitamins & Supplements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Complete daily multivitamin with minerals for overall health and wellness.", "1 tablet daily after meals", "/images/medicines/multivitamin.jpg", true, "Centrum", "Multivitamin Complex", "Bottle of 90 tablets", 350.00m, false, 250 },
                    { 10, "Vitamins & Supplements", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Premium fish oil capsules rich in EPA and DHA for heart and brain health.", "1 capsule twice daily", "/images/medicines/omega3.jpg", true, "HealthVit", "Omega-3 Fish Oil", "Bottle of 60 softgels", 450.00m, false, 200 },
                    { 11, "Digestive Health", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Proton pump inhibitor for acid reflux, heartburn, and gastric ulcers.", "1 capsule before breakfast", "/images/medicines/omeprazole.jpg", true, "Cipla", "Omeprazole 20mg", "Strip of 15 capsules", 75.00m, false, 300 },
                    { 12, "Digestive Health", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Anti-nausea medication for relief of nausea, vomiting, and bloating.", "1 tablet before meals", "/images/medicines/domperidone.jpg", true, "Torrent Pharma", "Domperidone 10mg", "Strip of 10 tablets", 55.00m, false, 250 },
                    { 13, "Allergy & Cold", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Antihistamine for allergic rhinitis, hay fever, and skin allergies.", "1 tablet daily", "/images/medicines/cetirizine.jpg", true, "Dr. Reddy's", "Cetirizine 10mg", "Strip of 10 tablets", 25.00m, false, 600 },
                    { 14, "Allergy & Cold", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Leukotriene receptor antagonist for asthma and allergic rhinitis.", "1 tablet daily at bedtime", "/images/medicines/montelukast.jpg", true, "Sun Pharma", "Montelukast 10mg", "Strip of 10 tablets", 140.00m, true, 8 },
                    { 15, "Allergy & Cold", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Effective cough suppressant for dry, tickly cough relief.", "10ml every 4-6 hours", "/images/medicines/coughsyrup.jpg", true, "Dabur", "Cough Syrup (Dextromethorphan)", "Bottle of 100ml", 85.00m, false, 180 },
                    { 16, "Skin Care", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Antifungal cream for athlete's foot, ringworm, and fungal skin infections.", "Apply twice daily", "/images/medicines/clotrimazole.jpg", true, "Glenmark", "Clotrimazole Cream 1%", "Tube of 20g", 95.00m, false, 150 },
                    { 17, "Skin Care", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Topical corticosteroid for eczema, dermatitis, and inflammatory skin conditions.", "Apply thin layer twice daily", "/images/medicines/betamethasone.jpg", true, "GSK", "Betamethasone Cream 0.1%", "Tube of 15g", 110.00m, true, 5 },
                    { 18, "Diabetes", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "First-line medication for type 2 diabetes mellitus management.", "1 tablet twice daily with meals", "/images/medicines/metformin.jpg", true, "USV", "Metformin 500mg", "Strip of 10 tablets", 45.00m, true, 400 },
                    { 19, "Heart & BP", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Calcium channel blocker for high blood pressure and chest pain (angina).", "1 tablet daily", "/images/medicines/amlodipine.jpg", true, "Pfizer", "Amlodipine 5mg", "Strip of 10 tablets", 55.00m, true, 3 },
                    { 20, "Heart & BP", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Low-dose aspirin for cardiovascular protection and blood thinning.", "1 tablet daily", "/images/medicines/aspirin.jpg", true, "Bayer", "Aspirin 75mg", "Strip of 14 tablets", 30.00m, false, 500 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "CreatedAt", "Email", "Name", "PasswordHash", "Phone", "Role" },
                values: new object[,]
                {
                    { 1, "123 Admin Street, Healthcare City", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@pharmacy.com", "Admin User", "$2a$11$QDnpIHxhg6/q24eKd.FaRuo5sox2v.mCULOLLdSWZ3novBPu2R.hC", "9876543210", "Admin" },
                    { 2, "456 Pharma Lane, Healthcare City", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "pharmacist@pharmacy.com", "Pharmacist User", "$2a$11$tLjCIUwx03BhC7lTbJMy7eB1RL5/pLW20vCPS4YZI6ce2WXco12MC", "9876543211", "Pharmacist" },
                    { 3, "789 Customer Road, Wellness Town", new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "john@example.com", "John Customer", "$2a$11$4iYRnrCg6ZlWNWqcsM8kB.i5mZALFh6gNZldeedzOkbY9ngH5wk/G", "9876543212", "Customer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MedicineId",
                table: "OrderItems",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PrescriptionId",
                table: "Orders",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_UserId",
                table: "Prescriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Medicines");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Prescriptions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
