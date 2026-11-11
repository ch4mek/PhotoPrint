using System.Text;

namespace Kontur.Core.Csv;

/// <summary>Минимальный quote-aware читатель CSV (наши сиды: разделитель запятая, кавычки ").</summary>
public static class Csv
{
    public static List<Dictionary<string, string>> Read(string path)
    {
        var rows = new List<Dictionary<string, string>>();
        using var reader = new StreamReader(path, Encoding.UTF8);
        var header = ParseLine(reader.ReadLine() ?? throw new InvalidOperationException($"Пустой CSV: {path}"));
        while (reader.ReadLine() is { } line)
        {
            if (line.Length == 0) continue;
            var values = ParseLine(line);
            var row = new Dictionary<string, string>(header.Count);
            for (var i = 0; i < header.Count; i++)
                row[header[i]] = i < values.Count ? values[i] : "";
            rows.Add(row);
        }
        return rows;
    }

    private static List<string> ParseLine(string line)
    {
        var values = new List<string>();
        var sb = new StringBuilder();
        var inQuotes = false;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                    else inQuotes = false;
                }
                else sb.Append(c);
            }
            else if (c == '"') inQuotes = true;
            else if (c == ',') { values.Add(sb.ToString()); sb.Clear(); }
            else sb.Append(c);
        }
        values.Add(sb.ToString());
        return values;
    }
}
