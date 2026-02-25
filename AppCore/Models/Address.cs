using AppCore.ValueObjects;

namespace AppCore.Models;

public class Address
{
    public int id { get; set; }
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string ZipCode { get; set; }
    public required Country Country { get; set; }
}