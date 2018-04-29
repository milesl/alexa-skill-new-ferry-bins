using NewFerryBins.Api.Skills;
using System.Net.Http;
using System.Web.Http;

namespace NewFerryBins.Api.Controllers
{
    public class NewFerryBinsController : ApiController
    {
        [Route("api/alexa/bins")]
        [HttpPost]
        public HttpResponseMessage Post()
        {
            var speechlet = new NewFerryBinsSkill();
            return speechlet.GetResponse(this.Request);
        }
    }
}