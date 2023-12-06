using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input.txt");
Console.WriteLine(Step1(lines));
Console.WriteLine(Step2(lines));

int Step1(string[] lines) {
  var times = NumbersRegex().Matches(lines[0]).Select(m => int.Parse(m.Value)).ToList();
  var distances = NumbersRegex().Matches(lines[1]).Select(m => int.Parse(m.Value)).ToList();
  var prod = 1;
  for (var i = 0; i < times.Count; i++) {
    var wins = 0;
    var time = times[i];
    var distance = distances[i];
    for (var press = 0; press <= time; press++)
      if (IsWin(time, press, distance)) wins++;
    prod *= wins;
  }
  return prod;
}

long Step2(string[] lines) {
  var time = long.Parse(
    NumbersRegex().Matches(lines[0])
    .Select(m => m.Value)
    .Aggregate((total, next) => total + next));
  var distance = long.Parse(
    NumbersRegex().Matches(lines[1])
    .Select(m => m.Value)
    .Aggregate((total, next) => total + next));
  long wins = 0;
  for (var press = 0; press <= time; press++) {
    if (IsWin(time, press, distance)) wins++;
  }
  return wins;
}

bool IsWin(long time, long press, long targetDistance) {
  var timeToRun = time - press;
  var distance = timeToRun * press;
  return distance > targetDistance;
}

partial class Program {
    [GeneratedRegex(@"\d+")]
    private static partial Regex NumbersRegex();
}