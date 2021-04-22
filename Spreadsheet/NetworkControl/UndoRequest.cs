using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{ // <summary>
    /// A Json object that represents a message from the client to the server
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class UndoRequest
    {
        public UndoRequest()
        {

        }

        public UndoRequest(string type)
        {
            this.type = type;
        }

        [JsonProperty(PropertyName = "requestType", Required = Required.Always)]
        public string type { get; private set; }
    }

}
