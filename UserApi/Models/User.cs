﻿namespace UserApi.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NickName { get; set; }
        public string? HashPassword { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
