using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Twilio;
using Twilio.Rest.Video.V1;
using Twilio.Rest.Video.V1.Room;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Twilio.Converters;
using System;
using System.Collections;
using RestSharp;
using System.Dynamic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Net.Http;

namespace VideoApplication
{
    public partial class TwilioVideo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


        }
        protected void RecordingBtn_Click(object sender, EventArgs e)
        {
            //StartRecording();
            // Find your Account SID and Auth Token at twilio.com/console
            const string apiKeySid = "SK0678c0ad0045e85ac68f91d3eca7c87c";
            const string apiKeySecret = "6AQNum22c9t20kZsbGXffSZQDpEyjyWs";
            // const string roomUniqueName = "Anu";
            TwilioClient.Init(apiKeySid, apiKeySecret);
            var rooms = RoomResource.Read(
           status: RoomResource.RoomStatusEnum.Completed,
           //uniqueName: "05212018014523086");
           uniqueName: "05212018045230018");
            foreach (var room in rooms)
            {
                Console.WriteLine(room.Sid);
                string roomSid = room.Sid;
                //Console.WriteLine(room.Sid);

                //const string roomSid = "RM9236a49ad89bde01060d416c418b1157";

                TwilioClient.Init(apiKeySid, apiKeySecret);

                var recordings = RecordingResource.Read(
                    groupingSid: new List<string>() { roomSid });

                foreach (var recording in recordings)
                {
                    Console.WriteLine(recording.Sid);
                    string recordingSid = recording.Sid;
                    var RetrieveRecording = RoomRecordingResource.Fetch(roomSid, recordingSid);
                    Console.WriteLine(RetrieveRecording.Type);
                    string uri = "https://video.twilio.com/v1/" +
                        $"Rooms/{roomSid}/" +
                        $"Recordings/{recordingSid}/" +
                        "Media/";

                    var request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKeySid + ":" + apiKeySecret)));
                    request.AllowAutoRedirect = false;
                    string responseBody = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
                    var mediaLocation = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody)["redirect_to"];

                    Console.WriteLine(mediaLocation);
                    //aws credentials;
                    //loc = aws 

                    new WebClient().DownloadFile(mediaLocation, @"C:\Users\bhanushree.rajanna\Desktop\TestR\" + recording.TrackName + RetrieveRecording.Type + "AudioVideo.mp4");
                }


            }
        }
       
        protected void RequestDownloadButtonClick(object sender, EventArgs e)
        {
            ButtonClick(sender, e);
        }


        private  void ButtonClick(object sender, EventArgs e)
        {
            
            dynamic videoLayout = new ExpandoObject();
            dynamic videoLayoutGrid = new ExpandoObject();
            videoLayoutGrid.video_sources = new string[] { "*" };
            videoLayout.grid = videoLayoutGrid;
            Dictionary<string, string> requestFormData = new Dictionary<string, string>();
            requestFormData.Add("RoomSid", "RM1c1588840e86e29906a0b51f7b3a4b20");
            requestFormData.Add("AudioSources", "*");
            //requestFormData.Add("StatusCallback", "https://twilio:4RSEBJAJ4BIB181DECS7K7M5FS81M@monitor.twvending.net/sendCall?message=finished%20transcoding%20video&number=7158213965");
            //requestFormData.Add("StatusCallbackMethod", "GET");
            requestFormData.Add("Format", "mp4");
            requestFormData.Add("VideoLayout", JsonConvert.SerializeObject(videoLayout));
            RestClient client = new RestClient("https://video.twilio.com/v1/Compositions");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Authorization", "Basic U0swNjc4YzBhZDAwNDVlODVhYzY4ZjkxZDNlY2E3Yzg3Yzo2QVFOdW0yMmM5dDIwa1pzYkdYZmZTWlFEcEV5anlXcw==");
            request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
            string thing = GenerateMutliPartFormFromDictionary(requestFormData);
            request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", thing, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string responseContent = response.Content;
            TwilioCompositionsResponse twilioCompositionsResponse = JsonConvert.DeserializeObject<TwilioCompositionsResponse>(responseContent);
            //var url = twilioCompositionsResponse.url;
            //using (var NewClient = new HttpClient())
            //{
            //    var req = NewClient.GetAsync(url).ContinueWith( res =>
            //    {
            //        var result = res.Result;
            //        if(result.StatusCode == System.Net.HttpStatusCode.OK)
            //        {
            //            var readData = result.Content.ReadAsStreamAsync();
            //            readData.Wait();
            //            var readStream = readData.Result;
                        
            //        }

            //    })
            //}
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


        protected void Conversation_Merge_Click(object sender, EventArgs e)
        {
            string CSID = Convert.ToString(ViewState["CSID"]);
            string mediaLoc = Convert.ToString(ViewState["mediaLoc"]);
            WebClient Client = new WebClient();
            string LocalLocation = @"C:\Users\bhanushree.rajanna\Desktop\TransferToAmazonVideoFolder\";
            string Local = System.IO.Path.Combine(LocalLocation + CSID + ".mp4");
            Client.DownloadFile(mediaLoc, Local);

        }

       protected string  RetrieveMedia()
        {
            WebClient webClient = new WebClient();
            string see = ViewState["see"].ToString();
            var req = (HttpWebRequest)WebRequest.Create(see);
            System.Threading.Thread.Sleep(10000);
            string twilioCompositionsresponse = ViewState["twilioCompositionsResponse"].ToString();       
            string responseBody = new StreamReader(req.GetResponse().GetResponseStream()).ReadToEnd();
            string CSID = Convert.ToString(ViewState["CSID"]);
            var mediaLoc = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody)["redirect_to"];         
           
            WebClient Client = new WebClient();
            string LocalLocation = @"C:\Users\bhanushree.rajanna\Desktop\TransferToAmazonVideoFolder\";
            string Local = System.IO.Path.Combine(LocalLocation + CSID + ".mp4");
            Client.DownloadFile(mediaLoc, Local);           
            return Local;
           
        }

    }
}



//Copy Paste of John's code for Video Conversation Stitiching in c# ( Converted from Node.js) 

//dynamic videoLayout = new ExpandoObject();
//dynamic videoLayoutGrid = new ExpandoObject();
//videoLayoutGrid.video_sources = new string[] { "*" };
//videoLayout.grid = videoLayoutGrid;

//Dictionary<string, string> requestFormData = new Dictionary<string, string>();
//requestFormData.Add("RoomSid", "RM1c1588840e86e29906a0b51f7b3a4b20");
//requestFormData.Add("AudioSources", "*");
//requestFormData.Add("StatusCallback", "https://twilio:4RSEBJAJ4BIB181DECS7K7M5FS81M@monitor.twvending.net/sendCall?message=finished%20transcoding%20video&number=7158213965");
//requestFormData.Add("StatusCallbackMethod", "GET");
//requestFormData.Add("Format", "mp4");
//requestFormData.Add("VideoLayout", JsonConvert.SerializeObject(videoLayout));

//RestClient client = new RestClient("https://video.twilio.com/v1/Compositions");
//var request = new RestRequest(Method.POST);
//request.AddHeader("Cache-Control", "no-cache");
//request.AddHeader("Authorization", "Basic U0swNjc4YzBhZDAwNDVlODVhYzY4ZjkxZDNlY2E3Yzg3Yzo2QVFOdW0yMmM5dDIwa1pzYkdYZmZTWlFEcEV5anlXcw==");
//request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
//string thing = GenerateMutliPartFormFromDictionary(requestFormData);
//request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", thing, ParameterType.RequestBody);
//IRestResponse response = client.Execute(request);
//string responseContent = response.Content;
//TwilioCompositionsResponse twilioCompositionsResponse = JsonConvert.DeserializeObject<TwilioCompositionsResponse>(responseContent);



//public static string GenerateMutliPartFormFromDictionary(Dictionary<string, string> dictionary)
//{
//    string multiPartFormString = "";
//    foreach (KeyValuePair<string, string> dictionaryEntry in dictionary)
//    {
//        multiPartFormString += $"------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"{dictionaryEntry.Key}\"\r\n\r\n{dictionaryEntry.Value}\r\n";
//    }
//    multiPartFormString += "------WebKitFormBoundary7MA4YWxkTrZu0gW--";
//    return multiPartFormString;
//}

//public class Grid
//{
//    public int x_pos { get; set; }
//    public int y_pos { get; set; }
//    public int z_pos { get; set; }
//    public int? width { get; set; }
//    public int? height { get; set; }
//    public int? max_columns { get; set; }
//    public int? max_rows { get; set; }
//    public List<string> cells_excluded { get; set; }
//    public string reuse { get; set; }
//    public List<string> video_sources { get; set; }
//    public List<string> video_sources_excluded { get; set; }
//}

//public class VideoLayout
//{
//    public Grid grid { get; set; }
//}

//public class Links
//{
//    public string media { get; set; }
//}

//public class TwilioCompositionsResponse
//{
//    public string status { get; set; }
//    public bool trim { get; set; }
//    public VideoLayout video_layout { get; set; }
//    public DateTime? date_completed { get; set; }
//    public string format { get; set; }
//    public string url { get; set; }
//    public int bitrate { get; set; }
//    public DateTime? date_deleted { get; set; }
//    public string account_sid { get; set; }
//    public int duration { get; set; }
//    public List<string> audio_sources { get; set; }
//    public string sid { get; set; }
//    public string room_sid { get; set; }
//    public DateTime date_created { get; set; }
//    public int size { get; set; }
//    public string resolution { get; set; }
//    public List<string> audio_sources_excluded { get; set; }
//    public Links links { get; set; }
//}



