// FILEPATH: /D:/test/test.cs
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static async Task Main(string[] args)
    {
        string ID = "👉air_table_ID";
        string Table = "👉air_table_TableID";
        string PAT = "👉air_table_PAT";

        // 你的個人存取令牌和基礎數據庫 ID
        string api_url = $"https://api.airtable.com/v0/{ID}/{Table}";

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {PAT}");

        // 新增資料
        async Task AddData(string field1, string value1, string field2, string value2)
        {
            var data = new
            {
                fields = new Dictionary<string, object>
                {
                    { field1, value1 },
                    { field2, value2 }
                }
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(api_url, content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("資料新增成功");
            }
            else
            {
                Console.WriteLine("資料新增失敗: " + await response.Content.ReadAsStringAsync());
            }
        }

        // 搜尋資料
        async Task SearchRecord(string field, string value)
        {
            var filterByFormula = $"({field} = '{value}')";
            var paramsString = $"filterByFormula = {filterByFormula}";
            Console.WriteLine(paramsString);

            var response = await client.GetAsync($"{api_url}?{paramsString}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                dynamic ?result= Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                if (result != null)
                {
                    var records = result.records;

                    // 顯示有幾筆資料
                    Console.WriteLine($"共有{records.Count}筆資料");
                    // 顯示每一筆資料
                    foreach (var record in records)
                    {
                        Console.WriteLine(record);
                    }
                }
                else
                {
                    Console.WriteLine("找不到資料");
                }
            }
            else
            {
                Console.WriteLine("資料查詢失敗: " + await response.Content.ReadAsStringAsync());
                Console.WriteLine("response.status_code: " + response.StatusCode);
            }
        }

        // 主程式
        // 跑一個迴圈
        for (int i = 1; i < 2; i++)
        {
            string field1 = "請假人";
            string value1 = "值" + i;
            string field2 = "代理人";
            string value2 = "值" + (i + 1);
            await AddData(field1, value1, field2, value2);
        }

        // 搜尋資料
        await SearchRecord("請假人", "值1");
    }
}
