using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{ // <summary>
    /// A Json object that represents a message from the client to the server
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class RequestType
    {
        public RequestType()
        {

        }

        public RequestType(string type, string name, string contents)
        {
            this.type = type;
            cellContents = contents;
            cellName = name;
        }

        public RequestType(string type, string name)
        {
            this.type = type;
            cellName = name;
        }

        [JsonProperty(PropertyName = "requestType", Required = Required.Always)]
        public string type { get; private set; }

        [JsonProperty(PropertyName = "cellName")]
        public string cellName { get; private set; }

        [JsonProperty(PropertyName = "contents")]
        public string cellContents { get; private set; }
    }

}
