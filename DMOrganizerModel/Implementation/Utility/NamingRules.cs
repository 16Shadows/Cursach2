namespace DMOrganizerModel.Implementation.Utility
{
    public static class NamingRules
    {
        public static bool IsValidName(string name)
        {
            //TODO: implement actual checks
            return name.Trim().Length > 0 && !name.Contains('$') && !name.Contains('#') && !name.Contains('/');
        }

        public static bool IsValidTag(string tag)
        {
            return tag.Trim().Length > 0;
        }
    }
}
