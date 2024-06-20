using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedcrawlnodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Executions_WebsiteRecords_WebsiteRecordId",
                table: "Executions");

            migrationBuilder.AlterColumn<Guid>(
                name: "WebsiteRecordId",
                table: "Executions",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "CrawlNode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CrawlTime = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OwnerId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ParentNodeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrawlNode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrawlNode_CrawlNode_ParentNodeId",
                        column: x => x.ParentNodeId,
                        principalTable: "CrawlNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CrawlNode_WebsiteRecords_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "WebsiteRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CrawlNode_OwnerId",
                table: "CrawlNode",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CrawlNode_ParentNodeId",
                table: "CrawlNode",
                column: "ParentNodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Executions_WebsiteRecords_WebsiteRecordId",
                table: "Executions",
                column: "WebsiteRecordId",
                principalTable: "WebsiteRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Executions_WebsiteRecords_WebsiteRecordId",
                table: "Executions");

            migrationBuilder.DropTable(
                name: "CrawlNode");

            migrationBuilder.AlterColumn<Guid>(
                name: "WebsiteRecordId",
                table: "Executions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_Executions_WebsiteRecords_WebsiteRecordId",
                table: "Executions",
                column: "WebsiteRecordId",
                principalTable: "WebsiteRecords",
                principalColumn: "Id");
        }
    }
}
