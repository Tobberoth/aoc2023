var lines = File.ReadAllLines("input.txt").Select(l => l.Select(c => c - '0')!.ToList()).ToList();
if (lines == null)
  throw new InvalidOperationException("Unable to read input");
var WIDTH = lines[0].Count;
var HEIGHT = lines.Count;
var startState = new State(0, 0, FromDirection.NONE, 0);
HashSet<State> Visited = [];
Dictionary<State, int> StateLog = [];
StateLog.Add(startState, 0);
PriorityQueue<State, int> Unvisited = new();
Unvisited.Enqueue(startState, 0);

var lowestHeat = 1_000_000;
while (Unvisited.Count > 0) {
  var current = Unvisited.Dequeue();
  var currentHeat = StateLog[current];
  foreach ((var adj, var heat) in GetAdjacent(current)) { // Use GetAdjacent2 for Step 2
    if (Visited.Contains(adj))
      continue;
    var newHeat = currentHeat + heat;
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
      lowestHeat = Math.Min(lowestHeat, newHeat);
    } else {
      Unvisited.Enqueue(adj, newHeat);
    }
  }
  Visited.Add(current);
}
Console.WriteLine(lowestHeat);

IEnumerable<(State state, int heat)> GetAdjacent(State start) {
  // UP
  if (start.Direction != FromDirection.NORTH && start.Y - 1 >= 0) {
    var newSteps = 1;
    if (start.Direction == FromDirection.SOUTH)
      newSteps = start.Steps + 1;
    if (newSteps < 4)
      yield return (new State(start.X, start.Y - 1, FromDirection.SOUTH, newSteps), lines[start.Y-1][start.X]);
  }
  // DOWN
  if (start.Direction != FromDirection.SOUTH && start.Y < HEIGHT-1) {
    var newSteps = 1;
    if (start.Direction == FromDirection.NORTH)
      newSteps = start.Steps + 1;
    if (newSteps < 4)
      yield return (new State(start.X, start.Y + 1, FromDirection.NORTH, newSteps), lines[start.Y+1][start.X]);
  }
  // WEST
  if (start.Direction != FromDirection.WEST && start.X - 1 > 0) {
    var newSteps = 1;
    if (start.Direction == FromDirection.EAST)
      newSteps = start.Steps + 1;
    if (newSteps < 4)
      yield return (new State(start.X - 1, start.Y, FromDirection.EAST, newSteps), lines[start.Y][start.X-1]);
  }
  // EAST
  if (start.Direction != FromDirection.EAST && start.X < WIDTH-1) {
    var newSteps = 1;
    if (start.Direction == FromDirection.WEST)
      newSteps = start.Steps + 1;
    if (newSteps < 4)
      yield return (new State(start.X + 1, start.Y, FromDirection.WEST, newSteps), lines[start.Y][start.X+1]);
  }
}

IEnumerable<(State state, int heat)> GetAdjacent2(State start) {
  if (lines == null) yield break;
  // Instead of just one step, always go for all the steps
  // SOUTH 
  if (start.Direction != FromDirection.NORTH && start.Direction != FromDirection.SOUTH) {
    var heatSum = 0;
    for (var newY = 1; newY <= 10; newY++) {
      if (start.Y + newY >= HEIGHT) break;
      heatSum += lines[start.Y+newY][start.X];
      if (newY > 3 && newY < 11)
        yield return (new State(start.X, start.Y + newY, FromDirection.NORTH, newY), heatSum);
    }
  }
  // NORTH
  if (start.Direction != FromDirection.SOUTH && start.Direction != FromDirection.NORTH) {
    var heatSum = 0;
    for (var newY = 1; newY <= 10; newY++) {
      if (start.Y - newY < 0) break;
      heatSum += lines[start.Y-newY][start.X];
      if (newY > 3 && newY < 11)
        yield return (new State(start.X, start.Y - newY, FromDirection.SOUTH, newY), heatSum);
    }
  }
  // WEST
  if (start.Direction != FromDirection.WEST && start.Direction != FromDirection.EAST) {
    var heatSum = 0;
    for (var newX = 1; newX <= 10; newX++) {
      if (start.X - newX < 0) break;
      heatSum += lines[start.Y][start.X-newX];
      if (newX > 3 && newX < 11)
        yield return (new State(start.X - newX, start.Y, FromDirection.EAST, newX), heatSum);
    }
  }
  // EAST
  if (start.Direction != FromDirection.WEST && start.Direction != FromDirection.EAST) {
    var heatSum = 0;
    for (var newX = 1; newX <= 10; newX++) {
      if (start.X + newX >= WIDTH) break;
      heatSum += lines[start.Y][start.X+newX];
      if (newX > 3 && newX < 11)
        yield return (new State(start.X + newX, start.Y, FromDirection.WEST, newX), heatSum);
    }
  }
}

public record State (int X, int Y, FromDirection Direction, int Steps);
public enum FromDirection { NONE, NORTH, EAST, SOUTH, WEST };