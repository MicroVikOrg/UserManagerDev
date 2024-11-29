using Microsoft.EntityFrameworkCore;
using System;

namespace UserManagerDev.Database.Entities
{
    public partial class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public virtual DbSet<Chat> Chats { get; set; }

        public virtual DbSet<ChatMember> ChatMembers { get; set; }

        public virtual DbSet<MediaFile> MediaFiles { get; set; }

        public virtual DbSet<Message> Messages { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserBlock> UserBlocks { get; set; }

        public virtual DbSet<UserPreference> UserPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasKey(e => e.ChatId).HasName("chats_pkey");

                entity.ToTable("chats");

                entity.Property(e => e.ChatId)
                    .ValueGeneratedNever()
                    .HasColumnName("chat_id");
                entity.Property(e => e.ChatDescription).HasColumnName("chat_description");
                entity.Property(e => e.ChatImageUrl).HasColumnName("chat_image_url");
                entity.Property(e => e.ChatName)
                    .HasMaxLength(100)
                    .HasColumnName("chat_name");
                entity.Property(e => e.ChatType)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("'private'::character varying")
                    .HasColumnName("chat_type");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at");
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true)
                    .HasColumnName("is_active");
                entity.Property(e => e.IsGroup)
                    .HasDefaultValue(false)
                    .HasColumnName("is_group");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<ChatMember>(entity =>
            {
                entity.HasKey(e => new { e.ChatId, e.UserId }).HasName("chat_members_pkey");

                entity.ToTable("chat_members");

                entity.Property(e => e.ChatId).HasColumnName("chat_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.CanAddMembers)
                    .HasDefaultValue(false)
                    .HasColumnName("can_add_members");
                entity.Property(e => e.CanPost)
                    .HasDefaultValue(true)
                    .HasColumnName("can_post");
                entity.Property(e => e.CanRemoveMembers)
                    .HasDefaultValue(false)
                    .HasColumnName("can_remove_members");
                entity.Property(e => e.JoinedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("joined_at");
                entity.Property(e => e.LeftAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("left_at");
                entity.Property(e => e.Role)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("'member'::character varying")
                    .HasColumnName("role");

                entity.HasOne(d => d.Chat).WithMany(p => p.ChatMembers)
                    .HasForeignKey(d => d.ChatId)
                    .HasConstraintName("chat_members_chat_id_fkey");

                entity.HasOne(d => d.User).WithMany(p => p.ChatMembers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("chat_members_user_id_fkey");
            });

            modelBuilder.Entity<MediaFile>(entity =>
            {
                entity.HasKey(e => e.MediaId).HasName("media_files_pkey");

                entity.ToTable("media_files");

                entity.Property(e => e.MediaId)
                    .ValueGeneratedNever()
                    .HasColumnName("media_id");
                entity.Property(e => e.FileSize).HasColumnName("file_size");
                entity.Property(e => e.FileType)
                    .HasMaxLength(50)
                    .HasColumnName("file_type");
                entity.Property(e => e.FileUrl).HasColumnName("file_url");
                entity.Property(e => e.IsPublic)
                    .HasDefaultValue(false)
                    .HasColumnName("is_public");
                entity.Property(e => e.UploadDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("upload_date");
                entity.Property(e => e.UploaderId).HasColumnName("uploader_id");

                entity.HasOne(d => d.Uploader).WithMany(p => p.MediaFiles)
                    .HasForeignKey(d => d.UploaderId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("media_files_uploader_id_fkey");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.MessageId).HasName("messages_pkey");

                entity.ToTable("messages");

                entity.HasIndex(e => new { e.ChatId, e.CreatedAt }, "idx_messages_chat_id_created_at");

                entity.Property(e => e.MessageId)
                    .ValueGeneratedNever()
                    .HasColumnName("message_id");
                entity.Property(e => e.AttachmentUrl).HasColumnName("attachment_url");
                entity.Property(e => e.ChatId).HasColumnName("chat_id");
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false)
                    .HasColumnName("is_deleted");
                entity.Property(e => e.IsEdited)
                    .HasDefaultValue(false)
                    .HasColumnName("is_edited");
                entity.Property(e => e.SenderId).HasColumnName("sender_id");
                entity.Property(e => e.Tags).HasColumnName("tags");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.Chat).WithMany(p => p.Messages)
                    .HasForeignKey(d => d.ChatId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("messages_chat_id_fkey");

                entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("messages_sender_id_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("users_pkey");

                entity.ToTable("users");

                entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

                entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("user_id");
                entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at");
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");
                entity.Property(e => e.LastSeen)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("last_seen");
                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(100)
                    .HasColumnName("password_hash");
                entity.Property(e => e.StatusMessage)
                    .HasMaxLength(255)
                    .HasColumnName("status_message");
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at");
                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<UserBlock>(entity =>
            {
                entity.HasKey(e => new { e.BlockerId, e.BlockedId }).HasName("user_blocks_pkey");

                entity.ToTable("user_blocks");

                entity.HasIndex(e => e.BlockerId, "idx_user_blocks_blocker_id");

                entity.Property(e => e.BlockerId).HasColumnName("blocker_id");
                entity.Property(e => e.BlockedId).HasColumnName("blocked_id");
                entity.Property(e => e.BlockedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("blocked_at");

                entity.HasOne(d => d.Blocked).WithMany(p => p.UserBlockBlockeds)
                    .HasForeignKey(d => d.BlockedId)
                    .HasConstraintName("user_blocks_blocked_id_fkey");

                entity.HasOne(d => d.Blocker).WithMany(p => p.UserBlockBlockers)
                    .HasForeignKey(d => d.BlockerId)
                    .HasConstraintName("user_blocks_blocker_id_fkey");
            });

            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("user_preferences_pkey");

                entity.ToTable("user_preferences");

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("user_id");
                entity.Property(e => e.NotificationSound)
                    .HasDefaultValue(true)
                    .HasColumnName("notification_sound");
                entity.Property(e => e.PrivacyLevel)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("'friends_only'::character varying")
                    .HasColumnName("privacy_level");
                entity.Property(e => e.ReceiveNotifications)
                    .HasDefaultValue(true)
                    .HasColumnName("receive_notifications");
                entity.Property(e => e.Theme)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("'light'::character varying")
                    .HasColumnName("theme");

                entity.HasOne(d => d.User).WithOne(p => p.UserPreference)
                    .HasForeignKey<UserPreference>(d => d.UserId)
                    .HasConstraintName("user_preferences_user_id_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}