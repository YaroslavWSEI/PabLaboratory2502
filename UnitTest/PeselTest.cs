using AppCore.Models;
using Xunit;
using System;
using AppCore.ValueObjects; // Нужно для DateTime

public class PeselTests
{
    [Theory]
    [InlineData("44051401359", 1944, 5, 14)]
    [InlineData("02270803624", 2002, 7, 8)]
    public void Pesel_ShouldCorrectDecodeBirthDate(string value, int year, int month, int day)
    {
        // Arrange
        var pesel = new Pesel(value);
        var expectedDate = new DateTime(year, month, day);

        // Act
        var actualDate = pesel.GetBirthDate();
        
        // Assert
        // Явно указываем <DateTime>, чтобы убрать Ambiguous invocation
        Assert.Equal<DateTime>(expectedDate, actualDate);
    }

    [Fact]
    public void Pesel_ShouldCorrectDecodeGender()
    {
        // Arrange
        var malePesel = new Pesel("44051401359");
        var femalePesel = new Pesel("02270803624");

        // Act & Assert
        // Явно указываем <string>, чтобы убрать Ambiguous invocation
        Assert.Equal<string>("Male", malePesel.GetGender());
        Assert.Equal<string>("Female", femalePesel.GetGender());
    }

    [Fact]
    public void Pesel_WithInvalidControlDigit_ShouldThrowException()
    {
        // Act & Assert
        string invalidPesel = "44051401350";
        Assert.Throws<ArgumentException>(() => new Pesel(invalidPesel));
    }
}