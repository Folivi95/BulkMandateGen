using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BulkMandateGen.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using BulkMandateGen.Data.ResponseVM;

namespace BulkMandateGen
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly HttpClient client = new HttpClient();

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<SetupMandate> SetupMandate { get; set; }

        public async Task OnGetAsync()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            SetupMandate = await _context.Mandates.ToListAsync();

            foreach (var mandate in SetupMandate)
            {
                var requestId = GenerateRandomNumber(6);
                var dataToHash = Environment.GetEnvironmentVariable("DEMO_MERCHANTID") + Environment.GetEnvironmentVariable("DEMO_SERVICETYPEID") + requestId + mandate.Amount + Environment.GetEnvironmentVariable("DEMO_APIKEY");
                var apiHash = ComputeSHAHash(dataToHash);

                string content = $"{{\"amount\":\"{mandate.Amount}\"," +
                            $"\"endDate\":\"{mandate.EndDate}\"," +
                            $"\"hash\":\"{apiHash}\"," +
                            $"\"mandateType\":\"{mandate.MandateType}\"," +
                            $"\"maxNoOfDebits\":\"{mandate.MaxNoOfDebits}\"," +
                            $"\"merchantId\":\"{Environment.GetEnvironmentVariable("DEMO_MERCHANTID")}\"," +
                            $"\"payerAccount\":\"{mandate.PayerAccount}\"," +
                            $"\"payerBankCode\":\"{mandate.PayerBankCode}\"," +
                            $"\"payerEmail\":\"{mandate.PayerEmail}\"," +
                            $"\"payerName\":\"{mandate.PayerName}\"," +
                            $"\"payerPhone\":\"{mandate.PayerPhone}\"," +
                            $"\"requestId\":\"{requestId}\"," +
                            $"\"serviceTypeId\":\"{Environment.GetEnvironmentVariable("DEMO_SERVICETYPEID")}\"," +
                            $"\"startDate\":\"{mandate.StartDate}\"}}";

                HttpResponseMessage recipientRes = await client.PostAsync($"{Environment.GetEnvironmentVariable("SETUP_MANDATE_ENDPOINT")}", new StringContent(content, Encoding.UTF8, "application/json"));

                if (recipientRes.IsSuccessStatusCode)
                {
                    try
                    {
                        string recipientData = await recipientRes.Content.ReadAsStringAsync();
                        recipientData = recipientData.Replace("jsonp (", "");
                        recipientData = recipientData.Replace(")", "");
                        ResponseVM res = JsonConvert.DeserializeObject<ResponseVM>(recipientData);

                        mandate.MandateId = res.mandateId;
                        mandate.RequestId = res.requestId;

                        _context.Mandates.Update(mandate);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error Occurred: {ex.Message}");
                    }
                }
            }

            //save changes to database
            await _context.SaveChangesAsync();

        }

        private static string ComputeSHAHash(string data)
        {
            SHA512Managed sha512 = new SHA512Managed();
            Byte[] EncryptedSHA512 = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
            sha512.Clear();
            string hashed = BitConverter.ToString(EncryptedSHA512).Replace("-", "").ToLower();
            return hashed;
        }

        private static string GenerateRandomNumber(int size)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            StringBuilder builder = new StringBuilder();
            string s;
            for (int i = 0; i < size; i++)
            {
                s = Convert.ToString(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(s);
            }

            return builder.ToString();
        }
    }
}
