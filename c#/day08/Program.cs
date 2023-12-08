var lines = File.ReadAllLines("input.txt");
var directions = lines.Take(1).First();
var map = lines[2..]
  .Select(l =>
    new KeyValuePair<string, (string left, string right)>(
      l.Split(" = ")[0],
      (
        l.Split(" = ")[1].Split(", ")[0][1..],
        l.Split(" = ")[1].Split(", ")[1][..^1])))
  .ToDictionary();

// Step 1
Console.WriteLine(CountToEnd(map, directions));

// Step 2
var periods = map.Where(kp => kp.Key.EndsWith('A')).Select(kp => CalcPeriod(kp.Key, directions)).ToList();
var maxPeriod = periods.Max();
for (long i = maxPeriod; i < long.MaxValue; i += maxPeriod) {
  var match = true;
  foreach (var p in periods) {
    if (i % p != 0) {
      match = false;
      break;
    }
  }
  if (match) {
    Console.WriteLine(i);
    break;
  }
}

int CountToEnd(Dictionary<string, (string left, string right)> map, string dir) {
  var steps = 0;
  var currentNode = map["AAA"];
  var previousStep = "";
  var i = 0;
  while (true) {
    if (previousStep == "ZZZ") break;
    if (i >= dir.Length)
      i = 0;
    char currentDir = dir[i];
    if (currentDir == 'R') {
      previousStep = currentNode.right;
      currentNode = map[currentNode.right];
    } else {
      previousStep = currentNode.left;
      currentNode = map[currentNode.left];
    }
    steps++;
    i++;
  }
  return steps;
}

long CalcPeriod(string ghost, string dir) {
  var steps = 0;
  var i = 0;
  while (true) {
    if (i >= dir.Length) i = 0;
    char currDir = dir[i];
    steps++;
    if (currDir == 'R')
      ghost = map[ghost].right;
    else if (currDir == 'L')
      ghost = map[ghost].left;
    if (ghost.EndsWith('Z'))
      return steps;
    i++;
  }
}