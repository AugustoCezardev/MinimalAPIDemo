using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Data.DTO;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Validations;
using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddScoped<IValidator<CouponCreateDTO>, CouponCreateValidation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/v1/coupons", (ILogger<Program> _logger) =>
{
    APIResponse response = new APIResponse();
    _logger.LogInformation("GetCoupons endpoint is called");

    var coupons = CouponStore.coupons;

    response.IsSuccess = true;
    response.Result = coupons;
    response.StatusCode = coupons.Count > 0 ? HttpStatusCode.OK : HttpStatusCode.NoContent;

    return Results.Ok(response);
}).WithName("GetCoupons")
.Produces<APIResponse>(200);

app.MapGet("/api/v1/coupon/{ind:int}", (int id) =>
{
    APIResponse response = new APIResponse();

    var coupon = CouponStore.coupons.FirstOrDefault(c => c.Id == id);
    if (coupon == null)
    {
        return Results.NoContent();
    }

    response.IsSuccess = true;
    response.Result = coupon;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetCoupon")
.Produces<Coupon>(200)
.Produces(400);

app.MapPost("/api/v1/coupon", (IMapper _mapper, IValidator<CouponCreateDTO> _validator, [FromBody] CouponCreateDTO couponDTO) =>
{
    APIResponse response = new APIResponse() { StatusCode = HttpStatusCode.BadRequest };

    var validationResult = _validator.Validate(couponDTO);
    if(!validationResult.IsValid)
    {
        response.ErrorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        response.IsSuccess = false;
        return Results.BadRequest(response);
    }

    if(CouponStore.coupons.FirstOrDefault(c => c.Name.ToLower() == couponDTO.Name.ToLower()) != null)
    {
        response.IsSuccess = false;
        response.ErrorMessages.Add("Coupon already exists");
        response.StatusCode = HttpStatusCode.BadRequest;
        return Results.BadRequest(response);
    }

    Coupon coupon = _mapper.Map<Coupon>(couponDTO);
    coupon.Id = CouponStore.coupons.Max(c => c.Id) + 1;
    coupon.Created = DateTime.Now;

    CouponStore.coupons.Add(coupon);

    response.IsSuccess = true;
    response.Result = _mapper.Map<CouponCreateDTO>(coupon);
    response.StatusCode = HttpStatusCode.Created;


    return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, response);
}).WithName("CreateCoupon")
.Accepts<CouponDTO>("application/json")
.Produces<APIResponse>(201)
.Produces(400);

app.MapPut("/api/v1/coupon/{id:int}", (int id, [FromBody] Coupon coupon) =>
{
    APIResponse response = new APIResponse();

    var existingCoupon = CouponStore.coupons.FirstOrDefault(c => c.Id == id);
    if (existingCoupon == null)
    {
        response.IsSuccess = false;
        response.StatusCode = HttpStatusCode.BadRequest;
        response.ErrorMessages.Add("Coupon not found");
        return Results.BadRequest(response);
    }
    existingCoupon.Name = coupon.Name;
    existingCoupon.Percent = coupon.Percent;
    existingCoupon.IsActive = coupon.IsActive;
    existingCoupon.LastUpdated = DateTime.Now;

    response.IsSuccess = true;
    response.Result = existingCoupon;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(existingCoupon);
}).WithName("UpdateCoupon")
.Produces<APIResponse>(200)
.Produces(400);

app.MapDelete("/api/v1/coupon/{ind:int}", (int id) =>
{
    APIResponse response = new APIResponse();
    var existingCoupon = CouponStore.coupons.FirstOrDefault(c => c.Id == id);
    if (existingCoupon == null)
    {
        response.IsSuccess = false;
        response.StatusCode = HttpStatusCode.BadRequest;
        response.ErrorMessages.Add("Coupon not found");
        return Results.BadRequest(response);
    }
    CouponStore.coupons.Remove(existingCoupon);

    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("DeletCoupon")
.Produces<APIResponse>(200)
.Produces<APIResponse>(400);


app.UseHttpsRedirection();

app.Run();
