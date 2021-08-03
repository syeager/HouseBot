namespace HouseBot.Server.Data
{
    public static class InputSanitizer
    {
        public static string Sanitize(this string value) => value.Trim().ToLower();
    }
}