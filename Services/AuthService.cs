using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth_web_2.Context;
using Auth_web_2.Dtos;
using Auth_web_2.Models;
using Auth_web_2.Responses;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Auth_web_2.Services;

public class AuthService
{
    //Db context
    private readonly MyDbContext context;
    private readonly IConfiguration configuration;

    public AuthService(MyDbContext context, IConfiguration configuration)
    {
        this.context = context;
        this.configuration = configuration;
    }


    //Register user
    public async Task<AuthResponse> RegisterUser(UserDto userDetails)
    {
        User? checkUser = await context.Users.FirstOrDefaultAsync((user) => user.Username == userDetails.Username);

        // Check if user already exists or not
        if (checkUser is not null)
        {
            return new AuthResponse
            {
                Status = AuthStatus.AlreadyExists
            };
        }


        // Creates new user in the database
        var newUser = new User
        {
            Username = userDetails.Username
        };

        newUser.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userDetails.Password, 12);
        context.Users.Add(newUser);
        await context.SaveChangesAsync();

        return new AuthResponse
        {
            Status = AuthStatus.Success
        };
    }

    public async Task<AuthResponse> LoginUser(UserDto userDetails)
    {
        User? checkUser = await context.Users.FirstOrDefaultAsync((user) => user.Username == userDetails.Username);

        //Check for existing user
        if (checkUser is null)
        {
            return new AuthResponse
            {
                Status = AuthStatus.NotFound
            };
        }

        // verifying password
        bool verify = BCrypt.Net.BCrypt.EnhancedVerify(userDetails.Password, checkUser.Password);

        if (!verify)
        {
            return new AuthResponse
            {
                Status = AuthStatus.UnAuthorised
            };
        }

        //Generating token
        var Token = GenerateJwtToken(checkUser);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(Token);

        return new AuthResponse
        {
            Status = AuthStatus.Success,
            Token = tokenString
        };


    }

    private JwtSecurityToken GenerateJwtToken(User user)
    {
        //claims of the user 
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name,user.Username),
            new Claim(ClaimTypes.Role,user.Role.ToString())
        };

        //getting key from appsetting
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JwtSettings:SecretKey")!));

        //Generating token
        var token = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("JwtSettings:Issuer"),
            audience: configuration.GetValue<string>("JwtSettings:Audience"),
            claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}
