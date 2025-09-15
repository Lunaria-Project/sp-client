public static class IntExtension
{ 
    public static string ToCommaString(this int value)
    {
        return value.ToString("N0");
    }
}
