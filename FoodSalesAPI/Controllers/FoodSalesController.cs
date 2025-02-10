using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodSalesAPI.Data;
using FoodSalesAPI.Models;
using OfficeOpenXml; // Import EPPlus
using System.IO;


namespace FoodSalesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodSalesController : ControllerBase
    {
        private readonly FoodSalesDbContext _context;

        public FoodSalesController(FoodSalesDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodSale>>> GetFoodSales()
        {
            return await _context.FoodSales.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FoodSale>> GetFoodSale(int id)
        {
            var foodSale = await _context.FoodSales.FindAsync(id);
            if (foodSale == null) return NotFound();
            return foodSale;
        }

        [HttpPost]
        public async Task<ActionResult<FoodSale>> CreateFoodSale(FoodSale foodSale)
        {
            _context.FoodSales.Add(foodSale);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFoodSale), new { id = foodSale.Id }, foodSale);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFoodSale(int id, FoodSale foodSale)
        {
            if (id != foodSale.Id) return BadRequest();
            _context.Entry(foodSale).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodSale(int id)
        {
            var foodSale = await _context.FoodSales.FindAsync(id);
            if (foodSale == null) return NotFound();
            _context.FoodSales.Remove(foodSale);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ฟังก์ชันใหม่สำหรับ Import Excel
[HttpPost("import-excel")]
public async Task<IActionResult> ImportExcel([FromForm] IFormFile file)
{
    if (file == null || file.Length <= 0)
        return BadRequest("กรุณาอัปโหลดไฟล์ Excel");

    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        return BadRequest("ไฟล์ต้องเป็นนามสกุล .xlsx");

    var newFoodSales = new List<FoodSale>(); // ✅ ย้ายมาตรงนี้

    using (var stream = new MemoryStream())
    {
        await file.CopyToAsync(stream);

        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets[0];

            if (worksheet.Dimension == null)
                return BadRequest("ไฟล์ Excel ไม่มีข้อมูล");

            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var foodSale = new FoodSale
                    {
                        Region = worksheet.Cells[row, 1].Value?.ToString() ?? "N/A",
                        Country = worksheet.Cells[row, 2].Value?.ToString() ?? "N/A",
                        ItemType = worksheet.Cells[row, 3].Value?.ToString() ?? "N/A",
                        SalesChannel = worksheet.Cells[row, 4].Value?.ToString() ?? "N/A",
                        OrderPriority = worksheet.Cells[row, 5].Value?.ToString() ?? "N/A",
                        OrderDate = DateTime.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out DateTime orderDate) ? orderDate : DateTime.MinValue,
                        OrderID = int.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out int orderID) ? orderID : 0,
                        ShipDate = DateTime.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out DateTime shipDate) ? shipDate : DateTime.MinValue,
                        UnitsSold = int.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out int unitsSold) ? unitsSold : 0,
                        UnitPrice = decimal.TryParse(worksheet.Cells[row, 10].Value?.ToString(), out decimal unitPrice) ? unitPrice : 0,
                        TotalRevenue = decimal.TryParse(worksheet.Cells[row, 11].Value?.ToString(), out decimal totalRevenue) ? totalRevenue : 0,
                        FoodName = worksheet.Cells[row, 12].Value?.ToString() ?? "Unknown",
                        Category = worksheet.Cells[row, 13].Value?.ToString() ?? "Unknown",
                        Price = decimal.TryParse(worksheet.Cells[row, 14].Value?.ToString(), out decimal price) ? price : 0,
                        DateSold = DateTime.TryParse(worksheet.Cells[row, 15].Value?.ToString(), out DateTime dateSold) ? dateSold : DateTime.MinValue
                    };

                    newFoodSales.Add(foodSale);
                }
                catch (Exception ex)
                {
                    return BadRequest($"เกิดข้อผิดพลาดที่แถว {row}: {ex.Message}");
                }
            }
        }
    }

    // ✅ ย้าย _context.SaveChangesAsync() ออกมาข้างนอก
    _context.FoodSales.AddRange(newFoodSales);
    await _context.SaveChangesAsync();

    return Ok($"นำเข้าข้อมูลสำเร็จ {newFoodSales.Count} รายการ");
}
    }
}
