using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS
{
    /// <summary>
    /// A Json object that represents a message from the server to the client
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class messageType
    {
        public messageType()
        {

        }

        [JsonProperty(PropertyName = "messageType", Required = Required.Always)]
        public string type { get; private set; }

        [JsonProperty(PropertyName = "cellName")]
        public string cellName { get; private set; }

        [JsonProperty(PropertyName = "contents")]
        public string cellContents { get; private set; }

        [JsonProperty(PropertyName = "message")]
        public string errorMessage { get; private set; }

        [JsonProperty(PropertyName = "selector")]
        public int selectorID { get; private set; }

        [JsonProperty(PropertyName = "selectorName")]
        public string selectorName { get; private set; }

        [JsonProperty(PropertyName = "user")]
        public int userID { get; private set; }

    }
}
