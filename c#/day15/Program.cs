var step1Answer = File.ReadAllText("input.txt")
  .Split(',')
  .Select(Hash)
  .Sum();
Console.WriteLine(step1Answer);

List<List<Lens>> Boxes = new(256);
for (var i = 0; i < 256; i++)
  Boxes.Add([]);
foreach (var cmd in File.ReadAllText("input.txt").Split(","))
  HandleCommand(cmd, Boxes);
Console.WriteLine(Boxes.Select(CalculateFocusingPower).Sum());

void HandleCommand(string command, List<List<Lens>> boxes) {
  if (command.EndsWith("-")) {
    // Decrease
    var label = command[..^1];
    var box = Hash(label);
    boxes[box] = boxes[box].Where(l => l.Label != label).ToList();
  } else {
    // Set
    var label = command.Split("=")[0];
    var focalLength = int.Parse(command.Split("=")[1]);
    var box = Hash(label);
    if (boxes[box].FindIndex(l => l.Label == label) is int i) {
      if (i > -1)
        boxes[box][i] = new Lens(label, focalLength);
      else
        boxes[box].Add(new Lens(label, focalLength));
    }
  }
}

long CalculateFocusingPower(List<Lens> lenses) {
  var sum = 0;
  for (var i = 1; i <= lenses.Count; i++) {
    var boxVal = Hash(lenses[i-1].Label) + 1;
    var focStr = lenses[i-1].FocalLength;
    sum +=  boxVal * i * focStr;
  }
  return sum;
}

static int Hash(string input) {
  var currentValue = 0;
  foreach (var c in input) {
    currentValue += c;
    currentValue *= 17;
    currentValue %= 256;
  }
  return currentValue;
}

record Lens(string Label, int FocalLength);