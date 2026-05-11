using System.Net;
using System.Net.Http.Json;
using Infrastructure.Context;
using Infrastructure.Entities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using AppCore.Dto;

public class QuizApiGetRequestTest : IClassFixture<QuizAppTestFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly QuizAppTestFactory<Program> _app;

    public QuizApiGetRequestTest(QuizAppTestFactory<Program> app)
    {
        _app = app;
        _client = app.CreateClient();

        using var scope = _app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<QuizDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var items = new HashSet<QuizItemEntity>
        {
            new()
            {
                Id = 1, 
                CorrectAnswer = "7", 
                Question = "2 + 5", 
                IncorrectAnswers = new HashSet<QuizItemAnswerEntity>
                {
                    new() {Id = 11, Answer = "5"},
                    new() {Id = 12, Answer = "6"},
                    new() {Id = 13, Answer = "8"},
                }
            }
        };

        context.Quizzes.Add(new QuizEntity
        {
            Id = 1,
            Title = "Matematyka",
            Items = items
        });

        context.SaveChanges();
    }

    [Fact]
    public async Task GetShouldReturnOkStatus()
    {
     
        var result = await _client.GetAsync("/api/quizzes"); 

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Contains("application/json", result.Content.Headers.ContentType?.ToString() ?? "");
    }

    [Fact]
    public async Task GetShouldReturnQuizzes()
    {
        var result = await _client.GetFromJsonAsync<List<QuizDto>>("/api/quizzes");

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Matematyka", result[0].Title);
    }
}