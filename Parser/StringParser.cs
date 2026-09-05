namespace BoomifyCS.Parser
{
    public class StringParser
    {
        public static string SliceStringFromDelimiter(string str, string delimiter)
        {
            int index = str.IndexOf(delimiter);

            if (index == -1)
            {
                return string.Empty;
            }

            return str[index..];
        }
    }
}
