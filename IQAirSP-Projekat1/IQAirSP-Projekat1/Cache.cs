using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQAirSP_Projekat1
{
    public class Cache
    {
        private static readonly ReaderWriterLockSlim cLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<string, IQAir> cache = new Dictionary<string, IQAir>();

        public static bool Sadrzi(string key)
        {
            cLock.EnterReadLock();
            var value = cache.ContainsKey(key);
            cLock.ExitReadLock();
            return value;
        }

        public static IQAir CitajIzKesa(string key)
        {
            cLock.EnterReadLock();
            try
            {
                if (cache.TryGetValue(key, out IQAir value) && value != null)
                    return value;
                else
                    throw new KeyNotFoundException($"Nema kljuca {key}");
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw;
            }
            finally
            {
                cLock.ExitReadLock();
            }
        }

        public static void UpisiUKes(string key, IQAir value)
        {
            cLock.EnterWriteLock();
            try
            {
                cache[key] = value;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw;
            }
            finally
            {
                cLock.ExitWriteLock();
            }
        }
    }
}
