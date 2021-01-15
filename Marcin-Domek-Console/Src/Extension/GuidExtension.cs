using System;

namespace Marcin_Domek_Server.Src.Extension
{
    internal static class GuidExtension
    {
        public static Guid ParseOrEmpty(this Guid value, string guidString)
        {
            try
            {
                return Guid.Parse(guidString);
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }
    }
}
