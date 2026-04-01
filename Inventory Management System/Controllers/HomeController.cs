using System.Data;
using System.Diagnostics;
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Data.SqlClient;

namespace Inventory_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly string connectionString;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            connectionString = config.GetConnectionString("DBCS");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult StockHistory()
        {
            return View();
        }

        public JsonResult GetCategories()
        {
            List<object> list = new List<object>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetCategories", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Name = dr["Name"].ToString()
                        });
                    }
                }
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public JsonResult GetSuppliers()
        {
            List<object> list = new List<object>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetSuppliers", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        list.Add(new
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Name = dr["Name"].ToString()
                        });
                    }
                }
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public JsonResult GetAll()
        {
            List<InventoryModel> list = new List<InventoryModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetProducts", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new InventoryModel
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Name = dr["Name"].ToString(),
                            Price = Convert.ToDecimal(dr["Price"]),
                            Quantity = Convert.ToInt32(dr["Quantity"]),
                            CategoryName = dr["CategoryName"].ToString(),
                            SupplierName = dr["SupplierName"].ToString()
                        });
                    }
                }

                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Create(InventoryModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertProduct", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                    cmd.Parameters.AddWithValue("@SupplierId", model.SupplierId);
                    cmd.Parameters.AddWithValue("@Price", model.Price);
                    cmd.Parameters.AddWithValue("@Quantity", model.Quantity);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Update(InventoryModel model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_UpdateProduct", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                    cmd.Parameters.AddWithValue("@SupplierId", model.SupplierId);
                    cmd.Parameters.AddWithValue("@Price", model.Price);
                    cmd.Parameters.AddWithValue("@Quantity", model.Quantity);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetById(int id)
        {
            InventoryModel p = new InventoryModel();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetproductById", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        p.Id = Convert.ToInt32(dr["id"]);
                        p.Name = dr["Name"].ToString();
                        p.Price = Convert.ToDecimal(dr["Price"]);
                        p.Quantity = Convert.ToInt32(dr["Quantity"]);
                        p.CategoryId = Convert.ToInt32(dr["CategoryId"]);
                        p.SupplierId = Convert.ToInt32(dr["SupplierId"]);

                    }
                }
                return Json(p);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(int Id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteProduct", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public JsonResult GetDashboard()
        {
            DashboardModel model = new DashboardModel();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                model.TotalProducts = Convert.ToInt32(
                    new SqlCommand("SELECT COUNT(*) FROM Product", con).ExecuteScalar());

                model.TotalCategories = Convert.ToInt32(
                    new SqlCommand("SELECT COUNT(*) FROM Category", con).ExecuteScalar());

                model.TotalSuppliers = Convert.ToInt32(
                    new SqlCommand("SELECT COUNT(*) FROM Supplier", con).ExecuteScalar());

                model.LowStock = Convert.ToInt32(
                    new SqlCommand("SELECT COUNT(*) FROM Product WHERE Quantity < 10", con).ExecuteScalar());
            }

            return Json(model);
        }

        [HttpPost]
        public JsonResult StockIn(int productId, int qty)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_StockIn", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.Parameters.AddWithValue("@Qty", qty);

                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult StockOut(int productId, int qty)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_StockOut", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.Parameters.AddWithValue("@Qty", qty);

                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true });

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
            
        }

        public JsonResult GetChartData()
        {
            List<object> data = new List<object>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
            SELECT c.Name, COUNT(p.Id) as Total
            FROM Category c
            LEFT JOIN Product p ON c.Id = p.CategoryId
            GROUP BY c.Name", con);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    data.Add(new
                    {
                        category = dr["Name"].ToString(),
                        total = Convert.ToInt32(dr["Total"])
                    });
                }
            }

            return Json(data);
        }

        public JsonResult GetStockHistory()
        {
            List<object> list = new List<object>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
            SELECT s.Id, p.Name, s.Type, s.Quantity, s.Date
            FROM StockTransaction s
            JOIN Product p ON s.ProductId = p.Id
            ORDER BY s.Date DESC", con);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new
                    {
                        id = dr["Id"],
                        product = dr["Name"].ToString(),
                        type = dr["Type"].ToString(),
                        qty = Convert.ToInt32(dr["Quantity"]),
                        date = Convert.ToDateTime(dr["Date"]).ToString("dd-MM-yyyy HH:mm")
                    });
                }
            }

            return Json(list);
        }
    }
}
