using System.Collections.ObjectModel;

// Step1
Console.WriteLine(GetAnswer());

// Real step 2
// Find modules leading to module before RX
Dictionary<string, Module> ModuleList = ReadInput("input.txt");
var beforeRx = ModuleList.FirstOrDefault(kp => kp.Value.Targets.Contains("rx")).Key;
var beforeIntro = ModuleList.Where(kp => kp.Value.Targets.Contains(beforeRx));
List<long> cycles = [];
foreach (var kp in beforeIntro) {
  cycles.Add(GetAnswer(beforeRx, kp.Key)+1);
}
for (long click = cycles.Max(); click < long.MaxValue; click += cycles.Max()) {
  var found = true;
  foreach (long c in cycles) {
    if (click % c != 0) {
      found = false;
      break;
    }
  }
  if (found) {
    Console.WriteLine(click);
    break;
  }
}

long GetAnswer(string beforeRx = null, string moduleToCheck = null) {
  Dictionary<string, Module> moduleList = ReadInput("input.txt");
  long lowSum = 0;
  long highSum = 0;
  int timesToClick = 1000;
  if (beforeRx != null)
    timesToClick = 20000;
  for (var i = 0; i < timesToClick; i++) {
    var lowPulseCount = 1; // button to broad
    var highPulseCount = 0;
    var first = new PulseAction(moduleList, "button", "broadcaster", Pulse.LOW);
    Queue<PulseAction> ActionsToRun = new();
    ActionsToRun.Enqueue(first);
    while (ActionsToRun.Count > 0) {
      var current = ActionsToRun.Dequeue();
      if (beforeRx != null && current.To == beforeRx && current.Pulse == Pulse.HIGH && current.From == moduleToCheck) {
        return i;
      }
      foreach (var newAction in current.Run(i)) {
        if (newAction.Pulse == Pulse.LOW)
          lowPulseCount++;
        else
          highPulseCount++;
        ActionsToRun.Enqueue(newAction);
      }
    }
    lowSum += lowPulseCount;
    highSum += highPulseCount;
  }
  return lowSum * highSum;
}

Dictionary<string, Module> ReadInput(string filePath) {
  Dictionary<string, Module> ret = new();
  var lines = File.ReadAllLines(filePath);
  foreach (var line in lines) {
    var source = line.Split(" -> ")[0];
    var targets = line.Split(" -> ")[1].Split(", ");
    switch (source[0]) {
      case 'b':
        var newModule = new Module();
        foreach (var target in targets)
          newModule.Targets.Add(target);
        ret.Add("broadcaster", newModule);
        break;
      case '%':
        var newFlipFlopModule = new FlipFlopModule();
        foreach (var target in targets)
          newFlipFlopModule.Targets.Add(target);
        ret.Add(source[1..], newFlipFlopModule);
        break;
      case '&':
        var newConjunctionModule = new ConjunctionModule();
        foreach (var target in targets)
          newConjunctionModule.Targets.Add(target);
        ret.Add(source[1..], newConjunctionModule);
        break;
    }
  }
  // Set up conjunction initial state
  foreach (var kp in ret) {
    foreach (var target in kp.Value.Targets) {
      if (ret.ContainsKey(target)) {
        var modTarget = ret[target];
        if (modTarget is ConjunctionModule conjTarget) {
          conjTarget.LatestInputs.Add(kp.Key, Pulse.LOW);
        }
      }
    }
  }
  return ret;
}

class PulseAction(Dictionary<string, Module> modulesList, string From, string To, Pulse Pulse) {
  public Dictionary<string, Module> ModulesList = modulesList;
  public string To { get; init; } = To;
  public string From { get; init; } = From;
  public Pulse Pulse { get; init; } = Pulse;
  public IEnumerable<PulseAction> Run(int buttonClicks) {
    if (!ModulesList.ContainsKey(To)) {
      yield break;
    }
    var toModule = ModulesList[To];
    if (From == "button") { // Always to broadcaster
      foreach (var target in toModule.Targets)
        yield return new PulseAction(ModulesList, To, target, Pulse.LOW);
      yield break;
    }
    if (toModule is FlipFlopModule toFlipModule) {
      if (Pulse == Pulse.LOW) {
        toFlipModule.Toggle = !toFlipModule.Toggle;
        foreach (var target in toFlipModule.Targets)
          yield return new PulseAction(
            ModulesList, To, target, toFlipModule.Toggle ? Pulse.HIGH : Pulse.LOW);
        yield break;
      }
    }
    if (toModule is ConjunctionModule conjModule) {
      conjModule.LatestInputs[From] = Pulse;
      if (conjModule.LatestInputs.All(kp => kp.Value == Pulse.HIGH)) {
        foreach (var target in conjModule.Targets)
          yield return new PulseAction(ModulesList, To, target, Pulse.LOW);
      } else {
        foreach (var target in conjModule.Targets)
          yield return new PulseAction(ModulesList, To, target, Pulse.HIGH);
      }
      yield break;
    }
    yield break;
  }
  public override string ToString() {
    return $"{From} -{Pulse}-> {To}";
  }
}

class Module {
  public Collection<string> Targets = [];
}

class FlipFlopModule : Module {
  public bool Toggle { get; set; }
}

class ConjunctionModule : Module {
  public Dictionary<string, Pulse> LatestInputs = [];
}

public enum Pulse { LOW, HIGH }