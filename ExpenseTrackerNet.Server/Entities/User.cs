﻿using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerNetApp.ApiService.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = [];
    }
}
