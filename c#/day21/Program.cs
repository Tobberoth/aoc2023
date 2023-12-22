Console.WriteLine(GetPossibleDestinations("input.txt", (65, 65), 1000));
return;

(long even, long uneven) GetPossibleDestinations(string filePath, (int x, int y) startLoc, int steps) {
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
      foreach (var adjacent in GetAdjacent(currentLoc, mapWidth, mapHeight)) {
        if (!Visited.Contains(adjacent) && map[adjacent.y][adjacent.x] == '.') {
          Visited.Add(adjacent);
          LocsToCheck.Enqueue(adjacent);
        }
      }
      if (step % 2 == 0 && (currentLoc.x + currentLoc.y) % 2 == 0)
        ret = (++ret.even, ret.uneven);
      else if (step % 2 == 1 && (currentLoc.x + currentLoc.y) % 2 == 1)
        ret = (ret.even, ++ret.uneven);
    }
    step++;
  }
  return ret;
}

IEnumerable<(int x, int y)> GetAdjacent((int x, int y) currentLoc, int mapWidth, int mapHeight) {
  if (currentLoc.y - 1 >= 0) yield return (currentLoc.x, currentLoc.y-1);
  if (currentLoc.y + 1 < mapHeight) yield return (currentLoc.x, currentLoc.y+1);
  if (currentLoc.x - 1 >= 0) yield return (currentLoc.x-1, currentLoc.y);
  if (currentLoc.x + 1 < mapWidth) yield return (currentLoc.x+1, currentLoc.y);
}