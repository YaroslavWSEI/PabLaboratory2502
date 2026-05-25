namespace AppCore.ValueObjects;

public record Pesel
{
    public string Value { get; init; }

    public Pesel(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException("Invalid PESEL number.");
        Value = value;
    }

    public static bool IsValid(string pesel)
    {
        if (pesel == null || pesel.Length != 11 || !pesel.All(char.IsDigit))
            return false;

        int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (pesel[i] - '0') * weights[i];

        int controlDigit = (10 - (sum % 10)) % 10;
        return controlDigit == (pesel[10] - '0');
    }

    public DateTime GetBirthDate()
    {
        int year = int.Parse(Value.Substring(0, 2));
        int month = int.Parse(Value.Substring(2, 2));
        int day = int.Parse(Value.Substring(4, 2));
        if (month > 80) { year += 1800; month -= 80; }
        else if (month > 60) { year += 2200; month -= 60; }
        else if (month > 40) { year += 2100; month -= 40; }
        else if (month > 20) { year += 2000; month -= 20; }
        else { year += 1900; }
        return new DateTime(year, month, day);
    }
    public string GetGender()
    {
        int genderDigit = Value[9] - '0';
        return genderDigit % 2 == 0 ? "Female" : "Male";
    }
    public override string ToString() => Value;
}