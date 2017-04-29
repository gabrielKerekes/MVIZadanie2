using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace MVIZadanie2XForms
{
    public class OnlineGradePredictor
    {
        private const string Url = "https://ussouthcentral.services.azureml.net/workspaces/4f80385372e643b8b3a09550996d7df8/services/aa13c293fb984377b1ee560817550f5d/execute?api-version=2.0&details=true";
        private const string ApiKey = "RJLBPMYyu5a2KAL3evPVymIbSngw+f+fJmPVzby56o1Qk3LLJmdLcikZe/riM7egiWFbSDlGrUVVdHyEAsW8hg==";

        public static string GetPredictedGrade(string name, string subjectDifficulty, string numOfHours)
        {
            var checkInputResult = CheckInput(subjectDifficulty, numOfHours);
            if (!string.IsNullOrEmpty(checkInputResult))
                return checkInputResult;

            var response = DoRequest(name, subjectDifficulty, numOfHours);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"The request failed with status code: {response.StatusCode}");
            }

            return ParseResponse(response.Content.ReadAsStringAsync().Result);
        }

        private static string CheckInput(string subjectDifficultyString, string numOfHoursString)
        {
            var subjectDifficulty = TryParseInt(subjectDifficultyString, "Obtiaznost predmetu musi byt cislo");
            if (subjectDifficulty > 4 || subjectDifficulty < 1)
                throw new Exception("Obtiaznost predmetu musi byt v intervale <1, 4>");

            var numOfHours = TryParseInt(numOfHoursString, "Pocet hodin musi byt cislo");
            if (numOfHours < 0)
                throw new Exception("Pocet hodin musi byt v intervale <0, ...>");

            return null;
        }

        private static int TryParseInt(string numberString, string errorMessage)
        {
            try
            {
                return int.Parse(numberString);
            }
            catch (Exception)
            {
                throw new Exception(errorMessage);
            }

        }

        private static HttpResponseMessage DoRequest(string name, string subjectDifficulty, string numOfHours)
        {
            var client = new HttpClient();
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
            client.BaseAddress = new Uri(Url);

            var str = $"{{\"Inputs\":{{\"input1\":{{\"ColumnNames\":[\"Meno\",\"Obtiaznost predmetu\",\"Pocet hodin ucenia\",\"Znamka\"],\"Values\":[[\"{name}\",\"{subjectDifficulty}\",\"{numOfHours}\",\"A\"]]}}}}}}";
            var requestContent = new StringContent(str, Encoding.UTF8, "application/json");

            return client.PostAsync("", requestContent).Result;
        }

        private static string ParseResponse(string response)
        {
            var responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
            var outputDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseDictionary["Results"].ToString());
            var firstOutputDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(outputDictionary["output1"].ToString());
            var valueDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(firstOutputDictionary["value"].ToString());
            var values = JsonConvert.DeserializeObject<object[]>(valueDictionary["Values"].ToString());
            values = JsonConvert.DeserializeObject<object[]>(values[0].ToString());
            
            return (string) values[7];
        }
    }
}
