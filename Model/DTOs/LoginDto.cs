using System;

namespace Project.APIs.Model.DTOs;

public class LoginDto
{
    public required string Username { get; set; }
    public required string HashPassword { get; set; }
}
