
var lines = File.ReadAllLines("input.txt");
var number_series = lines.Select(l => l.Split(" ").Select(int.Parse));
Console.WriteLine(number_series.Select(SeriesToExtrapolation).Sum());
Console.WriteLine(number_series.Select(SeriesToBackExtrapolation).Sum());

int SeriesToExtrapolation(IEnumerable<int> currentSeries) {
  var nextSeries = GetNextSeries(currentSeries);
  if (nextSeries.All(i => i == 0))
    return currentSeries.Last();
  return currentSeries.Last() + SeriesToExtrapolation(nextSeries);
}

int SeriesToBackExtrapolation(IEnumerable<int> currentSeries) {
  var nextSeries = GetNextSeries(currentSeries);
  if (nextSeries.All(i => i == 0))
    return currentSeries.First();
  return currentSeries.First() - SeriesToBackExtrapolation(nextSeries);
}

IEnumerable<int> GetNextSeries(IEnumerable<int> series) {
  var seriesList = series.ToList();
  for (var i = 1; i < seriesList.Count; i++)
    yield return seriesList[i] - seriesList[i-1];
}