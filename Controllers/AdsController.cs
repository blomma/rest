using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rest.Models;
using rest.Exceptions;
using rest.Services;
using System.Linq;
using System;

namespace rest.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase {
        private readonly AdService adService;

        public AdsController(AdService adService) {
            this.adService = adService;
        }

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<Ad>>> GetAds([FromQuery] Filter filter) {
            try {
                var ads = await adService.GetAds(filter);

                return Ok(ads);
            } catch (ArgumentOutOfRangeException e) {
                throw new HttpResponseException {
                    Status = 400,
                    Value = e.Message
                };
            } catch (System.Exception e) {
                throw new HttpResponseException {
                    Status = 500,
                    Value = e.Message
                };
            }
        }

        [HttpGet("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<Ad>> GetAd(string id) {
            var ad = await adService.GetAd(id);
            if (ad == null) {
                return NotFound();
            }

            return ad;
        }

        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<CreateAdDTO>> PostAd(CreateAdDTO adDTO) {
            if (adDTO == null || adDTO.Body == null || adDTO.Email == null || adDTO.Subject == null) {
                throw new HttpResponseException {
                    Status = 400,
                    Value = "Missing Body, Email or Subject in Ad"
                };
            }

            var ad = await adService.CreateAd(adDTO.Subject, adDTO.Body, adDTO.Email, adDTO.Price);

            return CreatedAtAction(nameof(GetAd), new { id = ad.Id }, ad);
        }

        [HttpDelete("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult<Ad>> DeleteAd(string id) {
            var ad = await adService.DeleteAd(id);
            if (ad == null) {
                return NotFound();
            }

            return ad;
        }
    }
}