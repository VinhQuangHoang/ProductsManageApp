using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql;
using ProductsManageApp.Models;
using X.PagedList;

namespace ProductsManageApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly string _connectionString;

        public ProductsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [Authorize]
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                var sqlQuery = @"
            SELECT * FROM ""Products""
            WHERE (@SearchString IS NULL OR ""Name"" ILIKE '%' || @SearchString || '%')
            ORDER BY ""Name""";

                var products = await dbConnection.QueryAsync<Product>(sqlQuery, new { SearchString = searchString });

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(products.ToPagedList(pageNumber, pageSize));
            }
        }



        [Authorize]
        public IActionResult Create()
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();
                var sqlQuery = @"SELECT ""Id"", ""Email"" FROM ""AspNetUsers""";

                var userIds = dbConnection.Query<User>(sqlQuery)
                                  .Select(u => new SelectListItem
                                  {
                                      Value = u.Id,
                                      Text = u.Email
                                  }).ToList();

                ViewData["UserIds"] = new SelectList(userIds, "Value", "Text");
            }

            return View();
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
                    {
                        dbConnection.Open();

                        var sqlQuery = @"
                    INSERT INTO ""Products"" (""Name"", ""Description"", ""Price"", ""UserId"", ""CreatedDate"")
                    VALUES (@Name, @Description, @Price, @UserId, @CreatedDate);";

                        await dbConnection.ExecuteAsync(sqlQuery, new
                        {
                            product.Name,
                            product.Description,
                            product.Price,
                            product.UserId,
                            CreatedDate = DateTime.UtcNow // Get realtime value
                        });
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");                    
                    return View(product);
                }
            }

            return View(product);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product;

            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();

                var sqlQuery = @"SELECT * FROM ""Products"" WHERE ""Id"" = @Id;";
                product = await dbConnection.QueryFirstOrDefaultAsync<Product>(sqlQuery, new { Id = id });

                if (product == null)
                {
                    return NotFound();
                }
            }

            return View(product);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product;

            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();

                var sqlQuery = @"
                SELECT * FROM ""Products"" WHERE ""Id"" = @Id;";

                product = await dbConnection.QueryFirstOrDefaultAsync<Product>(
                    sqlQuery,
                    new { Id = id }
                );
            }

            if (product == null)
            {
                return NotFound();
            }
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();
                var sqlQuery = @"SELECT ""Id"", ""Email"" FROM ""AspNetUsers""";

                var userIds = dbConnection.Query<User>(sqlQuery)
                                  .Select(u => new SelectListItem
                                  {
                                      Value = u.Id,
                                      Text = u.Email
                                  }).ToList();

                ViewData["UserIds"] = new SelectList(userIds, "Value", "Text");
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
                {
                    dbConnection.Open();

                    var sqlQuery = @"
                    UPDATE ""Products"" 
                    SET ""Name"" = @Name, 
                        ""Description"" = @Description, 
                        ""Price"" = @Price, 
                        ""CreatedDate"" = @CreatedDate,
                        ""UserId"" = @UserId
                    WHERE ""Id"" = @Id;";

                    await dbConnection.ExecuteAsync(sqlQuery, product);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product;

            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();

                var sqlQuery = @"
                SELECT * FROM ""Products"" WHERE ""Id"" = @Id;";

                product = await dbConnection.QueryFirstOrDefaultAsync<Product>(
                    sqlQuery,
                    new { Id = id }
                );
            }

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (IDbConnection dbConnection = new NpgsqlConnection(_connectionString))
            {
                dbConnection.Open();

                var sqlQuery = @"
                DELETE FROM ""Products"" WHERE ""Id"" = @Id;";

                await dbConnection.ExecuteAsync(sqlQuery, new { Id = id });
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ShowAll()
        {
            return RedirectToAction("Index", new { searchString = "" });
        }

    }
}
