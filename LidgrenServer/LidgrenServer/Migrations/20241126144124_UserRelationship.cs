using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LidgrenServer.Migrations
{
    public partial class UserRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_relationship",
                columns: table => new
                {
                    user_first_id = table.Column<int>(type: "int", nullable: false),
                    user_second_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_relationship", x => new { x.user_first_id, x.user_second_id });
                    table.ForeignKey(
                        name: "FK_user_relationship_users_user_first_id",
                        column: x => x.user_first_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_relationship_users_user_second_id",
                        column: x => x.user_second_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_user_relationship_user_second_id",
                table: "user_relationship",
                column: "user_second_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_relationship");
        }
    }
}
