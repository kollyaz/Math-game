using System.Diagnostics;


new MathGame().Run();

public class MathGame
{
  private readonly MathQuestionGenerator questionGenerator = new();
  private readonly List<string> prevGames = [];
  private int score;
  private DifficultyLevel difficultyLevel;
  private string playerName = string.Empty;
  private readonly Random random = new();


  public void Run()
  {
    Console.WriteLine("Hello player, whats your name?");
    playerName = Console.ReadLine()!;
    DateTime date = DateTime.UtcNow;

    Console.WriteLine("-------------------------------------------------------------------");
    Console.WriteLine($"Hello {playerName}, its {date.DayOfWeek} choose an operation to solve");
    ShowDifficultyMenu();
  }

  public void ShowDifficultyMenu()
  {
    while (true)
    {
      Console.WriteLine("Choose your difficulty level:");
      Console.WriteLine("1. Easy 2. Medium 3. Hard 4. Exit");

      int diffLevel = int.Parse(Console.ReadLine()!);

      if (diffLevel == 4) return;

      difficultyLevel = diffLevel switch
      {
        1 => DifficultyLevel.Easy,
        2 => DifficultyLevel.Medium,
        3 => DifficultyLevel.Hard,
        _ => throw new ArgumentOutOfRangeException(nameof(diffLevel))
      };
      ShowOperationMenu();
    }

  }

  public void ShowOperationMenu()
  {
    int gameSelected = 0;

    while (gameSelected != 7)
    {
      Console.WriteLine("1. Addition\n2. Subtraction\n3. Multiplication\n4. Division\n5. Play a Random game\n6. Check previous games\n7. Back to main menu");
      Console.WriteLine("-------------------------------------------------------------------");

      gameSelected = int.Parse(Console.ReadLine()!);

      switch (gameSelected)
      {
        case 1:
          PlayQuestion(MathOperation.Addition, difficultyLevel);
          break;
        case 2:
          PlayQuestion(MathOperation.Subtraction, difficultyLevel);
          break;
        case 3:
          PlayQuestion(MathOperation.Multiplication, difficultyLevel);
          break;
        case 4:
          PlayQuestion(MathOperation.Division, difficultyLevel);
          break;
        case 5:
          PlayRandomGame(difficultyLevel);
          break;
        case 6:
          ShowPreviousGames();
          break;
      }
    }


  }

  private void PlayQuestion(MathOperation operation, DifficultyLevel difficultyLevel)
  {
    Stopwatch stopwatch = new();

    MathQuestion question = questionGenerator.CreateQuestion(operation, difficultyLevel);
    Console.WriteLine(question);

    stopwatch.Start();

    int userAns = int.Parse(Console.ReadLine()!);

    stopwatch.Stop();

    bool isCorrect = question.Answer == userAns;

    if (isCorrect)
    {
      score++;
      Console.WriteLine("You are Correct!");
    }
    else
    {
      Console.WriteLine("Oops, you are Wrong!");
    }

    Console.WriteLine($"You took {stopwatch.Elapsed.TotalSeconds:F2} seconds");
    Console.WriteLine($"Your score: {score}");

    prevGames.Add($"{question} = {userAns} | ({(isCorrect ? "Correct" : "Not Correct")}) | {stopwatch.Elapsed.TotalSeconds:F2} seconds");
  }

  private void PlayRandomGame(DifficultyLevel difficultyLevel)
  {
    var operation = Enum.GetValues<MathOperation>();
    MathOperation randomOperation = operation[random.Next(operation.Length)];
    PlayQuestion(randomOperation, difficultyLevel);
  }

  private void ShowPreviousGames()
  {
    for (int i = 0; i < prevGames.Count; i++)
    {
      Console.WriteLine($"{i + 1} {prevGames[i]}");
    }

    Console.WriteLine($"Your total score is {score}");
  }


}

public class MathQuestionGenerator
{
  private readonly Random random = new();

  public MathQuestion CreateQuestion(MathOperation operations, DifficultyLevel difficultyLevel)
  {
    int maxNumber = difficultyLevel switch
    {
      DifficultyLevel.Easy => 10,
      DifficultyLevel.Medium => 100,
      DifficultyLevel.Hard => 1000,
      _ => throw new ArgumentOutOfRangeException(nameof(difficultyLevel))
    };

    int firstNumber = random.Next(1, maxNumber + 1);
    int secondNumber = random.Next(1, maxNumber + 1);

    if (operations == MathOperation.Division)
    {
      do
      {
        firstNumber = random.Next(1, maxNumber + 1);
        secondNumber = random.Next(1, maxNumber + 1);
      } while (firstNumber % secondNumber != 0);
    }

    return new MathQuestion(firstNumber, secondNumber, operations);
  }
}
public class MathQuestion
{
  public MathQuestion(int firstNumber, int secondNumber, MathOperation operation)
  {
    FirstNumber = firstNumber;
    SecondNumber = secondNumber;
    Operation = operation;
  }
  public int FirstNumber { get; }
  public int SecondNumber { get; }
  public MathOperation Operation { get; }

  public int Answer => Operation switch
  {
    MathOperation.Addition => FirstNumber + SecondNumber,
    MathOperation.Subtraction => FirstNumber - SecondNumber,
    MathOperation.Multiplication => FirstNumber * SecondNumber,
    MathOperation.Division => FirstNumber / SecondNumber,
    _ => throw new InvalidOperationException($"Unsupported operation: {Operation}")
  };

  public string Symbol => Operation switch
  {
    MathOperation.Addition => "+",
    MathOperation.Subtraction => "-",
    MathOperation.Multiplication => "*",
    MathOperation.Division => "/",
    _ => throw new InvalidOperationException($"Unsupported operation: {Operation}")

  };
  public override string ToString() => $"{FirstNumber}{Symbol}{SecondNumber}";

}

public enum MathOperation
{
  Addition,
  Subtraction,
  Multiplication,
  Division
}

public enum DifficultyLevel
{
  Easy,
  Medium,
  Hard,
}