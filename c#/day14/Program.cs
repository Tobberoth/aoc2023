Dictionary<string, (List<List<char>> newMap, int load)> MapToLoad = [];
const long CYCLE_AMOUNT = 1_000_000_000;
var Map = File.ReadAllLines("input.txt").Select(l => l.ToList()).ToList();
for (int c = 0; c < CYCLE_AMOUNT; c++) {
  Cycle(ref Map);
}
PrintMap(Map);
Console.WriteLine(MapToLoad.Count);
Console.WriteLine(CalculateLoad(Map));

int CalculateLoad(List<List<char>> map) {
  return GetAllRocks(map, "north").ToList().Select(c => Math.Abs(c.y - map.Count)).Sum();
}

void PrintMap(List<List<char>> map) {
  var lines = map.Select(l => string.Join("", l));
  foreach (var line in lines)
    Console.WriteLine(line);
}

IEnumerable<(int x, int y)> GetAllRocks(List<List<char>> map, string dir) {
  // Must grab rocks in order depending on direction
  for (var y = 0; y < map.Count; y++) {
    for (var x = 0; x < map[0].Count; x++) {
    }
  }
  switch (dir) {
    case "north":
      for (var y = 0; y < map.Count; y++) {
        for (var x = 0; x < map[0].Count; x++) {
          if (map[y][x] == 'O')
            yield return (x, y);
        }
      }
      break;
    case "south":
      for (var y = map.Count-1; y >= 0; y--) {
        for (var x = 0; x < map[0].Count; x++) {
          if (map[y][x] == 'O')
            yield return (x, y);
        }
      }
      break;
    case "west":
      for (var x = 0; x < map[0].Count; x++) {
        for (var y = map.Count-1; y >= 0; y--) {
          if (map[y][x] == 'O')
            yield return (x, y);
        }
      }
      break;
    case "east":
      for (var x = map[0].Count-1; x >= 0; x--) {
        for (var y = map.Count-1; y >= 0; y--) {
          if (map[y][x] == 'O')
            yield return (x, y);
        }
      }
      break;
    default:
      throw new InvalidOperationException("Not a valid direction");
  }
}

long Cycle(ref List<List<char>> map) {
  var northLoad = 0;
  var mapstring = string.Join("|", map.Select(l => string.Join("", l)));
  if (MapToLoad.ContainsKey(mapstring)) {
    map = MapToLoad[mapstring].newMap;
    return MapToLoad[mapstring].load;
  }
  RollNorth(map);
  RollWest(map);
  RollSouth(map);
  RollEast(map);
  northLoad = CalculateLoad(map);
  MapToLoad.Add(mapstring, ([..map], northLoad));
  return northLoad;
}

void RollDir(List<List<char>> map, (int x, int y) rock, int deltaX, int deltaY) {
  while (true) {
    if (!SpotEmpty(map, rock.x + deltaX, rock.y + deltaY))
      break;
    map[rock.y + deltaY][rock.x + deltaX] = 'O';
    map[rock.y][rock.x] = '.';
    rock = (rock.x + deltaX, rock.y + deltaY);
  }
}

void RollNorth(List<List<char>> map) {
  foreach (var rock in GetAllRocks(map, "north"))
    RollDir(map, rock, 0, -1);
}
void RollEast(List<List<char>> map) {
  foreach (var rock in GetAllRocks(map, "east"))
    RollDir(map, rock, 1, 0);
}
void RollSouth(List<List<char>> map) {
  foreach (var rock in GetAllRocks(map, "south"))
    RollDir(map, rock, 0, 1);
}
void RollWest(List<List<char>> map) {
  foreach (var rock in GetAllRocks(map, "west"))
    RollDir(map, rock, -1, 0);
}

bool SpotEmpty(List<List<char>> map, int x, int y) {
  return y > -1 && y < map.Count && x > -1 && x < map[0].Count && map[y][x] == '.';
}