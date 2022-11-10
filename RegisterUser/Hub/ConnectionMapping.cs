using System.Collections;

namespace RegisterUser.Hub
{
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            try
            {
                lock (_connections)
                {
                    HashSet<string> connections;
                    if (!_connections.TryGetValue(key, out connections))
                    {
                        connections = new HashSet<string>();
                        _connections.Add(key, connections);
                    }

                    lock (connections)
                    {
                        connections.Add(connectionId);
                    }
                }
            }
            catch (Exception e)
            {
                
            }

        }

        public IEnumerable<string> GetConnections(T key)
        {
            try
            {
                HashSet<string> connections;
                if (_connections.TryGetValue(key, out connections))
                {
                    return connections;
                }

                return Enumerable.Empty<string>();
            }
            catch (Exception e)
            {
                
            }

            return new List<string>();
        }

        public void Remove(T key, string connectionId)
        {
            try
            {
                lock (_connections)
                {
                    HashSet<string> connections;
                    if (!_connections.TryGetValue(key, out connections))
                    {
                        return;
                    }

                    lock (connections)
                    {
                        connections.Remove(connectionId);

                        if (connections.Count == 0)
                        {
                            _connections.Remove(key);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }
    }
}
