using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Login> Login { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidosItens { get; set; }
        public DbSet<Item> Itens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Login>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.Password)
                    .IsRequired();

                entity.Property(u => u.IdUsuario)
                    .IsRequired();

                entity.HasOne(l => l.Usuario)
                    .WithOne(u => u.Login)
                    .HasForeignKey<Login>(l => l.IdUsuario)
                    .IsRequired();

                entity.HasIndex(l => l.IdUsuario)
                    .IsUnique();
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Nome)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Telefone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(u => u.Rua)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Cidade)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Numero)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(u => u.Cep)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(u => u.CPF)
                    .IsRequired()
                    .HasMaxLength(11);

                entity.HasIndex(u => u.CPF)
                    .IsUnique();
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.IdUsuario)
                    .IsRequired();

                entity.Property(u => u.Finalizado)
                    .IsRequired();

                entity.HasOne(p => p.Usuario)
                    .WithMany(u => u.Pedidos)
                    .HasForeignKey(p => p.IdUsuario)
                    .IsRequired();
            });

            modelBuilder.Entity<PedidoItem>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.IdPedido)
                    .IsRequired();

                entity.Property(u => u.IdItem)
                    .IsRequired();

                entity.Property(u => u.Quantidade)
                    .IsRequired();

                entity.Property(u => u.ValorUnitario)
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();

                entity.HasOne(pi => pi.Pedido)
                    .WithMany(p => p.Itens)
                    .HasForeignKey(pi => pi.IdPedido)
                    .IsRequired();

                entity.HasOne(pi => pi.Item)
                    .WithMany()
                    .HasForeignKey(pi => pi.IdItem)
                    .IsRequired();
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Nome)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Preco)
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();

                entity.Property(u => u.Descricao)
                    .IsRequired();

                entity.Property(u => u.Estoque)
                    .IsRequired();

                entity.Property(u => u.DataCriacao)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();
            });
        }
    }
}
