using System.Configuration;

namespace AlexaTVInfoSkill.Service
{
    public class Config
    {

        public class DocumentDb
        {
            public static string Endpoint => ConfigurationManager.AppSettings["DocumentDbEndpoint"];

            public static string AuthKey => ConfigurationManager.AppSettings["DocumentDbAuthKey"];

            public static string DatabaseId => ConfigurationManager.AppSettings["DocumentDbDatabaseId"];
        }

    }
}
