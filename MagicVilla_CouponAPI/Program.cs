using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Data.DTO;
using MagicVilla_CouponAPI.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/v1/coupons", (ILogger<Program> _logger) =>
{
    _logger.LogInformation("GetCoupons endpoint is called");
    return Results.Ok(CouponStore.coupons);
}).WithName("GetCoupons")
.Produces<List<Coupon>>(200);

app.MapGet("/api/v1/coupon/{id: int}", ([FromRoute] int id) =>
{
    var coupon = CouponStore.coupons.FirstOrDefault(c => c.Id == id);
    if (coupon == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(coupon);
}).WithName("GetCoupon")
.Produces<Coupon>(200)
.Produces(400);

app.MapPost("/api/v1/coupon", ([FromBody] CouponCreateDTO couponDTO) =>
{
    CouponDTO coupon = new CouponDTO
    {
        Name = couponDTO.Name,
        Percent = couponDTO.Percent,
        IsActive = couponDTO.IsActive,
    };
    coupon.Id = CouponStore.coupons.Max(c => c.Id) + 1;
    coupon.Created = DateTime.Now;
    //CouponStore.coupons.Add(coupon);
    return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, coupon);
}).WithName("CreateCoupon")
.Accepts<CouponDTO>("application/json")
.Produces<CouponDTO>(201)
.Produces(400);

app.MapPut("/api/v1/coupon/{id: int}", ([FromRoute] int id, [FromBody] Coupon coupon) =>
{
    var existingCoupon = CouponStore.coupons.FirstOrDefault(c => c.Id == id);
    if (existingCoupon == null)
    {
        return Results.NotFound();
    }
    existingCoupon.Name = coupon.Name;
    existingCoupon.Percent = coupon.Percent;
    existingCoupon.IsActive = coupon.IsActive;
    existingCoupon.LastUpdated = DateTime.Now;
    return Results.Ok(existingCoupon);
}).WithName("UpdateCoupon")
.Produces(200)
.Produces(404);

app.MapDelete("/api/v1/coupon/{id: int}", ([FromRoute] int id) =>
{
    var existingCoupon = CouponStore.coupons.FirstOrDefault(c => c.Id == id);
    if (existingCoupon == null)
    {
        return Results.NotFound();
    }
    CouponStore.coupons.Remove(existingCoupon);
    return Results.NoContent();
}).WithName("DeletCoupon")
.Produces(200)
.Produces(400);


app.UseHttpsRedirection();

app.Run();
