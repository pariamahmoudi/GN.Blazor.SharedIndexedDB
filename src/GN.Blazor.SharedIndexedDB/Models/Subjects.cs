namespace GN.Blazor.SharedIndexedDB.Models
{
    public static class Subjects
    {
        public static string CreateDatabase = "create_database";
        public static string DeleteDatabase = "delete_database";
        public static string DatabaseExists = "database_exists";
        public static string CreateStore = "create_store";
        public static string StorePut = "store_put";
        public static string StoreCount = "store_count";
        public static string StoreFetch = "store_fetch";
        public static string GetDatabaseSchema = "get_schema";
        public static string Reply = "reply";
        public static string Error = "error";
        public static string DeleteById = "delete_by_id";
        public static string GetRecordByID = "get_by_id";
        public static string DeleteWithExpression = "delete_by_expression";
    }
}
