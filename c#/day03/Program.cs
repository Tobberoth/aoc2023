using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input.txt").ToList();
var WIDTH = lines[0].Length;
var HEIGHT = lines.Count;

// Find number
var sum = 0;
var re = new Regex(@"\d+");
var yCount = 0;
foreach (var line in lines) {
  var matches = re.Matches(line);
  foreach (Match match in matches) {
    var len = match.Length;
    if (HasSymbolAround(match.Index, yCount, len))
      sum += int.Parse(match.Value);
  }
  yCount++;
}
Console.WriteLine(sum);

// Find gears
long gearSum = 0;
var gearRe = new Regex(@"\*");
yCount = 0;
foreach (var line in lines) {
  var matches = gearRe.Matches(line);
  foreach (Match match in matches) {
    gearSum += HasNumbersAround(match.Index, yCount);
  }
  yCount++;
}
Console.WriteLine(gearSum);

int HasNumbersAround(int x, int y) {
  List<int> Numbers = [];
  Numbers.Add(CheckIfNumber(y, x-1));
  Numbers.Add(CheckIfNumber(y, x+1));
  Numbers.AddRange(CheckIfNumbersAbove(y, x));
  Numbers.AddRange(CheckIfNumbersBelow(y, x));
  Numbers.RemoveAll(n => n < 1);
  if (Numbers.Count == 2)
    return Numbers[0] * Numbers[1];
  return 0;
}

int CheckIfNumber(int y, int x) {
  if (y < 0 || y >= HEIGHT || x < 0 || x >= WIDTH) return 0;
  if (char.IsDigit(lines[y][x])) {
    // Go left until not digit
    while (true) {
      x--;
      if (x < 0) break;
      if (!char.IsDigit(lines[y][x])) break;
    }
    x++;
    // Go right until digit done
    var num = "";
    while (true) {
      if (char.IsDigit(lines[y][x]))
        num += lines[y][x];
      else
        break;
      x++;
      if (x >= WIDTH) break;
    }
    return int.Parse(num);
  }
  return 0;
}

List<int> CheckIfNumbersBelow(int y, int x) {
  return CheckIfNumbersAbove(y+2, x);
}
List<int> CheckIfNumbersAbove(int y, int x) {
  y -= 1;
  if (y < 0 || y >= HEIGHT) return [];
  var left = (x-1 < 0 || x-1 >= WIDTH ) ? '.' : lines[y][x-1];
  var mid = (x < 0 || x >= WIDTH ) ? '.' : lines[y][x];
  var right = (x+1 < 0 || x+1 >= WIDTH ) ? '.' : lines[y][x+1];

  List<int> ret = [];
  if (!char.IsDigit(mid)) {
    if (char.IsDigit(left))
      ret.Add(CheckIfNumber(y, x-1));
    if (char.IsDigit(right))
      ret.Add(CheckIfNumber(y, x+1));
  } else
    ret.Add(CheckIfNumber(y, x));

  return ret;
}

bool HasSymbolAround(int numX, int numY, int numLen) {
  for (var i = -1; i <= numLen; i++) {
    if (CheckGrid(numY-1, numX+i) || CheckGrid(numY, numX+i) || CheckGrid(numY+1, numX+i))
      return true;
  }
  return false;
}

bool CheckGrid(int y, int x) {
  if (y < 0 || y >= HEIGHT) return false;
  if (x < 0 || x >= WIDTH) return false;
  var target = lines[y][x];
  if (!char.IsDigit(target) && target != '.')
    return true;
  return false;
}