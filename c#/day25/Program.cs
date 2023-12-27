var graph = GenerateGraph("input.txt");
Dictionary<(string from, string to), int> Weights = [];
for (var round = 0; round < 2000; round++) {
  SimulateTraffic(graph, Weights);
}
var maxUsedPaths = Weights.OrderByDescending(kp => kp.Value).Take(3).ToList();
Console.WriteLine("Three connections to cut:");
foreach (var p in maxUsedPaths)
  Console.WriteLine(p.Key);

foreach (var p in maxUsedPaths) {
  var from = p.Key.from;
  var to = p.Key.to;
  graph[from].Adjacents.Remove(to);
  graph[to].Adjacents.Remove(from);
}
var graph1Nodes = CountGraphNodes(graph, maxUsedPaths[0].Key.from);
var graph2Nodes = CountGraphNodes(graph, maxUsedPaths[0].Key.to);
Console.WriteLine($"Answer: {graph1Nodes * graph2Nodes}");

int CountGraphNodes(Dictionary<string, Component> graph, string start) {
  HashSet<string> Visited = [];
  Queue<string> Unvisited = new();
  Unvisited.Enqueue(start);
  while (Unvisited.Count > 0) {
    var current = Unvisited.Dequeue();
    foreach (var adjacent in graph[current].Adjacents) {
      if (!Visited.Contains(adjacent)) {
        Visited.Add(adjacent);
        Unvisited.Enqueue(adjacent);
      }
    }
  }
  return Visited.Count;
}

Dictionary<string, Component> GenerateGraph(string fileName) {
  Dictionary<string, Component> AllComponents = [];
  foreach (var line in File.ReadAllLines(fileName)) {
    var components = line.Split(' ').Select(c => c.TrimEnd(':')).ToList();
    var baseComponent = components.First();
    if (!AllComponents.ContainsKey(baseComponent))
      AllComponents.Add(baseComponent, new Component(baseComponent));
    var connectedComponents = components[1..];
    foreach (var component in connectedComponents) {
      if (!AllComponents.ContainsKey(component))
        AllComponents.Add(component, new Component(component));
      AllComponents[baseComponent].Adjacents.Add(component);
      AllComponents[component].Adjacents.Add(baseComponent);
    }
  }
  return AllComponents;
}

Component GetRandomComponent(Dictionary<string, Component> AllComponents) {
  var keys = AllComponents.Keys.ToList();
  return AllComponents[keys[Random.Shared.Next(keys.Count)]];
}

// Find fastest path between two random components, putting weight on each edge used
void SimulateTraffic(Dictionary<string, Component> allComponents, Dictionary<(string from, string to), int> weights) {
  var startComponent = GetRandomComponent(allComponents);
  var endComponent = GetRandomComponent(allComponents);
  if (startComponent.Name == endComponent.Name) return; // This won't work
  Dictionary<string, int> ComponentsPassedToReach = [];
  Dictionary<string, string> Previous = [];
  ComponentsPassedToReach.Add(startComponent.Name, 0);
  Queue<Component> Unvisited = [];
  Unvisited.Enqueue(startComponent);
  HashSet<string> Visited = [];
  while (Unvisited.Count > 0) {
    var currentComponent = Unvisited.Dequeue();

    // Check adjacents
    foreach (var adjacent in currentComponent.Adjacents) {
      // If adjacent is endcomponent, likely found the fastest route already
      if (adjacent == endComponent.Name) {
        Previous[adjacent] = currentComponent.Name;
        Unvisited.Clear(); // End outer loop
        break;
      }

      // Else, update distance to adjacent if shorter
      if (ComponentsPassedToReach.ContainsKey(adjacent)) {
        if (ComponentsPassedToReach[adjacent] > ComponentsPassedToReach[currentComponent.Name] + 1) {
          ComponentsPassedToReach[adjacent] = ComponentsPassedToReach[currentComponent.Name] + 1;
          Previous[adjacent] = currentComponent.Name; 
        }
      } else {
        ComponentsPassedToReach.Add(adjacent, ComponentsPassedToReach[currentComponent.Name] + 1);
        Previous[adjacent] = currentComponent.Name;
      }

      // Add adjacent to queue
      Unvisited.Enqueue(allComponents[adjacent]);
    }

    Visited.Add(currentComponent.Name);
  }

  // Now that shortest path has been found, update edge weights
  var current = endComponent.Name;
  while (Previous.ContainsKey(current)) {
    List<string> comps = [Previous[current], current];
    comps.Sort();
    var path = (comps[0], comps[1]);
    if (Weights.ContainsKey(path))
      Weights[path]++;
    else
      Weights.Add(path, 1);
    current = Previous[current];
  }
}

class Component(string Name) {
  public string Name { get; set; } = Name;
  public HashSet<string> Adjacents { get; set;} = [];
  public override string ToString() =>
    $"{Name}";
}