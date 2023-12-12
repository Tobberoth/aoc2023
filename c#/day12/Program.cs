var lines = File.ReadAllLines("input.txt").ToList();
lines = lines.Where(l => l.Length > 5).ToList();
Dictionary<(string line, int group), int> GetAcceptedPositionsMemo = [];
var total = 0;
foreach (var line in lines) {
  var condition = line.Split(' ')[0];
  var groups = line.Split(" ")[1].Split(",").Select(int.Parse).ToList();
  var arrangements = GetArrangements(condition, groups);
  total += arrangements;
  Console.WriteLine(arrangements);
  //Console.WriteLine($"{condition} {string.Join(",", groups)} {arrangements}");
}
Console.WriteLine(total);

int GetArrangements(string line, List<int> groups) {
  if (groups.Count == 1)
    return GetAcceptedPositions(line, groups[0]);

  var first = groups[0];
  var rest = groups[1..];

  // for all accepted positions of first
  var sum = 0;
  var foundFill = false;
  for (var i = 0; i < line.Length; i++) {
    var positionOk = IsPositionOk(line[i..], first);
    if (positionOk) {
      if (i+first+1 <= line.Length) {
        var arrangements = GetArrangements(line[(i+first+1)..], rest);
        if (arrangements > 0 && line[i..(i+first)].Any(c => c == '#'))
          foundFill = true;
        sum += arrangements;
      }
    } else if (foundFill && (i+first+1) < line.Length && line[i+first] == '.') break;
    if (line[i] == '#')
      break; // Found a guaranteed hit, can't go further
  }
  // add results from recursively calling rest of line with rest of groups
  return sum;
}

int GetAcceptedPositions(string line, int group) {
  if (line.Length < group) return 0;

  if (GetAcceptedPositionsMemo.ContainsKey((line, group)))
    return GetAcceptedPositionsMemo[(line, group)];

  if (line.Contains('#')) {
    var validStart = line.LastIndexOf('#')-group+1;
    var validEnd = Math.Min(line.IndexOf('#')+group, line.Length);
    if (validEnd < validStart) return 0;
    var diff = Math.Abs(validStart - validEnd);
    line = line[Math.Max(validStart, 0)..(validStart+diff)];
  }
  if (line == "") return 0;
  if (GetAcceptedPositionsMemo.ContainsKey((line, group)))
    return GetAcceptedPositionsMemo[(line, group)];
  var sum = 0;
  if (IsPositionOk(line, group)) {
    sum++;
  }
  sum += GetAcceptedPositions(line[1..], group);
  GetAcceptedPositionsMemo.Add((line, group), sum);
  return sum;
}

bool IsPositionOk(string line, int group) {
  if (line.Length < group) return false;
  if (line.Length > group && line.Skip(group).Take(1).First() == '#') return false;
  return line.Take(group).All(c => c == '?' || c == '#');
}