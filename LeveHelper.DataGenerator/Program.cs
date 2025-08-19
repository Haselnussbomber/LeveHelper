using System.Text;
using HtmlAgilityPack;
using Lumina;
using Lumina.Excel.Sheets;
using Lumina.Extensions;

var issuers = new uint[] {
    1000970, // T'mokkri, Limsa Lominsa Upper Decks (11, 11)
    1000101, // Gontrant, New Gridania (11, 13)
    1001794, // Eustace, Ul'dah-Steps of Nald (1, -9)
    1004342, // Wyrkholsk, Lower La Noscea (31, 20)
    1001866, // Muriaule, Central Shroud (23, 19)
    1003888, // Graceful Song, Western Thanalan (26, 24)
    1001788, // Swygskyf, Western La Noscea (34, 31)
    1000105, // Tierney, Central Shroud (22, 22)
    1001796, // Totonowa, Western Thanalan (23, 16)
    1001791, // Orwen, Western La Noscea (27, 27)
    1000821, // Qina Lyehga, East Shroud (17, 27)
    1001799, // Poponagu, Eastern Thanalan (13, 24)
    1004347, // Ourawann, Lower La Noscea (23, 33)
    1004735, // Eugene, Lower La Noscea (23, 34)
    1000823, // Nyell, South Shroud (25, 20)
    1004737, // Cedrepierre, East Shroud (16, 27)
    1002365, // Esmond, Southern Thanalan (18, 13)
    1004739, // Kikiri, Eastern Thanalan (14, 23)
    1004344, // Nahctahr, Eastern La Noscea (30, 30)
    1002397, // Merthelin, South Shroud (16, 28)
    1002367, // Aileen, Eastern La Noscea (21, 21)
    1002384, // Cimeaurant, Coerthas Central Highlands (26, 28)
    1007068, // Haisie, Coerthas Central Highlands (25, 28)
    1004736, // C'lafumyn, Eastern La Noscea (33, 30)
    1004738, // H'amneko, South Shroud (17, 30)
    1004740, // Blue Herring, Southern Thanalan (19, 13)
    1002398, // Rurubana, Northern Thanalan (22, 29)
    1002401, // Voilinaut, Coerthas Central Highlands (12, 16)
    1007069, // Lodille, Coerthas Central Highlands (11, 16)
    1004348, // K'leytai, Mor Dhona (29, 12)
    1007070, // Eidhart, Mor Dhona (30, 12)
    1011208, // Eloin, Foundation (10, 10)
    1018997, // Keltraeng, Kugane (11, 9)
    1027847, // Eirikur, The Crystarium (9, 9)
    1037263, // Grigge, Old Sharlayan (12, 13)
    1048390, // Malihali, Tuliyollal (13, 12)
};

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.UserAgent.Clear();
httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("LeveHelper.DataGenerator/0.0.1");

var gameData = new GameData(args[0]);
var ENpcResidentSheet = gameData.Excel.GetSheet<ENpcResident>()!;
var LeveSheet = gameData.Excel.GetSheet<Leve>()!;

Directory.CreateDirectory("cache");

bool TryFindLeveByName(string title, out Leve leve)
{
    if (title == "Blood in the Water (20)") // => "Blood in the Water"
        return LeveSheet.TryGetRow(804, out leve);

    if (title == "Blood in the Water (68)") // => "Blood in the Water"
        return LeveSheet.TryGetRow(1400, out leve);

    if (title == "An Historical Flavor") // => "A Historical Flavor"
        return LeveSheet.TryGetRow(1644, out leve);

    var _leve = LeveSheet.FirstOrNull(leve => string.Equals(leve.Name.ToString(), title, StringComparison.InvariantCultureIgnoreCase));
    if (_leve.HasValue)
    {
        leve = _leve.Value;
        return true;
    }

    Console.WriteLine($"Leve {title} not found.");
    leve = default;
    return false;
};

string Indent(int level, string line) => $"{new string(' ', level * 4)}{line}";

var sb = new StringBuilder();
sb.AppendLine("using System.Collections.Generic;");
sb.AppendLine();
sb.AppendLine("namespace LeveHelper;");
sb.AppendLine();
sb.AppendLine("public static class Data");
sb.AppendLine("{");
sb.AppendLine(Indent(1, "public static readonly Dictionary<uint, uint[]> Issuers = new()"));
sb.AppendLine(Indent(1, "{"));

foreach (var issuerId in issuers)
{
    if (!ENpcResidentSheet.TryGetRow(issuerId, out var issuer))
        continue;

    var name = issuer.Singular.ToString();
    var body = string.Empty;
    var cacheFile = $"cache/{name}.html";

    if (File.Exists(cacheFile))
    {
        body = File.ReadAllText(cacheFile);
    }
    else
    {
        var url = $"https://ffxiv.gamerescape.com/wiki/{name}";
        Console.WriteLine($"Fetching {url}");
        body = await httpClient.GetStringAsync(url);
        File.WriteAllText(cacheFile, body);
    }

    var doc = new HtmlDocument();
    doc.LoadHtml(body);

    var levequestNodes = doc.GetElementbyId("Starts_Levequests")?
        .SelectNodes("../../../../tr/td/div/div/div/div/table[@class='GEtable sortable']/tbody/tr")?
        .Where(node => node.SelectSingleNode(".//td[1]/a") != null)?
        .ToList() ?? [];

    sb.AppendLine(Indent(2, $"// {name}"));
    sb.AppendLine(Indent(2, $"[{issuerId}] = ["));

    var leveNames = new HashSet<string>(); // for uniqueness

    foreach (var levequestNode in levequestNodes)
    {
        leveNames.Add(levequestNode.SelectSingleNode(".//td[1]/a")!.InnerText.Replace(" (Levequest)", ""));
    }

    foreach (var leveName in leveNames)
    {
        if (!TryFindLeveByName(leveName, out var leve))
        {
            Console.WriteLine($"Levequest '{leveName}' by issuer '{leveName}' ({issuerId}) not found");
            continue;
        }

        if (issuerId == 1004739 && leve.RowId == 1400) // Kikiri does not issue "Blood in the Water" for level 68
            continue;

        sb.AppendLine(Indent(3, $"{leve.RowId}, // {leveName}"));
    }


    if (issuerId == 1018997) // Keltraeng issues "Blood in the Water" for level 68
        sb.AppendLine(Indent(3, $"1400, // {LeveSheet.GetRow(1400)!.Name}"));

    sb.AppendLine(Indent(2, "],"));
}

sb.AppendLine(Indent(1, "};"));
sb.AppendLine("}");

File.WriteAllText(@"..\..\LeveHelper\Data.cs", sb.ToString());
Console.WriteLine("Data generated!");
