namespace AlzaBox;

public static class Utils
{
    public static int GetIdCounter(bool autoincrease = true)
    {
        string filePath = "./../../../persistId.txt";
        int counter = 1;

        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            if (int.TryParse(content, out int lastValue))
            {
                counter = lastValue + 1;
            }
        }

        if (autoincrease)
            File.WriteAllText(filePath, counter.ToString());

        return counter;
    }

    public static void SaveResponse(string response, string filename)
    {
        string logFolder = "./../../../Logs";
        if (!Directory.Exists(logFolder))
        {
            Directory.CreateDirectory(logFolder);
        }

        string filePath = Path.Combine(logFolder, $"{filename}.json");

        using JsonDocument doc = JsonDocument.Parse(response);
        string formattedJson = JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(filePath, formattedJson);
    }
}
