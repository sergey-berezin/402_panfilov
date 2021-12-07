using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RecognizerVM
{
    public class ClientSession
    {
        HttpClient client = new();


        public async Task<string> GetStringAsync(string url)
        {
            return await client.GetStringAsync(url);
        }

        public async Task PostStringAsync(string url, string message)
        {
            var json = JsonConvert.SerializeObject(message);
            var data = new StringContent(json, Encoding.Default, "application/json");
            await client.PostAsync(url, data);
        }

        public async Task DeleteAsync(string url)
        {
            await client.DeleteAsync(url);
        }

        public async Task<List<string>> GetClassListAsync(string url)
        {
            var respond = await client.GetStringAsync(url);
            var list = JsonConvert.DeserializeObject<List<string>>(respond);
            return list;
        }

        public async Task<List<byte[]>> GetImageByteListAsync(string url)
        {
            var respond = await client.GetStringAsync(url);
            var list = JsonConvert.DeserializeObject<List<byte[]>>(respond);
            return list;
        }
    }
}
