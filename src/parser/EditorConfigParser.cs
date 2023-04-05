public class EditorConfigParser
{
    private string fileName;
    private string text;

    public EditorConfigParser(string fileName, string text)
    {
        this.fileName = fileName;
        this.text = text;
    }

    public static EditorConfig Parse(string fileName)
    {
        var text = File.ReadAllText(fileName);
        var parser = new EditorConfigParser(fileName, text);
        return parser.ParseInternal();
    }

    public static EditorConfig Merge(IEnumerable<EditorConfig> configs)
    {
        var editorConfig = new EditorConfig();
        foreach (var config in configs)
        {
            foreach (var section in config.Settings)
            {
                foreach (var entry in section.Entries)
                {
                    foreach (var value in entry.Values)
                    {
                        editorConfig.Add(entry.Key, value.Value, value.Source.FileName, value.LineNumber, section.Name);
                    }
                }
            }
        }

        return editorConfig;
    }

    private EditorConfig ParseInternal()
    {
        var editorConfig = new EditorConfig();
        var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        uint lineIndex = 0;
        string sectionName = string.Empty;
        foreach (var line in lines)
        {
            lineIndex++;
            var trimmedLine = line.Trim();
            // skip comments and empty lines
            if (trimmedLine.StartsWith("#") || string.IsNullOrWhiteSpace(trimmedLine))
            {
                continue;
            }
            
            // detect section headers
            if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
            {
                // start new section
                sectionName = trimmedLine.Substring(1, trimmedLine.Length - 2);
            }

            var parts = trimmedLine.Split(new[] { '=' }, 2);
            if (parts.Length != 2)
            {
                continue;
            }

            var key = parts[0].Trim();
            var value = parts[1].Trim();
            editorConfig.Add(key, value,fileName,lineIndex, sectionName);
        }

        return editorConfig;
    }
}