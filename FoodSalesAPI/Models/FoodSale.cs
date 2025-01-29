using System;
using System.ComponentModel.DataAnnotations;

namespace FoodSalesAPI.Models
{
    public class FoodSale
{
    [Key]
    public int Id { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }
    public string ItemType { get; set; }
    public string SalesChannel { get; set; }
    public string OrderPriority { get; set; }
    public DateTime OrderDate { get; set; }
    public int OrderID { get; set; }
    public DateTime ShipDate { get; set; }
    public int UnitsSold { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalRevenue { get; set; }

    // เพิ่มคุณสมบัติใหม่
    public string FoodName { get; set; } // ชื่ออาหาร
    public string Category { get; set; } // หมวดหมู่
    public decimal Price { get; set; }   // ราคา
    public DateTime DateSold { get; set; } // วันที่ขาย
}

}
