using Microsoft.EntityFrameworkCore;

namespace chuyenNganh.websiteBanGiay.Data;

public partial class ChuyenNganhContext : DbContext
{
    public ChuyenNganhContext()
    {
    }

    public ChuyenNganhContext(DbContextOptions<ChuyenNganhContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<MailConfirmation> MailConfirmations { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductSize> ProductSizes { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WishList> WishLists { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-6JRBMVT\\SQLEXPRESS;Initial Catalog=Chuyen_Nganh;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Carts__D6AB58B9BD071A00");

            entity.HasIndex(e => e.UserId, "IX_Carts_User_ID");

            entity.Property(e => e.CartId).HasColumnName("Cart_ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_Date");
            entity.Property(e => e.IsActive).HasColumnName("Is_Active");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Carts__User_ID__4222D4EF");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__7B6515210AF7CCA4");

            entity.HasIndex(e => e.CartId, "IX_CartItems_Cart_ID");

            entity.HasIndex(e => e.ProductId, "IX_CartItems_Product_ID");

            entity.Property(e => e.CartItemId).HasColumnName("CartItem_ID");
            entity.Property(e => e.CartId).HasColumnName("Cart_ID");
            entity.Property(e => e.PriceAtTime)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Price_AtTime");
            entity.Property(e => e.ProductId).HasColumnName("Product_ID");
            entity.Property(e => e.Size)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__CartItems__Cart___46E78A0C");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__CartItems__Produ__47DBAE45");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__6DB38D6E6C123FB1");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("Category_Id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("Category_Name");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.ImageUrl).HasColumnName("Image_Url");
        });

        modelBuilder.Entity<MailConfirmation>(entity =>
        {
            entity.HasKey(e => e.MailId).HasName("PK__MailConf__8C1804FE6B01F6D2");

            entity.HasIndex(e => e.OrderId, "IX_MailConfirmations_Order_ID");

            entity.Property(e => e.MailId).HasColumnName("Mail_ID");
            entity.Property(e => e.EmailSent)
                .HasDefaultValue(false)
                .HasColumnName("Email_Sent");
            entity.Property(e => e.OrderId).HasColumnName("Order_ID");
            entity.Property(e => e.SentDate)
                .HasColumnType("datetime")
                .HasColumnName("Sent_Date");

            entity.HasOne(d => d.Order).WithMany(p => p.MailConfirmations)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__MailConfi__Order__5CD6CB2B");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__F1E4639B157A77E9");

            entity.HasIndex(e => e.UserId, "IX_Orders_User_ID");

            entity.Property(e => e.OrderId).HasColumnName("Order_ID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Order_Date");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(50)
                .HasColumnName("Order_Status");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Phone_Number");
            entity.Property(e => e.ShippingAddress)
                .HasMaxLength(255)
                .HasColumnName("Shipping_Address");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Total_Amount");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__User_ID__4AB81AF0");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__2F3022E2DEB81285");

            entity.HasIndex(e => e.OrderId, "IX_OrderItems_Order_ID");

            entity.HasIndex(e => e.ProductId, "IX_OrderItems_Product_ID");

            entity.Property(e => e.OrderItemId).HasColumnName("OrderItem_ID");
            entity.Property(e => e.OrderId).HasColumnName("Order_ID");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasColumnName("Product_ID");
            entity.Property(e => e.Size)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderItem__Order__4E88ABD4");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__OrderItem__Produ__4F7CD00D");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__9834FB9A7FF045B6");

            entity.HasIndex(e => e.CategoryId, "IX_Products_Category_Id");

            entity.Property(e => e.ProductId).HasColumnName("Product_ID");
            entity.Property(e => e.CategoryId).HasColumnName("Category_Id");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(CONVERT([date],getdate()))")
                .HasColumnName("Created_Date");
            entity.Property(e => e.ImageUrl).HasColumnName("Image_Url");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName)
                .HasMaxLength(64)
                .HasColumnName("Product_Name");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Products_Category");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.HasKey(e => e.ProductSizeId).HasName("PK__ProductS__A38AFB05FB617754");

            entity.ToTable("ProductSize");

            entity.Property(e => e.ProductSizeId).HasColumnName("ProductSize_ID");
            entity.Property(e => e.PriceAtTime)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Price_AtTime");
            entity.Property(e => e.ProductId).HasColumnName("Product_ID");
            entity.Property(e => e.Size).HasMaxLength(15);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSizes)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProductSi__Produ__02FC7413");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__F85DA7EBEF2F83A8");

            entity.HasIndex(e => e.ProductId, "IX_Reviews_Product_ID");

            entity.HasIndex(e => e.UserId, "IX_Reviews_User_ID");

            entity.Property(e => e.ReviewId).HasColumnName("Review_ID");
            entity.Property(e => e.Comment).HasMaxLength(255);
            entity.Property(e => e.ProductId).HasColumnName("Product_ID");
            entity.Property(e => e.ReviewDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Review_Date");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Reviews__Product__52593CB8");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reviews__User_ID__534D60F1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__206D9190324CF44D");

            entity.Property(e => e.UserId).HasColumnName("User_ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("Created_Date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("Full_Name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("User_Name");
        });

        modelBuilder.Entity<WishList>(entity =>
        {
            entity.HasKey(e => e.WishListId).HasName("PK__WishList__57A3DD8FA6F7C96A");

            entity.ToTable("WishList");

            entity.HasIndex(e => e.ProductId, "IX_WishList_Product_ID");

            entity.HasIndex(e => e.UserId, "IX_WishList_User_ID");

            entity.Property(e => e.WishListId).HasColumnName("WishList_ID");
            entity.Property(e => e.AddedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Added_Date");
            entity.Property(e => e.ProductId).HasColumnName("Product_ID");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.Product).WithMany(p => p.WishLists)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__WishList__Produc__59063A47");

            entity.HasOne(d => d.User).WithMany(p => p.WishLists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__WishList__User_I__5812160E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
