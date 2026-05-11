namespace AppCore.Dto;

public class QuizDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<QuizItemDto> Items { get; set; } = new();
}

public class QuizItemDto
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public List<QuizItemAnswerDto> IncorrectAnswers { get; set; } = new();
}

public class QuizItemAnswerDto
{
    public int Id { get; set; }
    public string Answer { get; set; } = string.Empty;
}