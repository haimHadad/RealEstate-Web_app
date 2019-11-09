using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RealEstate_Web_app.ModelBinders;
using RealEstate_Web_app.Models;
using Nethereum.Web3;

using Nethereum.Geth;

using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.CQS;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using System.Numerics;
using System.Threading;
using Nethereum.RPC.Eth.DTOs;
using NBitcoin;

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
                //acc.InfuraApiKey = "4dc41c6f591d4d61a3a2e32a219c6635";
                Account.ValidateAddress(acc.AccountAddress);
                var web3 = new Web3(acc.AccountNetwork + "" + acc.InfuraApiKey);
                acc.AccountBalance = await getAccountBalanceFromBlockChainAsync(acc, acc.AccountAddress);
                String declaredAddress = acc.AccountAddress;    //address from son = textbox of our web dApp = Account class that Haim created
                String realAddress = acc.Address;               //address from father = Nethereum Speciel Class of Account 
                if (declaredAddress.Equals(realAddress))        //If the addresses are matched => Login details are correct = > dApp will show wallt(account) content in the next view
                {
                    acc.IsValidated = true;
                    myAccount = acc;
                    //SmartContractInteractExample();
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


            //var balance = await web3.Eth.GetBalance.SendRequestAsync(acc.AccountAddress); ///*"https://ropsten.infura.io/v3/4dc41c6f591d4d61a3a2e32a219c6635"*/
            // var etherAmount = Web3.Convert.FromWei(balance.Value);
            //double tempBalance = (double)etherAmount;
            // tempBalance = Math.Truncate(tempBalance * 10000) / 10000;
            //acc.AccountBalance = tempBalance;//Complete details of myAccount (real ETH balance of the wallet)
            //acc.AccountBalance = Math.Round(tempBalance, 2); ;
            //double money = await acc.GetAccountBalance();                                         =>Ignore! 
            //var privateKey = "8a24eeca6f3d9fc95b27b187c7240ae9b279ed73484fba3e486fcb4afc463121";  =>Ignore!
            //var privateKey2 = "ce155c9664386764ee49f72aa0e5d2820c7dee301154b545e26e69f6408f4d34"; =>Ignore!
            //var ValidateAccount = new Nethereum.Web3.Accounts.Account(privateKey2);               =>Ignore!
            //var transaction = await web3.TransactionManager.SendTransactionAsync(acc.AccountAddress, acc.AccountAddress, new Nethereum.Hex.HexTypes.HexBigInteger(1)); =>Ignore!
            //var unlockAccountResult = await web3.Personal.UnlockAccount.SendRequestAsync(acc.AccountAddress, acc.AccountPassword, 60);                                 =>Ignore!
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


        async void SmartContractInteractExample()
        {
            var contractAddress = "0xEf0cFd55488895E63cA368D989405ab348F1BBd7"; //deployed using RemixIDE and MetaMask
            var contractABI = @"[{""constant"":true,""inputs"":[],""name"":""getBalance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address payable"",""name"":""_to"",""type"":""address""},{""internalType"":""uint256"",""name"":""_value"",""type"":""uint256""}],""name"":""send"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""payable"":true,""stateMutability"":""payable"",""type"":""fallback""}]";
            var senderAddressTest = myAccount.AccountAddress;
            var recipientAddressTest = "0x7988dfD8E9ceCb888C1AeA7Cb416D44C6160Ef80";
            var senderPrivateKeyTest = myAccount.AccountPassword;
            var accountTest = new Nethereum.Web3.Accounts.Account(senderPrivateKeyTest);
            var web3Test = new Web3(accountTest, myAccount.AccountNetwork + "" + myAccount.InfuraApiKey);

            //----------------------------------------- Interact  deployed contract (using RemixIDE and MetaMask --------------------------------------------

            /*var contractTest = web3Test.Eth.GetContract(contractABI, contractAddress);
            var transaction = await web3Test.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractAddress, 2.00m, 2, new BigInteger(25000));
            var wieEtherTest = 1000000000000000000; // = 1 ETH
            var getBalanceFunction = contractTest.GetFunction("getBalance");
            var resultGetBalance = await getBalanceFunction.CallAsync<UInt64>(); // work
            var sendFunction = contractTest.GetFunction("send");
            var gas = await sendFunction.EstimateGasAsync(senderAddressTest, null, null, recipientAddressTest, wieEtherTest);
            var receiptAmountSend =await sendFunction.SendTransactionAndWaitForReceiptAsync(senderAddressTest, gas, null, null, recipientAddressTest, wieEtherTest);
            int i = 1;*/

            //----------------------------------------- Interact deployed contract (using RemixIDE and Metamask --------------------------------------------


            //----------------------------------------- Deploy and interact contract using Nethereum only --------------------------------------------

            var contractByteCode = "0x608060405234801561001057600080fd5b5061011d806100206000396000f3fe60806040526004361060265760003560e01c806312065fe0146028578063d0679d3414604c575b005b348015603357600080fd5b50603a6095565b60408051918252519081900360200190f35b348015605757600080fd5b50608160048036036040811015606c57600080fd5b506001600160a01b038135169060200135609a565b604080519115158252519081900360200190f35b303190565b6000303182111560a957600080fd5b6040516001600160a01b0384169083156108fc029084906000818181858888f1935050505015801560de573d6000803e3d6000fd5b506001939250505056fea265627a7a723158207853790325ad5a0a48cccfd3f1d8bd7b195dafeaa0b0f8d9e899ea78a42e667a64736f6c634300050b0032";
            var receiptContract = await web3Test.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(contractABI, contractByteCode, senderAddressTest, new HexBigInteger(900000), null);
            var contractAddress2 = receiptContract.ContractAddress;
            var contractTest2 = web3Test.Eth.GetContract(contractABI, contractAddress2);
            var transaction2 = await web3Test.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractAddress2, 1.00m, 2, new BigInteger(25000));
            var wieEtherTest2 = 1000000000000000000; // = 1 ETH
            var getBalanceFunction2 = contractTest2.GetFunction("getBalance");
            var resultGetBalance2 = await getBalanceFunction2.CallAsync<UInt64>(); // work
            var sendFunction2 = contractTest2.GetFunction("send");
            var gas2 = await sendFunction2.EstimateGasAsync(senderAddressTest, null, null, recipientAddressTest, wieEtherTest2);
            var receiptAmountSend2 = await sendFunction2.SendTransactionAndWaitForReceiptAsync(senderAddressTest, gas2, null, null, recipientAddressTest, wieEtherTest2);
            int i2 = 1;

            //----------------------------------------- Deploy and interact contract using Nethereum only --------------------------------------------
            //var transaction = await web3Test.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(recipientAddressTest, 1.11m +3);
        }

        public async Task<double> getAccountBalanceFromBlockChainAsync(Account myAccount , String AccountAddress)
        {
            if(AccountAddress == null || myAccount == null)
                return -1;
            var myWeb3 = new Web3(myAccount.AccountNetwork + "" + myAccount.InfuraApiKey);
            var balance = await myWeb3.Eth.GetBalance.SendRequestAsync(AccountAddress); 
            var etherAmount = Web3.Convert.FromWei(balance.Value);
            double tempBalance = (double)etherAmount;
            tempBalance = Math.Truncate(tempBalance * 10000) / 10000;
            return tempBalance;
        }

    }

}