using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrawlNodes_CrawlNodes_ParentNodeId",
                table: "CrawlNodes");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentNodeId",
                table: "CrawlNodes",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_CrawlNodes_CrawlNodes_ParentNodeId",
                table: "CrawlNodes",
                column: "ParentNodeId",
                principalTable: "CrawlNodes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrawlNodes_CrawlNodes_ParentNodeId",
                table: "CrawlNodes");

            migrationBuilder.AlterColumn<Guid>(
                name: "ParentNodeId",
                table: "CrawlNodes",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_CrawlNodes_CrawlNodes_ParentNodeId",
                table: "CrawlNodes",
                column: "ParentNodeId",
                principalTable: "CrawlNodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
