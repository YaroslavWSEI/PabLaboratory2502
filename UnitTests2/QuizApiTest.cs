namespace UnitTests2;

public class QuizApiTest
{
    [Fact]
    public async void GetShouldReturnTwoQuizzes()
    {
        //Arrange
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
    
        //Act
        var result = await client.GetFromJsonAsync<List<QuizDto>>("/api/v1/quizzes");
    
        //Assert
        Assert.Equal(3, result.Count);
    }
}