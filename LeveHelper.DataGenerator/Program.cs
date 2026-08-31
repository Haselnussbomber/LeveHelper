using System.Text;
using System.Text.Json;
using Lumina;
using Lumina.Excel.Sheets;

var gameData = new GameData(args[0]);
var ENpcResidentSheet = gameData.Excel.GetSheet<ENpcResident>();
var LeveSheet = gameData.Excel.GetSheet<Leve>();

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.UserAgent.Clear();
httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("LeveHelper.DataGenerator/2.0.0");
var response = await httpClient.GetAsync("https://xivstats.com/data/LeveIssuers.json") ?? throw new Exception("Couldn't fetch LeveIssuers.json");
var stream = await response.Content.ReadAsStreamAsync();
var data = await JsonSerializer.DeserializeAsync<Dictionary<uint, LeveIssuer>>(stream) ?? throw new Exception("Couldn't deserialize LeveIssuers.json");

static string Indent(int level, string line) => new string(' ', level * 4) + line;

var sb = new StringBuilder();
sb.AppendLine("using System.Collections.Generic;");
sb.AppendLine();
sb.AppendLine("namespace LeveHelper;");
sb.AppendLine();
sb.AppendLine("public static class Data");
sb.AppendLine("{");
sb.AppendLine(Indent(1, "public static readonly Dictionary<uint, uint[]> Issuers = new()"));
sb.AppendLine(Indent(1, "{"));

foreach (var issuer in data.Values.OrderBy(issuer => issuer.ENpcBaseId))
{
    if (!ENpcResidentSheet.TryGetRow(issuer.ENpcBaseId, out var issuerRow))
        continue;

    sb.AppendLine(Indent(2, $"// {issuerRow.Singular}"));
    sb.AppendLine(Indent(2, $"[{issuer.ENpcBaseId}] = ["));

    var leveIds = issuer.Categories.Values
        .SelectMany(cat => cat.Types.Values)
        .SelectMany(type => type.LeveIds)
        .OrderBy(id => id);

    foreach (var leveId in leveIds)
    {
        if (!LeveSheet.TryGetRow(leveId, out var leveRow))
        {
            Console.WriteLine($"Levequest #{leveId} by issuer '{issuerRow.Singular}' ({issuerRow.RowId}) not found");
            continue;
        }

        sb.AppendLine(Indent(3, $"{leveRow.RowId}, // {leveRow.Name}"));
    }

    sb.AppendLine(Indent(2, "],"));
}

sb.AppendLine(Indent(1, "};"));
sb.AppendLine("}");

File.WriteAllText(@"..\..\LeveHelper\Data.cs", sb.ToString());
Console.WriteLine("Data generated!");

public class LeveIssuer
{
    /// <summary>
    /// GuildleveAssignment RowId
    /// </summary>
    public uint GuildleveAssignmentId { get; set; }

    /// <summary>
    /// ENpcBase RowId
    /// </summary>
    public uint ENpcBaseId { get; set; }

    /// <summary>
    /// Level RowId
    /// </summary>
    public uint LevelId { get; set; }

    /// <summary>
    /// Key: GuildleveAssignmentCategory RowId
    /// </summary>
    public Dictionary<uint, LeveAssignmentCategory> Categories { get; set; } = [];
}

public class LeveAssignmentCategory
{
    /// <summary>
    /// GuildleveAssignmentCategory RowId
    /// </summary>
    public uint CategoryId { get; set; }

    /// <summary>
    /// Key: GuildleveAssignmentCategory.Category Index
    /// </summary>
    public Dictionary<uint, LeveAssignmentCategoryType> Types { get; set; } = [];
}

public class LeveAssignmentCategoryType
{
    /// <summary>
    /// GuildleveAssignmentCategory.Category Index
    /// </summary>
    public uint CategoryIndex { get; set; }

    /// <summary>
    /// Leve RowIds
    /// </summary>
    public HashSet<ushort> LeveIds { get; set; } = [];
}
