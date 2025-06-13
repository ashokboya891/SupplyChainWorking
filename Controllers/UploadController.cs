using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.DatabaseContext;
using SupplyChain.DTOs;
using SupplyChain.Enum;
using SupplyChain.Enum;
using SupplyChain.IServiceContracts;
using SupplyChain.Models;
using System.Security.Claims;

namespace SupplyChain.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;
        public UploadController(IProductService productService,ApplicationDbContext context)
        {
            _productService = productService;
            this._context = context;
        }
        [HttpPost("upload-excel")]
        public async Task<IActionResult> UploadExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            try
            {
                int productsInserted = await _productService.UploadOrdersFromExcelFile(file);
                return Ok(new { Message = $"{productsInserted} products inserted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //[HttpPost("save-excel")]
        //public async Task<IActionResult> UploadExcel(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", userId);
        //    if (!Directory.Exists(uploadsFolder))
        //        Directory.CreateDirectory(uploadsFolder);

        //    var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        //    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    // Optional: Save file info in DB
        //     await _context.UploadedFiles.AddAsync(new UploadedFile { UserId = userId, FileName = uniqueFileName });
        //     await _context.SaveChangesAsync();

        //    return Ok(new { message = "File uploaded successfully", filePath });
        //}

        [Authorize]
        [HttpGet("my-files")]
        public IActionResult GetMyFiles()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", userId);

            if (!Directory.Exists(userFolder))
                return Ok(new List<string>());

            var files = Directory.GetFiles(userFolder)
                                 .Select(f => Path.GetFileName(f))
                                 .ToList();

            return Ok(files);
        }

        //[Authorize]
        [HttpGet("download-file/{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var baseDir = Path.Combine(Directory.GetCurrentDirectory(), "UploadedRequests");

            // 🔍 Recursively find the file
            var filePath = Directory.EnumerateFiles(baseDir, fileName, SearchOption.AllDirectories)
                                    .FirstOrDefault();

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // MIME for .xlsx
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, contentType, fileName);
        }


        //[Authorize]
        //[HttpPost("CreateRequest")]
        //[Consumes("multipart/form-data")] // ✅ Important!
        //public async Task<IActionResult> CreateRequest([FromForm]string title, [FromForm] IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("File not uploaded.");

        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedRequests", userId);
        //    if (!Directory.Exists(uploadsFolder))
        //        Directory.CreateDirectory(uploadsFolder);

        //    var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        //    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    var request = new CreateRequest
        //    {
        //        Title = title,
        //        FileName = uniqueFileName,
        //        OriginalFileName = file.FileName,
        //        Status = "Pending",
        //        CreatedByUserId = userId, // ✅ Save this
        //        Approvals = new List<Approval>
        //        {
        //             new Approval { Role = ApprovalRole.Finance, ApproverId = "Finance", Status = "Pending",Timestamp=DateTime.Now },
        //             new Approval { Role = ApprovalRole.Manager, ApproverId = "Manager", Status = "null",Timestamp=DateTime.Now},
        //            new Approval { Role = ApprovalRole.IT, ApproverId = "IT", Status = "null",Timestamp=DateTime.Now }
        //         }
        //    };

        //    _context.Requests.Add(request);
        //    await _context.SaveChangesAsync();

        //    return Ok(request);
        //}

        [Authorize]
        [HttpPost("CreateRequest")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateRequest([FromForm] RequestFormDto model)
        {
            if (model.file == null || model.file.Length == 0)
                return BadRequest("File not uploaded.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedRequests", userId);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{model.file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.file.CopyToAsync(stream);
            }

            // 🔄 Dynamically build approval stages
            var stages = await _context.CategoryApprovalStages
                .Where(s => s.CategoryId == model.CategoryId)
                .OrderBy(s => s.StageOrder)
                .ToListAsync();

            var approvals = stages.Select((stage, index) => new Approval
            {
                Role = stage.Role,
                ApproverId = stage.Role.ToString(),
                Status = index == 0 ? "Pending" : "null",
                Timestamp = DateTime.Now
            }).ToList();

            var request = new CreateRequest
            {
                Title = model.Title,
                FileName = uniqueFileName,
                OriginalFileName = model.file.FileName,
                Status = "Pending",
                CreatedByUserId = userId,
                CategoryId = model.CategoryId,
                Approvals = approvals
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(request);
        }

        [HttpGet("PendingRequests/{role}")]
        public async Task<IActionResult> GetPendingRequestsByRole(string role)
        {
            if (!Enum.ApprovalRole.TryParse<ApprovalRole>(role, true, out var parsedRole))
                return BadRequest("Invalid role specified.");

            var pendingApprovals = await _context.Approvals
                .Include(a => a.Request)
                    .ThenInclude(r => r.Comments)
                .Include(a => a.Request)
                    .ThenInclude(r => r.Category) // 👈 Include Category
                .Where(a => a.Role == parsedRole && a.Status == "Pending")
                .Select(a => new
                {
                    PendingApproval = new
                    {
                        a.Id,
                        a.RequestId,
                        a.Role,
                        a.ApproverId,
                        a.Status,
                        a.Timestamp
                    },
                    RequestTitle = a.Request.Title,
                    FileName = a.Request.FileName,
                    CategoryName = a.Request.Category.Name, // 👈 Add category name
                    ApprovalHistory = a.Request.Comments
                        .OrderBy(x => x.Timestamp)
                        .Select(x => new
                        {
                            x.Role,
                            x.ApproverId,
                            x.Status,
                            x.Comment,
                            x.Timestamp
                        })
                })
                .ToListAsync();

            return Ok(pendingApprovals);
        }

        //[HttpGet("PendingRequests/{role}")]
        //public async Task<IActionResult> GetPendingRequestsByRole(string role)
        //{
        //    if (!Enum.ApprovalRole.TryParse<ApprovalRole>(role, true, out var parsedRole))
        //        return BadRequest("Invalid role specified.");

        //    var pendingApprovals = await _context.Approvals
        //        .Include(a => a.Request)
        //            .ThenInclude(r => r.Comments)
        //        .Where(a => a.Role == parsedRole && a.Status == "Pending")
        //        .Select(a => new
        //        {
        //            PendingApproval = new
        //            {
        //                a.Id,
        //                a.RequestId,
        //                a.Role,
        //                a.ApproverId,
        //                a.Status,
        //                a.Timestamp
        //            },
        //            RequestTitle = a.Request.Title,
        //            FileName=a.Request.FileName,
        //            ApprovalHistory = a.Request.Comments
        //                .OrderBy(x => x.Timestamp)
        //                .Select(x => new
        //                {
        //                    x.Role,
        //                    x.ApproverId,
        //                    x.Status,
        //                    x.Comment,
        //                    x.Timestamp
        //                })
        //        })
        //        .ToListAsync();

        //    return Ok(pendingApprovals);
        //}
        [HttpGet("CategoryApprovalStages")]
        public async Task<IActionResult> GetCategoryApprovalStages()
        {
            var stages = await _context.Categories.ToListAsync();
            return Ok(stages);
        }

        [HttpPost("ApproveOrReject")]
        public async Task<IActionResult> ApproveOrReject(approveOrReject request)
        {
            var approval = await _context.Approvals
                .Include(a => a.Request)
                .Where(a => a.RequestId == request.requestId && a.Role == request.role)
                .FirstOrDefaultAsync();

            if (approval == null || approval.Status != "Pending")
                return BadRequest("Invalid or already processed approval.");

            if (string.IsNullOrWhiteSpace(request.comment))
                return BadRequest("Comment is required.");

            approval.Status = request.action;
            approval.Timestamp = DateTime.Now;

            // Store comment in separate table
            var commentEntry = new ApprovalComment
            {
                RequestId = request.requestId,
                Role = request.role,
                ApproverId = approval.ApproverId,
                Status = request.action,
                Comment = request.comment,
                Timestamp = DateTime.Now
            };
            _context.ApprovalComments.Add(commentEntry);

            if (request.action == "Approved")
            {

                // Get current request's categoryId and current stage info
                var currentRequest = approval.Request;
                var currentStage = await _context.CategoryApprovalStages
                    .FirstOrDefaultAsync(s => s.CategoryId == currentRequest.CategoryId && s.Role == request.role);

                if (currentStage == null)
                    return BadRequest("Approval stage not found.");

                // Find the next stage
                var nextStage = await _context.CategoryApprovalStages
                    .Where(s => s.CategoryId == currentRequest.CategoryId && s.StageOrder > currentStage.StageOrder)
                    .OrderBy(s => s.StageOrder)
                    .FirstOrDefaultAsync();

                //var nextRole = (ApprovalRole)((int)request.role + 1);
                //var nextApproval = await _context.Approvals
                //    .FirstOrDefaultAsync(a => a.RequestId == request.requestId && a.Role == nextRole);

                if (nextStage != null)
                {
                    var nextApproval = await _context.Approvals
                      .FirstOrDefaultAsync(a => a.RequestId == request.requestId && a.Role == nextStage.Role);

                    if (nextApproval != null)
                    {
                        nextApproval.Status = "Pending";
                        nextApproval.Timestamp = DateTime.Now;
                    }
                    //nextApproval.Status = "Pending";
                }
                else
                {
                    approval.Request.Status = "Approved";

                    // 🔁 Move file to orders folder
                    var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedRequests", approval.Request.CreatedByUserId, approval.Request.FileName);
                    //var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedRequests", approval.ApproverId, approval.Request.FileName);
                    var targetFolder = Path.Combine(Directory.GetCurrentDirectory(), "Orders");
                    if (!Directory.Exists(targetFolder))
                        Directory.CreateDirectory(targetFolder);
                    var targetPath = Path.Combine(targetFolder, approval.Request.FileName);
                    if (System.IO.File.Exists(sourcePath))
                    {
                        System.IO.File.Copy(sourcePath, targetPath, true); // or Move
                    }
                    else
                    {
                        return NotFound("Source file not found.");
                    }
                }
                
            }

            //if (action == "Approved")
            //{
            //    var nextRole = (ApprovalRole)((int)role + 1);
            //    var nextApproval = await _context.Approvals
            //        .FirstOrDefaultAsync(a => a.RequestId == requestId && a.Role == nextRole);

            //    nextApproval.Status = "Pending";
            //    if (nextApproval == null)
            //    {
            //        approval.Request.Status = "Approved";
            //    }
            //}
            else if (request.action == "Rejected")
            {
                var prevRole = (ApprovalRole)((int)request.role - 1);
                var previousApproval = await _context.Approvals
                    .FirstOrDefaultAsync(a => a.RequestId == request.requestId && a.Role == prevRole);

                if (previousApproval != null)
                {
                    previousApproval.Status = "Pending";
                    previousApproval.Timestamp = DateTime.MinValue;
                }

                approval.Request.Status = "Rejected";
            }

            await _context.SaveChangesAsync();
            return Ok("Action recorded.");
        }
    }
}
