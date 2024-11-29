using System;

namespace UserManagerDev.Database.Entities
{
    public partial class User
    {
        public Guid UserId { get; set; }

        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? AvatarUrl { get; set; }

        public string? StatusMessage { get; set; }

        public DateTime? LastSeen { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<ChatMember> ChatMembers { get; set; } = new List<ChatMember>();

        public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

        public virtual ICollection<UserBlock> UserBlockBlockeds { get; set; } = new List<UserBlock>();

        public virtual ICollection<UserBlock> UserBlockBlockers { get; set; } = new List<UserBlock>();

        public virtual UserPreference? UserPreference { get; set; }
    }
}