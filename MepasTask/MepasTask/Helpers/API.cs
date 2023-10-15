using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Text;
using System.Security.Claims;
using System.Net.Http.Headers;
using MepasTask.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using DocumentFormat.OpenXml.Spreadsheet;
using MepasTask.App.Interfaces;

namespace MepasTask.UI.Helpers
{
    public class API<T> where T : class
    {
        private readonly IUnitOfWork unitOfWork;

        public API(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private static HttpClient _client;
        public static string ApiAddress = "https://localhost:7113/api";

        private static string GetTokenFromClaim()
        {
            var context = new HttpContextAccessor().HttpContext;
            if (context == null || !context.User.Identity.IsAuthenticated)
            {
                return null;
            }
            return context.User.FindFirstValue("token");
        }

        public static async Task<IReadOnlyList<T>> SelectAsyncToList(string apiController, string apiMethod, Dictionary<string, string>? qsParams)
        {
            string token = GetTokenFromClaim();
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ApiAddress);
                string uri;
                uri = "";
                if (qsParams == null)
                {
                    uri = ApiAddress + "/" + apiController + "/" + apiMethod;
                }
                else
                {
                    uri = QueryHelpers.AddQueryString(ApiAddress + "/" + apiController + "/" + apiMethod, qsParams);
                }
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                using (var response = await httpClient.GetAsync(uri))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var result = JsonConvert.DeserializeObject<List<T>>(apiResponse);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }
        }

        public static async Task<string> SelectAsyncFirstOrDefault(string apiController, string apiMethod)
        {
            

            string token = GetTokenFromClaim();

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ApiAddress);
                string uri;
                uri = ApiAddress + "/" + apiController + "/" + apiMethod;
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                using (var response = await httpClient.GetAsync(uri))
                {
                    try
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(apiResponse);
                        return result.ToString();
                    }
                    catch (Exception ex)
                    {
                        return "Error " + ex.InnerException;
                    }
                }
            }

        }

        public static async Task<T> SelectAsyncFirstOrDefault(string apiController, string apiMethod, Dictionary<string, string>? qsParams)
        {
            
            string token = GetTokenFromClaim();
            using (var httpClient = new HttpClient())
            {

                httpClient.BaseAddress = new Uri(ApiAddress);
                string uri;
                if (qsParams == null)
                {
                    uri = ApiAddress + "/" + apiController + "/" + apiMethod;
                }
                else
                {
                    uri = QueryHelpers.AddQueryString(ApiAddress + "/" + apiController + "/" + apiMethod, qsParams);
                }

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }


                using (var response = await httpClient.GetAsync(uri))
                {
                    try
                    {
                       
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(apiResponse);
                       
                        return result;
                    }
                    catch (Exception ex)
                    {
                        
                        return null;
                    }
                }
            }

        }

        public static async Task<T> SelectPostAsyncFirstOrDefault(string apiController, string apiMethod, Dictionary<string, string> qsParams)
        {

            using (var content = new FormUrlEncodedContent(qsParams))
            {
                string token = GetTokenFromClaim();
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(ApiAddress);
                    var uri = QueryHelpers.AddQueryString(ApiAddress + "/" + apiController + "/" + apiMethod, qsParams);

                    if (!string.IsNullOrEmpty(token))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                    try
                    {
                        using (var response = await httpClient.PostAsync(uri, content))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<T>(apiResponse);
                            return result;
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
            }
        }



        public static async Task<bool> DeleteAsync(string apiController, string apiMethod, int id)
        {
            
            string token = GetTokenFromClaim();

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(ApiAddress);
                string uri = ApiAddress + "/" + apiController + "/" + apiMethod + "?id=" + id.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await httpClient.DeleteAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else return false;
            }

          

        }
        public static async Task<bool> SaveAsync(string apiController, string apiMethod, T entity)
        {
            string token = GetTokenFromClaim();

            using (var httpClient = new HttpClient())
            {
                
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                try
                {
                    if (httpClient.BaseAddress == null)
                    {
                        httpClient.BaseAddress = new Uri(ApiAddress);
                    }
                    string uri = ApiAddress + "/" + apiController + "/" + apiMethod;
                    HttpContent body = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(uri, body);
                    var contents = await response.Content.ReadAsStringAsync();
                    if (contents == "false")
                    {
                        return false;
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

       

    }
}
