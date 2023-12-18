var hexes = File.ReadAllLines("input.txt").Select(l => l.Split(' ').Last());
(long x, long y) current = (0, 0);
List<(long x, long y)> Coords = [current];
var sumSteps = 0;
foreach (var hex in hexes) {
  var steps = int.Parse(hex[2..^2], System.Globalization.NumberStyles.HexNumber);
  sumSteps += steps;
  var newCoord = hex[^2..^1] switch {
    "0" => (current.x+steps, current.y),
    "1" => (current.x, current.y+steps),
    "2" => (current.x-steps, current.y),
    "3" => (current.x, current.y-steps),
  };
  Coords.Add(newCoord);
  current = newCoord;
}

Console.WriteLine(CalcAreaShoelace(Coords) + sumSteps/2+1);

static double CalcAreaShoelace(List<(long x, long y)> coords) {
  long sum = 0;
  for (var i = 0; i < coords.Count-1; i++) {
    var first = coords[i];
    var second = coords[i+1];
    var one = first.x * second.y;
    var two = first.y * second.x;
    sum += one - two;
  }
  return sum/2d;
}