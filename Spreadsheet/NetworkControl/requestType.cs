using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkControl
{
    class requestType
    {
        // <summary>
        /// A Json object that represents a message from the client to the server
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        class messageType
        {
            [JsonProperty(PropertyName = "requestType", Required = Required.Always)]
            public int type { get; private set; }

            [JsonProperty(PropertyName = "cellName")]
            public int cellName { get; private set; }

            [JsonProperty(PropertyName = "contents")]
            public int cellContents { get; private set; }
        }
    }
}
