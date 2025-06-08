using System;

namespace Auth_web_2.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public Roles Role { get; set; } = Roles.User;
}

public enum Roles
{
    Admin, User
}
