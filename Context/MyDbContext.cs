using System;
using Auth_web_2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Auth_web_2.Context;

public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{

    public DbSet<User> Users { get; set; }
}
