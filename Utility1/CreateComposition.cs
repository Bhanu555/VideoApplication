using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Twilio;
using Twilio.Rest.Video.V1;
using static Twilio.Rest.Video.V1.CompositionResource;

namespace CompositionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var definition = new
            {
                grid = new
                {
                    max_rows = 0,
                    video_sources = new string[]{ "", "", ""}
                }
            };
            var videoLayoutJson = @"{grid:{max_rows:1,video_sources:[""*"",""*"",""*""]}}";
            var json = JsonConvert.DeserializeAnonymousType(videoLayoutJson, definition);

            TwilioClient.Init("SKd8aac04eba0fa8ed056c6263990fa85c", "uUiLZ8XytHGuo15BPnU4EuZ5kdywBDWO");
            try
            {
                var compositionResource = CompositionResource.Create(
                    roomSid: "RM1c1588840e86e29906a0b51f7b3a4b20",
                    audioSources: new List<string>{"*"},
                    videoLayout: json,
                    //statusCallback: new Uri("http://my.server.org/callbacks"),
                    format: FormatEnum.Mp4);

                Console.WriteLine($"Composition Resource: {compositionResource.Sid}");
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred: {ex.Message}");
            }
        }
    }
}
