﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FitAplikacja.Infrastructure.Migrations
{
    public partial class MakeProductsAssignable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_ApplicationUserId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ApplicationUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Added",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "AssignedProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Added = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignedProducts_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssignedProductProduct",
                columns: table => new
                {
                    AssignedProductsId = table.Column<int>(type: "int", nullable: false),
                    ProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedProductProduct", x => new { x.AssignedProductsId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_AssignedProductProduct_AssignedProducts_AssignedProductsId",
                        column: x => x.AssignedProductsId,
                        principalTable: "AssignedProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignedProductProduct_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAssignedProducts",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignedProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssignedProducts", x => new { x.ApplicationUserId, x.AssignedProductId });
                    table.ForeignKey(
                        name: "FK_UserAssignedProducts_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAssignedProducts_AssignedProducts_AssignedProductId",
                        column: x => x.AssignedProductId,
                        principalTable: "AssignedProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignedProductProduct_ProductsId",
                table: "AssignedProductProduct",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedProducts_ApplicationUserId",
                table: "AssignedProducts",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignedProducts_AssignedProductId",
                table: "UserAssignedProducts",
                column: "AssignedProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignedProductProduct");

            migrationBuilder.DropTable(
                name: "UserAssignedProducts");

            migrationBuilder.DropTable(
                name: "AssignedProducts");

            migrationBuilder.AddColumn<DateTime>(
                name: "Added",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ApplicationUserId",
                table: "Products",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_ApplicationUserId",
                table: "Products",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
