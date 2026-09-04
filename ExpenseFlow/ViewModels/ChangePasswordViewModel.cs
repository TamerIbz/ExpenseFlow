using System.ComponentModel.DataAnnotations;

namespace ExpenseFlow.ViewModels;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; }=string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    [StringLength(40,MinimumLength = 8, ErrorMessage="the {0} must be at {2} and at max {1} characters long.")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    [Compare("ConfirmNewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string NewPassword { get; set; }=string.Empty;
    [Required(ErrorMessage = "Confirm Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string ConfirmNewPassword { get; set; }=string.Empty;
}