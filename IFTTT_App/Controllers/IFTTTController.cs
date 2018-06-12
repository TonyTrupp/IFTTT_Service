using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;

namespace IFTTT_App.Controllers
{
	[Produces("application/json")]
	[Route("/ifttt/v1")]
	public class IFTTTController : ControllerBase
    {
		private IConfiguration _configuration;

		public IFTTTController (IConfiguration configuration){
			_configuration = configuration; 
		}
		
		[HttpGet]      
		public ActionResult Get()
        {
			return NotFound("no trigger slug specified");
        }

		[HttpGet("triggers/scheduled_event")]
		[HttpPost("triggers/scheduled_event")]
		public ActionResult ScheduledEvents([FromBody] IFTTTRequestBody bodyData)
		{
            if(!ServiceKeyAuthorized()){
				return UnauthorizedErrorResponse();
            }

			var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); 
            
			var newEventsList = new List<ScheduledEvent>();
			newEventsList.Add(new ScheduledEvent
			{
				TriggerIngredient = "Tom",
				meta = new EventMetaData
				{
					id = timestamp+"-123",
					timestamp = (int)timestamp
				}
			});      
			newEventsList.Add(new ScheduledEvent { 
				TriggerIngredient = "Dick",
                meta = new EventMetaData
                {
					id = timestamp+"-234",
					timestamp = ((int)timestamp)-1
                } 
			});         
			newEventsList.Add(new ScheduledEvent { 
				TriggerIngredient = "Harry",
                meta = new EventMetaData
                {
					id = timestamp+"-345",
					timestamp = ((int)timestamp)-2
                } 
			});

			if(bodyData?.limit!=null && bodyData.limit>=0){
				newEventsList = newEventsList.Take<ScheduledEvent>(bodyData.limit).ToList<ScheduledEvent>();
			}

			return Ok(new
            {
				data = newEventsList
            });
        }

		//Used only by IFTTT for validation 
        [HttpGet("status")]
		[HttpPost("status")]
        public IActionResult Status()
        {
			if(!ServiceKeyAuthorized()){
				return UnauthorizedErrorResponse();
			}     

			return Ok(new
            {
                data = new
                {
                    success = true
                }
            });
        }

        //Used only by IFTTT for validation 
		[HttpGet("test/setup")]
		[HttpPost("test/setup")]
		public IActionResult TestSetup()
		{
            if(!ServiceKeyAuthorized()){
				return UnauthorizedErrorResponse();
            } 

			return Ok(new { 
				data = new { 
					samples = new {} 
				} 
			});
        }

		protected bool ServiceKeyAuthorized(){
			var headers = HttpContext.Request?.Headers;
            var reqKey = headers["IFTTT-Channel-Key"];
            var savedKey = _configuration["IFTTT_Service_Key"];
			return (reqKey == savedKey);
		}

		protected JsonResult UnauthorizedErrorResponse()
		{
			return new JsonResult(new
			{
				errors = new Object[] { new { message = "Unauthorized. Incorrect Channel Key" }  }
			}){
				StatusCode = 401 // Status code here 
			};
		}
    }   

	public class IFTTTRequestBody
    {
		public int limit { get; set; } = 50;
    }

	class ScheduledEvent {
		[JsonProperty(PropertyName = "trigger_ingredient")]
		public string TriggerIngredient { get; set; }
		public EventMetaData meta { get; set; }
	}

	class EventMetaData
    {
		public string id { get; set; }
        public int timestamp { get; set; }
    }
}
