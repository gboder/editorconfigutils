public class EditorConfig
{
    public List<EditorConfigSection> Settings { get; } = new List<EditorConfigSection>();
    public void Add(string key, string value, string fileName, uint lineNumber, string sectionName)
    {
        var entry = Settings.FirstOrDefault(x => x.Name == sectionName);
        if (entry == null)
        {
            entry = new EditorConfigSection(sectionName, new List<EditorConfigEntry>());
            Settings.Add(entry);
        }
        var valueEntry = entry.Entries.FirstOrDefault(x => x.Key == key);
        if (valueEntry == null)
        {
            valueEntry = new EditorConfigEntry(key, new List<EditorConfigValue>());
            entry.Entries.Add(valueEntry);
        }
        valueEntry.Values.Add(new EditorConfigValue(value, new EditorConfigSource(fileName), lineNumber));
    }
}
public record EditorConfigSource(string FileName);

public record EditorConfigValue(string Value, EditorConfigSource Source, uint LineNumber);

public record EditorConfigEntry(string Key, List<EditorConfigValue> Values);

public record EditorConfigSection(string Name, List<EditorConfigEntry> Entries);
