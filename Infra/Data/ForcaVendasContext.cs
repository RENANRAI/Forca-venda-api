using ForcaVendas.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Forca_venda_api.Infra.Data;

public class ForcaVendasContext : DbContext
{
    public ForcaVendasContext(DbContextOptions<ForcaVendasContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoItem> PedidoItens => Set<PedidoItem>();
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<EmpresaFilialIntegrada> EmpresasFiliaisIntegradas => Set<EmpresaFilialIntegrada>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // EMPRESA FILIAL INTEGRADA
        var empFilInt = modelBuilder.Entity<EmpresaFilialIntegrada>();
        empFilInt.ToTable("EmpresasFiliaisIntegradas");
        empFilInt.HasKey(e => e.Id);

        empFilInt.Property(e => e.CodEmp)
            .IsRequired()
            .HasMaxLength(10);

        empFilInt.Property(e => e.CodFil)
            .IsRequired()
            .HasMaxLength(10);

       /* empFilInt.Property(e => e.NomeEmpresa)
            .HasMaxLength(200);

        empFilInt.Property(e => e.NomeFilial)
            .HasMaxLength(200);*/

        empFilInt.Property(e => e.SitReg)
            .IsRequired();

        empFilInt.Property(e => e.DatCri)
            .IsRequired();

        empFilInt.Property(e => e.DatAtu);

        // garante 1 registro por CodEmp + CodFil
        empFilInt
            .HasIndex(e => new { e.CodEmp, e.CodFil })
            .IsUnique();


        // A PARTIR DAQUI seguem os mapeamentos que você já tinha:
        // EMPRESA
        var empresa = modelBuilder.Entity<Empresa>();
        empresa.ToTable("Empresas");
        empresa.HasKey(e => e.Id);
        empresa.Property(e => e.CodigoErp).IsRequired().HasMaxLength(10);
        empresa.Property(e => e.Nome).IsRequired().HasMaxLength(200);
        empresa.Property(e => e.Cnpj).HasMaxLength(20);
        empresa.HasIndex(e => e.CodigoErp).IsUnique();

        // FILIAL
        var filial = modelBuilder.Entity<Filial>();
        filial.ToTable("Filiais");
        filial.HasKey(f => f.Id);
        filial.Property(f => f.CodigoErp).IsRequired().HasMaxLength(10);
        filial.Property(f => f.Nome).IsRequired().HasMaxLength(200);
        filial.Property(f => f.Cnpj).HasMaxLength(20);
        filial.Property(f => f.Cidade).HasMaxLength(100);
        filial.Property(f => f.Uf).HasMaxLength(2);

        filial
            .HasOne(f => f.Empresa)
            .WithMany()              // se você não tiver coleção na Empresa
            .HasForeignKey(f => f.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);

        // CLIENTE
        var cliente = modelBuilder.Entity<Cliente>();
        cliente.ToTable("Clientes");
        cliente.HasKey(c => c.Id);
        cliente.Property(c => c.Nome).IsRequired().HasMaxLength(200);
        cliente.Property(c => c.Documento).IsRequired().HasMaxLength(20);
        cliente.Property(c => c.Cidade).HasMaxLength(100);
        cliente.Property(c => c.Uf).HasMaxLength(2);
        cliente.Property(c => c.CodigoErp).HasMaxLength(50);
        cliente.HasIndex(c => c.CodigoErp);

        // PRODUTO
        var produto = modelBuilder.Entity<Produto>();
        produto.ToTable("Produtos");
        produto.HasKey(p => p.Id);
        produto.Property(p => p.CodigoExterno).IsRequired().HasMaxLength(50);
        produto.Property(p => p.Nome).IsRequired().HasMaxLength(200);
        produto.Property(p => p.Unidade).HasMaxLength(10);
        produto.Property(p => p.PrecoBase).HasColumnType("decimal(18,4)");
        produto.HasIndex(p => p.CodigoExterno).IsUnique();

        // PEDIDO
        var pedido = modelBuilder.Entity<Pedido>();
        pedido.ToTable("Pedidos");
        pedido.HasKey(p => p.Id);
        pedido.Property(p => p.ValorTotal).HasColumnType("decimal(18,4)");
        pedido
            .HasOne(p => p.Cliente)
            .WithMany(c => c.Pedidos)
            .HasForeignKey(p => p.ClienteId);

        // PEDIDO ITEM
        var item = modelBuilder.Entity<PedidoItem>();
        item.ToTable("PedidoItens");
        item.HasKey(i => i.Id);
        item.Property(i => i.Quantidade).HasColumnType("decimal(18,4)");
        item.Property(i => i.PrecoUnitario).HasColumnType("decimal(18,4)");
        item.Property(i => i.PercDesconto).HasColumnType("decimal(5,2)");
        item.Property(i => i.ValorTotal).HasColumnType("decimal(18,4)");

        item
            .HasOne(i => i.Pedido)
            .WithMany(p => p.Itens)
            .HasForeignKey(i => i.PedidoId);

        item
            .HasOne(i => i.Produto)
            .WithMany(p => p.Itens)
            .HasForeignKey(i => i.ProdutoId);
    }

}
