namespace AlquilerDeLibros.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using AlquilerDeLibros.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<AlquilerDeLibros.Models.Cartera>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "AlquilerDeLibros.Models.Cartera";
        }

        protected override void Seed(AlquilerDeLibros.Models.Cartera context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.


        }
    }
}
