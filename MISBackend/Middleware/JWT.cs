﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MISBackend.Middleware
{
    public class JWT
    {
        public static string GenerateToken(IConfiguration _configuration, Guid Id)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? ""));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"] ?? "",
                audience: _configuration["JwtSettings:Audience"] ?? "",
                expires: DateTime.Now.AddHours(4),
                signingCredentials: credentials,
                claims: new[]
                {
                    new Claim(ClaimTypes.PrimarySid, Id.ToString()), // Contoh ID pengguna
                    new Claim(ClaimTypes.Role, "Admin")
                }
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
