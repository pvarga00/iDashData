using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp;
using Newtonsoft.Json;

namespace iDashData
{
    class Cherwell
    {
        private Task<string> _cherwellAuthToken;
        private bool refreshToken = false;

        public Cherwell()
        {
            this._cherwellAuthToken = GetCherwellToken();

            //set timer to get refresh token
        }


        // GET: /Cherwell/
        public async Task<IRestResponse> CherwellIndex()
        {

            // API Key: 616302a7-e09b-42e1-9fae-941e39e77e46
            // API token lifespan is set to 60 minutes
            // Refresh token lifespan is set to 1440 minutes

            /* 
            // Good Links <Please do not remove>
            // 
            // Cherwell Swagger: http://ql1cwbeta1/CherwellAPI/Swagger/ui/index#/
            // 
            // Cherwell Examples: https://cherwellsupport.com/CherwellAPI/Documentation/en-US/csm_rest_ad_hoc_search_with_filter.html
            // 
            // Cherwell Community Forums: https://www.cherwell.com/community/builders-network/f/rest_web_api
            // 
            // Good Links <Please do not remove>
            */



            //Call john's cherwell api using the token we got in the constructor
            RestClient client = new RestClient("http://ql1cwbeta1/CherwellAPI/api/V1/getsearchresults");
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "f05a3f8f-0f92-5ad5-709b-926c7ec774be");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", "bearer " + this._cherwellAuthToken);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept", "application/json, text/json, application/xml, text/xml");
            request.AddParameter("application/json", "{\r\n  \"filters\": [\r\n    {\r\n      \"fieldId\": \"BO:9344be92d5b7b4c290437c4110bc5b7147c9e3e98a,FI:93e3c20d1258b4b6bec3a8450fb3b517eebee9a591\",\r\n      \"operator\": \"eq\",\r\n      \"value\": true\r\n    },\r\n\t{\r\n      \"fieldId\": \"BO:9344be92d5b7b4c290437c4110bc5b7147c9e3e98a,FI:940cce26244fe45d09aab6437cb343fc381e889595\",\r\n      \"operator\": \"eq\",\r\n      \"value\": \"93fdd1ac89221e1b0634044813b3295c19eec77e45\"\r\n    }\r\n  ],\r\n  \"association\": \"Problem\",\r\n  \"busObId\": \"9344be92d5b7b4c290437c4110bc5b7147c9e3e98a\",\r\n  \"includeAllFields\": false,\r\n  \"includeSchema\": false,\r\n  \"pageNumber\": 0,\r\n  \"pageSize\": 0,\r\n  \"scope\": \"Global\",\r\n  \"scopeOwner\": \"(None)\",\r\n  \"searchName\": \"apitestSearch\"\r\n}", ParameterType.RequestBody);
            IRestResponse response = await RestClientExtensions.ExecuteAsync(client, request);

            return response;
        }

        public async Task<string> GetCherwellToken()
        {

            try
            {
                string clientId = "616302a7-e09b-42e1-9fae-941e39e77e46";

                string postmanToken = "a515c88b-1797-98ee-148a-d16d93450eac";

                string username = "iDashboard";

                string password = "Rock123";

                string token = "";


                var client = new RestClient("http://ql1cwbeta1/CherwellAPI/token?auth_mode=Internal&api_key=616302a7-e09b-42e1-9fae-941e39e77e46");
                var request = new RestRequest(Method.POST);
                request.AddHeader("postman-token", postmanToken);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("accept", "application/json");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", "grant_type=password&client_id=" + clientId + "&username=" + username + "&password=" + password, ParameterType.RequestBody);


                RestResponse response = await RestClientExtensions.ExecuteAsync(client, request);

                dynamic deserializedResponse = JsonConvert.DeserializeObject(response.Content);

                token = deserializedResponse["access_token"];

                return token;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
