using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQAirSP_Projekat1
{
    public class Cache
    {
        private static readonly int capacity = 5;
        private static readonly TimeSpan ttl = new TimeSpan(0, 0, 15);
        private static readonly ReaderWriterLockSlim cLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<string, LinkedListNode<(string key, IQAir value, DateTime timestamp)>> cacheDictionary = new Dictionary<string, LinkedListNode<(string key, IQAir value, DateTime timestamp)>>();
        private static readonly LinkedList<(string key, IQAir value, DateTime timestamp)> lruList = new LinkedList<(string key, IQAir value, DateTime timestamp)>();

        public static bool Sadrzi(string key)
        {
            cLock.EnterReadLock();
            if (cacheDictionary.TryGetValue(key, out var node) && node != null)
            {
                if (DateTime.Now - node.Value.timestamp <= ttl)
                {
                    cLock.ExitReadLock();
                    return true;
                }
                else
                {
                    RemoveNode(node);
                    cLock.ExitReadLock();
                    return false;
                }
            }
            else
            {
                cLock.ExitReadLock();
                return false;

            }
        }

        public static IQAir CitajIzKesa(string key)
        {
            cLock.EnterReadLock();

            if (cacheDictionary.TryGetValue(key, out var node))
            {
                if (DateTime.Now - node.Value.timestamp > ttl)
                {
                    RemoveNode(node);
                    cLock.ExitReadLock();
                    return default(IQAir);
                }

                lruList.Remove(node);
                lruList.AddFirst(node);
                cLock.ExitReadLock();
                return node.Value.value;
            }
            else
            {
                cLock.ExitReadLock();
                return default(IQAir);
            }
        }

        public static void UpisiUKes(string key, IQAir value)
        {
            cLock.EnterWriteLock();
            if (cacheDictionary.TryGetValue(key, out var node))
            {
                lruList.Remove(node);
            }
            else
            {
                if (cacheDictionary.Count >= capacity)
                {
                    RemoveExpiredOrLRU();
                }
            }

            var newNode = new LinkedListNode<(string key, IQAir value, DateTime timestamp)>((key, value, DateTime.Now));
            lruList.AddFirst(newNode);
            cacheDictionary[key] = newNode;
            cLock.ExitWriteLock();
        }

        private static void RemoveNode(LinkedListNode<(string key, IQAir value, DateTime timestamp)> node)
        {
            lruList.Remove(node);
            cacheDictionary.Remove(node.Value.key);
        }
        private static void RemoveExpiredOrLRU()
        {
            var currentNode = lruList.Last;
            while (currentNode != null && DateTime.Now - currentNode.Value.timestamp > ttl)
            {
                var previousNode = currentNode.Previous;
                RemoveNode(currentNode);
                currentNode = previousNode;
            }

            if (currentNode != null && cacheDictionary.Count >= capacity)
            {
                RemoveNode(currentNode);
            }
        }
    }
}
