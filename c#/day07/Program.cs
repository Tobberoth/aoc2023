#define STEP1 // Define to run step 1, remove to run step 2

var lines = File.ReadAllLines("input.txt");

List<(List<char> Hand, HandType type, int Bid)> Hands = [];
foreach (var line in lines) {
  var hand = line.Split(' ')[0].ToList();
  var bid = int.Parse(line.Split(' ')[1]);
  var type = FindType(hand);
  Hands.Add((hand, type, bid));
}

Hands.Sort(new HandTypeComparer());
Hands.Reverse();
long sum = 0;
for (var i = 0; i < Hands.Count; i++)
  sum += Hands[i].Bid * (i+1);
Console.WriteLine(sum);

static HandType FindType(List<char> hand) {
  var groups = hand.GroupBy(c => c);
  var groupsCount = groups.Count();
  var jokers = groups.FirstOrDefault(g => g.Key == 'J');
  var jokersCount = jokers?.Count() ?? 0;
  if (groupsCount == 1) return HandType.FIVE_KIND;
  if (groupsCount == 2 && (groups.First().Count() == 4 || groups.Last().Count() == 4)) {
    #if !STEP1
      if (jokersCount > 0) return HandType.FIVE_KIND;
    #endif
    return HandType.FOUR_KIND;
  }
  if (groupsCount == 2 && groups.Any(g => g.Count() == 3) && groups.Any(g => g.Count() == 2)) {
    #if !STEP1
      if (jokersCount > 0) return HandType.FIVE_KIND;
    #endif
    return HandType.FULL_HOUSE;
  }
  if (groups.Any(g => g.Count() == 3)) {
    #if !STEP1
      if (jokersCount == 3) return HandType.FOUR_KIND;
      // two jokers in this case would be handled by full house logic
      if (jokersCount == 1) return HandType.FOUR_KIND;
    #endif
    return HandType.THREE_KIND;
  }
  if (groups.Where(g => g.Count() == 2).Count() == 2) {
    #if !STEP1
      if (jokersCount == 2) return HandType.FOUR_KIND;
      if (jokersCount == 1) return HandType.FULL_HOUSE;
    #endif
    return HandType.TWO_PAIR;
  }
  if (groups.Any(g => g.Count() == 2)) {
    #if !STEP1
      if (jokersCount == 2) return HandType.THREE_KIND;
      if (jokersCount == 1) return HandType.THREE_KIND;
    #endif
    return HandType.ONE_PAIR;
  }
  #if !STEP1
  if (jokersCount > 0) return HandType.ONE_PAIR;
  #endif
  return HandType.HIGH_CARD;
}

public class HandTypeComparer : IComparer<(List<char> hand, HandType type, int bid)> {
    readonly Dictionary<char, int> CardMap = new() {
    { 'A', 1 },
    { 'K', 2 },
    { 'Q', 3 },
#if STEP1
    { 'J', 4 },
#else
    { 'J', 14 },
#endif
    { 'T', 5 },
    { '9', 6 },
    { '8', 7 },
    { '7', 8 },
    { '6', 9 },
    { '5', 10 },
    { '4', 11 },
    { '3', 12 },
    { '2', 13 },
  };

  int IComparer<(List<char> hand, HandType type, int bid)>.Compare((List<char> hand, HandType type, int bid) x, (List<char> hand, HandType type, int bid) y) {
    if ((int)x.type < (int)y.type)
      return -1;
    if ((int)y.type < (int)x.type)
      return 1;
    for (var i = 0; i < x.hand.Count; i++) {
      if (CardMap[x.hand[i]] < CardMap[y.hand[i]])
        return -1;
      if (CardMap[x.hand[i]] > CardMap[y.hand[i]])
        return 1;
    }
    return 0;
  }
}

enum HandType { FIVE_KIND, FOUR_KIND, FULL_HOUSE, THREE_KIND, TWO_PAIR, ONE_PAIR, HIGH_CARD };