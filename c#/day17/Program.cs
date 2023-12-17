var lines = File.ReadAllLines("input.txt").Select(l => l.Select(c => c - '0').ToList()).ToList();
var WIDTH = lines[0].Count;
var HEIGHT = lines.Count;
var startState = new State(0, 0, FromDirection.NONE, 0);
HashSet<State> Visited = [];
Dictionary<State, int> StateLog = [];
StateLog.Add(startState, 0);
PriorityQueue<State, int> Unvisited = new();
Unvisited.Enqueue(startState, 0);

var done = false;
while (!done) {
  var current = Unvisited.Dequeue();
  var currentHeat = StateLog[current];
  foreach (var adj in GetAdjacent(current)) {
    if (Visited.Contains(adj))
      continue;
    var newHeat = currentHeat + lines[adj.Y][adj.X];
    if (StateLog.ContainsKey(adj)) {
      if (StateLog[adj] > newHeat) {
        StateLog[adj] = newHeat;
      } else {
        continue;
      }
    } else {
      StateLog.Add(adj, newHeat);
    }

    if (adj.X == WIDTH-1 && adj.Y == HEIGHT-1) {
      Console.WriteLine(newHeat);
      done = true;
      break;
    }
    Unvisited.Enqueue(adj, newHeat);
  }
  Visited.Add(current);
}

IEnumerable<State> GetAdjacent(State start) {
  // UP
  if (start.Direction != FromDirection.NORTH && start.Y - 1 >= 0) {
    var newSteps = 1;
    if (start.Direction == FromDirection.SOUTH)
      newSteps = start.Steps + 1;
    if (newSteps < 4)
      yield return new State(start.X, start.Y - 1, FromDirection.SOUTH, newSteps);
  }
  // DOWN
  if (start.Direction != FromDirection.SOUTH && start.Y < HEIGHT-1) {
    var newSteps = 1;
    if (start.Direction == FromDirection.NORTH)
      newSteps = start.Steps + 1;
    if (newSteps < 4)
      yield return new State(start.X, start.Y + 1, FromDirection.NORTH, newSteps);
  }
  // WEST
  if (start.Direction != FromDirection.WEST && start.X - 1 > 0) {
    var newSteps = 1;
    if (start.Direction == FromDirection.EAST)
      newSteps = start.Steps + 1;
    if (newSteps < 4)
      yield return new State(start.X - 1, start.Y, FromDirection.EAST, newSteps);
  }
  // EAST
  if (start.Direction != FromDirection.EAST && start.X < WIDTH-1) {
    var newSteps = 1;
    if (start.Direction == FromDirection.WEST)
      newSteps = start.Steps + 1;
    if (newSteps < 4)
      yield return new State(start.X + 1, start.Y, FromDirection.WEST, newSteps);
  }
}

public record State (int X, int Y, FromDirection Direction, int Steps);
public enum FromDirection { NONE, NORTH, EAST, SOUTH, WEST };