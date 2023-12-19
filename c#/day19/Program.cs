var lines = File.ReadAllLines("input.txt");
var rules = lines
  .Where(l => !l.StartsWith('{') && !string.IsNullOrWhiteSpace(l))
  .Select(StringToWorkflow)
  .ToList();

// STEP 1
var parts = lines.Where(l => l.StartsWith('{')).Select(StringToPart).ToList();
List<Part> listA = [];
var currentRule = rules.First(r => r.Name == "in");
foreach (var part in parts) {
  while (true) {
    var next = currentRule.Run(part);
    if (next == "A") {
      listA.Add(part);
      currentRule = rules.First(r => r.Name == "in");
      break;
    } else if (next == "R") {
      // No need to save rejected, just restart
      currentRule = rules.First(r => r.Name == "in");
      break;
    } else
      currentRule = rules.First(r => r.Name == next);
  }
}
Console.WriteLine("Step 1: " + listA.Sum(p => p.X + p.M + p.A + p.S));

// STEP 2
var inRule = rules.First(r => r.Name == "in");
var allAcceptedBroadRules = GetBroadRules(
  inRule,
  new BroadRule(1, 4001, 1, 4001, 1, 4001, 1, 4001, ' ')).Where(r => r.Target == 'A').ToList();

long sum = allAcceptedBroadRules.Select(r =>
  (r.EndingX - r.StartingX) *
  (r.EndingM - r.StartingM) *
  (r.EndingA - r.StartingA) *
  (r.EndingS - r.StartingS)
).Sum();
Console.WriteLine("Step 2: " + sum);


IEnumerable<BroadRule> GetBroadRules(Workflow wf, BroadRule init) {
  var subRules = wf.Rule.Split(',');
  foreach (var subRule in subRules) {
    if (subRule.Contains(':')) {
      var target = subRule.Split(':')[1];
      var condition = subRule.Split(':')[0];

      var param = "";
      var value = 0;
      var starting = false;
      if (condition.Contains('<')) {
        param = condition.Split('<')[0];
        value = int.Parse(condition.Split('<')[1]);
      } else {
        param = condition.Split('>')[0];
        value = int.Parse(condition.Split('>')[1]);
        starting = true;
      }

      var newBroadRule = new BroadRule(
        StartingX: param == "x" && starting ? value+1 : init.StartingX, 
        EndingX: param == "x" && !starting ? value : init.EndingX,
        StartingM: param == "m" && starting ? value+1 : init.StartingM,
        EndingM: param == "m" && !starting ? value : init.EndingM,
        StartingA: param == "a" && starting ? value+1 : init.StartingA,
        EndingA: param == "a" && !starting ? value : init.EndingA,
        StartingS: param == "s" && starting ? value+1 : init.StartingS,
        EndingS: param == "s" && !starting ? value : init.EndingS,
        Target: target[0]
      );
      init = new BroadRule(
        StartingX: param == "x" && !starting ? value : init.StartingX, 
        EndingX: param == "x" && starting ? value+1 : init.EndingX,
        StartingM: param == "m" && !starting ? value : init.StartingM,
        EndingM: param == "m" && starting ? value+1 : init.EndingM,
        StartingA: param == "a" && !starting ? value : init.StartingA,
        EndingA: param == "a" && starting ? value+1 : init.EndingA,
        StartingS: param == "s" && !starting ? value : init.StartingS,
        EndingS: param == "s" && starting ? value+1 : init.EndingS,
        Target: ' '
      );

      if (target == "A" || target == "R") {
        yield return newBroadRule;
      } else {
        var subBroadRules = GetBroadRules(rules.First(r => r.Name == target), newBroadRule);
        foreach (var subBroadRule in subBroadRules)
          yield return subBroadRule;
      }
    } else {
      var target = subRule;
      if (target == "A" || target == "R") {
        yield return new BroadRule(
          init.StartingX, init.EndingX,
          init.StartingM, init.EndingM,
          init.StartingA, init.EndingA,
          init.StartingS, init.EndingS,
          target[0]
        );
      } else {
        var subBroadRules = GetBroadRules(rules.First(r => r.Name == target), init);
        foreach (var subBroadRule in subBroadRules)
          yield return subBroadRule;
      }
    }
  }
}

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

record BroadRule(
  long StartingX, long EndingX,
  long StartingM, long EndingM,
  long StartingA, long EndingA,
  long StartingS, long EndingS,
  char Target
);