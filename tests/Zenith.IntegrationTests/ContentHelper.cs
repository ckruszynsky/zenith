﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Zenith.Core.Features.Users;
using Zenith.Core.Features.Users.Dtos;

namespace Zenith.IntegrationTests
{
    public static class ContentHelper
    {
        public static StringContent GetRequestContent(object request)
        {
            return new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        }

        public static async Task<StringContent> GetRequestContentWithAuthorization(object request, HttpClient client, LoginUserDto user)
        {            
            var response = await client.PostAsync("/api/users/login", GetRequestContent(user));
            var responseContent = await GetResponseContent<UserViewModel>(response);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, responseContent.Token);
            return GetRequestContent(request);
        }

        public static async Task GetRequestWithAuthorization(HttpClient client, LoginUserDto user)
        {           
            var response = await client.PostAsync("/api/users/login", GetRequestContent(user));
            var responseContent = await GetResponseContent<UserViewModel>(response);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", responseContent.Token);
            GetRequestContent(null);
        }

        public static async Task<T> GetResponseContent<T>(HttpResponseMessage response)
        {
            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(stringResponse);
            return result;
        }
    }
}
