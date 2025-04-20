using Microsoft.AspNetCore.Mvc;
using DinkToPdf;
using DinkToPdf.Contracts;
using System;

namespace puppeteer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfGenerationController : ControllerBase
    {
        private readonly IConverter _converter;

        //  Inject IConverter via constructor
        public PdfGenerationController(IConverter converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }
        [HttpPost("generate")]
        public IActionResult GeneratePdf([FromBody] PdfRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.HtmlContent))
                return BadRequest("HTML content is required");

            Console.WriteLine("📄 HTML content received for PDF generation.");

            try
            {
                var doc = new HtmlToPdfDocument
                {
                    GlobalSettings = new GlobalSettings
                    {
                        Margins = new MarginSettings { Top = 0, Bottom = 10, Left = 0, Right = 0 }, // ✅ Ensure footer space
                        Out = null,
                        PaperSize = PaperKind.A4,
                        DocumentTitle = "Generated PDF",
                        UseCompression = true
                    },
                    Objects = {
                new ObjectSettings
                {
                   HtmlContent = request.HtmlContent,
                    WebSettings = new WebSettings
                    {
                        DefaultEncoding = "utf-8",
                        LoadImages = true,
                        EnableJavascript = true,
                        Background = true,
                        EnableIntelligentShrinking = true
                    },

                    // ✅ Updated FooterSettings (Right-Aligned Page Numbers)
                    FooterSettings = new FooterSettings
                    {
                        Right = "Page [page] of [toPage]", // ✅ Moves page numbers to the right
                        FontSize = 10,
                        Spacing = 2,
                        
                    }
                }
            }
                };

                // Convert HTML to PDF
                byte[] pdfBytes = _converter.Convert(doc);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    return StatusCode(500, "PDF generation failed (empty output).");
                }

                Console.WriteLine("✅ PDF successfully generated!");
                return File(pdfBytes, "application/pdf", "document.pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during PDF generation: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

        // ✅ Request model to capture HTML content for PDF generation
        public class PdfRequest
        {
            public string HtmlContent { get; set; }
        }

    }
