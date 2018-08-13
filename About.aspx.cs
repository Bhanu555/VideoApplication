using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Twilio;
using Twilio.Rest.Video.V1;
using static Twilio.Rest.Video.V1.CompositionResource;
using RestSharp;
using System.Dynamic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Text;

namespace VideoApplication
{
    public partial class About : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }



        protected void click_Click(object sender, EventArgs e)
        {

            dynamic videoLayout = new ExpandoObject();
            dynamic videoLayoutGrid = new ExpandoObject();
            videoLayoutGrid.video_sources = new string[] { "*" };
            videoLayout.grid = videoLayoutGrid;
            Dictionary<string, string> requestFormData = new Dictionary<string, string>();
           // requestFormData.Add("RoomSid", "RM1c1588840e86e29906a0b51f7b3a4b20");
           // requestFormData.Add("AudioSources", "*");
           //// requestFormData.Add("StatusCallback", "https://twilio:4RSEBJAJ4BIB181DECS7K7M5FS81M@monitor.twvending.net/sendCall?message=finished%20transcoding%20video&number=7158213965");
           // //requestFormData.Add("StatusCallbackMethod", "GET");
           // requestFormData.Add("Format", "mp4");
           // requestFormData.Add("VideoLayout", JsonConvert.SerializeObject(videoLayout));
           // RestClient client = new RestClient("https://video.twilio.com/v1/Compositions");
           // var request = new RestRequest(Method.POST);
           // request.AddHeader("Cache-Control", "no-cache");
           // request.AddHeader("Authorization", "Basic U0swNjc4YzBhZDAwNDVlODVhYzY4ZjkxZDNlY2E3Yzg3Yzo2QVFOdW0yMmM5dDIwa1pzYkdYZmZTWlFEcEV5anlXcw==");
           // request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
           // string thing = GenerateMutliPartFormFromDictionary(requestFormData);
           // request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", thing, ParameterType.RequestBody);
           // IRestResponse response = client.Execute(request);
           // string responseContent = response.Content;
           // TwilioCompositionsResponse twilioCompositionsResponse = JsonConvert.DeserializeObject<TwilioCompositionsResponse>(responseContent);
            //Console.WriteLine(twilioCompositionsResponse.url);
           
            if (twilioCompositionsResponse.status == "completed")
            {
                var uri = "https://video.twilio.com/v1/Compositions/CJe37c4b4844a3ccf2fa3b4147ff23132e/Media";
                const string apiKeySid = "SKd8aac04eba0fa8ed056c6263990fa85c";
                const string apiKeySecret = "uUiLZ8XytHGuo15BPnU4EuZ5kdywBDWO";
                TwilioClient.Init(apiKeySid, apiKeySecret);

                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKeySid + ":" + apiKeySecret)));
               request.AllowAutoRedirect = false;
              request.UserAgent = "console-app";
               string responseBody = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
                var mediaLocation = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody)["redirect_to"];

                Console.WriteLine(mediaLocation);
               new WebClient().DownloadFile(mediaLocation, @"C:\Users\bhanushree.rajanna\Desktop\TransferToAmazonVideoFolder\z.mp4");
            }

            

        }
        public static string GenerateMutliPartFormFromDictionary(Dictionary<string, string> dictionary)
        {
            string multiPartFormString = "";
            foreach (KeyValuePair<string, string> dictionaryEntry in dictionary)
            {
                multiPartFormString += $"------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"{dictionaryEntry.Key}\"\r\n\r\n{dictionaryEntry.Value}\r\n";
            }
            multiPartFormString += "------WebKitFormBoundary7MA4YWxkTrZu0gW--";
            return multiPartFormString;
        }
        public class Grid
        {
            public int x_pos { get; set; }
            public int y_pos { get; set; }
            public int z_pos { get; set; }
            public int? width { get; set; }
            public int? height { get; set; }
            public int? max_columns { get; set; }
            public int? max_rows { get; set; }
            public List<string> cells_excluded { get; set; }
            public string reuse { get; set; }
            public List<string> video_sources { get; set; }
            public List<string> video_sources_excluded { get; set; }
        }
        public class VideoLayout
        {
            public Grid grid { get; set; }
        }
        public class Links
        {
            public string media { get; set; }
        }
        public class TwilioCompositionsResponse
        {
            public string status { get; set; }
            public bool trim { get; set; }
            public VideoLayout video_layout { get; set; }
            public DateTime? date_completed { get; set; }
            public string format { get; set; }
            public string url { get; set; }
            public int bitrate { get; set; }
            public DateTime? date_deleted { get; set; }
            public string account_sid { get; set; }
            public int duration { get; set; }
            public List<string> audio_sources { get; set; }
            public string sid { get; set; }
            public string room_sid { get; set; }
            public DateTime date_created { get; set; }
            public int size { get; set; }
            public string resolution { get; set; }
            public List<string> audio_sources_excluded { get; set; }
            public Links links { get; set; }
        }
    }
}
