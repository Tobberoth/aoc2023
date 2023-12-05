var lines = File.ReadAllLines("input.txt").ToList();
var seeds = lines[0].Split(' ')[1..].Select(long.Parse).ToList();
List<(long, long)> seedRange = [];
for (int i = 0; i < seeds.Count(); i += 2) {
  seedRange.Add((seeds[i], seeds[+1]));
}

List<List<MapRange>> Maps = new List<List<MapRange>>();
List<MapRange> currentMap = null;
foreach (var line in lines.Skip(1)) {
  if (line == string.Empty) continue;
  if (!char.IsDigit(line[0])) {
    currentMap = new();
    Maps.Add(currentMap);
  }
  else {
    if (line.Split(' ').Select(long.Parse).ToArray() is [long destStart, long sourceStart, long length])
      currentMap.Add(new MapRange(destStart, sourceStart, length));
  }
}

// TESTS
Maps.Reverse();
//MapRange prevLow = null;
var first = true;
for (var i = 0; i < Maps.Count; i++) {
  // Find lowest range
  if (first) {
    //prevLow = map.OrderBy(t => t.DestinationStart).First();
    Maps[i].RemoveAll(r => r != Maps[i].OrderBy(t => t.DestinationStart).First());
    first = false;
    continue;
  }
  List<MapRange> overlaps = [];
  foreach (var prev in Maps[i-1]) {
    overlaps.AddRange(Maps[i].Where(t => IsOverlap(t, prev)));
  }
  Maps[i].RemoveAll(r => !overlaps.Contains(r));
}
Maps.Reverse();

// Step 1
/*
List<long> locations = [];
long currentVal = 0;
foreach (var seed in seeds) {
  currentVal = seed;
  foreach (var map in Maps) {
    currentVal = Translate(currentVal, map);
  }
  locations.Add(currentVal);
}
*/

// Step 2

var minSeed = Maps[0].Min(t => t.SourceStart);
var maxSeed = Maps[0].Max(t => t.SourceStart+t.Length);

List<long> locations = [];
long currentVal = 0;
foreach (var sr in seedRange.Where(s => s.Item1 <= maxSeed && s.Item2 >= minSeed)) {
  var start = Math.Max(sr.Item1, minSeed);
  var end = Math.Min(sr.Item2, maxSeed);
  foreach (var seed in Enumerable.Range((int)start, (int)start-(int)end)) {
    currentVal = seed;
    foreach (var map in Maps) {
      currentVal = Translate(currentVal, map);
    }
    locations.Add(currentVal);
  }
}


Console.WriteLine(locations.Min());

long Translate(long currentVal, List<MapRange> map) {
  foreach (var range in map) {
    if (currentVal >= range.SourceStart && currentVal <= range.SourceStart + range.Length)
      return (currentVal - range.SourceStart) + range.DestinationStart;
  }
  return currentVal;
}

bool IsOverlap(MapRange from, MapRange to) {
  // If both start and end is below dest start false
  if (from.DestinationStart + from.Length < to.SourceStart) return false;
  // if both start and end is above dest end false
  if (from.DestinationStart > to.SourceStart + to.Length) return false;
  return true;
}

record MapRange(long DestinationStart, long SourceStart, long Length);