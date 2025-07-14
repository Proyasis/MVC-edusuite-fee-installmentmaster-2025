using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Services;

namespace CITS.EduSuite.UI
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

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
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
       
    }
    public class SettingsMapping
    {
        ISettingsService settingsService;
        public SettingsMapping()
        {
            this.settingsService = new SettingsService();
        }
        public void SyncOrderSettings()
        {
            settingsService.SyncApplicationSettings();
            settingsService.SyncEmployeeSettings();
            settingsService.SyncLibrarySettings();
        }

    }
}