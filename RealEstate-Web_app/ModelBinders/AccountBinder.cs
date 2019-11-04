
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RealEstate_Web_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace RealEstate_Web_app.ModelBinders
{
    public class AccountBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
           
                if (bindingContext == null)
                {
                    throw new ArgumentNullException(nameof(bindingContext));
                }

                var modelName = bindingContext.ModelName;

                HttpContext httpContext = bindingContext.HttpContext;
                String _accountAddress = httpContext.Request.Form["AccountAddress"];
                String _accountPassword = httpContext.Request.Form["AccountPassword"];
                String _accountNetwork = httpContext.Request.Form["AccountNetwork"];

            try
            {
                Account model = new Account(_accountAddress, _accountPassword, _accountNetwork);
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
