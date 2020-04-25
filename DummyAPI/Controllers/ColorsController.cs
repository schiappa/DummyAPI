using Microsoft.AspNetCore.Mvc;

namespace DummyAPI.Controllers
{
    public class ColorsController : BaseAPIController
    {
        [Route("GetBestColorCombination")]
        [HttpGet]
        public ActionResult GetBestColorCombination()
        {
            return Ok("Black and yellow!");
        }

        [Route("GetBestColorCombinationAsASong")]
        [HttpGet]
        public ActionResult GetBestColorCombinationAsASong()
        {
            return Ok("https://www.youtube.com/watch?v=nWAGLkyxQG0");
        }
    }
}
