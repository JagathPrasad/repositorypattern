using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Stripe;
using QUONOW.Libraries;
using QUONOW.Pattern.UnitOfWork;


namespace QUONOW.Models
{
    public class Customer
    {
        #region Properties
        public string CustomerId { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string CardId { get; set; }
        public string Number { get; set; }
        public string ExpirationYear { get; set; }
        public string ExpirationMonth { get; set; }
        public string Cvv { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public int Amount { get; set; }
        public string Interval { get; set; }
        public string productId { get; set; }
        public string UserId { get; set; }
        public string ErrorMsg { get; set; }
        public string ChargeId { get; set; }
        public string SubscriptionId { get; set; }
        public string Token { get; set; }
        public string SourceToken { get; set; }
        public string BankTransactionRefNo { get; set; }
        public string AdvisorName { get; set; }
        public string Phone { get; set; }
        public string FirmName { get; set; }
        public string Address1 { get; set; }
        public string AddressOne { get; set; }
        public string userToken { get; set; }
        public string pubId { get; set; }

        public string single { get; set; }

        public string couple { get; set; }
        #endregion

        private UnitOfWork unit;
        public Customer()
        {
            QUONOWEntities db = new QUONOWEntities();
            this.unit = new UnitOfWork(db);
        }

        public bool CreateCustmer()
        {
            try
            {
                Utility util = new Utility();
                var userDetails = util.GetUserDetailsByToken(this.userToken);
                if (userDetails != null)
                {
                    if (!string.IsNullOrEmpty(userDetails.CustomerId))
                    {
                        this.CustomerId = userDetails.CustomerId;
                        this.Email = userDetails.Email;
                        this.Description = "testing";
                        this.UserId = userDetails.Id.ToString();
                        return true;
                    }
                    else
                    {
                        StripeCustomerService CustomerService = new StripeCustomerService();
                        StripeCustomerCreateOptions Customer = new StripeCustomerCreateOptions();
                        CustomerService.ApiKey = "sk_test_9wnKd0ODnOXr00bysp5h7BD7";
                        this.Email = userDetails.Email;
                        this.Description = "testing";
                        Customer.Email = userDetails.Email;
                        Customer.Description = "testing";
                        Customer.SourceToken = this.CardId;
                        StripeCustomer StripUser = CustomerService.Create(Customer);
                        if (!string.IsNullOrEmpty(StripUser.Id))
                        {
                            var getUserDetails = this.unit._userRepository.SelectAll().Where(x => x.Id == userDetails.Id).FirstOrDefault();
                            getUserDetails.CustomerId = StripUser.Id;
                            this.unit._userRepository.Update(getUserDetails);
                            this.unit.Commit();
                            this.CustomerId = StripUser.Id;
                            this.UserId = userDetails.Id;
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }



        public bool CreateCardDetails()
        {
            try
            {
                SourceCard card = new SourceCard();
                card.Number = this.Number;
                card.ExpirationYear = this.ExpirationYear;
                card.ExpirationMonth = this.ExpirationMonth;
                card.Cvc = this.Cvv;
                card.Name = this.Name;
                StripeCardService cardService = new StripeCardService(Params.stripeApiKey);
                StripeCardCreateOptions cardoption = new StripeCardCreateOptions();
                cardoption.SourceCard = card;
                var cardinfo = cardService.Create(this.CustomerId, cardoption);
                if (!string.IsNullOrEmpty(cardinfo.Id))
                {
                    this.CardId = cardinfo.Id;
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }

        //private bool CallCustomerCreation()
        //{
        //    if (String.IsNullOrEmpty(this.CustomerId))
        //    {
        //        Customer NewCustomer = new Customer();
        //        NewCustomer.Email = this.Email;
        //        NewCustomer.Description = this.Description;
        //        NewCustomer.SourceToken = this.CardId;
        //        try
        //        {
        //            StripeCustomer stpCustomer = NewCustomer.AddCustomer();
        //            this.CustomerId = stpCustomer.Id;
        //        }
        //        catch (Exception ex)
        //        {
        //            this.ErrorMsg = ex.Message;
        //            ErrorLog.Log(ex);
        //            return false;
        //        }
        //    }
        //    return true;
        //}
        //private StripeCustomer AddCustomer()
        //{
        //    StripeError err = new StripeError();
        //    StripeCustomerCreateOptions Customer = new StripeCustomerCreateOptions();
        //    Customer.Email = this.Email;
        //    Customer.Description = this.Description;
        //    Customer.SourceToken = this.CardId;
        //    StripeCustomerService CustomerService = new StripeCustomerService();
        //    CustomerService.ApiKey = "sk_test_9wnKd0ODnOXr00bysp5h7BD7";
        //    StripeCustomer StripUser = CustomerService.Create(Customer);
        //    this.CustomerId = StripUser.Id;
        //    return StripUser;
        //}

        public bool Payment()
        {
            if (CreateCustmer())
            {
                if (CreateCardDetails())
                {
                    try
                    {
                        StripeChargeService service = new StripeChargeService(Params.stripeApiKey);
                        StripeChargeCreateOptions options = new StripeChargeCreateOptions();
                        options.Currency = string.IsNullOrEmpty(this.Currency) ? "inr" : "inr";
                        options.Amount = this.Amount * 100;
                        options.CustomerId = this.CustomerId;
                        options.Description = this.Description;
                        options.ReceiptEmail = this.Email;
                        options.SourceTokenOrExistingSourceId = this.CardId;
                        var getPayment = service.Create(options);
                        if (!string.IsNullOrEmpty(getPayment.Id))
                        {
                            Payment pay = new Models.Payment();
                            pay.Id = Guid.NewGuid();
                            pay.TransactionNo = getPayment.Id;
                            pay.CustomerId = this.CustomerId;
                            pay.Userid = new Guid(this.UserId);
                            pay.CardId = this.CardId;
                            pay.IsActive = true;
                            pay.IsDeleted = false;
                            this.unit._paymentRepository.Save(pay);
                            this.unit.Commit();
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {

                        return false;
                    }

                }
            }

            return false;
        }


        public bool CreateStripeRefund()
        {
            try
            {
                StripeRefundCreateOptions refundcreat = new StripeRefundCreateOptions();
                refundcreat.Amount = this.Amount;
                //  refundcreat.Reason = StripeParams.StripeRefundReason;
                //  var service = new StripeRefundService(StripeParams.ApiKey);
                //   var creatingService = service.Create(this.ChargeId, refundcreat);//later will add some functionality
            }
            catch (Exception ex)
            {
                //  ErrorLog.Log(ex);
                return false;

            }

            return true;

        }


    }
}