namespace AlquilerDeLibros.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigrations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "prest.AlquileresDeLibro",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoAlquiler = c.String(nullable: false, maxLength: 3),
                        FechaAlquiler = c.DateTime(nullable: false),
                        FechaDevo = c.DateTime(nullable: false),
                        FechaRealDevolucion = c.DateTime(nullable: false),
                        ClienteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("prest.Clientes", t => t.ClienteId)
                .Index(t => t.ClienteId);
            
            CreateTable(
                "prest.Clientes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoDeCliente = c.String(nullable: false, maxLength: 3),
                        NombresDelCliente = c.String(nullable: false, maxLength: 50),
                        ApellidosDelCliente = c.String(nullable: false, maxLength: 50),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "prest.DetallesAlquiler",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AlquilerId = c.Int(nullable: false),
                        CopiaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("prest.AlquileresDeLibro", t => t.AlquilerId)
                .ForeignKey("prest.CopiasDeLibro", t => t.CopiaId)
                .Index(t => t.AlquilerId)
                .Index(t => t.CopiaId);
            
            CreateTable(
                "prest.CopiasDeLibro",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumeroCopia = c.Int(nullable: false),
                        LibroId = c.Int(nullable: false),
                        TituloDeLibro = c.String(maxLength: 50),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("prest.Libros", t => t.LibroId)
                .Index(t => t.LibroId);
            
            CreateTable(
                "prest.Libros",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoDeLibro = c.String(nullable: false, maxLength: 3),
                        TituloDeLibro = c.String(nullable: false, maxLength: 50),
                        ISBN = c.String(nullable: false, maxLength: 13),
                        Autor = c.String(nullable: false, maxLength: 100),
                        Portada = c.Binary(),
                        MateriaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("prest.Materias", t => t.MateriaId)
                .Index(t => t.MateriaId);
            
            CreateTable(
                "prest.Materias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoDeMateria = c.String(nullable: false, maxLength: 3),
                        DescripcionDeMateria = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "prest.ValoracionesLibro",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Puntaje = c.Int(nullable: false),
                        Comentario = c.String(nullable: false, maxLength: 100),
                        Sugerencia = c.String(maxLength: 60),
                        ClienteId = c.Int(nullable: false),
                        LibroId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("prest.Clientes", t => t.ClienteId)
                .ForeignKey("prest.Libros", t => t.LibroId)
                .Index(t => t.ClienteId)
                .Index(t => t.LibroId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("prest.ValoracionesLibro", "LibroId", "prest.Libros");
            DropForeignKey("prest.ValoracionesLibro", "ClienteId", "prest.Clientes");
            DropForeignKey("prest.Libros", "MateriaId", "prest.Materias");
            DropForeignKey("prest.CopiasDeLibro", "LibroId", "prest.Libros");
            DropForeignKey("prest.DetallesAlquiler", "CopiaId", "prest.CopiasDeLibro");
            DropForeignKey("prest.DetallesAlquiler", "AlquilerId", "prest.AlquileresDeLibro");
            DropForeignKey("prest.AlquileresDeLibro", "ClienteId", "prest.Clientes");
            DropIndex("prest.ValoracionesLibro", new[] { "LibroId" });
            DropIndex("prest.ValoracionesLibro", new[] { "ClienteId" });
            DropIndex("prest.Libros", new[] { "MateriaId" });
            DropIndex("prest.CopiasDeLibro", new[] { "LibroId" });
            DropIndex("prest.DetallesAlquiler", new[] { "CopiaId" });
            DropIndex("prest.DetallesAlquiler", new[] { "AlquilerId" });
            DropIndex("prest.AlquileresDeLibro", new[] { "ClienteId" });
            DropTable("prest.ValoracionesLibro");
            DropTable("prest.Materias");
            DropTable("prest.Libros");
            DropTable("prest.CopiasDeLibro");
            DropTable("prest.DetallesAlquiler");
            DropTable("prest.Clientes");
            DropTable("prest.AlquileresDeLibro");
        }
    }
}
