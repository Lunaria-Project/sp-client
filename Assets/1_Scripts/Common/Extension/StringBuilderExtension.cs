using System.Text;

public static class StringBuilderExtension
{
    private const string Indent = "    ";
    
    public static StringBuilder AppendIndentedLine(this StringBuilder stringBuilder, string text, int indentCount)
    {
        for (var i = 0; i < indentCount; i++)
        {
            stringBuilder.Append(Indent); 
        }
        stringBuilder.Append(text); 
        stringBuilder.AppendLine();
        return stringBuilder;
    }
}