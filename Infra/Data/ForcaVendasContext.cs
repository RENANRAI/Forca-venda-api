using ForcaVendas.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ForcaVendas.Api.Infra.Data;

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
    public DbSet<ClienteParametrosFilial> ClienteParametrosFiliais { get; set; } = default!;

    public DbSet<Filial> Filiais => Set<Filial>();

    public DbSet<Representante> Representantes => Set<Representante>();
    public DbSet<RepresentanteParametrosEmpresa> RepresentanteParametrosEmpresas => Set<RepresentanteParametrosEmpresa>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // EMPRESA FILIAL INTEGRADA
        var empFilInt = modelBuilder.Entity<EmpresaFilialIntegrada>();
        empFilInt.ToTable("EmpresasFiliaisIntegradas");
        empFilInt.HasKey(e => e.Id);

        empFilInt.Property(e => e.CodEmp)
            .IsRequired();


        empFilInt.Property(e => e.CodFil)
            .IsRequired();


        empFilInt.Property(e => e.NomEmp)
                .HasMaxLength(200);

        empFilInt.Property(e => e.NomFil)
            .HasMaxLength(200);

        empFilInt.Property(e => e.SitReg)
            .IsRequired();

        empFilInt.Property(e => e.DatCri)
            .IsRequired();

        empFilInt.Property(e => e.DatAtu);

        // garante 1 registro por CodEmp + CodFil
        empFilInt
            .HasIndex(e => new { e.CodEmp, e.CodFil })
            .IsUnique();



        // ============================
        // EMPRESA
        // ============================
        var empresa = modelBuilder.Entity<Empresa>();
        empresa.ToTable("Empresas");
        empresa.HasKey(e => e.Id);
        empresa.Property(e => e.CodEmp).IsRequired();
        empresa.HasIndex(e => e.CodEmp).IsUnique();
        empresa.Property(e => e.NomEmp).IsRequired().HasMaxLength(200);
        empresa.Property(e => e.NumCgc).HasMaxLength(20);



        // ============================
        // FILIAL
        // ============================
        var filial = modelBuilder.Entity<Filial>();
        filial.ToTable("Filiais");
        filial.HasKey(f => f.Id);

        filial.Property(f => f.CodEmp).IsRequired();
        filial.Property(f => f.CodFil).IsRequired();

        filial.Property(f => f.NomFil).IsRequired().HasMaxLength(200);
        filial.Property(f => f.NumCgc).HasMaxLength(20);
        filial.Property(f => f.EndFil).HasMaxLength(200);
        filial.Property(f => f.NenFil).HasMaxLength(20);
        filial.Property(f => f.BaiFil).HasMaxLength(100);
        filial.Property(f => f.CidFil).HasMaxLength(100);
        filial.Property(f => f.SigUfs).HasMaxLength(2);
        filial.Property(f => f.CepFil).HasMaxLength(20);

        // garante unicidade por empresa + filial
        filial
            .HasIndex(f => new { f.CodEmp, f.CodFil })
            .IsUnique();

        // relação opcional com Empresa via CodEmp (não por Id)
        filial
            .HasOne(f => f.Empresa)
            .WithMany()
            .HasPrincipalKey(e => e.CodEmp)  // Empresa.CodEmp é único
            .HasForeignKey(f => f.CodEmp);   // Filial.CodEmp



        // ============================
        // CLIENTE
        // ============================
        var cliente = modelBuilder.Entity<Cliente>();
        cliente.ToTable("Clientes");
        cliente.HasKey(c => c.Id);
        cliente.Property(c => c.NomCli).IsRequired().HasMaxLength(200);
        cliente.Property(c => c.CgcCpf).IsRequired().HasMaxLength(20);
        cliente.Property(c => c.CidCli).HasMaxLength(100);
        cliente.Property(c => c.SigUfs).HasMaxLength(2);
        cliente.Property(c => c.CodCli);
        cliente.HasIndex(c => c.CodCli);

        //CIENTE PARAMETROS FILIAL
        var cliPar = modelBuilder.Entity<ClienteParametrosFilial>();
        cliPar.ToTable("ClienteParametrosFilial");
        cliPar.HasKey(x => x.Id);

        // Índice único por cliente + empresa + filial
        cliPar.HasIndex(x => new { x.CodCli, x.CodEmp, x.CodFil })
              .IsUnique();

        // Exemplos de tipos/precisão – ajuste conforme seu CREATE:
        cliPar.Property(x => x.VlrLim).HasColumnType("decimal(18,4)");
        cliPar.Property(x => x.SalDup).HasColumnType("decimal(18,4)");
        cliPar.Property(x => x.SalOut).HasColumnType("decimal(18,4)");
        cliPar.Property(x => x.SalCre).HasColumnType("decimal(18,4)");
        cliPar.Property(x => x.PerDsc).HasColumnType("decimal(18,4)");
        cliPar.Property(x => x.PerFre).HasColumnType("decimal(18,4)");
        cliPar.Property(x => x.PerIss).HasColumnType("decimal(18,4)");

        cliPar.Property(x => x.LimApr).HasMaxLength(1);
        cliPar.Property(x => x.CifFob).HasMaxLength(3);
        cliPar.Property(x => x.CodTab).HasMaxLength(10);
        cliPar.Property(x => x.CodCpg).HasMaxLength(10);

        // SitReg alinhado com NOT NULL + default 1 no banco
        cliPar.Property(e => e.SitReg)
              .HasDefaultValue(true)
              .IsRequired();

        // Campos de controle
        cliPar.Property(x => x.SitReg).HasDefaultValue(true);
        cliPar.Property(x => x.DatCri).HasColumnType("datetime2");
        cliPar.Property(x => x.DatAtu).HasColumnType("datetime2");



        // ============================
        // REPRESENTANTE
        // ============================
        var rep = modelBuilder.Entity<Representante>();

        rep.ToTable("Representante");
        rep.HasKey(r => r.CodRep);

        rep.Property(r => r.NomRep)
            .IsRequired()
            .HasMaxLength(150);

        rep.Property(r => r.ApeRep)
            .HasMaxLength(100);

        rep.Property(r => r.BaiRep).HasMaxLength(100);
        rep.Property(r => r.CidRep).HasMaxLength(100);
        rep.Property(r => r.SigUfs).HasMaxLength(2);

        rep.Property(r => r.EndRep).HasMaxLength(200);
        rep.Property(r => r.CepRep).HasMaxLength(15);
        rep.Property(r => r.CgcCpf).HasMaxLength(20);
        rep.Property(r => r.InsEst).HasMaxLength(30);
        rep.Property(r => r.InsMun).HasMaxLength(30);

        rep.Property(r => r.FonRep).HasMaxLength(30);
        rep.Property(r => r.FonRe2).HasMaxLength(30);
        rep.Property(r => r.FonRe3).HasMaxLength(30);
        rep.Property(r => r.FaxRep).HasMaxLength(30);

        rep.Property(r => r.IntNet).HasMaxLength(150);
        rep.Property(r => r.TipRep).HasMaxLength(1);
        rep.Property(r => r.SitRep).HasMaxLength(1);
        rep.Property(r => r.SitWmw).HasMaxLength(1);

        rep.Property(r => r.CalIns).HasMaxLength(1);
        rep.Property(r => r.CalIrf).HasMaxLength(1);
        rep.Property(r => r.CalIss).HasMaxLength(1);
        rep.Property(r => r.GerTit).HasMaxLength(1);

        rep.Property(r => r.NumRge).HasMaxLength(50);
        rep.Property(r => r.OrgRge).HasMaxLength(50);
        rep.Property(r => r.EenRep).HasMaxLength(10);
        rep.Property(r => r.CplEnd).HasMaxLength(100);
        rep.Property(r => r.NenRep).HasMaxLength(50);
        rep.Property(r => r.SenRep).HasMaxLength(50);
        rep.Property(r => r.ZipCod).HasMaxLength(20);

        rep.Property(r => r.SitReg).HasDefaultValue(true);
        rep.Property(r => r.DatCri).HasColumnType("datetime2");
        rep.Property(r => r.DatAtuApp).HasColumnType("datetime2");

        // ============================
        // REPRESENTANTE PARAMETROS EMPRESA
        // ============================
        var repParam = modelBuilder.Entity<RepresentanteParametrosEmpresa>();

        repParam.ToTable("RepresentanteParametrosEmpresa");
        repParam.HasKey(p => p.Id);

        // índice único por empresa + representante
        repParam
            .HasIndex(p => new { p.CodEmp, p.CodRep })
            .IsUnique();

        // relacionamento com Representante
        repParam
            .HasOne<Representante>()
            .WithMany() // se você tiver ICollection<RepresentanteParametrosEmpresa> no Representante, pode substituir por .WithMany(r => r.ParametrosEmpresas)
            .HasForeignKey(p => p.CodRep);

        // Alguns exemplos de tipos / tamanho (batendo com o CREATE que você já tem):
        repParam.Property(p => p.PerCom).HasColumnType("decimal(18,4)");
        repParam.Property(p => p.PerCos).HasColumnType("decimal(18,4)");
        repParam.Property(p => p.ComFat).HasColumnType("decimal(18,4)");
        repParam.Property(p => p.ComRec).HasColumnType("decimal(18,4)");
        repParam.Property(p => p.PerIrf).HasColumnType("decimal(18,4)");
        repParam.Property(p => p.PerIss).HasColumnType("decimal(18,4)");
        repParam.Property(p => p.PerIns).HasColumnType("decimal(18,4)");
        repParam.Property(p => p.VenVmp).HasColumnType("decimal(18,4)");
        repParam.Property(p => p.RecVmt).HasColumnType("decimal(18,4)");

        repParam.Property(p => p.CodRve).HasMaxLength(10);
        repParam.Property(p => p.CodBan).HasMaxLength(20);
        repParam.Property(p => p.CodAge).HasMaxLength(20);
        repParam.Property(p => p.CcbRep).HasMaxLength(30);

        repParam.Property(p => p.AvaObs).HasMaxLength(255);
        repParam.Property(p => p.RepAud).HasMaxLength(30);

        repParam.Property(p => p.SitReg).HasDefaultValue(true);
        repParam.Property(p => p.DatCri).HasColumnType("datetime2");
        repParam.Property(p => p.DatAtu).HasColumnType("datetime2");

        // ============================
        // PRODUTO
        // ============================
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
