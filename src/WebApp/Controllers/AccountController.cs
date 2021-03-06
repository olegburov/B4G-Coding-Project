﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services.Email;
using WebApp.Services.Zillow;
using WebApp.Services.Zillow.Helpers;
using WebApp.Services.Zillow.SearchResults;

namespace WebApp.Controllers
{
  [Route("[controller]/[action]")]
  public class AccountController : Controller
  {
    private ILogger<AccountController> logger;
    private AppDbContext dbContext;
    private ZillowClient zillowClient;
    private RentCalculator rentCalculator;
    private EmailClient emailClient;

    public AccountController(ILogger<AccountController> logger, AppDbContext context, ZillowClient client, RentCalculator rentCalculator, EmailClient emailClient)
    {
      this.logger = logger;
      this.dbContext = context;
      this.zillowClient = client;
      this.rentCalculator = rentCalculator;
      this.emailClient = emailClient;
    }

    [HttpGet]
    public IActionResult Join()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Join(AccountModel account)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      try
      {
        account.IpAddress = HttpContext.Connection.RemoteIpAddress.GetAddressBytes();
        this.dbContext.Accounts.Add(account);
        await this.dbContext.SaveChangesAsync();
      }
      catch(Exception ex)
      {
        this.logger.LogError($"An error occurred while saving entity '{nameof(AccountModel)}': {ex.ToString()}");
      }

      TempData.Clear();
      TempData.Add("Account.Id", account.Id);
      TempData.Add("Account.FirstName", account.FirstName);

      return RedirectToAction("Address");
    }

    [HttpGet]
    public IActionResult Address()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Address(AddressModel address)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      var result = this.dbContext.Addresses.Where(w => w.GooglePlaceId.Equals(address.GooglePlaceId));
      if (!result.Any())
      {
        try
        {
          this.dbContext.Addresses.Add(address);
          await this.dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
          this.logger.LogError($"An error occurred while saving entity '{nameof(AddressModel)}': {ex.ToString()}");
        }
      }
      else
      {
        address = result.First();
      }

      TempData.Remove("Address.Id");
      TempData.Add("Address.Id", address.Id);

      return RedirectToAction("Customize", address);
    }

    [HttpGet]
    public async Task<IActionResult> Customize(AddressModel address)
    {
      var result = await this.zillowClient.GetDeepSearchResults(address);
      var valuationRange = result.RentZestimate?.ValuationRange;

      if (result.Zestimate == null)
      {
        valuationRange = ValuationRange.Empty;
        ViewData["Zestimate.Alert"] = "No data has been found for the input address.";
      }
      else if (valuationRange == null)
      {
        valuationRange = this.rentCalculator.GetRentBasedOnHomePrice(result.Zestimate.Amount);
      }

      ViewData["Zestimate.Low"] = valuationRange.Low;
      ViewData["Zestimate.High"] = valuationRange.High;

      TempData.Keep();

      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Customize(RentEstimateModel rentEstimate)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      rentEstimate.AccountId = (Guid)TempData["Account.Id"];
      rentEstimate.AddressId = (Guid)TempData["Address.Id"];

      try
      { 
        this.dbContext.RentEstimates.Add(rentEstimate);
        await this.dbContext.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        this.logger.LogError($"An error occurred while saving entity '{nameof(RentEstimateModel)}': {ex.ToString()}");
      }

      TempData.Keep();

      return RedirectToAction("ThankYou");
    }

    public IActionResult ThankYou()
    {
      var accountId = (Guid)TempData["Account.Id"];
      var account = this.dbContext.Accounts.Find(accountId);

      this.emailClient.SendSignupMessage(account);

      TempData.Keep();

      return View();
    }

    public IActionResult VerifyEmail(string email)
    {
      var result = this.dbContext.Accounts.Where(account => account.Email.Equals(email));

      if (result.Any())
      {
        return Json($"Email is already taken.");
      }

      return Json(true);
    }
  }
}