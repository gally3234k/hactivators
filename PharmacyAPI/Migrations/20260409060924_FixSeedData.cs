using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$edR8FoCTXi0E7SB3Je32Ze9A6DXfQCPxZipT2hxFVuIcGNftigMMa");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$0E5s7kw4UaiNeZ0mm0SWnOP4gsc82GGq.6PA6gvWZ.UjzIuvcCARS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$Mj0XmcxisGbHWG8OShMhzuQPNzZ6ZWr1wKeHdw.nii2.CNFkBStnq");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$QDnpIHxhg6/q24eKd.FaRuo5sox2v.mCULOLLdSWZ3novBPu2R.hC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$tLjCIUwx03BhC7lTbJMy7eB1RL5/pLW20vCPS4YZI6ce2WXco12MC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$4iYRnrCg6ZlWNWqcsM8kB.i5mZALFh6gNZldeedzOkbY9ngH5wk/G");
        }
    }
}
