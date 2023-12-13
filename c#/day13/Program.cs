List<List<string>> Patterns = ReadInput("input.txt");
var Step1Ans = Patterns.Select(p => GetPatternValue(p, default)).Sum(c => c.left + c.up * 100);
Console.WriteLine("Step 1: " + Step1Ans);
var Step2Ans = Patterns.Select(GetNewReflection).Sum(c => c.left + c.up * 100);
Console.WriteLine("Step 2: " + Step2Ans);
return;

List<List<string>> ReadInput(string filePath) {
  List<List<string>> ret = [];
  var lines = File.ReadAllLines(filePath);
  List<string> tempList = [];
  foreach (var line in lines) {
    if (string.IsNullOrEmpty(line)) {
      ret.Add(tempList);
      tempList = [];
    } else
      tempList.Add(line);
  }
  ret.Add(tempList);
  return ret;
}

(int left, int up) GetNewReflection(List<string> pattern) {
  // Get initial refraction line
  var initial = GetPatternValue(pattern, default);
  for (var x = 0; x < pattern[0].Length; x++) {
    for (var y = 0; y < pattern.Count; y++) {
      // Toggle this cell
      var prev = pattern[y][x];
      var newLine = pattern[y].Remove(x, 1);
      pattern[y] = newLine.Insert(x, prev == '.' ? "#" : ".");
      var newReflect = GetPatternValue(pattern, initial);
      if (newReflect != (0, 0)) // check if new refraction line
        return newReflect;
      // Toggle back
      newLine = pattern[y].Remove(x, 1);
      pattern[y] = newLine.Insert(x, prev.ToString());
    }
  }
  return (0, 0);
}

(int left, int up) GetPatternValue(List<string> pattern, (int, int) skipPattern = default) {
  if (skipPattern == default) skipPattern = (-1, -1);
  var height = pattern.Count;
  var width = pattern[0].Length;
  for (var x = 0; x < width; x++) {
    var reflect = true;
    var maxSame = 1_000_000;
    for (var y = 0; y < height; y++) {
      var same = CheckVertical(pattern[y], x);
      if (same == 0) {
        reflect = false;
        break;
      }
      if (maxSame == 1_000_000)
        maxSame = same;
      if (same != maxSame) {
        reflect = false;
        break;
      }
    }
    if (reflect) {
      if ((x+1, 0) != skipPattern)
        return (x+1, 0);
    }
  }

  for (var y = 0; y < height; y++) {
    var reflect = true;
    var maxSame = 1_000_000;
    for (var x = 0; x < width; x++) {
      var same = CheckHorizontal(pattern, y, x);
      if (same == 0) {
        reflect = false;
        break;
      }
      if (maxSame == 1_000_000)
        maxSame = same;
      if (same != maxSame) {
        reflect = false;
        break;
      }
    }
    if (reflect) {
      if ((0, y+1) != skipPattern)
      return (0, y+1);
    }
  }
  return (0, 0);
}

int CheckVertical(string line, int x) {
  var xLeft = x;
  var xRight = x+1;
  var same = 0;
  while (true) {
    if (xLeft < 0 || xRight >= line.Length) break;
    if (line[xLeft] == line[xRight])
      same++;
    else
      break;
    xLeft--;
    xRight++;
  }
  return same;
}

int CheckHorizontal(List<string> pattern, int y, int x) {
  var yUp = y;
  var yDown = y + 1;
  var same = 0;
  while (true) {
    if (yUp < 0 || yDown >= pattern.Count) break; 
    if (pattern[yUp][x] == pattern[yDown][x])
      same++;
    else
      return 0; // Missed proper match
    yUp--;
    yDown++;
  }
  return same;
}