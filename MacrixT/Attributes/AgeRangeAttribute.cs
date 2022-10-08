using System;
using System.ComponentModel.DataAnnotations;

namespace MacrixT.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class AgeDateTimeBasedAttribute : ValidationAttribute
{
    public int MinAge { get; }
    
    public int MaxAge { get; }

    public AgeDateTimeBasedAttribute(int minAge, int maxAge)
    {
        MinAge = minAge;
        MaxAge = maxAge;
    }
    
    public override bool IsValid(object? value)
    {
        if (value is not DateTime dt)
        {
            return true;
        }

        var age = DateTime.Today.Year - dt.Year;
        return age >= MinAge && age <= MaxAge;
    }
    
    public override string FormatErrorMessage(string _)
    {
        return $"Age has to be between {MinAge} and {MaxAge}.";
    }
}