var lines = File.ReadAllLines("input.txt").ToList();
var smallmap = Expand(lines);
var galaxies = GetGalaxies(smallmap);
var allCombinations = GenerateAllCombinations(galaxies);
var sum = allCombinations.Select(c => CalculateShortestPath(c.Item1, c.Item2)).Sum();
Console.WriteLine(sum);

(var rows, var columns) = GetOpen(lines);
galaxies = GetGalaxies(lines);
allCombinations = GenerateAllCombinations(galaxies);
var enlargement = 1_000_000 - 1;
var sum2 = allCombinations.Select(c => CalculateEnlargedPath(c.Item1, c.Item2, rows, columns, enlargement)).Sum();
Console.WriteLine(sum2);

long CalculateEnlargedPath((int x, int y) g1, (int x, int y) g2, List<bool> rows, List<bool> columns, int enlargement) {
  var xDiff = g1.x - g2.x;
  var xEnargement = columns[Math.Min(g1.x, g2.x)..Math.Max(g1.x, g2.x)].Count(c => c == false) * enlargement;
  var yDiff = g1.y - g2.y;
  var yEnargement = rows[Math.Min(g1.y, g2.y)..Math.Max(g1.y, g2.y)].Count(c => c == true) * enlargement;
  return Math.Abs(xDiff) + xEnargement + Math.Abs(yDiff) + yEnargement;
}

(List<bool> Rows, List<bool> Columns) GetOpen(List<string> map) {
  List<bool> Rows = [];
  List<bool> Columns = [];
  for (var x = 0; x < map[0].Length; x++)
    Columns.Add(false);
  for (var y = 0; y < map.Count; y++) {
    var foundGalaxy = false;
    for (var x = 0; x < map[0].Length; x++) {
      if (map[y][x] == '#') {
        foundGalaxy = true;
        Columns[x] = true;
      }
    }
    Rows.Add(!foundGalaxy);
  }
  return (Rows, Columns);
}

List<string> Expand(List<string> lines) {
  List<bool> Columns = [];
  for (var i = 0; i < lines[1].Length; i++)
    Columns.Add(false);
  List<string> ret = [];
  foreach (var line in lines) {
    bool foundGalaxy = false;
    for (var x = 0; x < line.Length; x++) {
      if (line[x] == '#') {
        Columns[x] = true;
        foundGalaxy = true;
      }
    }

    if (!foundGalaxy) {
      ret.Add(line);
      ret.Add(line);
    } else
      ret.Add(line);
  }

  for (var x = lines[0].Length-1; x >= 0; x--) {
    if (!Columns[x]) {
      ret = ret.Select(l => l.Insert(x, ".")).ToList();
    }
  }

  return ret;
}

List<(int x, int y)> GetGalaxies(List<string> map) {
  List<(int x, int y)> ret = [];
  for (var y = 0; y < map.Count; y++) {
    for (var x = 0; x < map[0].Length; x++) {
      if (map[y][x] == '#') {
        ret.Add((x, y));
      }
    }
  }
  return ret;
}

List<((int x, int y), (int x, int y))> GenerateAllCombinations(List<(int x, int y)> input) {
  List<((int x, int y), (int x, int y))> ret = [];
  if (input.Count < 1) return ret;
  var first = input[0];
  var rest = input[1..];
  ret.AddRange(rest.Select(g => (first, g)));
  ret.AddRange(GenerateAllCombinations(rest));
  return ret;
}

long CalculateShortestPath((int x, int y) g1, (int x, int y) g2) {
  var x = Math.Abs(g1.x - g2.x);
  var y = Math.Abs(g1.y - g2.y);
  return x + y;
}