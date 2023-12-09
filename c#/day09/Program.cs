
var lines = File.ReadAllLines("input.txt").ToList();
var number_series = lines.Select(l => l.Split(" ").Select(int.Parse).ToList()).ToList();

var step1 = number_series.Select(SeriesToExtrapolation).Sum();
Console.WriteLine(step1);

var step2 = number_series.Select(SeriesToBackExtrapolation).Sum();
Console.WriteLine(step2);

int SeriesToExtrapolation(List<int> series) {
  var currentSeries = series;
  var nextSeries = GetNextSeries(series);
  if (nextSeries.All(i => i == 0))
    return currentSeries.Last();
  else
    return currentSeries.Last() + SeriesToExtrapolation(nextSeries);
}

int SeriesToBackExtrapolation(List<int> series) {
  var currentSeries = series;
  var nextSeries = GetNextSeries(series);
  if (nextSeries.All(i => i == 0))
    return currentSeries.First();
  else
    return currentSeries.First() - SeriesToBackExtrapolation(nextSeries);
}

List<int> GetNextSeries(List<int> series) {
  List<int> ret = [];
  for (var i = 1; i < series.Count; i++)
    ret.Add(series[i] - series[i-1]);
  return ret;
}