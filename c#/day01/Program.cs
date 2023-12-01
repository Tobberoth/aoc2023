var sum1 = File.ReadAllLines("input.txt")
  .Select(s => s.Where(char.IsDigit))
  .Sum(s => int.Parse(s.First().ToString() + s.Last().ToString()));
Console.WriteLine(sum1);

var sum2 = File.ReadAllLines("input.txt")
  .Sum(s => int.Parse(FindFirstDigit(s) + FindLastDigit(s)));
Console.WriteLine(sum2);

string FindFirstDigit(string line) {
  List<(int, string)> numbers = [];
  numbers.Add((line.IndexOf("one"), "1"));
  numbers.Add((line.IndexOf("two"), "2"));
  numbers.Add((line.IndexOf("three"), "3"));
  numbers.Add((line.IndexOf("four"), "4"));
  numbers.Add((line.IndexOf("five"), "5"));
  numbers.Add((line.IndexOf("six"), "6"));
  numbers.Add((line.IndexOf("seven"), "7"));
  numbers.Add((line.IndexOf("eight"), "8"));
  numbers.Add((line.IndexOf("nine"), "9"));
  for (var i = 1; i <= 9; i++) {
    numbers.Add((line.IndexOf(i.ToString()), i.ToString()));
  }
  numbers = [.. numbers.Where(kp => kp.Item1 != -1).OrderBy(kp => kp.Item1)];
  return numbers.First().Item2.ToString();
}

string FindLastDigit(string line) {
  List<(int, string)> numbers = [];
  numbers.Add((line.LastIndexOf("one"), "1"));
  numbers.Add((line.LastIndexOf("two"), "2"));
  numbers.Add((line.LastIndexOf("three"), "3"));
  numbers.Add((line.LastIndexOf("four"), "4"));
  numbers.Add((line.LastIndexOf("five"), "5"));
  numbers.Add((line.LastIndexOf("six"), "6"));
  numbers.Add((line.LastIndexOf("seven"), "7"));
  numbers.Add((line.LastIndexOf("eight"), "8"));
  numbers.Add((line.LastIndexOf("nine"), "9"));
  for (var i = 1; i <= 9; i++) {
    numbers.Add((line.LastIndexOf(i.ToString()), i.ToString()));
  }
  numbers = [.. numbers.Where(kp => kp.Item1 != -1).OrderByDescending(kp => kp.Item1)];
  return numbers.First().Item2.ToString();
}