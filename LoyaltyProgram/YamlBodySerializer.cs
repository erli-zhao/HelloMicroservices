﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nancy;
using Nancy.Responses.Negotiation;
using YamlDotNet.Serialization;
namespace LoyaltyProgram
{
    public class YamlBodySerializer : IResponseProcessor
    {
        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings {
            get
            {
                yield return new Tuple<string, MediaRange>("yaml", new MediaRange("application/yaml"));
            }
        }

        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            return requestedMediaRange.Subtype.ToString().EndsWith("yaml") ?
                new ProcessorMatch
                {
                    ModelResult = MatchResult.DontCare,
                    RequestedContentTypeResult = MatchResult.NonExactMatch
                }
                : ProcessorMatch.None;
        }

        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        =>
            new Response
            {
                Contents = stream =>
                {
                    var yamlSerializer = new Serializer();
                    var streamWriter = new StreamWriter(stream);
                    yamlSerializer.Serialize(streamWriter, model);
                    streamWriter.Flush();
                },
                ContentType = "application/yaml"
            };
    }
}
