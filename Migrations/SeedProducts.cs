using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class SeedProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Name", "Price" },
                values: new object[,]
                {
                    { "Milk (1 Gallon)", 3.99m },
                    { "Eggs (Dozen)", 4.49m },
                    { "Bread (Loaf)", 2.99m },
                    { "Chicken Breast (1 lb)", 5.99m },
                    { "Ground Beef (1 lb)", 6.49m },
                    { "Rice (5 lb bag)", 8.99m },
                    { "Pasta (1 lb)", 1.99m },
                    { "Tomatoes (1 lb)", 2.49m },
                    { "Potatoes (5 lb bag)", 4.99m },
                    { "Bananas (1 lb)", 0.79m },
                    { "Apples (1 lb)", 1.99m },
                    { "Orange Juice (1/2 Gallon)", 3.49m },
                    { "Coffee (1 lb)", 9.99m },
                    { "Sugar (5 lb bag)", 4.49m },
                    { "Flour (5 lb bag)", 3.99m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Name",
                keyValues: new object[]
                {
                    "Milk (1 Gallon)",
                    "Eggs (Dozen)",
                    "Bread (Loaf)",
                    "Chicken Breast (1 lb)",
                    "Ground Beef (1 lb)",
                    "Rice (5 lb bag)",
                    "Pasta (1 lb)",
                    "Tomatoes (1 lb)",
                    "Potatoes (5 lb bag)",
                    "Bananas (1 lb)",
                    "Apples (1 lb)",
                    "Orange Juice (1/2 Gallon)",
                    "Coffee (1 lb)",
                    "Sugar (5 lb bag)",
                    "Flour (5 lb bag)"
                });
        }
    }
} 