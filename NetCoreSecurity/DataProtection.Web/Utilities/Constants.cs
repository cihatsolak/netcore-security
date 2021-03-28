namespace DataProtection.Web.Utilities
{
    /// <summary>
    /// Sabit değerleri tanımlayacağım sınıf
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Url üzerinde query string ifadesi. Örn= {baseurl}?q=
        /// </summary>
        public const string QueryStringPrefix = "q";

        /// <summary>
        /// Url üzerindeki query string ifadesinin ömrü Örn= {baseurl}?q=deneme&t=5
        /// </summary>
        public const string TimeStringPrefix = "t";
    }
}
