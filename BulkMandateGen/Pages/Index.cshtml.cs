using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BulkMandateGen.Data.ResponseVM;
using BulkMandateGen.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;

namespace BulkMandateGen.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly HttpClient client = new HttpClient();

        [BindProperty]
        public CSVModel cSVModel { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            string filePath = string.Empty;
            string[] columns = Array.Empty<string>();
            string csv = string.Empty;
            int snCount = 0;

            //initialize http client
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (ModelState.IsValid)
            {
                if (cSVModel.File.Length > 0)
                {
                    string path = Path.Combine(_environment.ContentRootPath, cSVModel.File.FileName);

                    using (var filestream = new FileStream(path, FileMode.Create))
                    {
                        await cSVModel.File.CopyToAsync(filestream);
                    }

                    filePath = Path.GetFullPath(cSVModel.File.FileName);
                    using (TextFieldParser csvParser = new TextFieldParser(filePath))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        columns = csvParser.ReadFields();

                        foreach (var item in columns)
                        {
                            csv += item.ToString().Replace(",", ";").Trim() + ",";
                        }

                        //add new line
                        csv += "\r\n";

                        while (!csvParser.EndOfData)
                        {
                            // Read current line fields, pointer moves to the next line.
                            string[] fields = csvParser.ReadFields();
                            string name = fields[1];
                            string email = fields[2];
                            string phone = fields[3];
                            string bankCode = fields[4];
                            string account = fields[5];
                            string amount = fields[6];
                            string startDate = fields[7];
                            string endDate = fields[8];
                            string maxNoDebits = fields[9];
                            string mandateRRR = string.Empty;
                            string requestId = GenerateRandomNumber(7);
                            string mandateUrl = string.Empty;

                            //increment snCount
                            ++snCount;

                            //make request to generate mandate id
                            string dataToHash = Environment.GetEnvironmentVariable("DEMO_MERCHANTID") + 
                                Environment.GetEnvironmentVariable("DEMO_SERVICETYPEID") + requestId + 
                                amount + Environment.GetEnvironmentVariable("DEMO_APIKEY");
                            var apiHash = ComputeSHAHash(dataToHash);

                            string content = $"{{\"amount\":\"{amount}\"," +
                            $"\"endDate\":\"{endDate}\"," +
                            $"\"hash\":\"{apiHash}\"," +
                            $"\"mandateType\":\"DD\"," +
                            $"\"maxNoOfDebits\":\"{maxNoDebits}\"," +
                            $"\"merchantId\":\"{Environment.GetEnvironmentVariable("DEMO_MERCHANTID")}\"," +
                            $"\"payerAccount\":\"{account}\"," +
                            $"\"payerBankCode\":\"{bankCode}\"," +
                            $"\"payerEmail\":\"{email}\"," +
                            $"\"payerName\":\"{name}\"," +
                            $"\"payerPhone\":\"{phone}\"," +
                            $"\"requestId\":\"{requestId}\"," +
                            $"\"serviceTypeId\":\"{Environment.GetEnvironmentVariable("DEMO_SERVICETYPEID")}\"," +
                            $"\"startDate\":\"{startDate}\"}}";

                            HttpResponseMessage recipientRes = await client.PostAsync($"{Environment.GetEnvironmentVariable("SETUP_MANDATE_ENDPOINT")}", new StringContent(content, Encoding.UTF8, "application/json"));

                            if (recipientRes.IsSuccessStatusCode)
                            {
                                try
                                {
                                    string recipientData = await recipientRes.Content.ReadAsStringAsync();
                                    recipientData = recipientData.Replace("jsonp (", "");
                                    recipientData = recipientData.Replace(")", "");
                                    ResponseVM res = JsonConvert.DeserializeObject<ResponseVM>(recipientData);

                                    mandateRRR = res.mandateId;
                                    requestId = res.requestId;

                                    //get mandate form URL
                                    var valuesToHash = Environment.GetEnvironmentVariable("DEMO_MERCHANTID") + Environment.GetEnvironmentVariable("DEMO_APIKEY") + res.requestId;
                                    var mandateFormApiHash = ComputeSHAHash(valuesToHash);
                                    mandateUrl = $"https://login.remita.net/remita/ecomm/mandate/form/{Environment.GetEnvironmentVariable("DEMO_MERCHANTID")}" +
                                        $"/{mandateFormApiHash}/{res.mandateId}/{res.requestId}/rest.reg";
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogDebug($"Error Occurred: {ex.Message}");
                                }
                            }


                            //write data into csv file as response
                            csv += snCount.ToString().Replace(",", ";").Trim() + ',' +
                                name + ',' + email + ',' + phone + ',' + bankCode + ',' + account + ',' +
                                amount + ',' + startDate + ',' + endDate + ',' + maxNoDebits + ',' +
                                mandateRRR + ',' + mandateUrl + ',' + requestId + ',';

                            //add new line in csv file
                            csv += "\r\n";
                        }
                    }

                    //delete file created
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug($"Failed to Delete file with error: {ex.Message}");
                    }

                    byte[] csvBytes = Encoding.UTF8.GetBytes(csv);
                    var result = new FileContentResult(csvBytes, "text/csv");
                    result.FileDownloadName = $"generatedMandates{DateTime.Now:dd - MM - yyyy}.csv";
                    return result;
                }
            }

            return null;
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
