using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using dice_roller.model;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace dice_roller
{
    public static class rollSimple
    {
        [FunctionName("roll-simple")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "roll/{dice}")]
            HttpRequest req, string dice, ILogger log
        )
        {
            log.LogInformation("TRIGGERED function [ 'roll-simple' ]");
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            log.LogInformation($"PARSE parameter [ 'dice' ] value [ '{dice}' ]");
            string pattern = @"^(\d+)d(\d+)([\+\-]\d+)?$";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(dice);

            log.LogInformation($"TEST parameter value MATCHES pattern [ '{pattern}' ]: [ '{m.Success}' ]");
            if (!m.Success)
            {
                ErrorMessage errorMessage = new ErrorMessage(
                    "BadRequest",
                    dice,
                    $"Request doesn't match needed format. Expected RegEx pattern: {pattern}"
                );
                string errorMessageJson = JsonConvert.SerializeObject(errorMessage, jsonSerializerSettings);
                return new BadRequestObjectResult(errorMessageJson);
            }

            Group countGroup = m.Groups[1];
            int count = Convert.ToInt32(countGroup.Value);
            Group dieGroup = m.Groups[2];
            int die = Convert.ToInt32(dieGroup.Value);
            Group modifierGroup = m.Groups[3];
            int modifier = (modifierGroup.Length > 0) ? Convert.ToInt32(modifierGroup.Value) : 0;

            log.LogInformation($"GET [ '{count}' ] random numbers BETWEEN [ '1 - {die}' ]");
            var rand = new Random();
            DiceToss diceToss = new DiceToss(dice);
            for (int i = 0; i < count; i++)
            {
                diceToss.Tosses.Add(rand.Next(1, die));
            }

            log.LogInformation($"GET sum OF random numbers AND add modifier [ '{modifier}' ]");
            diceToss.Result = diceToss.Tosses.Sum() + modifier;

            string responseMessageJson = JsonConvert.SerializeObject(diceToss, jsonSerializerSettings);
            return new OkObjectResult(responseMessageJson);
        }
    }
}
