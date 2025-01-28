using System.ComponentModel.DataAnnotations;
using SyncIT.Sync.Models;

namespace SyncIT.Web.Util;

public class EmailValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string email) return new ValidationResult("Email must be a string");
        
        return !EmailAddress.IsValid(email) ? new ValidationResult("Invalid email address") : ValidationResult.Success!;
    }
}