var lines = File.ReadAllLines("input.txt");

Console.WriteLine(lines.Select(GetAllArrangementsOver).Sum());

int GetAllArrangementsOver(string line) {
  var conditions = line.Split(' ')[0];
  var groups = line.Split(' ')[1].Split(',').Select(int.Parse).ToList();
  return GetAllArrangements(conditions, groups);
}

int GetAllArrangements(string conditions, List<int> groups) {
  if (groups.Sum() + groups.Count - 1 > conditions.Length) return 0;

  var sum = 0;
  var first = groups[0];
  var rest = groups[1..];
  // This is not enough, if there are large consecutive early this needs to be cut as well
  // Need to cover enough as well, not just blanks
  //var restCover = rest.Sum() + rest.Count;
  var restCover = GetFirstFreeIndexBackwards(conditions, rest.Sum() + rest.Count - 1);
  if (restCover == -1) return 0; // Rest doesn't fit
  var space = conditions[..restCover];
  Console.WriteLine($"{first} {space}");

  // Find all possible solitons for first in space
  // how
  /*
  if (space.Length == first)
    sum += 1;
  else if (Regex.IsMatch(space, @"#{" + first + "}"))
    sum += 1;
  else {

  }
  */

  // Then add rest
  var nextFree = FindFirstFreeIndex(conditions, first);
  Console.WriteLine($"{rest} {conditions[nextFree..]}");
  /*
  sum += GetAllArrangements(conditions[nextFree..], groups[1..]);
  */

  return groups.Sum();
}

int FindFirstFreeIndex(string line, int spaceNeeded) {
  var open = 0;
  var i = 0;
  foreach (char c in line) {
    if (c == '?')
      open++;
    if (c == '#')
      open = 1; // Reset since this must be covered
    if (c == '.')
      open = 0;
    if (open == spaceNeeded)
      return i+2; // Cover this and one open
    i++;
  }
  return -1;
}

int GetFirstFreeIndexBackwards(string line, int spaceNeeded) {
  // TODO: spaceneeded isn't accurate since it includes separating .
  // Maybe count them separately

  var sum = 0;
  for (var i = line.Length-1; i >= 0; i--) {
    if (line[i] == '#' || line[i] == '?')
      sum++;
    if (sum == spaceNeeded)
      return i;
  }
  return -1;
}