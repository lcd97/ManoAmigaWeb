namespace AlquilerDeLibros.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdquisicionMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("prest.Libros", "Adquisicion", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("prest.Libros", "Adquisicion");
        }
    }
}
