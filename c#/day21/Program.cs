using System.Collections.ObjectModel;

var Map = File.ReadAllLines("input.txt").ToList();
const int stepsToTake = 64;
(int x, int y, int stepsTaken) CurrentPosition = GetSPos(Map);
HashSet<(int x, int y)> Reached = [(CurrentPosition.x, CurrentPosition.y)];
Queue<(int x, int y, int stepsTaken)> PositionsToCheck = [];
PositionsToCheck.Enqueue(CurrentPosition);
while (true) {
  CurrentPosition = PositionsToCheck.Dequeue();
  if (CurrentPosition.stepsTaken > stepsToTake-1) break;
  var valids = GetValidAdjacent(Reached, Map, (CurrentPosition.x, CurrentPosition.y));

  foreach (var valid in valids) {
    Reached.Add(valid);
    PositionsToCheck.Enqueue((valid.x, valid.y, CurrentPosition.stepsTaken + 1));
  }
}
// Like a chess board, reachable positions toggle each step, get only valid ones
Console.WriteLine(Reached.Where(r => (r.x + r.y) % 2 == 0).Count());

(int x, int y, int stepsTaken) GetSPos(List<string> map) {
  for (var y = 0; y < map.Count; y++) {
    for (var x = 0; x < map[0].Length; x++) {
      if (map[y][x] == 'S')
        return (x, y, 0);
    }
  }
  throw new InvalidOperationException("Could not find S position in input");
}

Collection<(int x, int y)> GetValidAdjacent(HashSet<(int x, int y)> reached, List<string> map, (int x, int y) fromPos) {
  Collection<(int x, int y)> ret = [];
  // Check up
  var up = (fromPos.x, y: fromPos.y-1);
  if (!reached.Contains(up) && map[up.y][up.x] == '.')
    ret.Add(up);
  // Check right
  var right = (x: fromPos.x+1, fromPos.y);
  if (!reached.Contains(right) && map[right.y][right.x] == '.')
    ret.Add(right);
  // check down
  var down = (fromPos.x, y: fromPos.y+1);
  if (!reached.Contains(down) && map[down.y][down.x] == '.')
    ret.Add(down);
  // check left
  var left = (x: fromPos.x-1, fromPos.y);
  if (!reached.Contains(left) && map[left.y][left.x] == '.')
    ret.Add(left);
  return ret;
}