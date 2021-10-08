using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
namespace WebApp_1.Infrastructure
{
    public class CustomHealthCheck : IHealthCheck
    {
        private string url;
        public CustomHealthCheck(string apiUrl)
        {
            url = apiUrl;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("/random");
                if (response.IsSuccessStatusCode)
                {
                    var responseContet = await response.Content.ReadAsStringAsync();
                    return await Task.FromResult(HealthCheckResult.Healthy($"A healthy result : {responseContet}"));
                }
                else
                {
                    return await Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus,  
                        $"An unhealthy result, StatusCode : {(int)response.StatusCode}  Reason : {response.ReasonPhrase}"));
                }
            }
           
        }
    }
}
