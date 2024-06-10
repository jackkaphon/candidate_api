﻿using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Dto.UserDto
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }
    }
}
