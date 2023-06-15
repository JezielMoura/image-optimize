using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;

namespace ImageOptimize.Pages;

[RequestSizeLimit(500_000_000)]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }

    public IActionResult OnPost(string format, int width, int quality)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var img in Request.Form.Files)
            {
                var name = $"{img.FileName.Split('.')[0]}.{format}";
                var entry = archive.CreateEntry(name, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                var image = Image.Load(img.OpenReadStream());

                image.Mutate(c => c.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Pad,
                    PremultiplyAlpha = true,
                    Size = new Size { Width = width }
                }).BackgroundColor(new Rgba32(255, 255, 255, 0)));

                image.Save(entryStream, GetImageEncoder(format, quality));
            }
        }

        return File(memoryStream.ToArray(), "application/zip", "images.zip");
    }

    private static IImageEncoder GetImageEncoder(string format, int quality) => 
        format switch
        {
            "webp" =>  new WebpEncoder { Quality = quality },
            "png" => new PngEncoder(),
            _ => new JpegEncoder { Quality = quality }
        };
}
