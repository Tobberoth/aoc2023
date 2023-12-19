var lines = File.ReadAllLines("input.txt");
var rules = lines
  .Where(l => !l.StartsWith('{') && !string.IsNullOrWhiteSpace(l))
  .Select(StringToWorkflow)
  .ToList();
var parts = lines.Where(l => l.StartsWith('{')).Select(StringToPart).ToList();
List<Part> A = [];
List<Part> R = [];

var currentRule = rules.First(r => r.Name == "in");
foreach (var part in parts) {
  while (true) {
    var next = currentRule.Run(part);
    if (next == "A") {
      A.Add(part);
      currentRule = rules.First(r => r.Name == "in");
      break;
    } else if (next == "R") {
      R.Add(part);
      currentRule = rules.First(r => r.Name == "in");
      break;
    } else
      currentRule = rules.First(r => r.Name == next);
  }
}

Console.WriteLine(A.Sum(p => p.X + p.M + p.A + p.S));

static Workflow StringToWorkflow(string input) {
  return new Workflow {
    Name = input.Split('{')[0],
    Rule = input.Split('{')[1].TrimEnd('}')
  };
}

static Part StringToPart(string input) {
  var arguments = input.TrimStart('{').TrimEnd('}').Split(',');
  int x = 0, m = 0, a = 0, s = 0;
  foreach (var argument in arguments) {
    var name = argument.Split('=')[0];
    var val = int.Parse(argument.Split('=')[1]);
    switch (name) {
      case "x":
        x = val;
        break;
      case "m":
        m = val;
        break;
      case "a":
        a = val;
        break;
      case "s":
        s = val;
        break;
    }
  }
  return new Part(x, m, a, s);
}

class Workflow {
  public required string Name { get; set; }
  public required string Rule { get; set; }
  public string Run(Part part) {
    var commands = Rule.Split(',');
    foreach (var command in commands) {
      if (!command.Contains(':')) return command;
      var test = command.Split(':')[0];
      var target = command.Split(':')[1];
      if (test.Contains('<')) {
        var argument = test.Split('<')[0];
        var value = int.Parse(test.Split('<')[1]);
        if ((int)part.GetType().GetProperty(argument.ToUpper()).GetValue(part) < value)
          return target;
      } else {
        var argument = test.Split('>')[0];
        var value = int.Parse(test.Split('>')[1]);
        if ((int)part.GetType().GetProperty(argument.ToUpper()).GetValue(part) > value)
          return target;
      }
    }
    throw new InvalidOperationException($"{Name} is not a valid rule");
  }
}

record Part(int X, int M, int A, int S);