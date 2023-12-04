var lines = File.ReadAllLines("input.txt");
int sum = 0;
List<int> cardAmounts = [];
for (var i = 0; i < lines.Length; i++) {
  cardAmounts.Add(1);
}
var current_card_index = 0;
foreach (var line in lines) {
  var cardSide = line.Split(':')[0];
  var cardID = int.Parse(cardSide.Replace(" ", "").Replace("Card", ""));
  var numberSide = line.Split(':')[1];
  var winning = numberSide.Split('|')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
  var numbers = numberSide.Split('|')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
  var matches = numbers.Intersect(winning).Count();
  for (var next_i = current_card_index+1; next_i <= current_card_index+matches; next_i++)
    cardAmounts[next_i] += cardAmounts[current_card_index] * 1;
  sum += (int)Math.Floor(Math.Pow(2, matches-1));
  current_card_index++;
}
Console.WriteLine(sum);
Console.WriteLine(cardAmounts.Sum());