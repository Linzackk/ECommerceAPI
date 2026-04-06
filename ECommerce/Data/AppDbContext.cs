using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        DbSet<Login> Login { get; set; }
        DbSet<Usuario> Usuarios { get; set; }
        DbSet<Pedido> Pedidos { get; set; }
        DbSet<PedidoItem> PedidosItens { get; set; }
        DbSet<Item> Itens { get; set; }

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
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.IdUsuario)
                    .IsRequired();

                entity.Property(u => u.Finalizado)
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
                    .IsRequired();
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Nome)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Preco)
                    .IsRequired();

                entity.Property(u => u.Descricao)
                    .IsRequired();

                entity.Property(u => u.Estoque)
                    .IsRequired();

                entity.Property(u => u.DataCriacao)
                    .IsRequired();
            });
        }
    }
}
