namespace AlquilerDeLibros.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PerfilClienteMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("prest.Clientes", "Foto", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("prest.Clientes", "Foto");
        }
    }
}
