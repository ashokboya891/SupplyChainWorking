using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SupplyChain.DatabaseContext;
using SupplyChain.DTOs;
using SupplyChain.Entities;
using SupplyChain.Models;
using SupplyChain.Services;
using System.Data;
using System.Security.Claims;

namespace SupplyChain.Controllers
{
    [Authorize(policy: "RequireAdminRole")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> user, RoleManager<ApplicationRole> roleManager,ApplicationDbContext context)
        {
            this._userManager = user;
            _roleManager = roleManager;
            this._context = context;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var result = await _context.RestockRequests
             .Where(r => r.Status=="Pending").Select(r => new RestockRequestDto
             {
                 RequestId = r.RequestId,
                 Status = r.Status,
                 @ShortageQty = r.ShortageQty,
                 ProductName = r.Product.Name,
                 ProductId = r.ProductId,
                 threshold = r.Product.Threshold,
                 CreatedDate=r.CreatedDate
             })
                .OrderByDescending(r => r.RequestId)
                .ToListAsync();

            if (result == null)
                return NotFound("No matching restock request found.");

            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> FullFillRequest([FromBody] StockFillDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Admin not found.");

            try
            {
                var sql = "EXEC [dbo].[FulfillOrder] @RequestId, @AdminId, @ShortageQty, @Status";

                var parameters = new[]
                {
            new SqlParameter("@RequestId", dto.RequestId),
            new SqlParameter("@AdminId", userId), // Using authenticated user's ID
            new SqlParameter("@ShortageQty", dto.Quantity),
            new SqlParameter("@Status", dto.Status ?? "Completed") // Default to "Completed" if null
        };

                await _context.Database.ExecuteSqlRawAsync(sql, parameters);
                return Ok(new
                {
                    Success = true,
                    Message = "Order fulfilled successfully",
                    RequestId = dto.RequestId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Failed to fulfill order",
                    Error = ex.Message,
                    RequestId = dto.RequestId
                });
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPaidUnpaidTransactions(string status)
        {
            if(status!=null && status=="Paid")
            {
                var paidData=await _context.Orders.Where(o => o.PaymentStatus == "Paid").Include(o => o.OrderItems)
                    .Select(o => new OrderDto
                    {
                        OrderId = o.OrderId,
                        UserId = o.UserId,
                        RazorpayOrderId = o.RazorpayOrderId,
                        RazorpayPaymentId = o.RazorpayPaymentId,
                        PaymentStatus = o.PaymentStatus,
                        TotalAmount = o.TotalAmount,
                        OrderDate = o.OrderDate,
                        PaidAt=o.PaidAt,
                        OrderItems= o.OrderItems.Select(oi => new OrderItemDto
                        {
                            ProductId = oi.ProductId,
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice,
                            ProductName = oi.Product.Name
                        }).ToList()

                    })
                    .ToListAsync();
                return Ok(paidData);
            }
            else if (status != null && status == "Unpaid")
            {
                var unpaidData = await _context.Orders.Where(o => o.PaymentStatus == "Pending")
                    .Select(o => new OrderDto
                    {
                        OrderId = o.OrderId,
                        UserId = o.UserId,
                        RazorpayOrderId = o.RazorpayOrderId,
                        RazorpayPaymentId = o.RazorpayPaymentId,
                        PaymentStatus = o.PaymentStatus,
                        TotalAmount = o.TotalAmount,
                        OrderDate = o.OrderDate,
                    })
                    .ToListAsync();
                return Ok(unpaidData);
            }
            else
            {
                return BadRequest("Invalid status parameter. Use 'Paid' or 'Unpaid'.");
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetInventoryLogs()
        {
            var logs = await _context.InventoryLogs
        .Include(i => i.Product)
        .Include(i => i.User)
        .Select(l => new InventoryLogDto
        {
            LogId = l.LogId,
            CreatedDate = l.CreatedDate,
            QuantityChange = l.QuantityChange,
            Product = new ProductDto
            {
                ProductId = l.Product.ProductId,
                Name = l.Product.Name,
            },
            User = new UserDto
            {
                UserId = l.User.Id,
                UserName = l.User.UserName
            }
        })
        .ToListAsync();
            //        var logs = await _context.InventoryLogs
            //.Include(i => i.Product)
            //.Include(i => i.User)
            //.ToListAsync();


            return Ok(logs);
        }

        [HttpGet("users-with-roles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var users = _userManager.Users.ToList(); // Get all users

            var usersWithRoles = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                usersWithRoles.Add(new
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return Ok(usersWithRoles);
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{userId}")]
        public async Task<IActionResult> EditRoles(Guid userId, [FromBody] List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("User not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Ensure selected roles exist
            foreach (var role in selectedRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = role });
                }
            }

            // Add new roles that are not already assigned
            var rolesToAdd = selectedRoles.Except(currentRoles);
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
                return BadRequest("Failed to add roles.");

            // Remove roles that are not selected anymore
            var rolesToRemove = currentRoles.Except(selectedRoles);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
                return BadRequest("Failed to remove roles.");

            var updatedRoles = await _userManager.GetRolesAsync(user);
            return Ok(new { user.Id, user.UserName, UpdatedRoles = updatedRoles });
        }




        //[Authorize(policy: "RequireAdminRole")]
        //[HttpPost("edit-roles/{username}")]
        //public async Task<IActionResult> EditRoles(string username, [FromQuery] string roles)
        //{
        //    if (string.IsNullOrEmpty(roles))
        //        return BadRequest("You must select at least one role.");

        //    // Split incoming roles by comma and remove whitespace
        //    var selectedRoles = roles.Split(',').Select(r => r.Trim()).ToArray();

        //    // Find user by username
        //    var user = await _userManager.FindByNameAsync(username);
        //    if (user == null)
        //        return NotFound("User not found.");

        //    // Get current roles of the user
        //    var currentRoles = await _userManager.GetRolesAsync(user);

        //    // Add new roles that the user doesn't already have
        //    var addRolesResult = await _userManager.AddToRolesAsync(user, selectedRoles.Except(currentRoles));
        //    if (!addRolesResult.Succeeded)
        //        return BadRequest("Failed to add user to roles.");

        //    // Remove roles that are no longer selected
        //    var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles.Except(selectedRoles));
        //    if (!removeRolesResult.Succeeded)
        //        return BadRequest("Failed to remove user from roles.");

        //    // Return the updated roles for confirmation
        //    var updatedRoles = await _userManager.GetRolesAsync(user);
        //    return Ok(updatedRoles);
        //}


        //[Authorize(policy: "RequireAdminRole")]
        //[HttpGet("users-with-roles")]
        //public async Task<IActionResult> GetRoGetUsersWithRolesles()
        //{
        // var roles =   await _userManager.GetRolesAsync();
        //    return roles;
        //}
    }
}
