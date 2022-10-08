using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace MacrixT.Validation;

public abstract class PropertyValidateModel : ObservableObject, IDataErrorInfo    
{    
    public string Error => string.Empty;

    public string this[string columnName]    
    {
        get
        {
            var prop = GetType()?.GetProperty(columnName);
            if (prop is null)
            {
                return string.Empty;
            }

            var value = prop.GetValue(this);
            
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateProperty
            (
                value,
                new ValidationContext(this) { MemberName = columnName },
                validationResults
            );

            if (isValid)
            {
                return string.Empty;
            }

            return validationResults.First().ErrorMessage ?? string.Empty;
        }
    }

    public bool IsValid() =>
        Validator.TryValidateObject(this, new ValidationContext(this), new List<ValidationResult>(), true);
} 