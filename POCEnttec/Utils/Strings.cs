namespace POCEnttec.Utils
{
    public static class Strings
    {
        public static string NullTerminated(this string input) => input + char.MinValue;
    }
}