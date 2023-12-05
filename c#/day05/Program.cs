var lines = File.ReadAllLines("input.txt").ToList();
var seeds = lines[0].Split(' ')[1..].Select(long.Parse).ToList();
List<List<MapRange>> Maps = [];
List<MapRange> currentMap = null;
foreach (var line in lines.Skip(1)) {
  if (line == string.Empty) continue;
  if (!char.IsDigit(line[0])) {
    currentMap = new();
    Maps.Add(currentMap);
  } else {
    if (line.Split(' ').Select(long.Parse).ToArray() is [long destStart, long sourceStart, long length])
      currentMap.Add(new MapRange(destStart, sourceStart, length));
  }
}

// STEP 1
List<long> locations = [];
long currentVal = 0;
foreach (var seed in seeds) {
  currentVal = seed;
  foreach (var map in Maps) {
    currentVal = Translate(currentVal, map);
  }
  locations.Add(currentVal);
}
Console.WriteLine("Step 1: " + locations.Min());

// STEP 2
List<(long, long)> seedRange = [];
for (int i = 0; i < seeds.Count; i += 2) {
  seedRange.Add((seeds[i], seeds[i+1]));
}
for (var i = 0; i < Maps.Count; i++) {
  Maps[i] = [.. Maps[i].OrderBy(m => m.SourceStart)];
}
Dictionary<(long, long, int), long> MemoSolve = [];
Console.WriteLine("Step 2: " + seedRange.Select(sr => SolveRange(sr.Item1, sr.Item1 + sr.Item2, 0)).Min());

long Translate(long currentVal, List<MapRange> map) {
  foreach (var range in map) {
    if (currentVal >= range.SourceStart && currentVal <= range.SourceStart + range.Length)
      return currentVal - range.SourceStart + range.DestinationStart;
  }
  return currentVal;
}

long SolveRange(long start, long end, int nextMap) {
  if (MemoSolve.ContainsKey((start, end, nextMap)))
    return MemoSolve[(start, end, nextMap)];
  if (nextMap >= Maps.Count)
    return start;
  long minOut = 1_000_000_000;
  var ranges = MapAllRanges(start, end, nextMap).ToList();
  foreach (var r in ranges) {
    minOut = Math.Min(minOut, SolveRange(r.start, r.end, nextMap + 1));
  }
  MemoSolve.Add((start, end, nextMap), minOut);
  return minOut;
}

// Apply range to ALL MapRanges in map, and get a final complete list of valid ranges
IEnumerable<(long start, long end)> MapAllRanges(long start, long end, int mapIndex) {
  var currentStart = start;
  foreach (var mr in Maps[mapIndex]) {
    if (currentStart > mr.SourceStart && currentStart < mr.SourceStart + mr.Length) {
      // Check if going past
      var diff = currentStart - mr.SourceStart;
      if (end > mr.SourceStart + mr.Length) {
        // Return full map
        yield return (mr.DestinationStart+diff, mr.DestinationStart+mr.Length);
      } else {
        // return partial map
        var endDiff = mr.SourceStart + mr.Length - end;
        yield return (mr.DestinationStart+diff, (mr.DestinationStart + mr.Length) - endDiff);
        yield break;
      }
      currentStart = mr.SourceStart + mr.Length;
      continue;
    }
    if (currentStart < mr.SourceStart) {
      if (mr.SourceStart > end) {
        // Done
        yield return (currentStart, end);
        yield break;
      }
      yield return (currentStart, mr.SourceStart-1);
      currentStart = mr.SourceStart;
    }
    if (currentStart == mr.SourceStart) {
      // Check if going past
      if (end > mr.SourceStart + mr.Length) {
        // Return full map
        yield return (mr.DestinationStart, mr.DestinationStart+mr.Length);
      } else {
        // return partial map
        yield return (mr.DestinationStart, end - currentStart);
        yield break;
      }
      currentStart = mr.SourceStart + mr.Length;
    }
  }
  yield return (currentStart, end);
}

record MapRange(long DestinationStart, long SourceStart, long Length);