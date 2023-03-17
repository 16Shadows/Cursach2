namespace DMOrganizerModel.Implementation.Utility
{
    public static class NamingRules
    {
        public static bool IsValidName(string name)
        {
            //TODO: implement actual checks
            return name.Length > 0;
        }

        public static bool IsValidTag(string tag)
        {
            return tag.Length > 0;
        }
    }
}
