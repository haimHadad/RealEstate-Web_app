using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Web3;
using RealEstate_Web_app.ModelBinders;
using RealEstate_Web_app.Models;

namespace RealEstate_Web_app.Controllers
{
    public class SendDepositETHController : Controller
    {

        [HttpPost]
        public IActionResult SendMoney(String AddressTo, String Amount)
        {
           /* double amountToSend = Convert.ToDouble(Amount);
            Account transferFrom = AccountController.myAccount;
            var url = "" + transferFrom.AccountNetwork + "" + transferFrom.InfuraApiKey;
            var myWeb3 = new Web3(transferFrom, url);
            transferFrom.AccountBalance = amountToSend;  //for example
            ModelState.Clear();*/
            return View();
        }

        /*try
            {
                Account.ValidateAddress(AddressTo);

            }
            catch(Exception e)
            {
                return;
            }*/



        [HttpPost]
        public IActionResult UpdateBalance(Account Wallet)
        {
            //Account transferFrom = AccountController.myAccount;
            Wallet.AccountBalance = 1;
            ModelState.Clear();
            return View(Wallet);
        }


        [HttpPost]
        public IActionResult TransferEth([ModelBinder(typeof(SendEthTransactionBinder))] SendEthTransaction tnc)
        {
            return View();
        }
    }
}