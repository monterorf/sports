using Microsoft.AspNetCore.Mvc;

namespace Sports.API.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly string _uploadsPath = "wwwroot/uploads";

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromQuery] string? folder = "general")
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                // Validar tipos permitidos
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    return BadRequest("File type not allowed");

                // Crear carpeta si no existe
                var folderPath = Path.Combine(_uploadsPath, folder);
                Directory.CreateDirectory(folderPath);

                // Generar nombre único
                var fileName = $"{Guid.NewGuid()}-{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(folderPath, fileName);

                // Guardar archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Devolver URL relativa
                var fileUrl = $"/uploads/{folder}/{fileName}";
                return Ok(new { success = true, fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}
