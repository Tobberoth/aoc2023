List<List<string>> Map = new(10000) { new(10000) };
Map[0].Add("#");
var fromPos = (x: 0, y: 0);
foreach (var line in File.ReadAllLines("input.txt")) {
  var dir = line[0];
  var steps = int.Parse(line.Split(' ')[1]);
  var rgb = line.Split(' ')[2].TrimStart('(').TrimEnd(')');
  fromPos = Draw(Map, dir, steps, rgb, fromPos.x, fromPos.y);
}
Fill(Map);
Console.WriteLine(Map.Select(l => l.Count(c => c != "")).Sum());

(int x, int y) Draw(List<List<string>> map, char direction, int steps, string rgb, int fromX, int fromY) {
  switch (direction) {
    case 'U':
      for (var i = 0; i < steps; i++) {
        var rowY = fromY - 1;
        if (rowY < 0) {
          Map.Insert(0, new List<string>(10000));
          Map[0] = Map[1].Select(s => "").ToList();
          rowY++;
          Map[rowY][fromX] = rgb;
        } else {
          Map[rowY][fromX] = rgb;
          fromY--;
        }
      }
      break;
    case 'R':
      for (var i = 0; i < steps; i++) {
        var colX = fromX + 1;
        if (colX >= Map[0].Count) {
          Map.ForEach(l => l.Add(""));
          Map[fromY][colX] = rgb;
        } else
          Map[fromY][colX] = rgb;
        fromX++;
      }
      break;
    case 'D':
      for (var i = 0; i < steps; i++) {
        var rowY = fromY + 1;
        if (rowY >= Map.Count) {
          var newList = Map.Last().Select(s => "").ToList();
          Map.Add(newList);
          Map[rowY][fromX] = rgb;
        } else 
          Map[rowY][fromX] = rgb;
        fromY++;
      }
      break;
    case 'L':
      for (var i = 0; i < steps; i++) {
        var colX = fromX - 1;
        if (colX < 0) {
          Map.ForEach(l => l.Insert(0, ""));
          colX = 0;
          Map[fromY][colX] = rgb;
        } else {
          Map[fromY][colX] = rgb;
          fromX--;
        }
      }
    break;
  }
  return (fromX, fromY);
}

void Fill(List<List<string>> map) {
  // Find point to start filling from
  var height = map.Count;
  var width = map[0].Count;
  var midY = height/2 - 1;
  if (height % 2 > 0)
    midY++;
  var insideX = 0;
  for (var x = 0; x < width; x++) {
    var cell = map[midY][x];
    if (cell != "") {
      insideX = x+1;
      break;
    }
  }
  
  HashSet<(int x, int y)> PositionsToFill = [(insideX, midY)];
  while (PositionsToFill.Count > 0) {
    var currentPos = PositionsToFill.First();
    PositionsToFill.Remove(currentPos);
    // Get all adjacent
    if (map[currentPos.y-1][currentPos.x] == "")
      PositionsToFill.Add((currentPos.x, currentPos.y-1));
    if (map[currentPos.y+1][currentPos.x] == "")
      PositionsToFill.Add((currentPos.x, currentPos.y+1));
    if (map[currentPos.y][currentPos.x-1] == "")
      PositionsToFill.Add((currentPos.x-1, currentPos.y));
    if (map[currentPos.y][currentPos.x+1] == "")
      PositionsToFill.Add((currentPos.x+1, currentPos.y));
    // Fill
    map[currentPos.y][currentPos.x] = "#FFFFFF";
  }
  Console.WriteLine("Filling done");
}