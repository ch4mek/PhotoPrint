using System.Text;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests;

internal sealed class XUnitOutputAdapter : TextWriter
{
    private readonly ITestOutputHelper _output;

    public XUnitOutputAdapter(ITestOutputHelper output)
    {
        _output = output;
        Encoding = Encoding.UTF8;
    }

    public override void WriteLine(string? value)
    {
        _output.WriteLine(value);
    }

    public override Encoding Encoding { get; }
}
