using System;
using System.ComponentModel.DataAnnotations;
using MacrixT.Attributes;
using MacrixT.Validation;

namespace MacrixT.Models;

public class Person : PropertyValidateModel, IEquatable<Person>
{
    public Guid Id { get; set; }
    
    private string _firstName;
    
    [Required]
    [StringLength(10)]
    [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Only letters are expected")]
    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }
    
    private string _lastName;
    
    [Required]
    [StringLength(20)]
    [RegularExpression("^[a-zA-Z-]*$", ErrorMessage = "Only letters or dash are expected. e.g. Linek or Linek - Linkowski")]
    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }

    private string _streetName;
    
    [Required]
    [StringLength(20)]
    [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only letters and numbers are expected")]
    public string StreetName
    {
        get => _streetName;
        set => SetProperty(ref _streetName, value);
    }

    private string _houseNumber;
    
    [Required]
    [StringLength(7)]
    [RegularExpression("^[a-zA-Z0-9-\\/]*$", ErrorMessage = "Only letters and numbers are expected")]
    public string HouseNumber
    {
        get => _houseNumber;
        set => SetProperty(ref _houseNumber, value);
    }

    private string _apartmentNumber;
    
    [StringLength(7)]
    [RegularExpression("^[a-zA-Z0-9-\\/]*$", ErrorMessage = "Only letters and numbers are expected")]
    public string ApartmentNumber
    {
        get => _apartmentNumber;
        set => SetProperty(ref _apartmentNumber, value);
    }

    private string _postalCode;
    
    [Required]
    [RegularExpression("\\d{5}", ErrorMessage = "Five digits expected as a Postal Code, e.g. 12345")]
    public string PostalCode
    {
        get => _postalCode;
        set => SetProperty(ref _postalCode, value);
    }

    private string _phoneNumber;
    
    [Required]
    [Phone]
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }

    private DateTime _dateOfBirth = DateTime.Today.AddYears(-20);
    
    [Required]
    [AgeDateTimeBased(18, 130)]
    public DateTime DateOfBirth
    {
        get => _dateOfBirth;
        set
        {
            SetProperty(ref _dateOfBirth, value);
            OnPropertyChanged(nameof(Age));
        }
    }

    public int Age => DateTime.Today.Year - DateOfBirth.Year;

    public bool Equals(Person? other)
    {
        return Id.Equals(other?.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == this.GetType() && Equals((Person)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();
}