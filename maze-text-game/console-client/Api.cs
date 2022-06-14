using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace console_client
{
    internal class Api
    {

        static string apiBase = "http://ec2-13-245-128-230.af-south-1.compute.amazonaws.com";
        
        static string token = null;

        public static bool Auth(string name) {
            WebRequest req = WebRequest.Create($"{apiBase}/auth");
            req.Method = "POST";

            AddHeader(req);
            AddBody(req, new { 
                PlayerName = name,
            });

            var response = (HttpWebResponse) req.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK) {
                dynamic obj = ReadResponseStream(response.GetResponseStream());
                token = obj.Token;
                return true;
            }

            return false;
        }

        public static bool RunCommand(string gameId, string cmd) {
            WebRequest req = WebRequest.Create($"{apiBase}/Command");
            req.Method = "POST";

            AddHeader(req);
            AddBody(req, new {
                GameGuid = gameId,
                Command = cmd,
            });

            try
            {
                var response = (HttpWebResponse)req.GetResponse();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch {
                return false;
            }            
        }

        public static dynamic CreateGame(int playerLimit, int mapWidth, int mapHeight) {
            WebRequest req = WebRequest.Create($"{apiBase}/Game/create");
            req.Method = "POST";
            
            AddHeader(req);
            AddBody(req, new {
                PlayerLimit = playerLimit,
                MapWidth = mapWidth,
                MapHeight = mapHeight,
            });

            dynamic obj = ReadResponseStream(req.GetResponse().GetResponseStream());
            return obj;
        }

        public static bool JoinGame(string gameId) {
            WebRequest req = WebRequest.Create($"{apiBase}/Game/join");
            req.Method = "POST";

            AddHeader(req);
            AddBody(req, new {
                GameGuid = gameId,
            });

            var response = req.GetResponse();
            
            return ((HttpWebResponse) response).StatusCode == HttpStatusCode.OK;
        }

        public static bool VoteStart(string gameId) {
            WebRequest req = WebRequest.Create($"{apiBase}/Game/vote_start");
            req.Method= "POST";

            AddHeader(req);
            AddBody(req, new
            {
                GameGuid = gameId,
            });

            return ((HttpWebResponse)req.GetResponse()).StatusCode == HttpStatusCode.OK;
        }

        public static dynamic GetGame(string gameId) {
            WebRequest req = WebRequest.Create($"{apiBase}/Game?gameId={gameId}");
            req.Method = "GET";

            AddHeader(req);

            dynamic obj = ReadResponseStream(req.GetResponse().GetResponseStream());
            return obj;
        }

        private static dynamic ReadResponseStream(Stream responseStream) {
            using (StreamReader reader = new StreamReader(responseStream)) {
                return JsonConvert.DeserializeObject(reader.ReadToEnd());
            }            
        }

        private static void AddHeader(WebRequest req) { 
            req.Headers.Add("Authorization", $"Bearer {token}");
        }

        private static void AddBody(WebRequest req, object body) {
            req.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(req.GetRequestStream())) {
                string json = System.Text.Json.JsonSerializer.Serialize(body);

                streamWriter.Write(json);
            }
        }
    }
}
