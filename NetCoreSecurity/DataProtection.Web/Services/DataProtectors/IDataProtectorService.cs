namespace DataProtection.Web.Services.DataProtectors
{
    /// <summary>
    /// Veriyi şifreleme ve şifresini çözme
    /// </summary>
    public interface IDataProtectorService
    {
        /// <summary>
        /// Veri Şifreleme
        /// </summary>
        /// <param name="value">Şifrelenecek veri</param>
        /// <param name="minute">Şifrelenecek verinin süresi</param>
        /// <returns>Şifrelenmiş veri</returns>
        string Protect(string value, int minute = 0);

        /// <summary>
        /// Veri Şifre Çözücü
        /// </summary>
        /// <param name="value">Şifrelenmiş veri</param>
        /// <returns>Şifresi çözülmüş veri</returns>
        string Unprotect(string value);
    }
}
