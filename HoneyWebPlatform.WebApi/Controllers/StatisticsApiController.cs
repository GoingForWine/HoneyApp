namespace HoneyWebPlatform.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Services.Data.Interfaces;
    using Services.Data.Models.Statistics;

    [ApiController]
    [Route("api/statistics")]
    public class StatisticsApiController : ControllerBase
    {
        private readonly IHoneyService honeyService;
        private readonly IPropolisService propolisService;

        public StatisticsApiController(IHoneyService honeyService, IPropolisService propolisService)
        {
            this.honeyService = honeyService;
            this.propolisService = propolisService;
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

                //serviceModel = 
                //    await propolisService.GetStatisticsAsync();

                return Ok(serviceModel);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}