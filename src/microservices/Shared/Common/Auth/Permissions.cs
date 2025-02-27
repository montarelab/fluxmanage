namespace Common.Auth;

public enum Resources
{
    Task,
    Project
}

public enum Actions
{
    Create,
    Read,
    Update,
    Delete,
    Assignee
}

public static class Permissions
{
    public static string GetPermission(Resources resources, Actions actions)
    {
        return $"{resources}.{actions}";
    }
}

