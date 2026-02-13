using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTaskManagement.Application.DTOs.User;

public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}