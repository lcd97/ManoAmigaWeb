namespace AlquilerDeLibros.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SqlMigrations : DbMigration
    {
        public override void Up()
        {
            AlterColumn("prest.Clientes", "CodigoDeCliente", c => c.String(nullable: false, maxLength: 16));
        }
        
        public override void Down()
        {
            AlterColumn("prest.Clientes", "CodigoDeCliente", c => c.String(nullable: false, maxLength: 3));
        }
    }
}
