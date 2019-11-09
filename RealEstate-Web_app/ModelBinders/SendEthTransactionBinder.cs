using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RealEstate_Web_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate_Web_app.ModelBinders
{
    public class SendEthTransactionBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {

            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;

            HttpContext httpContext = bindingContext.HttpContext;
            String _AddressTo = httpContext.Request.Form["RecipientAddressInput"];
            String _amountTo = httpContext.Request.Form["RecipientAmmountInput"];
            double amountToDouble = Convert.ToDouble(_amountTo);

            try
            {
                Account.ValidateAddress(_AddressTo);
                SendEthTransaction model = new SendEthTransaction(_AddressTo, amountToDouble);
                bindingContext.Result = ModelBindingResult.Success(model);

            }
            catch (Exception e)
            {
                //Do nothing, a null will be returned
            }

            //TODO: validate and update model state if not valid
            //...
            return Task.CompletedTask;


        }


    }
}
