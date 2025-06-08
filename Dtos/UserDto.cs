using System;
using System.ComponentModel.DataAnnotations;

namespace Auth_web_2.Dtos;

public class UserDto
{
    [Required(ErrorMessage = "Username is required")]
    [MinLength(4, ErrorMessage = "Minimun username length is 4 digits")]
    public string Username { get; set; } = string.Empty;


    [Required(ErrorMessage = "Password is required")]
    [MinLength(4, ErrorMessage = "Minimun Password length is 4 digits")]
    public string Password { get; set; } = string.Empty;
}
