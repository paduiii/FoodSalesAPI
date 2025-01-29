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

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var foodSale = new FoodSale
                        {
                            FoodName = worksheet.Cells[row, 1].Value?.ToString(),
                            Category = worksheet.Cells[row, 2].Value?.ToString(),
                            Price = Convert.ToDecimal(worksheet.Cells[row, 3].Value),
                            DateSold = DateTime.Parse(worksheet.Cells[row, 4].Value?.ToString())
                        };

                        _context.FoodSales.Add(foodSale);
                    }

                    await _context.SaveChangesAsync();
                }
            }

            return Ok("นำเข้าข้อมูลสำเร็จ");
        }
    }
}
