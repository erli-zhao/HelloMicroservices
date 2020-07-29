using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;
using YamlDotNet.Serialization;

namespace LoyaltyProgram
{
    public class YamlSerializerDeserializer:IBodyDeserializer
    {
        public bool CanDeserialize(MediaRange mediaRange, BindingContext context)
            => mediaRange.Subtype.ToString().EndsWith("yaml");

        public object Deserialize(MediaRange mediaRange, Stream bodyStream, BindingContext context)
        {
            var yamlDeserializer = new Deserializer();
            var reader = new StreamReader(bodyStream);
            return yamlDeserializer.Deserialize(reader, context.DestinationType);
        }
    }
}
