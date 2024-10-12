using MagicVilla_CouponAPI.Models;

namespace MagicVilla_CouponAPI.Data
{
    public static class CouponStore
    {
        public static List<Coupon> coupons = new List<Coupon>
        {
            new Coupon
            {
                Id = 1,
                Name = "Summer Sale",
                Percent = 20,
                IsActive = true
            },
            new Coupon
            {
                Id = 2,
                Name = "Winter Sale",
                Percent = 30,
                IsActive = true
            },
            new Coupon
            {
                Id = 3,
                Name = "Spring Sale",
                Percent = 25,
                IsActive = true
            }
        };
    }
}
