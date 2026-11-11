using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Utils.Extensions;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Modsen.Portal.Utilities.Controllers.Congratulations;

[AllowAnonymous]
[Area("[Shared] Congratulations")]
[Route("api/celebrations")]
public class CongratulationsController : Controller
{
    private static readonly Dictionary<CongratulationType, string> s_congratulations =
        new()
        {
            { CongratulationType.None, "./Controllers/Congratulations/DOB.jpg" },
            { CongratulationType.DOB, "./Controllers/Congratulations/DOB.jpg" },
            { CongratulationType.ProbationEnd, "./Controllers/Congratulations/ProbationEnd.jpg" },
            { CongratulationType.WorkAnniversary, "./Controllers/Congratulations/WorkAnniversary.jpg" }
        };

    /// <summary>
    /// Get celebrations
    /// Allowed role: Anonymous
    /// </summary>
    /// <param name="name">Name</param>
    /// <param name="type">Type</param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Get([FromQuery] string name, [FromQuery] CongratulationType type)
    {
        var filePath = GetFilePath(type);
        if (filePath.IsNullOrWhiteSpace())
        {
            return NotFound();
        }

        var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
        var fontCollection = new FontCollection();
        var fontFamily = fontCollection.Add(Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "./Controllers/Congratulations/helvetica_bold.otf"));
        var font = new Font(fontFamily, 44f, FontStyle.Bold);
        var brush = type == CongratulationType.ProbationEnd
            ? Brushes.Solid(Color.Black)
            : Brushes.Solid(Color.White);
        var options = new RichTextOptions(font)
        {
            Origin = new PointF(88, 540),
            TabWidth = 8,
            WrappingLength = 1400,
            HorizontalAlignment = HorizontalAlignment.Left
        };

        using var image = Image.Load(fullPath);
        using var memoryStream = new MemoryStream();

        image.Mutate(x => x.DrawText(options, name, brush));
        image.Save(memoryStream, JpegFormat.Instance);

        return File(memoryStream.ToArray(), "image/jpeg");
    }

    private static string? GetFilePath(CongratulationType type)
    {
        return s_congratulations.GetValueOrDefault(type);
    }
}

public enum CongratulationType
{
    None = 0,
    DOB = 1,
    ProbationEnd = 2,
    WorkAnniversary = 3
}
