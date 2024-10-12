using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCloner.Models
{
    public enum ConnectionStatus { ConnectionSuccess, ConnectionFailure }
    public interface IRequester
    {
        // Retrieves the list of IP addresses from the server
        public List<string> GetAllIPAddresses();

        // Attempts to connect to a given IP address
        public ConnectionStatus ConnectToClient(string ipAddress, int portNumber);
    }
}
