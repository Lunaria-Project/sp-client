public static class LongExtension
{ 
    public static string ToCommaString(this long value)
    {
        return value.ToString("N0");
    }
}
