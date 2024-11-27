using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LidgrenServer.Migrations
{
    public partial class UpdateUserRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_relationship_users_user_second_id",
                table: "user_relationship");

            migrationBuilder.DropIndex(
                name: "IX_users_username",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_user_relationship_users_user_second_id",
                table: "user_relationship",
                column: "user_second_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_relationship_users_user_second_id",
                table: "user_relationship");

            migrationBuilder.DropIndex(
                name: "IX_users_username",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username");

            migrationBuilder.AddForeignKey(
                name: "FK_user_relationship_users_user_second_id",
                table: "user_relationship",
                column: "user_second_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
