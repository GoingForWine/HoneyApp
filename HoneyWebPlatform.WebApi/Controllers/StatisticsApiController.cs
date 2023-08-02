namespace HoneyWebPlatform.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Services.Data.Interfaces;
    using Services.Data.Models.Statistics;

    [Route("api/statistics")]
    [ApiController]
    public class StatisticsApiController : ControllerBase
    {
        private readonly IHoneyService honeyService;

        public StatisticsApiController(IHoneyService honeyService)
        {
            this.honeyService = honeyService;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(StatisticsServiceModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                StatisticsServiceModel serviceModel =
                    await honeyService.GetStatisticsAsync();

                return Ok(serviceModel);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}