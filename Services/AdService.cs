using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rest.Extensions;
using rest.Models;

namespace rest.Services {

    public class AdService {
        private readonly AdContext _context;

        public AdService(AdContext context) {
            _context = context;
        }

        private IEnumerable<Ad> FilterAds(Filter filter, IEnumerable<Ad> ads) {
            if (ads.Count() == 0) {
                return ads;
            }

            if (string.IsNullOrWhiteSpace(filter.OrderBy)) {
                if (filter.ThenBy?.Count() > 0) {
                    throw new ArgumentOutOfRangeException($"ThenBy sorting is missing OrderBy");
                }

                return ads;
            }

            return ads.OrderBy(filter);
        }

        public async Task<IEnumerable<Ad>> GetAds(Filter filter) {
            var ads = await _context.Ads.ToListAsync();

            return FilterAds(filter, ads).ToList();
        }

        public async Task<Ad> GetAd(string id) {
            var ad = await _context.Ads.FindAsync(id);

            return ad;
        }

        public async Task<Ad> CreateAd(string subject, string body, string email, double? price) {
            var ad = new Ad {
                Id = Guid.NewGuid().ToString(),
                Subject = subject,
                Body = body,
                Email = email,
                Price = price,
                CreatedUtc = DateTime.UtcNow
            };

            _context.Ads.Add(ad);

            await _context.SaveChangesAsync();

            return ad;
        }

        public async Task<Ad> DeleteAd(string id) {
            var ad = await _context.Ads.FindAsync(id);
            if (ad == null) {
                return null;
            }

            _context.Ads.Remove(ad);
            await _context.SaveChangesAsync();

            return ad;
        }
    }
}