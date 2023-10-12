import * as path from "https://deno.land/std@0.203.0/path/mod.ts";
import * as fs from "https://deno.land/std@0.203.0/fs/mod.ts";
import * as cheerio from "https://esm.sh/cheerio@0.22.0";

const root = "F:\\cs\\ffxiv\\Tools\\ExdExport\\bin\\Debug\\net7.0-windows\\out\\en\\";

const issuers = [
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
];

interface ExcelRow {
  "@rowId": number;
  "@subRowId": number;
}

interface ExcelRowReference {
  "@type": string;
  "@rowId": number;
}

class ExcelSheet<ExcelRowType extends ExcelRow> {
  rows: ExcelRowType[];

  constructor(name: string) {
    this.rows = JSON.parse(new TextDecoder().decode(Deno.readFileSync(path.join(root, `${name}.json`)))).rows;
  }

  getRow(rowId: number) {
    return this.rows.find(row => row["@rowId"] == rowId);
  }

  getSubRow(rowId: number, subRowId: number) {
    return this.rows.find(row => row["@rowId"] == rowId && row["@subRowId"] == subRowId);
  }

  findRow(fn: (row: ExcelRow) => boolean) {
    return this.rows.find(fn);
  }
}

interface ENpcResidentRow extends ExcelRow {
  Singular: string;
}

interface LeveRow extends ExcelRow {
  Name: string;
  PlaceNameIssued: ExcelRow;
}

class LeveExd extends ExcelSheet<LeveRow> {
  findByName(name: string) {
    switch (name) {
      case "Blood in the Water (20)":
        return this.getRow(804);
      case "Blood in the Water (68)":
        return this.getRow(1400);
      case "Perhaps Not-So-Common": // Perhaps Not-so-common
        return this.getRow(1393);
      case "An Historical Flavor": // A Historical Flavor
        return this.getRow(1644);
    }
    return this.rows.find(row => row.Name == name);
  }
}

const ENpcResident = new ExcelSheet<ENpcResidentRow>("ENpcResident");
const Leve = new LeveExd("Leve");

const fetchLevesFromIssuer = async (name: string) => {
  try { Deno.mkdirSync("cache"); } catch {}

  const cacheFileName = path.join(Deno.cwd(), "cache", `${name}.html`);

  let body = "";

  if (fs.existsSync(cacheFileName)) {
    body = new TextDecoder().decode(Deno.readFileSync(cacheFileName));
  } else {
    const slug = name.replaceAll(" ", "_").replaceAll("'", "%27");
    const res = await fetch(`https://ffxiv.gamerescape.com/wiki/${slug}`);
    body = await res.text();
    Deno.writeFileSync(cacheFileName, new TextEncoder().encode(body));
  }

  const $ = cheerio.load(body);

  const leves = $("[id='Starts_Levequests']").parent().parent().parent().parent().find(".tabber .tabbertab .tabbertab table tr");
  const leveNames = new Set<string>();
  for (let i = 1; i < leves.length; i++) {
    const title = $(leves[i]).find("td:first-child > a").text().replaceAll("(Levequest)", "").trim();
    if (!title)
      continue;
    leveNames.add(title);
  }

  return Array.from(leveNames).map((name) => [Leve.findByName(name), name]) as [LeveRow, string][];
}

const parts = [];
for (const issuer of issuers.map(id => ENpcResident.getRow(id))) {
  const leves = await fetchLevesFromIssuer(issuer!.Singular);
  const lines = [];
  for (const [leve, name] of leves) {
    lines.push(`            ${leve?.["@rowId"]}, // ${name}`);
  }

  parts.push(`        // ${issuer?.Singular}
        [${issuer?.["@rowId"]}] = new uint[] {
${lines.join("\n")}
        },`)
}

const cs = `using System.Collections.Generic;

namespace LeveHelper;

public static class Data
{
    public static readonly Dictionary<uint, uint[]> Issuers = new()
    {
${parts.join("\n")}
    };
}
`;

Deno.writeFileSync("Data.cs", new TextEncoder().encode(cs));