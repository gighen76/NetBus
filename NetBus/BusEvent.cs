using System;
using System.Collections.Generic;

namespace NetBus
{

    public class BusEvent
    {

        const string HEADER_PREFIX = "Header_";

        public BusEvent(byte[] message, BusTopic topic,  BusEvent parentEvent = null)
        {
            Id = Guid.NewGuid();
            ParentId = parentEvent?.Id ?? Id;
            OriginId = parentEvent?.OriginId ?? Id;
            Topic = topic;
            Message = message;
            Headers = new Dictionary<string, string>();
        }

        public BusEvent(byte[] message, IDictionary<string, string> busHeaders)
        {

            if (!busHeaders.ContainsKey("TopicName") || !BusTopic.TryParse(busHeaders["TopicName"], out BusTopic topic) ||
                !busHeaders.ContainsKey("Id") || !Guid.TryParse(busHeaders["Id"], out Guid id) ||
                !busHeaders.ContainsKey("ParentId") || !Guid.TryParse(busHeaders["ParentId"], out Guid parentId) ||
                !busHeaders.ContainsKey("OriginId") || !Guid.TryParse(busHeaders["OriginId"], out Guid originId))
            {
                throw new ArgumentException(nameof(busHeaders));
            }

            Id = id;
            ParentId = parentId;
            OriginId = originId;
            Topic = topic;
            Message = message;
            Headers = new Dictionary<string, string>();
            foreach(var busHeader in busHeaders)
            {
                if (busHeader.Key.StartsWith(HEADER_PREFIX))
                {
                    Headers.Add(busHeader.Key.Substring(HEADER_PREFIX.Length), busHeader.Value);
                }
            }
        }


        public Guid Id { get; }
        public Guid ParentId { get; }
        public Guid OriginId { get; }

        public BusTopic Topic { get; }
        public byte[] Message { get; }
        public IDictionary<string, string> Headers { get; }

        public IDictionary<string,string> GetBusHeaders()
        {
            var busHeaders = new Dictionary<string, string>
            {
                { "TopicName", Topic.Name },
                { "Id", Id.ToString() },
                { "ParentId", ParentId.ToString() },
                { "OriginId", OriginId.ToString() }
            };
            foreach(var header in Headers)
            {
                busHeaders.Add(HEADER_PREFIX + header.Key, header.Value);
            }
            return busHeaders;
        }

    }

}