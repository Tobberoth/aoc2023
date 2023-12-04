var lines = File.ReadAllLines("input.txt");
int sum = 0;
List<int> cardAmounts = lines.Select(l => 1).ToList();

var current_card_index = 0;
foreach (var line in lines) {
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