using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class QuizDbContext : DbContext
{
    public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options) { }

    public DbSet<QuizEntity> Quizzes { get; set; }
    public DbSet<QuizItemEntity> QuizItems { get; set; }
    public DbSet<QuizItemAnswerEntity> QuizItemAnswers { get; set; }
}