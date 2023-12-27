var lines = File.ReadAllLines("input.txt").Select(l => new Line(l)).ToList();

Console.WriteLine(Step1(lines, 200000000000000, 400000000000000));

long Step1(List<Line> lines, double minPos, double maxPos) {
  long futureSum = 0;
  Queue<Line> LinesToCompare = new();
  foreach (var line in lines)
    LinesToCompare.Enqueue(line);
  while (LinesToCompare.Count > 0) {
    var currentLine = LinesToCompare.Dequeue();
    foreach (var line in LinesToCompare) {
      var intersection = Line.GetIntersection(currentLine, line);
      if (intersection == (-1, -1)) {
        Console.WriteLine($"{currentLine} {line} = Parallel");
        continue;
      }
      if (intersection.x < minPos || intersection.x > maxPos || intersection.y < minPos || intersection.y > maxPos) {
        Console.WriteLine($"{currentLine} {line} = Cross outside test area at ({intersection.x}, {intersection.y})");
        continue;
      }
      // Check if intersection in the past or future
      var future = false;
      if (currentLine.IsPointInTheFuture(intersection) && line.IsPointInTheFuture(intersection))
        future = true;
      if (future) {
        Console.WriteLine($"{currentLine} {line} = Cross at ({intersection.x}, {intersection.y}) inside test area in the future");
        futureSum++;
      } else
        Console.WriteLine($"{currentLine} {line} = Cross at ({intersection.x}, {intersection.y}) inside test area in the past");
    }
  }
  return futureSum;
}

public class Line {
  public (double x, double y) StartPoint;
  public (double deltax, double deltay) delta;
  public (double x, double y) EndPoint => (StartPoint.x + delta.deltax, StartPoint.y + delta.deltay);

  public Line(string line) {
    var startLine = line.Split(" @ ")[0];
    var startData = startLine.Split(", ").Select(double.Parse).ToList();
    StartPoint = (startData[0], startData[1]);
    var deltaLine = line.Split(" @ ")[1];
    var deltaData = deltaLine.Split(", ").Select(double.Parse).ToList();
    // For some reason, small deltas can cause issues with calculation
    delta = (deltaData[0]*2000, deltaData[1]*2000);
  }

  public bool IsPointInTheFuture((double x, double y) point) {
    bool goingTowardsX = false;
    if ((delta.deltax > 0 && point.x > StartPoint.x) || (delta.deltax < 0 && point.x < StartPoint.x))
      goingTowardsX = true;
    return goingTowardsX;
  }

  public string GetSlopeForm() {
    var slope = (double)(EndPoint.y - StartPoint.y) / (EndPoint.x - StartPoint.x);
    var mx = slope * StartPoint.x;
    var b = StartPoint.y - mx;
    return $"y = {slope}x + {b}";
  }

  public static (double x, double y) GetIntersection(Line l1, Line l2) {
    var a1 = l1.StartPoint;
    var a2 = l1.EndPoint;
    var b1 = l2.StartPoint;
    var b2 = l2.EndPoint;

    double denominator = ((a1.x - a2.x) * (b1.y - b2.y) - (a1.y - a2.y) * (b1.x - b2.x));
    if (denominator == 0)
      return (-1, -1);
    var x =
      (((a1.x * a2.y) - (a1.y * a2.x)) * (b1.x - b2.x) - (a1.x - a2.x) * (b1.x * b2.y - b1.y * b2.x)) / denominator;
    var y = 
      (((a1.x * a2.y) - (a1.y * a2.x)) * (b1.y - b2.y) - (a1.y - a2.y) * (b1.x * b2.y - b1.y * b2.x)) / denominator;

    return (x, y);
  }
}