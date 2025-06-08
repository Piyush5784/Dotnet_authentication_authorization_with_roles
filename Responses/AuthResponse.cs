using System;
using System.IdentityModel.Tokens.Jwt;

namespace Auth_web_2.Responses;

public class AuthResponse
{
    public string? Message { get; set; }
    public AuthStatus Status { get; set; }
    public string? Token { get; set; }
}

public enum AuthStatus
{
    NotFound, UnAuthorised, InvalidCredentails, AlreadyExists, Success
}
