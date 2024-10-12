using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCloner.Models
{
    public interface IResponder
    {
        // Starts listening for requests on the specified IP address
        public void ListenForRequests(string ipAddress);

        // Sends a response to a client at the given IP address
        public void ReplyTo(string message, string ipAddress);
    }
}
