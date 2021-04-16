using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkControl
{
    /// <summary>
    /// A Json object that represents a message from the server to the client
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    class messageType
    {
        [JsonProperty(PropertyName = "messageType", Required = Required.Always)]
        public int type { get; private set; }

        [JsonProperty(PropertyName = "cellName")]
        public int cellName { get; private set; }

        [JsonProperty(PropertyName = "contents")]
        public int cellContents { get; private set; }

        [JsonProperty(PropertyName = "message")]
        public int errorMessage { get; private set; }

        [JsonProperty(PropertyName = "selector")]
        public int selectorID { get; private set; }

        [JsonProperty(PropertyName = "selectorName")]
        public int selectorName { get; private set; }

        [JsonProperty(PropertyName = "user")]
        public int userID { get; private set; }

    }
}
