namespace Bugger.Core;

public static class TaskView
{
    // A completed task stays in the list for the rest of its completion day, then disappears.
    public static bool Listed(TaskItem task, DateTime now) => task.Status != "Done" || task.CompletedAt?.Date == now.Date;

    // A finished one-time task has no future occurrence, so its record is dropped once its day has passed.
    public static bool Retired(TaskItem task, DateTime now) => task.Status == "Done" && task.OneTime
        && (task.CompletedAt?.Date ?? DateTime.MinValue) < now.Date;

    public static bool VisibleToday(TaskItem task, DateTime now) => task.Enabled && (task.Scheduled(now.Date)
        || (task.Status == "Pending" && task.WindowEnd > now) || (task.Status == "Done" && task.CompletedAt?.Date == now.Date));

    public static DateTime? NextScheduledStart(TaskItem task, DateTime now)
    {
        if (!task.Enabled) return null;
        if (task.OneTime)
        {
            var start = task.OneTimeDate.Date + task.StartTime;
            return start > now ? start : null;
        }
        for (var offset = 0; offset <= 7; offset++)
        {
            var day = now.Date.AddDays(offset);
            var start = day + task.StartTime;
            if (start > now && task.Scheduled(day) && !(task.Status == "Done" && task.OccurrenceDate == day)) return start;
        }
        return null;
    }
}
