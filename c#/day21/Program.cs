var maxsteps = 26501365; // Steps to walk
var boxWidth = 131; // Size of graph
var lengthFromMid = (boxWidth - 1) / 2;
long shortSide = ((maxsteps - lengthFromMid) / boxWidth)-1;
long longSide = shortSide + 1;

// CALCULATION
// Using precalculated numbers
// For smaller steps than target, such as 327, GetPossibleDestinationLooping will print the destinations for each needed section
var evenFilled = 7307;
var unevenFilled = 7274;
long filled = shortSide * shortSide * unevenFilled;
filled += longSide * longSide * evenFilled;
long tips = 5514 + 5470 + 5503 + 5481;
long rightToTop = 919 * longSide + 6391 * shortSide;
long rightToBot = 923 * longSide + 6397 * shortSide;
long leftToTop = 921 * longSide + 6386 * shortSide;
long leftToBot = 938 * longSide + 6358 * shortSide;

// Step 1
Console.WriteLine(GetPossibleDestinationsLooping("input.txt", (65,65), 64));

// Step 2
Console.WriteLine($"Calculated: {tips + filled + rightToTop + rightToBot + leftToTop + leftToBot}");

(long even, long uneven) GetPossibleDestinationsLooping(string filePath, (int x, int y) startLoc, int steps) {
  var map = File.ReadAllLines(filePath).Select(l => l.Replace('S', '.')).ToList();
  var mapWidth = map[0].Length;
  var mapHeight = map.Count;

  HashSet<(int x, int y)> Visited = [startLoc];
  Queue<(int x, int y)> LocsToCheck = [];
  LocsToCheck.Enqueue(startLoc);

  var ret = (even: 0, uneven: 0);
  var step = 0;
  while (step <= steps) {
    var queueSize = LocsToCheck.Count;
    while (queueSize-- > 0) {
      var currentLoc = LocsToCheck.Dequeue();
      foreach (var adjacent in GetAdjacentLooping(currentLoc, mapWidth, mapHeight)) {
        if (!Visited.Contains(adjacent) && map[(20 * mapHeight + adjacent.y) % mapHeight][(20 * mapWidth + adjacent.x) % mapWidth] == '.') {
          Visited.Add(adjacent);
          LocsToCheck.Enqueue(adjacent);
        }
      }
      if (step % 2 == 0)
        ret = (++ret.even, ret.uneven);
      else if (step % 2 == 1)
        ret = (ret.even, ++ret.uneven);
    }
    step++;
  }
  PrintBoxInteresting(Visited, 0, 0, boxWidth, "CENTER ");
  PrintBoxInteresting(Visited, 131, 0, boxWidth, "NEXT PARITY ");
  PrintBoxInteresting(Visited, maxsteps - lengthFromMid, 0, mapWidth, "RIGHT TIP ");
  PrintBoxInteresting(Visited, -(maxsteps - lengthFromMid), 0, mapWidth, "LEFT TIP ");
  PrintBoxInteresting(Visited, 0, -(maxsteps - lengthFromMid), mapWidth, "TOP TIP ");
  PrintBoxInteresting(Visited, 0, maxsteps - lengthFromMid, mapWidth, "BOTTOM TIP ");
  PrintBoxInteresting(Visited, maxsteps - lengthFromMid, -131, mapWidth, "RT SMALL ");
  PrintBoxInteresting(Visited, maxsteps - lengthFromMid - mapWidth, -131, mapWidth, "RT BIG ");
  PrintBoxInteresting(Visited, maxsteps - lengthFromMid, 131, mapWidth, "RB SMALL ");
  PrintBoxInteresting(Visited, maxsteps - lengthFromMid - mapWidth, 131, mapWidth, "RB BIG ");
  PrintBoxInteresting(Visited, -(maxsteps - lengthFromMid), -131, mapWidth, "LT SMALL ");
  PrintBoxInteresting(Visited, -(maxsteps - lengthFromMid - mapWidth), -131, mapWidth, "LT BIG ");
  PrintBoxInteresting(Visited, -(maxsteps - lengthFromMid), 131, mapWidth, "LB SMALL ");
  PrintBoxInteresting(Visited, -(maxsteps - lengthFromMid - mapWidth), 131, mapWidth, "LB BIG ");
  return ret;
}

void PrintBoxInteresting(HashSet<(int x, int y)> Visited, int X, int Y, int width, string comment = "") {
  width--;
  var minX = Math.Min(X, X+width);
  var maxX = Math.Max(X, X+width);
  var minY = Math.Min(Y, Y+width);
  var maxY = Math.Max(Y, Y+width);
  HashSet<(int x, int y)> Interesting = Visited.Where(t => t.x <= maxX && t.x >= minX && t.y >= minY && t.y <= maxY).ToHashSet();
  Console.WriteLine(comment + Interesting.Count(v => (Math.Abs(v.x)+Math.Abs(v.y)) % 2 == 1));
}

IEnumerable<(int x, int y)> GetAdjacentLooping((int x, int y) currentLoc, int mapWidth, int mapHeight) {
  yield return (currentLoc.x, currentLoc.y-1);
  yield return (currentLoc.x, currentLoc.y+1);
  yield return (currentLoc.x - 1, currentLoc.y);
  yield return (currentLoc.x + 1, currentLoc.y);
}