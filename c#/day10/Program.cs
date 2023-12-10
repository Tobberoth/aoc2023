var Lines = File.ReadAllLines("input.txt").ToList();
var AreaMap = Lines.Select(l => l.Select(c => c).ToList()).ToList();
(int x, int y) StartPos = (0, 0);
for (var y = 0; y < Lines.Count; y++) {
  if (Lines[y].IndexOf('S') > -1)
    StartPos = (Lines[y].IndexOf('S'), y);
}
(var startSymbol, var nextPos) = GetStartSymbol(Lines, StartPos.x, StartPos.y);
Console.WriteLine(Step1(Lines, AreaMap, StartPos));
Console.WriteLine(Step2(Lines, AreaMap, StartPos));

int Step1(List<string> lines, List<List<char>> areaMap, (int x, int y) prevPos) {
  var steps = 0;
  while (true) {
    // Write area map for use in step 2
    if (areaMap[prevPos.y][prevPos.x] != 'S')
      areaMap[prevPos.y][prevPos.x] = 'O';
    var newPos = GetNextMove(lines, prevPos, nextPos);
    if (newPos.x == -1)
      break;
    (prevPos, nextPos) = (nextPos, newPos);
    steps++;
  }
  return (int)Math.Ceiling(steps / 2f);
}

int Step2(List<string> lines, List<List<char>> areaMap, (int x, int y) startPos) {
  lines[startPos.y] = lines[startPos.y].Replace('S', startSymbol); // Set actual symbol for start
  areaMap[startPos.y][startPos.x] = 'O'; // Complete pipe in area map
  for (int y = 0; y < lines.Count; y++) {
    var currentlyOut = true;
    for (int x = 0; x < lines[0].Length; x++) {
      if (areaMap[y][x] == 'O') {
        if (lines[y][x] == '|') {
          currentlyOut = !currentlyOut;
          continue;
        } else if (lines[y][x] == 'F') {
          // Find 7 (no charge) or J (toggle)
          while (true) {
            x++;
            if (lines[y][x] == '7')
              break;
            else if (lines[y][x] == 'J') {
              currentlyOut = !currentlyOut;
              break;
            }
          }
          continue;
        } else if (lines[y][x] == 'L') {
          // Find 7 (toggle) or J (no change)
          while (true) {
            x++;
            if (lines[y][x] == '7') {
              currentlyOut = !currentlyOut;
              break;
            } else if (lines[y][x] == 'J')
              break;
          }
          continue;
        }
      }
      areaMap[y][x] = !currentlyOut ? 'I' : 'X';
    }
  }
  return areaMap.Sum(l => l.Count(c => c == 'I'));
}

(int x, int y) GetNextMove(List<string> lines, (int x, int y) from, (int x, int y) current) {
  char currentPipe = lines[current.y][current.x];
  if (currentPipe == 'S') return (-1, -1); // Done
  return currentPipe switch {
    '|' => (from.y < current.y) ? (current.x, current.y + 1) : (current.x, current.y - 1),
    '-' => (from.x < current.x) ? (current.x + 1, current.y) : (current.x - 1, current.y),
    'L' => (from.y < current.y) ? (current.x + 1, current.y) : (current.x, current.y - 1),
    'J' => (from.y < current.y) ? (current.x - 1, current.y) : (current.x, current.y - 1),
    '7' => (from.y > current.y) ? (current.x - 1, current.y) : (current.x, current.y + 1),
    'F' => (from.y > current.y) ? (current.x + 1, current.y) : (current.x, current.y + 1),
    _ => throw new InvalidOperationException("Can't find next pipe piece")
  };
}

(char sSymbol, (int, int) nextPos) GetStartSymbol(List<string> map, int x, int y) {
  var topConnect = ((char[])['|', '7', 'F']).Contains(map[y-1][x]);
  var bottomConnect = ((char[])['|', 'J', 'L']).Contains(map[y+1][x]);
  var rightConnect = ((char[])['-', 'J', '7']).Contains(map[y][x+1]);
  var leftConnect = ((char[])['-', 'F', 'L']).Contains(map[y][x-1]);
  if (topConnect && bottomConnect) return ('|', (x, y-1));
  if (rightConnect && leftConnect) return ('-', (x+1, y));
  if (topConnect && rightConnect) return ('L', (x, y-1));
  if (topConnect && leftConnect) return ('J', (x, y-1));
  if (bottomConnect && rightConnect) return ('7', (x, y+1));
  if (bottomConnect && leftConnect) return ('F', (x, y+1));
  throw new InvalidOperationException("Couldn't identify symbol");
}