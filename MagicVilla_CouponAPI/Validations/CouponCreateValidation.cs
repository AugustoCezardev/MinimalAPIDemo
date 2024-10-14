using FluentValidation;
using MagicVilla_CouponAPI.Data.DTO;

namespace MagicVilla_CouponAPI.Validations
{
    public class CouponCreateValidation: AbstractValidator<CouponCreateDTO>
    {
        public CouponCreateValidation()
        {
            RuleFor(c => c.Name).NotEmpty().MaximumLength(50);
            RuleFor(c => c.Percent).InclusiveBetween(0, 100);
        }
    }
}
