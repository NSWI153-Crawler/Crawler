using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class renametableCrawlNodetoCrawlNodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrawlNode_CrawlNode_ParentNodeId",
                table: "CrawlNode");

            migrationBuilder.DropForeignKey(
                name: "FK_CrawlNode_WebsiteRecords_OwnerId",
                table: "CrawlNode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CrawlNode",
                table: "CrawlNode");

            migrationBuilder.RenameTable(
                name: "CrawlNode",
                newName: "CrawlNodes");

            migrationBuilder.RenameIndex(
                name: "IX_CrawlNode_ParentNodeId",
                table: "CrawlNodes",
                newName: "IX_CrawlNodes_ParentNodeId");

            migrationBuilder.RenameIndex(
                name: "IX_CrawlNode_OwnerId",
                table: "CrawlNodes",
                newName: "IX_CrawlNodes_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CrawlNodes",
                table: "CrawlNodes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CrawlNodes_CrawlNodes_ParentNodeId",
                table: "CrawlNodes",
                column: "ParentNodeId",
                principalTable: "CrawlNodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CrawlNodes_WebsiteRecords_OwnerId",
                table: "CrawlNodes",
                column: "OwnerId",
                principalTable: "WebsiteRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrawlNodes_CrawlNodes_ParentNodeId",
                table: "CrawlNodes");

            migrationBuilder.DropForeignKey(
                name: "FK_CrawlNodes_WebsiteRecords_OwnerId",
                table: "CrawlNodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CrawlNodes",
                table: "CrawlNodes");

            migrationBuilder.RenameTable(
                name: "CrawlNodes",
                newName: "CrawlNode");

            migrationBuilder.RenameIndex(
                name: "IX_CrawlNodes_ParentNodeId",
                table: "CrawlNode",
                newName: "IX_CrawlNode_ParentNodeId");

            migrationBuilder.RenameIndex(
                name: "IX_CrawlNodes_OwnerId",
                table: "CrawlNode",
                newName: "IX_CrawlNode_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CrawlNode",
                table: "CrawlNode",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CrawlNode_CrawlNode_ParentNodeId",
                table: "CrawlNode",
                column: "ParentNodeId",
                principalTable: "CrawlNode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CrawlNode_WebsiteRecords_OwnerId",
                table: "CrawlNode",
                column: "OwnerId",
                principalTable: "WebsiteRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
