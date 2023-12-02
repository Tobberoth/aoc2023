var maxRed = 12;
var maxGreen = 13;
var maxBlue = 14;

var sum = 0;
var leastProd = 0;

var lines = File.ReadAllLines("input.txt");
foreach (var line in lines) {
  var goodID = true;
  var gameId = int.Parse(line.Split(':')[0].Split(' ')[1]);
  var rest = line.Split(':')[1];
  var games = rest.Split(';');

  var leastRed = 0;
  var leastGreen = 0;
  var leastBlue = 0;

  foreach (var game in games) {
    var data = game.Split(',');
    foreach (var d in data.Select(s => s.Trim())) {
      var amount = int.Parse(d.Split(' ')[0]);
      var color = d.Split(' ')[1];
      switch (color) {
        case "red":
          leastRed = Math.Max(leastRed, amount);
          if (amount > maxRed) {
            goodID = false;
          }
          break;
        case "green":
          leastGreen = Math.Max(leastGreen, amount);
          if (amount > maxGreen) {
            goodID = false;
          }
        break;
        case "blue":
          leastBlue = Math.Max(leastBlue, amount);
          if (amount > maxBlue) {
            goodID = false;
          }
        break;
      }
    }
  }
  leastProd += leastRed * leastGreen * leastBlue;
  if (goodID)
    sum += gameId;
}
Console.WriteLine(sum);
Console.WriteLine(leastProd);