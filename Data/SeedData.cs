using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Pg1.Data;
using Pg1.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pg1.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // Roles y usuarios
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string roleName = "Administrador";

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"No se pudo crear el rol '{roleName}': {string.Join(", ", roleResult.Errors)}");
                }
            }

            string adminEmail = "admin@ggstore.com";
            string adminPassword = "Admin1234!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createUserResult = await userManager.CreateAsync(newAdmin, adminPassword);
                if (!createUserResult.Succeeded)
                    throw new Exception($"Error al crear el usuario administrador: {string.Join(", ", createUserResult.Errors)}");

                var addToRoleResult = await userManager.AddToRoleAsync(newAdmin, roleName);
                if (!addToRoleResult.Succeeded)
                    throw new Exception($"No se pudo asignar el rol '{roleName}' al usuario administrador: {string.Join(", ", addToRoleResult.Errors)}");
            }

            // Insertar categorías iniciales
            await SeedCategorias(serviceProvider);
        }

        private static async Task SeedCategorias(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Categorias.Any())
            {
                var categorias = new Categoria[]
                {
                    new Categoria { Nombre = "Monitores", Descripcion = "Pantallas y monitores para gaming y oficina" },
                    new Categoria { Nombre = "Sillas Gamer", Descripcion = "Sillas ergonómicas para gamers" },
                    new Categoria { Nombre = "Teclados", Descripcion = "Teclados mecánicos y membrana para gaming" },
                    new Categoria { Nombre = "Mouses Gamer", Descripcion = "Ratones con alta precisión para gaming" },
                    new Categoria { Nombre = "GPU", Descripcion = "Tarjetas gráficas de última generación" },
                    new Categoria { Nombre = "Laptops", Descripcion = "Computadoras portátiles para gaming y trabajo" }
                };

                context.Categorias.AddRange(categorias);
                await context.SaveChangesAsync();
            }
        }
    }
}
