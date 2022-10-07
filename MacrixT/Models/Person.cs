using System;

namespace MacrixT.Models;

public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string StreetName { get; set; }
    public string HouseNumber { get; set; }
    public string ApartmentNumber { get; set; }
    public string PostalCode { get; set; }
    public string PhoneNumber { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public int Age => 0; // todo
}