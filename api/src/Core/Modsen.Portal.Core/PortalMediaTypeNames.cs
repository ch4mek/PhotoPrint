using System.Net.Mime;

namespace Modsen.Portal.Core;

public static class PortalMediaTypeNames
{
    public const string Excel = "application/vnd.ms-excel";
    public const string ExcelX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public const string Word = "application/msword";
    public const string WordX = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    public const string PowerPointX = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
    public const string Pdf = "application/pdf";
    public const string ZipResponse = "application/zip";

    public const string VndRar = "application/vnd.rar";

    public static readonly string[] Rar = [
        "application/x-rar-compressed",
        VndRar,
        "application/x-compressed",
        MediaTypeNames.Application.Octet
    ];

    public static readonly string[] Zip = [
        MediaTypeNames.Application.Zip,
        "application/x-zip-compressed",
        "multipart/x-zip",
        MediaTypeNames.Application.Octet
    ];

    public static readonly string[] Md = [
        MediaTypeNames.Text.Markdown,
        MediaTypeNames.Text.Plain,
        MediaTypeNames.Application.Octet
    ];
}
