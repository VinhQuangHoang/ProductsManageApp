//using Dapper;
//using Npgsql;
//using ProductsManageApp.Models;


//public class ProductRepository : IProductRepository
//{
//    private readonly string _connectionString;

//    public ProductRepository(IConfiguration configuration)
//    {
//        _connectionString = configuration.GetConnectionString("DefaultConnection");
//    }

//    public async Task<IEnumerable<Product>> GetProductsAsync(string searchString = null, int? page = null, int pageSize = 10)
//    {
//        using (var connection = new NpgsqlConnection(_connectionString))
//        {
//            connection.Open();

//            var sql = @"
//                SELECT p.*, u.*
//                FROM ""Products"" p
//                LEFT JOIN ""AspNetUsers"" u ON p.""UserId"" = u.""Id""
//                WHERE (@SearchString IS NULL OR p.""Name"" ILIKE '%' || @SearchString || '%')
//                ORDER BY p.""CreatedDate"" DESC
//                LIMIT @Limit OFFSET @Offset;";

//            int offset = (page ?? 1 - 1) * pageSize;
//            var parameters = new { SearchString = searchString, Limit = pageSize, Offset = offset };

//            var products = await connection.QueryAsync<Product, User, Product>(sql,
//                (product, user) =>
//                {
//                    product.Users = user;
//                    return product;
//                },
//                parameters,
//                splitOn: "UserId");

//            return products;
//        }
//    }

//    public async Task<int> GetTotalProductsAsync(string searchString = null)
//    {
//        using (var connection = new NpgsqlConnection(_connectionString))
//        {
//            connection.Open();

//            var sql = @"
//                SELECT COUNT(*)
//                FROM ""Products""
//                WHERE @SearchString IS NULL OR ""Name"" ILIKE '%' || @SearchString || '%';";

//            var parameters = new { SearchString = searchString };

//            var count = await connection.ExecuteScalarAsync<int>(sql, parameters);

//            return count;
//        }
//    }

//    //public async Task<Product> GetProductByIdAsync(int id)
//    //{
//    //    using (var connection = new NpgsqlConnection(_connectionString))
//    //    {
//    //        connection.Open();

//    //        var sql = @"
//    //            SELECT p.*, u.*
//    //            FROM ""Products"" p
//    //            LEFT JOIN ""AspNetUsers"" u ON p.""UserId"" = u.""Id""
//    //            WHERE p.""Id"" = @Id;";

//    //        var parameters = new { Id = id };

//    //        var product = await connection.QueryAsync<Product, User, Product>(sql,
//    //            (prod, user) =>
//    //            {
//    //                prod.Users = user;
//    //                return prod;
//    //            },
//    //            parameters,
//    //            splitOn: "UserId");

//    //        return product;
//    //    }
//    //}
//    public async Task<Product> GetProductByIdAsync(int id)
//    {
//        using (var connection = new NpgsqlConnection(_connectionString))
//        {
//            connection.Open();

//            var sql = @"
//            SELECT p.*, u.*
//            FROM ""Products"" p
//            LEFT JOIN ""AspNetUsers"" u ON p.""UserId"" = u.""Id""
//            WHERE p.""Id"" = @Id;";

//            var parameters = new { Id = id };

//            // Execute and return the first product (or null if none found)
//            var products = await connection.QueryAsync<Product, User, Product>(sql,
//                (prod, user) =>
//                {
//                    prod.Users = user;
//                    return prod;
//                },
//                parameters,
//                splitOn: "UserId");

//            return products.FirstOrDefault(); // Return the first product (or null)
//        }
//    }


//    public async Task CreateProductAsync(Product product)
//    {
//        using (var connection = new NpgsqlConnection(_connectionString))
//        {
//            connection.Open();

//            var sql = @"
//                INSERT INTO ""Products"" (""Name"", ""Description"", ""Price"", ""CreatedDate"", ""UserId"")
//                VALUES (@Name, @Description, @Price, @CreatedDate, @UserId);";

//            await connection.ExecuteAsync(sql, product);
//        }
//    }

//    public async Task UpdateProductAsync(Product product)
//    {
//        using (var connection = new NpgsqlConnection(_connectionString))
//        {
//            connection.Open();

//            var sql = @"
//                UPDATE ""Products""
//                SET ""Name"" = @Name,
//                    ""Description"" = @Description,
//                    ""Price"" = @Price,
//                    ""UserId"" = @UserId
//                WHERE ""Id"" = @Id;";

//            await connection.ExecuteAsync(sql, product);
//        }
//    }

//    public async Task DeleteProductAsync(int id)
//    {
//        using (var connection = new NpgsqlConnection(_connectionString))
//        {
//            connection.Open();

//            var sql = @"DELETE FROM ""Products"" WHERE ""Id"" = @Id;";

//            await connection.ExecuteAsync(sql, new { Id = id });
//        }
//    }
//}
