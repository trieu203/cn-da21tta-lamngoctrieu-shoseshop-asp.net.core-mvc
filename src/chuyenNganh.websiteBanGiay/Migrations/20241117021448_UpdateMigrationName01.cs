using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace chuyenNganh.websiteBanGiay.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMigrationName01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Category_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category_Name = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(CONVERT([date],getdate()))"),
                    Image_Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__6DB38D6E6C123FB1", x => x.Category_Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Full_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Role = table.Column<string>(type: "varchar(16)", unicode: false, maxLength: 16, nullable: true),
                    Created_Date = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__206D9190324CF44D", x => x.User_ID);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Product_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product_Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<int>(type: "int", nullable: false),
                    Category_Id = table.Column<int>(type: "int", nullable: true),
                    Created_Date = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(CONVERT([date],getdate()))"),
                    Image_Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Products__9834FB9A7FF045B6", x => x.Product_ID);
                    table.ForeignKey(
                        name: "FK_Products_Category",
                        column: x => x.Category_Id,
                        principalTable: "Category",
                        principalColumn: "Category_Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Cart_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_ID = table.Column<int>(type: "int", nullable: true),
                    Created_Date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Carts__D6AB58B9BD071A00", x => x.Cart_ID);
                    table.ForeignKey(
                        name: "FK__Carts__User_ID__4222D4EF",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Order_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_ID = table.Column<int>(type: "int", nullable: true),
                    Order_Date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Total_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Order_Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Shipping_Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Phone_Number = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__F1E4639B157A77E9", x => x.Order_ID);
                    table.ForeignKey(
                        name: "FK__Orders__User_ID__4AB81AF0",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID");
                });

            migrationBuilder.CreateTable(
                name: "ProductSize",
                columns: table => new
                {
                    ProductSize_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product_ID = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price_AtTime = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductS__A38AFB05FB617754", x => x.ProductSize_ID);
                    table.ForeignKey(
                        name: "FK__ProductSi__Produ__02FC7413",
                        column: x => x.Product_ID,
                        principalTable: "Products",
                        principalColumn: "Product_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Review_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product_ID = table.Column<int>(type: "int", nullable: true),
                    User_ID = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Review_Date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reviews__F85DA7EBEF2F83A8", x => x.Review_ID);
                    table.ForeignKey(
                        name: "FK__Reviews__Product__52593CB8",
                        column: x => x.Product_ID,
                        principalTable: "Products",
                        principalColumn: "Product_ID");
                    table.ForeignKey(
                        name: "FK__Reviews__User_ID__534D60F1",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID");
                });

            migrationBuilder.CreateTable(
                name: "WishList",
                columns: table => new
                {
                    WishList_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_ID = table.Column<int>(type: "int", nullable: true),
                    Product_ID = table.Column<int>(type: "int", nullable: true),
                    Added_Date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WishList__57A3DD8FA6F7C96A", x => x.WishList_ID);
                    table.ForeignKey(
                        name: "FK__WishList__Produc__59063A47",
                        column: x => x.Product_ID,
                        principalTable: "Products",
                        principalColumn: "Product_ID");
                    table.ForeignKey(
                        name: "FK__WishList__User_I__5812160E",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID");
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CartItem_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cart_ID = table.Column<int>(type: "int", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Product_ID = table.Column<int>(type: "int", nullable: true),
                    Size = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price_AtTime = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CartItem__7B6515210AF7CCA4", x => x.CartItem_ID);
                    table.ForeignKey(
                        name: "FK__CartItems__Cart___46E78A0C",
                        column: x => x.Cart_ID,
                        principalTable: "Carts",
                        principalColumn: "Cart_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__CartItems__Produ__47DBAE45",
                        column: x => x.Product_ID,
                        principalTable: "Products",
                        principalColumn: "Product_ID");
                });

            migrationBuilder.CreateTable(
                name: "MailConfirmations",
                columns: table => new
                {
                    Mail_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order_ID = table.Column<int>(type: "int", nullable: true),
                    Email_Sent = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    Sent_Date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MailConf__8C1804FE6B01F6D2", x => x.Mail_ID);
                    table.ForeignKey(
                        name: "FK__MailConfi__Order__5CD6CB2B",
                        column: x => x.Order_ID,
                        principalTable: "Orders",
                        principalColumn: "Order_ID");
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItem_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order_ID = table.Column<int>(type: "int", nullable: true),
                    Product_ID = table.Column<int>(type: "int", nullable: true),
                    Size = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderIte__2F3022E2DEB81285", x => x.OrderItem_ID);
                    table.ForeignKey(
                        name: "FK__OrderItem__Order__4E88ABD4",
                        column: x => x.Order_ID,
                        principalTable: "Orders",
                        principalColumn: "Order_ID");
                    table.ForeignKey(
                        name: "FK__OrderItem__Produ__4F7CD00D",
                        column: x => x.Product_ID,
                        principalTable: "Products",
                        principalColumn: "Product_ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_Cart_ID",
                table: "CartItems",
                column: "Cart_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_Product_ID",
                table: "CartItems",
                column: "Product_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_User_ID",
                table: "Carts",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MailConfirmations_Order_ID",
                table: "MailConfirmations",
                column: "Order_ID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_Order_ID",
                table: "OrderItems",
                column: "Order_ID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_Product_ID",
                table: "OrderItems",
                column: "Product_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_User_ID",
                table: "Orders",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category_Id",
                table: "Products",
                column: "Category_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSize_Product_ID",
                table: "ProductSize",
                column: "Product_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Product_ID",
                table: "Reviews",
                column: "Product_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_User_ID",
                table: "Reviews",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WishList_Product_ID",
                table: "WishList",
                column: "Product_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WishList_User_ID",
                table: "WishList",
                column: "User_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "MailConfirmations");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ProductSize");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "WishList");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
