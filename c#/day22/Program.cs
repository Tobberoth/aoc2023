HashSet<Vector3D> AllPositions = [];
var Bricks = File.ReadAllLines("input.txt").Select(l => new Brick(l)).ToList();
Bricks = Bricks.OrderBy(b => b.Positions.Min(p => p.Z)).ToList();
foreach (var brick in Bricks)
  foreach (var pos in brick.Positions)
    AllPositions.Add(pos);
var fell = int.MaxValue;
while (fell > 0) {
  fell = DropAll(Bricks, AllPositions);
}
Bricks = [.. Bricks.OrderBy(b => b.Positions.Min(p => p.Z))];

// STEP 1
var sum = 0;
foreach (var brick in Bricks) {
  foreach (var pos in brick.Positions)
    AllPositions.Remove(pos);
  List<Brick> tempStack = [..Bricks];
  var index = tempStack.IndexOf(brick);
  tempStack[index..].ForEach(b => b.Stable = false);
  tempStack.Remove(brick);
  if (!TestDrop(tempStack))
    sum++;
  foreach (var pos in brick.Positions)
    AllPositions.Add(pos);
  
}
Console.WriteLine(sum);

// STEP 2
long sum2 = 0;
foreach (var obrick in Bricks) {
  var thisBricks = new List<Brick>();
  foreach (var brickx in Bricks)
    thisBricks.Add(brickx.Copy());
  HashSet<Vector3D> allPositionsCopy = [..AllPositions];
  foreach (var pos in obrick.Positions)
    allPositionsCopy.Remove(pos);
  List<Brick> tempStack = [..thisBricks];
  var index = tempStack.FindIndex(b => b.Positions.SequenceEqual(obrick.Positions));
  tempStack[index..].ForEach(b => b.Stable = false);
  tempStack.RemoveAt(index);
  sum2 += DropAll(tempStack, allPositionsCopy);
}
Console.WriteLine(sum2);

int DropAll(List<Brick> bricks, HashSet<Vector3D> allPositions) {
  var fell = 0;
  foreach (var brick in bricks) {
    var newPos = brick.Drop();
    if (newPos.Count > 0) {
      foreach (var pos in brick.Positions)
        allPositions.Remove(pos);
      var collided = CheckAllCollisions(newPos, allPositions);
      if (collided) {
        foreach (var pos in brick.Positions)
          allPositions.Add(pos);
        brick.Stable = true;
      } else {
        fell++;
        brick.Positions = newPos;
        foreach (var pos in newPos)
          allPositions.Add(pos);
      }
    }
  }
  return fell;
}

bool TestDrop(List<Brick> bricks) {
  foreach (var brick in bricks) {
    var newPos = brick.Drop();
    if (newPos.Count > 0) {
      foreach (var pos in brick.Positions)
        AllPositions.Remove(pos);
      var collided = CheckAllCollisions(newPos, AllPositions);
      foreach (var pos in brick.Positions)
        AllPositions.Add(pos);
      if (!collided) {
        return true;
      }
    }
  }
  return false;
}

bool CheckAllCollisions(List<Vector3D> newPos, HashSet<Vector3D> Filled) =>
  newPos.Any(Filled.Contains);

class Brick {
  public List<Vector3D> Positions { get; set; } = [];
  public bool Stable { get; set; }
  public Brick() { }
  public Brick(string line) {
    var start = line.Split('~')[0].Split(',').Select(int.Parse).ToList();
    var end  = line.Split('~')[1].Split(',').Select(int.Parse).ToList();
    for (var pos = 0; pos < 3; pos++) {
      if (start[pos] == end[pos]) continue;
      var diff = end[pos] - start[pos];
      for (var step = 0; step <= diff; step++) {
        switch (pos) {
          case 0:
            Positions.Add(new Vector3D(start[0]+step, start[1], start[2]));
            break;
          case 1:
            Positions.Add(new Vector3D(start[0], start[1]+step, start[2]));
            break;
          default:
            Positions.Add(new Vector3D(start[0], start[1], start[2]+step));
            break;
        }
      }
    }
    if (Positions.Count == 0)
      Positions.Add(new Vector3D(start[0], start[1], start[2]));
  }

  public List<Vector3D> Drop() {
    if (Stable) return [];
    var ret = Positions.Select(p => new Vector3D(p.X, p.Y, p.Z-1)).ToList();
    if (ret.All(p => p.Z > 0)) return ret;
    Stable = true; // Grounded
    return [];
  }

  public Brick Copy() {
    var newBrick = new Brick { Stable = Stable };
    foreach (var pos in Positions)
      newBrick.Positions.Add(pos);
    return newBrick;
  }
}

record Vector3D(int X, int Y, int Z);