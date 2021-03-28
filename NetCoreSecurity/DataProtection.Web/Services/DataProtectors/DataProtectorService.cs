using Microsoft.AspNetCore.DataProtection;
using System;

namespace DataProtection.Web.Services.DataProtectors
{
    /// <summary>
    /// Veriyi şifreleme ve şifresini çözme Singleton Service
    /// </summary>
    public class DataProtectorService : IDataProtectorService
    {
        private readonly IDataProtector _dataProtector;
        private readonly ITimeLimitedDataProtector _timeLimitedDataProtector;

        public DataProtectorService(IDataProtectionProvider provider)
        {
            _dataProtector = provider.CreateProtector(GetType().FullName);
            _timeLimitedDataProtector = _dataProtector.ToTimeLimitedDataProtector();
        }

        /// <summary>
        /// Veri Şifreleme
        /// </summary>
        /// <param name="value">Şifrelenecek veri</param>
        /// <param name="minute">Şifrelenecek verinin süresi</param>
        /// <returns>Şifrelenmiş veri</returns>
        public string Protect(string value, int minute = 0)
        {
            if (0 >= minute)
            {
                return _dataProtector.Protect(value);
            }

            return _timeLimitedDataProtector.Protect(value, TimeSpan.FromMinutes(minute));
        }

        /// <summary>
        /// Veri Şifre Çözücü
        /// </summary>
        /// <param name="value">Şifrelenmiş veri</param>
        /// <returns>Şifresi çözülmüş veri</returns>
        public string Unprotect(string value)
        {
            return _dataProtector.Unprotect(value);
        }
    }
}
