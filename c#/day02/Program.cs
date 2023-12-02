var maxRed = 12;
var maxGreen = 13;
var maxBlue = 14;

var sum = 0;
var atLeastProd = 0;

var lines = File.ReadAllLines("input.txt");
foreach (var line in lines) {
  var gameId = int.Parse(line.Split(':')[0].Split(' ')[1]);
  var rest = line.Split(':')[1];
  var games = rest.Split(';');

  var highRed = 0;
  var highGreen = 0;
  var highBlue = 0;

  foreach (var game in games) {
    var data = game.Split(',');
    foreach (var d in data.Select(s => s.Trim())) {
      var amount = int.Parse(d.Split(' ')[0]);
      var color = d.Split(' ')[1];
      switch (color) {
        case "red":
          highRed = Math.Max(highRed, amount);
          break;
        case "green":
          highGreen = Math.Max(highGreen, amount);
        break;
        case "blue":
          highBlue = Math.Max(highBlue, amount);
        break;
      }
    }
  }
  atLeastProd += highRed * highGreen * highBlue;
  if (highRed <= maxRed && highGreen <= maxGreen && highBlue <= maxBlue)
    sum += gameId;
}
Console.WriteLine(sum);
Console.WriteLine(atLeastProd);