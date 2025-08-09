using testingSite.Data;
using testingSite.Models;

public class AppLogger : IAppLogger
{
    private readonly AppDbContext _context;

    public AppLogger(AppDbContext context)
    {
        _context = context;
    }

    public void Log(int userId, string actionType, string actionText)
    {
        var log = new Log
        {
            UserId = userId,
            ActionType = actionType,
            ActionText = actionText,
            Timestamp = DateTime.Now
        };
        _context.Logs.Add(log);
        _context.SaveChanges();
    }
    
    
}
