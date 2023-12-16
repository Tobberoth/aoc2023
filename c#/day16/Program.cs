// Step 1
Console.WriteLine(Solve(new Beam { Direction = Direction.EAST, Position = (-1, 0) }));

// Step 2
var lines = File.ReadAllLines("input.txt").ToList();
var step2Answer = 0;
for (var i = 0; i < lines[0].Length; i++) {
  step2Answer = Math.Max(step2Answer, Solve(new Beam { Direction = Direction.SOUTH, Position = (i, -1) }));
  step2Answer = Math.Max(step2Answer, Solve(new Beam { Direction = Direction.NORTH, Position = (i, lines.Count) }));
}
for (var i = 0; i < lines.Count; i++) {
  step2Answer = Math.Max(step2Answer, Solve(new Beam { Direction = Direction.EAST, Position = (-1, i) }));
  step2Answer = Math.Max(step2Answer, Solve(new Beam { Direction = Direction.WEST, Position = (lines[0].Length, i) }));
}
Console.WriteLine(step2Answer);

int Solve(Beam startBeam) {
  var lines = File.ReadAllLines("input.txt").ToList();
  var WIDTH = lines[0].Length;
  var HEIGHT = lines.Count;
  List<List<Tile>> Map = [];
  foreach (var line in lines) {
    List<Tile> row = [];
    foreach (char c in line)
      row.Add(new Tile(c));
    Map.Add(row);
  }

  List<Beam> Beams = [];
  Beams.Add(startBeam);
  while (Beams.Count != 0) {
    foreach (var beam in Beams.ToList()) {
      var newBeam = Update(Map, beam);
      if (newBeam != null)
        Beams.Add(newBeam);
      if (IsOutside(beam, WIDTH, HEIGHT))
        Beams.Remove(beam);
    }
  }
  return Map.Select(l => l.Count(t => t.IsEnergized)).Sum();
}

Beam? Update(List<List<Tile>> map, Beam beam) {
  beam.Position = GetNextPosition(beam);
  if (IsOutside(beam, map[0].Count, map.Count))
    return null;

  var currentTile = map[beam.Position.y][beam.Position.x];
  // If mirror
  if (currentTile.Representation == '\\' || currentTile.Representation == '/') {
    if (currentTile.Representation == '\\') {
      beam.Direction = beam.Direction switch {
        Direction.NORTH => Direction.WEST,
        Direction.SOUTH => Direction.EAST,
        Direction.EAST => Direction.SOUTH,
        Direction.WEST => Direction.NORTH
      };
    }
    if (currentTile.Representation == '/') {
      beam.Direction = beam.Direction switch {
        Direction.NORTH => Direction.EAST,
        Direction.SOUTH => Direction.WEST,
        Direction.EAST => Direction.NORTH,
        Direction.WEST => Direction.SOUTH
      };
    }
  }
  // If splitter
  if (currentTile.Representation == '|' || currentTile.Representation == '-') {
    if (currentTile.IsEnergized) { // No point in reusing splitters
      beam.Position = (-1, -1);
      return null;
    }
    if (currentTile.Representation == '|') {
      if (beam.Direction == Direction.EAST || beam.Direction == Direction.WEST) {
        beam.Direction = Direction.SOUTH;
        currentTile.IsEnergized = true;
        return new Beam {
          Direction = Direction.NORTH,
          Position = (beam.Position.x, beam.Position.y)
        };
      }
    }
    if (currentTile.Representation == '-') {
      if (beam.Direction == Direction.NORTH || beam.Direction == Direction.SOUTH) {
        beam.Direction = Direction.EAST;
        currentTile.IsEnergized = true;
        return new Beam {
          Direction = Direction.WEST,
          Position = (beam.Position.x, beam.Position.y)
        };
      }
    }
  }
  currentTile.IsEnergized = true;
  return null;
}

(int x, int y) GetNextPosition(Beam beam) =>
  beam.Direction switch {
    Direction.NORTH => (beam.Position.x, beam.Position.y-1),
    Direction.SOUTH => (beam.Position.x, beam.Position.y+1),
    Direction.EAST => (beam.Position.x+1, beam.Position.y),
    Direction.WEST => (beam.Position.x-1, beam.Position.y)
  };

bool IsOutside(Beam beam, int width, int height) =>
  beam.Position.x < 0 ||
  beam.Position.x >= width ||
  beam.Position.y < 0 ||
  beam.Position.y >= height;

class Beam {
  public Direction Direction { get; set; }
  public (int x, int y) Position { get; set; }
}

class Tile(char input) {
  public bool IsEnergized { get; set; } = false;
  public char Representation { get; set; } = input;
}

enum Direction { NORTH, EAST, SOUTH, WEST };