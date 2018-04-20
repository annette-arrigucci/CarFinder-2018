using CarFinder.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Bing;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet;
using Newtonsoft.Json.Linq;

namespace CarFinder.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    //this is a quick and dirty solution to allow cross-origin resource sharing
    [RoutePrefix("api/Car")]
    public class CarController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //Calling SQL stored procedures
        /// <summary>
        /// Get list of all years in car database
        /// </summary>
        /// <returns></returns>
        [Route("Years")]
        public async Task<List<string>> GetYears()
        {
            return await db.GetYears();
        }

        /// <summary>
        /// Get all car makes according to a specified year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [Route("{year}/Makes")]
        public async Task<List<string>> GetMakes(string year)
        {
            return await db.GetMakes(year);
            //TO DO: Need to create this stored procedure
        }

        /// <summary>
        /// Get all models for a specified year and car make.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <returns></returns>
        [Route("{year}/{make}/Models")]
        public async Task<List<string>> GetModels(string year, string make)
        {
            return await db.GetModels(year, make);
        }

        /// <summary>
        /// Get all trims models for a specified year, car make and model.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{year}/{make}/{model}/Trims")]
        public async Task<List<string>> GetTrims(string year, string make, string model)
        {
            return await db.GetTrims(year, make, model);
        }

        /// <summary>
        /// Get all details on a specified car according to year, make, model and trim.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        [Route("{year}/{make}/{model}/{trim}")]
        public async Task<List<Car>> GetCarByYearMakeModelTrim(string year, string make, string model, string trim)
        {
            return await db.GetCarByYearMakeModelTrim(year, make, model, trim);
        }

        /// <summary>
        /// Get all details on a specified car according to year, make, model and trim.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        [Route("{year}/{make}/{model}")]
        public async Task<List<Car>> GetCarByYearMakeModel(string year, string make, string model)
        {
            return await db.GetCarByYearMakeModel(year, make, model);
        }

        ///////////// RETURNING DATA TO VIEW ///////////////
        // api/Car/getCar?year=2014&make=Kia&Model=Soul&Trim=4dr%20Wagon%20(1.6L%204cyl%206A)
        /// <summary>
        /// Get all car details, image, and recall info for given year, make, model, and trim.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        [Route("getCar")]
        public async Task<IHttpActionResult> GetCarData(string year = "", string make = "", string model = "", string trim = "")
        {
            HttpResponseMessage response;
            var content = "";

            //This is a call to "your" personal API to get a car. 
            //You may need to change the method name 

            List<Car> carList = await GetCarByYearMakeModelTrim(year, make, model, trim);
            var singleCar = carList.First();
            var car = new CarViewModel
            {
                Car = singleCar,
                Recalls = content,
                Image = ""

            };

            //Get recall Data

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://one.nhtsa.gov/");
                try
                {
                    response = await client.GetAsync("webapi/api/Recalls/vehicle/modelyear/" + year + "/make/"
                        + make + "/model/" + model + "?format=json");
                    content = await response.Content.ReadAsStringAsync();
                    
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }
            //car.Recalls = JsonConvert.DeserializeObject(content);
            car.Recalls = content;


            //////////////////////////////   My Bing Search   //////////////////////////////////////////////////////////

            /*string query = year + " " + make + " " + model + " " + trim;

            string rootUri = "https://api.datamarket.azure.com/Bing/Search";

            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUri));

            var accountKey = ConfigurationManager.AppSettings["searchKey"]; ;

            bingContainer.Credentials = new NetworkCredential(accountKey, accountKey);


            var imageQuery = bingContainer.Image(query, null, null, null, null, null, null);

            var imageResults = imageQuery.Execute().ToList();


            car.Image = imageResults.First().MediaUrl;*/

            var client2 = new HttpClient();
            var accountKey = ConfigurationManager.AppSettings["arrigucci-searchimages"];
            // Request headers  
            client2.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", accountKey);
            // Request parameters
            string query = year + " " + make + " " + model + " " + trim;
            string count = "1";
            string offset = "0";
            //string mkt = "en-us";
            var ImgSearchEndPoint = "https://api.cognitive.microsoft.com/bing/v7.0/images/search?";
            var result = await client2.GetAsync(string.Format("{0}q={1}&count={2}&offset={3}", ImgSearchEndPoint, WebUtility.UrlEncode(query), count, offset));
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(json);
            var items = data.value;
            car.Image = items[0].contentUrl;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            return Ok(car);

        }

        ///////////// RETURNING DATA TO VIEW ///////////////
        // api/Car/getCarNoTrim?year=2014&make=Kia&Model=Soul
        /// <summary>
        /// Get all car details, image, and recall info for given year, make, model, and trim.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="make"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("getCarNoTrim")]
        public async Task<IHttpActionResult> GetCarData(string year = "", string make = "", string model = "")
        {
            HttpResponseMessage response;
            var content = "";

            //This is a call to "your" personal API to get a car. 
            //You may need to change the method name

            List<Car> carList = await GetCarByYearMakeModel(year, make, model);
            var singleCar = carList.First();
            var car = new CarViewModel
            {
                Car = singleCar,
                Recalls = content,
                Image = ""

            };

            //Get recall Data

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://one.nhtsa.gov/");
                try
                {
                    response = await client.GetAsync("webapi/api/Recalls/vehicle/modelyear/" + year + "/make/"
                        + make + "/model/" + model + "?format=json");
                    content = await response.Content.ReadAsStringAsync();

                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }
            //car.Recalls = JsonConvert.DeserializeObject(content);
            car.Recalls = content;


            //////////////////////////////   My Bing Search   //////////////////////////////////////////////////////////

            /*string query = year + " " + make + " " + model;

            string rootUri = "https://api.datamarket.azure.com/Bing/Search";

            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUri));

            var accountKey = ConfigurationManager.AppSettings["searchKey"]; ;

            bingContainer.Credentials = new NetworkCredential(accountKey, accountKey);


            var imageQuery = bingContainer.Image(query, null, null, null, null, null, null);

            var imageResults = imageQuery.Execute().ToList();


            car.Image = imageResults.First().MediaUrl;*/

            var client2 = new HttpClient();
            var accountKey = ConfigurationManager.AppSettings["arrigucci-searchimages"];
            // Request headers  
            client2.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", accountKey);
            // Request parameters
            string query = year + " " + make + " " + model;
            string count = "1";
            string offset = "0";
            //string mkt = "en-us";
            var ImgSearchEndPoint = "https://api.cognitive.microsoft.com/bing/v7.0/images/search?";
            var result = await client2.GetAsync(string.Format("{0}q={1}&count={2}&offset={3}", ImgSearchEndPoint, WebUtility.UrlEncode(query), count, offset));
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(json);
            var items = data.value;
            car.Image = items[0].contentUrl;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            return Ok(car);

        }
    }
}
