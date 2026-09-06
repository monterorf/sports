using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sports.API.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sports.API.Tests.UnitTests.Controllers
{
    public class FilesControllerTests
    {
        [Fact]
        public async Task UploadFile_ReturnsBadRequest_WhenFileIsNull()
        {
            var controller = new FilesController();

            var result = await controller.UploadFile(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UploadFile_ReturnsBadRequest_WhenExtensionNotAllowed()
        {
            var controller = new FilesController();

            var content = Encoding.UTF8.GetBytes("dummy");
            using var stream = new MemoryStream(content);
            var formFile = new FormFile(stream, 0, stream.Length, "file", "test.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            var result = await controller.UploadFile(formFile);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UploadFile_SavesFileAndReturnsOk_WhenValidImage()
        {
            var controller = new FilesController();

            var content = Encoding.UTF8.GetBytes("image-bytes");
            using var stream = new MemoryStream(content);
            var fileName = "sample.jpg";
            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            // use a unique folder to avoid collisions and make cleanup predictable
            var folder = "testfolder-" + Guid.NewGuid().ToString("N");
            var result = await controller.UploadFile(formFile, folder);

            var okResult = Assert.IsType<OkObjectResult>(result);

            // The controller saves under "wwwroot/uploads/{folder}"
            var uploadDir = Path.Combine("wwwroot", "uploads", folder);
            Assert.True(Directory.Exists(uploadDir), $"Expected upload directory to exist: {uploadDir}");

            var files = Directory.GetFiles(uploadDir);
            Assert.NotEmpty(files);

            // cleanup
            foreach (var f in files) File.Delete(f);
            try
            {
                Directory.Delete(uploadDir);
                // try to remove parent uploads folder if empty (optional)
                var parent = Path.Combine("wwwroot", "uploads");
                if (Directory.Exists(parent) && Directory.GetFiles(parent).Length == 0 && Directory.GetDirectories(parent).Length == 0)
                    Directory.Delete(parent);
            }
            catch
            {
                // ignore cleanup errors in test
            }
        }
    }
}
