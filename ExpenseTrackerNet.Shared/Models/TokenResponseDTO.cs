﻿namespace ExpenseTrackerNet.Shared.Models
{
    public class TokenResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
