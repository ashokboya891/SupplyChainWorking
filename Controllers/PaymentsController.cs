using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using SupplyChain.DatabaseContext;
using SupplyChain.DTOs;
using SupplyChain.Services;



namespace SupplyChain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] PaymentRequest request)
        {
            if (request.Items == null || !request.Items.Any())
                return BadRequest("No items found in the request.");

            // 1. Calculate amount
            decimal totalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice);
            int amountInPaise = (int)(totalAmount * 100);

            // 2. Create Razorpay order
            RazorpayClient client = new RazorpayClient("rzp_test_QXHInQ5xIrE7dd", "ZkNoeXtH6avmtvSqWmJmksZj");
            var options = new Dictionary<string, object>
              {
            { "amount", amountInPaise },
        { "currency", "INR" },
        { "receipt", Guid.NewGuid().ToString() },
        { "payment_capture", 1 }
        };
            var razorpayOrder = client.Order.Create(options);

            // 3. Save to DB
            var parameters = new[]
            {
        new SqlParameter("@UserId", request.UserId),
        new SqlParameter("@OrderDate", DateTime.Now),
        new SqlParameter("@RazorpayOrderId", razorpayOrder["id"].ToString()),
        new SqlParameter("@PaymentStatus", "Pending"),
        OrderService.GetOrderItemsTVP(request.Items)
    };

            await _context.Database.ExecuteSqlRawAsync("EXEC PlaceOrder @UserId, @OrderDate, @RazorpayOrderId, @PaymentStatus, @OrderItems", parameters);

            // 4. Return to frontend
            return Ok(new
            {
                razorpayOrderId = razorpayOrder.Attributes["id"].ToString(),
                amount = Convert.ToInt32(razorpayOrder.Attributes["amount"]),
                currency = razorpayOrder.Attributes["currency"].ToString(),
                message = "Order created and saved successfully."
            });
        }


        //[HttpPost("create-order")]
        //public async Task<IActionResult> CreateOrder([FromBody] PaymentRequest request)
        //{
        //    if (request.Items == null || !request.Items.Any())
        //        return BadRequest("No items found in the request.");

        //    // 1. Calculate total amount
        //    decimal totalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice);  // e.g. ₹
        //    int amountInPaise = (int)(totalAmount * 100); // Convert ₹ to paisa for Razorpay

        //    // 2. Create Razorpay Order
        //    RazorpayClient client = new RazorpayClient("rzp_test_QXHInQ5xIrE7dd", "ZkNoeXtH6avmtvSqWmJmksZj");

        //    Dictionary<string, object> options = new Dictionary<string, object>
        //    {
        //         { "amount", amountInPaise },  // must be in paisa
        //         { "currency", "INR" },
        //            { "receipt", Guid.NewGuid().ToString() },
        //        { "payment_capture", 1 }
        //    };

        //    Razorpay.Api.Order razorpayOrder = client.Order.Create(options);

        //    // 3. Save Order in DB using stored procedure
        //    var parameters = new[]
        //    {
        //        new SqlParameter("@UserId", request.UserId),
        //        new  SqlParameter("@OrderDate", DateTime.Now),
        //        OrderService.GetOrderItemsTVP(request.Items) // Convert list to TVP
        //    };

        //    await _context.Database.ExecuteSqlRawAsync("EXEC PlaceOrder @UserId, @OrderDate, @OrderItems", parameters);

        //    return Ok(new
        //    {
        //        razorpayOrderId = razorpayOrder.Attributes["id"].ToString(),
        //        amount = Convert.ToInt32(razorpayOrder.Attributes["amount"]),
        //        currency = razorpayOrder.Attributes["currency"].ToString(),
        //        message = "Order created and saved successfully."
        //    });


        //}

        //public class OrderItemDto
        //{
        //    public int ProductId { get; set; }
        //    public int Quantity { get; set; }
        //    public decimal UnitPrice { get; set; }
        //    public string ProductName { get; set; }
        //}

        public class PaymentRequest
        {
            public Guid UserId { get; set; }
            public List<OrderItemDto> Items { get; set; }
        }

 

        //[HttpPost("verify")]
        //public IActionResult VerifyPayment([FromBody] PaymentVerificationRequest request)
        //{
        //    string keySecret = "ZkNoeXtH6avmtvSqWmJmksZj";

        //    string payload = $"{request.razorpay_order_id}|{request.razorpay_payment_id}";

        //    string generatedSignature;
        //    using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(keySecret)))
        //    {
        //        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        //        generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
        //    }

        //    if (generatedSignature == request.razorpay_signature)
        //    {
        //        // ✅ Valid payment
        //        // You can save it to DB, update order status, etc.
        //        return Ok(new { status = "success" });
        //    }

        //    // ❌ Invalid signature
        //    return BadRequest(new { status = "failure" });
        //}

        public class RazorpayVerifyDto
        {
            public string razorpay_payment_id { get; set; }
            public string razorpay_order_id { get; set; }
            public string razorpay_signature { get; set; }
        }

        //[HttpPost("verify")]
        //public IActionResult VerifyPayment([FromBody] RazorpayVerifyDto data)
        //{
        //    var key = "ZkNoeXtH6avmtvSqWmJmksZj";
        //    string generatedSignature = GenerateSignature(data.razorpay_order_id + "|" + data.razorpay_payment_id, key);

        //    if (generatedSignature == data.razorpay_signature)
        //    {
        //        return Ok(new { status = "success" });
        //    }

        //    return BadRequest(new { status = "failed" });
        //}


        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] RazorpayVerifyDto data)
        {
            var key = "ZkNoeXtH6avmtvSqWmJmksZj";
            string generatedSignature = GenerateSignature(data.razorpay_order_id + "|" + data.razorpay_payment_id, key);

            if (generatedSignature == data.razorpay_signature)
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.RazorpayOrderId == data.razorpay_order_id);

                if (order == null)
                    return NotFound("Order not found");

                order.PaymentStatus = "Paid";
                order.RazorpayPaymentId = data.razorpay_payment_id;
                order.RazorpaySignature = data.razorpay_signature;
                order.PaidAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { status = "success" });
            }

            return BadRequest(new { status = "failed" });
        }

        private string GenerateSignature(string text, string secret)
        {
            var encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(text);
            using var hmacsha256 = new System.Security.Cryptography.HMACSHA256(keyByte);
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
        }


    }
}
