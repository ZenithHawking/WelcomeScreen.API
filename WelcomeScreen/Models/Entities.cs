namespace WelcomeScreen.API.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Admin";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class WelcomeScreen
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Active, Completed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public DataSource? DataSource { get; set; }
    public List<EventField> Fields { get; set; } = new();
    public ScreenConfig? ScreenConfig { get; set; }
}

public class DataSource
{
    public int Id { get; set; }
    public int WelcomeScreenId { get; set; }
    public string DbType { get; set; } = "SqlServer";
    public string ConnectionString { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string TriggerColumn { get; set; } = string.Empty;
    public long LastPolledId { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public WelcomeScreen WelcomeScreen { get; set; } = null!;
}

public class EventField
{
    public int Id { get; set; }
    public int WelcomeScreenId { get; set; }
    public string SourceColumn { get; set; } = string.Empty;
    public string? DisplayLabel { get; set; }
    public int Order { get; set; }
    public string FontFamily { get; set; } = "Inter";
    public int FontSize { get; set; } = 48;
    public string FontColor { get; set; } = "#FFFFFF";
    public string FontWeight { get; set; } = "bold";
    public string TextAlign { get; set; } = "center";
    public double PositionX { get; set; } = 50;
    public double PositionY { get; set; } = 50;
    public double Width { get; set; } = 80;
    public string AnimationType { get; set; } = "fadeIn";
    public int AnimationDuration { get; set; } = 1000;
    public int AnimationDelay { get; set; } = 0;

    public WelcomeScreen WelcomeScreen { get; set; } = null!;
}

public class MediaFile
{
    public int Id { get; set; }
    public int? WelcomeScreenId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty; // image/png, video/mp4
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public WelcomeScreen? WelcomeScreen { get; set; }
}

public class ScreenConfig
{
    public int Id { get; set; }
    public int WelcomeScreenId { get; set; }
    public int? BackgroundMediaId { get; set; }
    public string BackgroundColor { get; set; } = "#000000";
    public int CanvasWidth { get; set; } = 1920;
    public int CanvasHeight { get; set; } = 1080;
    public string? LayoutJson { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public WelcomeScreen WelcomeScreen { get; set; } = null!;
    public MediaFile? BackgroundMedia { get; set; }
}