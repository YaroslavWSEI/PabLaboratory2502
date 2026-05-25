using AppCore.Models;
using Xunit;
using System;
using AppCore.ValueObjects;

namespace UnitTests;

public class PeselTests
{
    [Theory]
    [InlineData("44051401359", 1944, 5,  14)]
    [InlineData("02270803628", 2002, 7,   8)]
    [InlineData("00810100020", 1800, 1,   1)]
    public void GetBirthDate_ShouldReturnCorrectDate(
        string pesel, int year, int month, int day)
    {
        var p = new Pesel(pesel);
        Assert.Equal(new DateTime(year, month, day), p.GetBirthDate());
    }
    [Theory]
    [InlineData("44051401359", true)]
    [InlineData("44051401350", false)]
    [InlineData("12345678901", false)]
    public void IsValid_ShouldMatchExpectedForWellFormedInput(string pesel, bool expected)
    {
        Assert.Equal(expected, Pesel.IsValid(pesel));
    }

    [Theory]
    [InlineData(null,          false)]
    [InlineData("",            false)]
    [InlineData("123",         false)]
    [InlineData("1234567890a", false)]
    public void IsValid_ShouldReturnFalse_ForMalformedInput(string? pesel, bool _)
    {
        Assert.False(Pesel.IsValid(pesel!));
    }

    [Fact]
    public void Constructor_InvalidPesel_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Pesel("12345678901"));
    }

    [Fact]
    public void Constructor_ValidPesel_ShouldStoreValue()
    {
        var p = new Pesel("44051401359");
        Assert.Equal("44051401359", p.Value);
    }
    [Theory]
    [InlineData("44051401359", "Male")]
    [InlineData("02270803628", "Female")]
    public void GetGender_ShouldReturnCorrect(string pesel, string expected)
    {
        var p = new Pesel(pesel);
        Assert.Equal(expected, p.GetGender());
    }
}