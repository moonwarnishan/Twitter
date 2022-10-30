namespace RegisterUser.DatabaseSetting
{
    public class DatabaseSetting
    {
        public string usersCollectionName { get; set; }

        public string userSearchCollection { get; set; }
        public string userTimelineCollection { get; set; }
        public string databaseName { get; set; }
        public string connectionString { get; set; }
        public string passwordResetCollection { get; set; }
    }
}
