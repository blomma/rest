using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rest.Models;
using rest.Exceptions;
using rest.Services;
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
        public async Task<ActionResult<PagedResponse<Ad>>> GetAds([FromQuery] Filter filter) {
            try {
                var result = await adService.GetAds(filter);

                var pagedResult = new PagedResponse<Ad>();
                pagedResult.Items = result.Ads;

                // TODO: Functional, but factor out into something more generic once final
                if (filter.Page != null && filter.Limit != null) {
                    Filter nextFilter = filter.Clone() as Filter;
                    nextFilter.Page = nextFilter.Page + 1;
                    var nextLimit = nextFilter.Page * nextFilter.Limit;
                    String nextUrl = nextLimit <= result.TotalCount
                        ? Url.Action(null, null, nextFilter, Request.Scheme)
                        : null;

                    Filter previousFilter = filter.Clone() as Filter;
                    previousFilter.Page = previousFilter.Page - 1;
                    String previousUrl = previousFilter.Page > 0
                        ? Url.Action("GetAds", null, previousFilter, Request.Scheme)
                        : null;

                    pagedResult.NextPage = !String.IsNullOrWhiteSpace(nextUrl) ? new Uri(nextUrl) : null;
                    pagedResult.PreviousPage = !String.IsNullOrWhiteSpace(previousUrl) ? new Uri(previousUrl) : null;
                }

                return Ok(pagedResult);
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
        public async Task<ActionResult<Ad>> GetAd(string id) {
            var ad = await adService.GetAd(id);
            if (ad == null) {
                return NotFound();
            }

            return ad;
        }

        [HttpPost]
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
        public async Task<ActionResult<Ad>> DeleteAd(string id) {
            var ad = await adService.DeleteAd(id);
            if (ad == null) {
                return NotFound();
            }

            return ad;
        }
    }
}