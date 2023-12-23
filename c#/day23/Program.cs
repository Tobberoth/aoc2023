var Map = File.ReadAllLines("input.txt").ToList();
var startPoint = GetStartPoint(Map);
var endPoint = GetEndPoint(Map);

// Step 1 (Dumb solution, but quick)
List<Walker> DoneWalkers = [];
var firstWalker = new Walker(Map, [startPoint], null, true) {
  WalkedSteps = 1,
  CurrentPos = (startPoint.x, startPoint.y + 1)
};
List<Walker> Walkers = [firstWalker];
while (Walkers.Count > 0) {
  // for each walker
  foreach (var walker in Walkers.ToList()) {
    var adj = walker.GetAdjacents().ToList();
    if (adj.Count == 0) {
      Walkers.Remove(walker); // Stuck
    }
    if (adj.Count == 1) {
      walker.WalkedSteps++;
      walker.CurrentPos = adj[0];
      walker.Visited.Add(walker.CurrentPos);
      if (adj[0] == endPoint) {
        Walkers.Remove(walker);
        DoneWalkers.Add(walker);
      }
    } else {
      // multiply
      Walkers.Remove(walker);
      foreach (var a in adj) {
        var newWalker = new Walker(Map, [.. walker.Visited], null, true) {
            WalkedSteps = walker.WalkedSteps + 1,
            CurrentPos = a
        };
        newWalker.Visited.Add(newWalker.CurrentPos);
        Walkers.Add(newWalker);
        if (a == endPoint) {
          Walkers.Remove(newWalker);
          DoneWalkers.Add(newWalker);
          break;
        }
      }
    }
  }
}
Console.WriteLine(DoneWalkers.Max(w => w.WalkedSteps));

// Step 2 (Slow because of no memoization, but fine)
var graph = BuildGraph(Map, startPoint, endPoint);
Console.WriteLine(GetLongestWalk(graph, [], startPoint, endPoint));

long GetLongestWalk(Dictionary<(int x, int y), Node> graph, HashSet<(int x, int y)> Visited, (int x, int y) current, (int x, int y) end) {
  if (current == end) return 0;
  var currentNode = graph[current];
  List<long> Distances = [];
  foreach (var adj in currentNode.Adjacents.Values) {
    var adjNode = adj.Item1;
    var distance = adj.Item2;
    if (Visited.Contains(adjNode.Coord)) continue;
    Distances.Add(distance + GetLongestWalk(graph, [..Visited, currentNode.Coord], adjNode.Coord, end));
  }
  if (!Distances.Any())
    return 0;
  return Distances.Max();
}

(int x, int y) GetFirstOpenPoint(List<string> map, int y) {
  for (var x = 0; x < map[0].Length; x++) {
    if (map[y][x] == '.')
      return (x, y);
  }
  throw new InvalidOperationException("Couldn't find open point");
}

(int x, int y) GetStartPoint(List<string> map) =>
  GetFirstOpenPoint(map, 0);

(int x, int y) GetEndPoint(List<string> map) =>
  GetFirstOpenPoint(map, map.Count-1);

Dictionary<(int x, int y), Node> BuildGraph(List<string> map, (int x, int y) startPoint, (int x, int y) endPoint) {
  Dictionary<(int x, int y), Node> AllNodes = [];
  var startNode = new Node(startPoint.x, startPoint.y);
  AllNodes.Add((startNode.X, startNode.Y), startNode);
  var endNode = new Node(endPoint.x, endPoint.y);
  AllNodes.Add((endNode.X, endNode.Y), endNode);

  var firstWalker = new Walker(map, [startPoint], startNode) {
    WalkedSteps = 1,
    WentDirection = Directions.DOWN,
    CurrentPos = (startPoint.x, startPoint.y + 1)
  };
  List<Walker> Walkers = [firstWalker];
  while (Walkers.Count > 0) {
    // for each walker
    foreach (var walker in Walkers.ToList()) {
      var adj = walker.GetAdjacents().ToList();
      if (adj.Count == 0) {
        Walkers.Remove(walker); // Stuck
        walker.StartNode.Adjacents.Add(walker.WentDirection, (null, -1)); // this path is meaningless
        continue;
      }
      if (adj.Count == 1) {
        walker.WalkedSteps++;
        walker.PreviousPos = walker.CurrentPos;
        walker.Visited.Add(walker.CurrentPos);
        walker.CurrentPos = adj[0];
        if (adj[0] == endPoint) {
          walker.StartNode.Adjacents.Add(walker.WentDirection, (endNode, walker.WalkedSteps));
          endNode.Adjacents.Add(Directions.UP, (walker.StartNode, walker.WalkedSteps));
          Walkers.Remove(walker);
        }
      } else {
        // multiply
        if (AllNodes.ContainsKey((walker.CurrentPos.x, walker.CurrentPos.y))) {
          // node already calculated
          var targetNode = AllNodes[(walker.CurrentPos.x, walker.CurrentPos.y)];
          var cameFrom = walker.PreviousPos switch {
            _ when walker.PreviousPos == (walker.CurrentPos.x, walker.CurrentPos.y - 1) => Directions.UP,
            _ when walker.PreviousPos == (walker.CurrentPos.x, walker.CurrentPos.y + 1) => Directions.DOWN,
            _ when walker.PreviousPos == (walker.CurrentPos.x - 1, walker.CurrentPos.y) => Directions.LEFT,
            _ when walker.PreviousPos == (walker.CurrentPos.x + 1, walker.CurrentPos.y) => Directions.RIGHT,
          };
          if (!targetNode.Adjacents.ContainsKey(cameFrom)) {
            targetNode.Adjacents.Add(cameFrom, (walker.StartNode, walker.WalkedSteps));
            walker.StartNode.Adjacents.Add(walker.WentDirection, (targetNode, walker.WalkedSteps));
          }
          Walkers.Remove(walker);
        } else {
          // Add how I came here
          var newNode = new Node(walker.CurrentPos.x, walker.CurrentPos.y);
          AllNodes.Add(walker.CurrentPos, newNode);
          // where did I come from
          var cameFrom = walker.PreviousPos switch {
            _ when walker.PreviousPos == (walker.CurrentPos.x, walker.CurrentPos.y - 1) => Directions.UP,
            _ when walker.PreviousPos == (walker.CurrentPos.x, walker.CurrentPos.y + 1) => Directions.DOWN,
            _ when walker.PreviousPos == (walker.CurrentPos.x - 1, walker.CurrentPos.y) => Directions.LEFT,
            _ when walker.PreviousPos == (walker.CurrentPos.x + 1, walker.CurrentPos.y) => Directions.RIGHT,
          };
          newNode.Adjacents.Add(cameFrom, (walker.StartNode, walker.WalkedSteps));
          walker.StartNode.Adjacents.Add(walker.WentDirection, (newNode, walker.WalkedSteps));
          // Send out new walkers from this node
          Walkers.Remove(walker);
          foreach (var adjPos in adj) {
            var going = adjPos switch {
              _ when adjPos == (walker.CurrentPos.x, walker.CurrentPos.y - 1) => Directions.UP,
              _ when adjPos == (walker.CurrentPos.x, walker.CurrentPos.y + 1) => Directions.DOWN,
              _ when adjPos == (walker.CurrentPos.x - 1, walker.CurrentPos.y) => Directions.LEFT,
              _ when adjPos == (walker.CurrentPos.x + 1, walker.CurrentPos.y) => Directions.RIGHT,
            };
            if (newNode.Adjacents.ContainsKey(going)) continue;
            var newWalker = new Walker(map, [walker.CurrentPos], newNode) { CurrentPos = adjPos };
            newWalker.WalkedSteps++;
            newWalker.WentDirection = going;
            Walkers.Add(newWalker);
          }
        }
      }
    }
  }
  return AllNodes;
}

public class Walker(List<string> Map, HashSet<(int x, int y)> Visited, Node StartNode, bool StoppedBySlopes = false) {
  public HashSet<(int x, int y)> Visited { get; set; } = Visited;
  public Node StartNode { get; set; } = StartNode;
  public Directions WentDirection { get; set; }
  public int WalkedSteps { get; set; }
  public (int x, int y) PreviousPos { get; set; }
  public (int x, int y) CurrentPos { get; set; }
  char[] walkableAll = ['.', '>', '<', 'v', '^'];
  char[] walkableUp = ['.', '>', '<', '^'];
  char[] walkableDown = ['.', '>', '<', 'v'];
  char[] walkableLeft = ['.', '<', 'v', '^'];
  char[] walkableRight = ['.', '>', 'v', '^'];
  public IEnumerable<(int x, int y)> GetAdjacents() {
    var right = (x: CurrentPos.x+1, CurrentPos.y);
    var left = (x: CurrentPos.x-1, CurrentPos.y);
    var up = (CurrentPos.x, y: CurrentPos.y-1);
    var down = (CurrentPos.x, y: CurrentPos.y+1);
    if (!StoppedBySlopes) {
      if (!Visited.Contains(right) && walkableAll.Contains(Map[right.y][right.x])) yield return right;
      if (!Visited.Contains(left) && walkableAll.Contains(Map[left.y][left.x])) yield return left;
      if (!Visited.Contains(up) && walkableAll.Contains(Map[up.y][up.x])) yield return up;
      if (!Visited.Contains(down) && walkableAll.Contains(Map[down.y][down.x])) yield return down;
    } else {
      if (!Visited.Contains(right) && walkableRight.Contains(Map[right.y][right.x])) yield return right;
      if (!Visited.Contains(left) && walkableLeft.Contains(Map[left.y][left.x])) yield return left;
      if (!Visited.Contains(up) && walkableUp.Contains(Map[up.y][up.x])) yield return up;
      if (!Visited.Contains(down) && walkableDown.Contains(Map[down.y][down.x])) yield return down;
    }
  }
}


public class Node(int X, int Y) {
  public int X { get; set; } = X;
  public int Y { get; set; } = Y;
  public (int x, int y) Coord { get; set; }= (X, Y);
  public Dictionary<Directions, (Node, int)> Adjacents { get; set; } = [];
}

public enum Directions { UP, DOWN, LEFT, RIGHT };