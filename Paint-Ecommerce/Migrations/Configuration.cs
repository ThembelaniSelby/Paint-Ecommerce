namespace Paint_Ecommerce.Migrations
{
    using Paint_Ecommerce.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Paint_Ecommerce.Models.Db>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Paint_Ecommerce.Models.Db context)
        {
            using (Db db = new Db())
            {
                if (db.Roles.Any(x => x.Name.Equals("Admin")))
                {

                }
                else
                {

                    Role Role1 = new Role()

                    {

                        Name = "Admin",


                    };

                    Role Role2 = new Role()

                    {
                        Name = ("User"),

                    };



                    Role Role3 = new Role()

                    {
                        Name = ("Driver"),

                    };


                    db.Roles.Add(Role1);
                    db.Roles.Add(Role2);
                    db.Roles.Add(Role3);

                    db.SaveChanges();

                    UserRole userRoles = new UserRole()

                    {
                        RoleId = 1
                    };

                    db.UserRoles.Add(userRoles);
                    db.SaveChanges();
                    int id = userRoles.UserId;

                    User userDTO = new User()
                    {
                        Id = id,
                        FirstName = ("Admin"),
                        LastName = ("Admin"),
                        EmailAddress = "Admin@gmail.com",
                        Password = ("Admin@123"),
                        PhoneNumber = "0635073827",
                        Status ="VERIFIED"

                    };
                    db.Users.Add(userDTO);
                    db.SaveChanges();

                }

            }
        }
    }
}
