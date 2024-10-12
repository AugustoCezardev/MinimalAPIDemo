using AutoMapper;
using MagicVilla_CouponAPI.Data.DTO;
using MagicVilla_CouponAPI.Models;

namespace MagicVilla_CouponAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Coupon, CouponDTO>().ReverseMap();
            CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
        }
    }
}
