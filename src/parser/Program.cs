// Get all the .editorconfig files within the data directory
// and parse them into a list of EditorConfig objects
var editorConfigs = Directory.GetFiles("../../data", "*.editorconfig", SearchOption.AllDirectories)
    .Select(EditorConfigParser.Parse);

var merged = EditorConfigParser.Merge(editorConfigs);


// create a file with the unified result stored
using var file = File.CreateText("unified.editorconfig");

// dump the results to the console
foreach (var editorConfig in merged.Settings)
{
    System.Console.WriteLine();
    Console.WriteLine($"[{editorConfig.Name}]");
    file.WriteLine($"[{editorConfig.Name}]");
    foreach (var entry in editorConfig.Entries)
    {
        // given the key has only one value or all values are the same dump one line.
        if (entry.Values.Count == 1 || entry.Values.Select(x => x.Value).Distinct().Count() == 1)
        {
            Console.WriteLine($"\t{entry.Key} = {entry.Values[0].Value}");
            // out to unified file
            file.WriteLine($"{entry.Key} = {entry.Values[0].Value}");
        }
        else
        {
            // prompt the user to select a value
            var selected = PromptAndSelect(entry);
            Console.WriteLine($"\t{selected.Key} = {selected.Values[0].Value}");
            // out to unified file
            file.WriteLine($"{selected.Key} = {selected.Values[0].Value}");

            // otherwise dump each value on a separate line
            // Console.WriteLine($"\t{entry.Key}");
            // foreach (var value in entry.Values)
            // {
            //     Console.WriteLine($"\t\t{value.Value}   #   {value.Source.FileName}:{value.LineNumber}");
            // }
        }
    }
}


EditorConfigEntry PromptAndSelect(EditorConfigEntry entry)
{
    var values = entry.Values.Select(x => x.Value).Distinct().ToList();
    if (values.Count == 1)
    {
        return entry;
    }

    Console.WriteLine($"Select a value for {entry.Key}:");
    for (int i = 0; i < values.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {values[i]} (@{entry.Values[i].Source.FileName}:{entry.Values[i].LineNumber}))");
    }

    var input = Console.ReadKey().KeyChar.ToString();
    if (int.TryParse(input, out var index) && index > 0 && index <= values.Count)
    {
        var value = values[index - 1];
        var newEntry = new EditorConfigEntry(entry.Key, new List<EditorConfigValue>());
        foreach (var editorConfigValue in entry.Values)
        {
            if (editorConfigValue.Value == value)
            {
                newEntry.Values.Add(editorConfigValue);
            }
        }

        return newEntry;
    }

    return entry;
}