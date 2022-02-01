namespace Vit.Extensions
{
    public static partial class Ioc_Populate_Extensions
    {

        public static Ioc.Ioc Populate(this Ioc.Ioc data, string configPath = "Ioc")
        {
            data?.rootServiceCollection.Populate(configPath);
            return data;
        }
 

    }
}
