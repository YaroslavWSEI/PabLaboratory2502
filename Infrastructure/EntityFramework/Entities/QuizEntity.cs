namespace Infrastructure.Entities;

public class QuizEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    
    public ICollection<QuizItemEntity> Items { get; set; } = new HashSet<QuizItemEntity>();
}