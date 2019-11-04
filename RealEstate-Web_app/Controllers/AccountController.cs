using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealEstate_Web_app.ModelBinders;
using RealEstate_Web_app.Models;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;


namespace RealEstate_Web_app.Controllers
{
    public class AccountController : Controller
    {
        public static Account myAccount { get; set; }


        /* [HttpPost]
          public IActionResult Login(Account acc)
          {
              return View("Login", acc);
          }*/


        [HttpPost]
        public async Task<IActionResult> LoginAsync([ModelBinder(typeof(AccountBinder))] Account acc)
        {
            myAccount = null; //A precautionary step
            try
            {
                if (acc == null)
                {
                    throw new WrongAccountDetails("Incorrect account or password");
                }
                acc.InfuraApiKey = "4dc41c6f591d4d61a3a2e32a219c6635";
                Account.ValidateAddress(acc.AccountAddress);
                var web3 = new Web3(acc.AccountNetwork + "" + acc.InfuraApiKey);
                
                var balance = await web3.Eth.GetBalance.SendRequestAsync(acc.AccountAddress); ///*"https://ropsten.infura.io/v3/4dc41c6f591d4d61a3a2e32a219c6635"*/
                var etherAmount = Web3.Convert.FromWei(balance.Value);
                double tempBalance = (double)etherAmount;
                
                //acc.AccountBalance = Math.Round(tempBalance, 4);
                tempBalance = Math.Truncate(tempBalance * 10000) / 10000;

                acc.AccountBalance = tempBalance;//Complete details of myAccount (real ETH balance of the wallet)
                //acc.AccountBalance = Math.Round(tempBalance, 2); ;
                //double money = await acc.GetAccountBalance();                                         =>Ignore! 
                //var privateKey = "8a24eeca6f3d9fc95b27b187c7240ae9b279ed73484fba3e486fcb4afc463121";  =>Ignore!
                //var privateKey2 = "ce155c9664386764ee49f72aa0e5d2820c7dee301154b545e26e69f6408f4d34"; =>Ignore!
                //var ValidateAccount = new Nethereum.Web3.Accounts.Account(privateKey2);               =>Ignore!
                //var transaction = await web3.TransactionManager.SendTransactionAsync(acc.AccountAddress, acc.AccountAddress, new Nethereum.Hex.HexTypes.HexBigInteger(1)); =>Ignore!
                //var unlockAccountResult = await web3.Personal.UnlockAccount.SendRequestAsync(acc.AccountAddress, acc.AccountPassword, 60);                                 =>Ignore!

                String declaredAddress = acc.AccountAddress;    //address from son = textbox of our web dApp = Account class that Haim created
                String realAddress = acc.Address;               //address from father = Nethereum Speciel Class of Account 
                if (declaredAddress.Equals(realAddress))        //If the addresses are matched => Login details are correct = > dApp will show wallt(account) content in the next view
                {
                    acc.IsValidated = true;
                    myAccount = acc;
                    return View("Login", acc);
                }
                else
                    throw new WrongAccountDetails("Incorrect account or password");                   
            } //Until here we check if the login details are correct, in case they are, dApp will continue to the next view (Ethereum wallet detials)

            catch (WrongAccountDetails ex) //in case of incorrect account
            {
                acc = null;
                ViewData["ErrorMessage"] = ex.Message; 
                return View("LoginFail");
            }
        }

        [HttpPost]
        public IActionResult LoginError()
        {
            myAccount = null;
            return RedirectToAction("Index", "Home");

        }

        public IActionResult LogOut()
        {
            myAccount = null;
            return RedirectToAction("Index", "Home");

        }
    }

}