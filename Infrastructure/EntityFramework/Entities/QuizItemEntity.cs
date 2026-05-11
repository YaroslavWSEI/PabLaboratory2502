namespace Infrastructure.Entities;

public class QuizItemEntity
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public ICollection<QuizItemAnswerEntity> IncorrectAnswers { get; set; } = new HashSet<QuizItemAnswerEntity>();
}