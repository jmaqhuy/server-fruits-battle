using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace LidgrenServer.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "character",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    hp = table.Column<int>(type: "int", nullable: false),
                    damage = table.Column<int>(type: "int", nullable: false),
                    armor = table.Column<int>(type: "int", nullable: false),
                    stamina = table.Column<int>(type: "int", nullable: false),
                    luck = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    image_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    effect_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    value = table.Column<int>(type: "int", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    target = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    type = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    related_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rank",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    asset_name = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    max_star = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rank", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "seasons",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seasons", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    registered_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    display_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    coin = table.Column<int>(type: "int", nullable: false),
                    isVerifyEmail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    password = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shop",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop", x => x.id);
                    table.ForeignKey(
                        name: "FK_shop_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "login_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    login_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    logout_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    is_online_now = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_login_history_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_characters",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    character_id = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<int>(type: "int", nullable: false),
                    experience = table.Column<int>(type: "int", nullable: false),
                    is_selected = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    hp_point = table.Column<int>(type: "int", nullable: false),
                    damage_point = table.Column<int>(type: "int", nullable: false),
                    armor_point = table.Column<int>(type: "int", nullable: false),
                    luck_point = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_characters", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_characters_character_character_id",
                        column: x => x.character_id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_characters_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_inventories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_inventories", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_inventories_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_inventories_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_ranks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    rank_id = table.Column<int>(type: "int", nullable: false),
                    season_id = table.Column<int>(type: "int", nullable: false),
                    current_star = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_ranks", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_ranks_rank_rank_id",
                        column: x => x.rank_id,
                        principalTable: "rank",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_ranks_seasons_season_id",
                        column: x => x.season_id,
                        principalTable: "seasons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_ranks_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

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
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_User_LoginHistory_IsLoginNow",
                table: "login_history",
                columns: new[] { "user_id", "is_online_now" });

            migrationBuilder.CreateIndex(
                name: "IX_shop_product_id",
                table: "shop",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_characters_character_id",
                table: "user_characters",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserCharacter_UserId_CharacterId",
                table: "user_characters",
                columns: new[] { "user_id", "character_id" });

            migrationBuilder.CreateIndex(
                name: "IX_UserCharacter_UserId_IsSelected",
                table: "user_characters",
                columns: new[] { "user_id", "is_selected" });

            migrationBuilder.CreateIndex(
                name: "IX_user_inventories_product_id",
                table: "user_inventories",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_inventories_user_id",
                table: "user_inventories",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_ranks_rank_id",
                table: "user_ranks",
                column: "rank_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_ranks_season_id",
                table: "user_ranks",
                column: "season_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_ranks_user_id",
                table: "user_ranks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_relationship_user_second_id",
                table: "user_relationship",
                column: "user_second_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "login_history");

            migrationBuilder.DropTable(
                name: "shop");

            migrationBuilder.DropTable(
                name: "user_characters");

            migrationBuilder.DropTable(
                name: "user_inventories");

            migrationBuilder.DropTable(
                name: "user_ranks");

            migrationBuilder.DropTable(
                name: "user_relationship");

            migrationBuilder.DropTable(
                name: "character");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "rank");

            migrationBuilder.DropTable(
                name: "seasons");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
