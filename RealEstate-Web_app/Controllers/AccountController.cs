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
                    //await SmartContractInteractExample();
                    await DeployAndSignSalesContract();
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


        async Task SmartContractInteractExample()
        {
            //var contractABI = @"[{""constant"":true,""inputs"":[],""name"":""getBalance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address payable"",""name"":""_to"",""type"":""address""},{""internalType"":""uint256"",""name"":""_value"",""type"":""uint256""}],""name"":""send"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""payable"":true,""stateMutability"":""payable"",""type"":""fallback""}]";
            //var contractByteCode = "0x608060405234801561001057600080fd5b5061011d806100206000396000f3fe60806040526004361060265760003560e01c806312065fe0146028578063d0679d3414604c575b005b348015603357600080fd5b50603a6095565b60408051918252519081900360200190f35b348015605757600080fd5b50608160048036036040811015606c57600080fd5b506001600160a01b038135169060200135609a565b604080519115158252519081900360200190f35b303190565b6000303182111560a957600080fd5b6040516001600160a01b0384169083156108fc029084906000818181858888f1935050505015801560de573d6000803e3d6000fd5b506001939250505056fea265627a7a723158207853790325ad5a0a48cccfd3f1d8bd7b195dafeaa0b0f8d9e899ea78a42e667a64736f6c634300050b0032";
            

            var contractABI = @"[{""constant"":true,""inputs"":[],""name"":""getBalance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getBuyer"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address payable"",""name"":""_to"",""type"":""address""},{""internalType"":""uint256"",""name"":""_value"",""type"":""uint256""}],""name"":""send"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""payable"":true,""stateMutability"":""payable"",""type"":""fallback""}]";
            var contractByteCode = "0x608060405234801561001057600080fd5b50610178806100206000396000f3fe6080604052600436106100345760003560e01c806312065fe014610048578063603daf9a1461006f578063d0679d34146100a0575b600080546001600160a01b03191633179055005b34801561005457600080fd5b5061005d6100ed565b60408051918252519081900360200190f35b34801561007b57600080fd5b506100846100f2565b604080516001600160a01b039092168252519081900360200190f35b3480156100ac57600080fd5b506100d9600480360360408110156100c357600080fd5b506001600160a01b038135169060200135610101565b604080519115158252519081900360200190f35b303190565b6000546001600160a01b031690565b6040516000906001600160a01b0384169083156108fc0290849084818181858888f19350505050158015610139573d6000803e3d6000fd5b506001939250505056fea265627a7a723158201a9f4f8f24e6bad4cf02c24fcedbb062140b4456fa74fa80410eee465a0cf65564736f6c634300050b0032";

            var senderAddress = myAccount.AccountAddress;
            var recipientAddress = "0x7988dfD8E9ceCb888C1AeA7Cb416D44C6160Ef80";
            var senderPrivateKey = myAccount.AccountPassword;
            var accountTest = new Nethereum.Web3.Accounts.Account(senderPrivateKey);
            var web3Test = new Web3(accountTest, myAccount.AccountNetwork + "" + myAccount.InfuraApiKey);

            var payerAccount = new Nethereum.Web3.Accounts.Account("ce155c9664386764ee49f72aa0e5d2820c7dee301154b545e26e69f6408f4d34");
            var web3Payer = new Web3(payerAccount, myAccount.AccountNetwork + "" + myAccount.InfuraApiKey);

            //----------------------------------------- Deploy and interact contract using Nethereum only --------------------------------------------

            double EtherToSend =1.0;
            decimal moneyDeafult = 1.00m;
            decimal EtherToSendDecimal = Convert.ToDecimal(EtherToSend);

            
            var estimateGas = await web3Test.Eth.DeployContract.EstimateGasAsync(contractABI, contractByteCode, senderAddress, null);
            var receiptContract = await web3Test.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(contractABI, contractByteCode, senderAddress, estimateGas, null); //deploy the contract, 900000=time to deploy-constant, after null, we can add parameters to the constructor of the contract

            //var receiptContract = await web3Test.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(contractABI, contractByteCode, senderAddress, new HexBigInteger(900000), null); //deploy the contract, 900000=time to deploy-constant, after null, we can add parameters to the constructor of the contract
            var contractAddress = receiptContract.ContractAddress; //after deployment, we get contrqact address. 
            var ContractDeployedInstance = web3Test.Eth.GetContract(contractABI, contractAddress); //read instance of the contract
            //var transaction = await web3Test.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractAddress, EtherToSendDecimal, 4, new BigInteger(45000)); //send money to the contract, everything is constant except the address and the amount - 1.00m = 1ether in decimal. the 2 after 1.00m is gas price which means the speed for mining.

            var transaction = await web3Payer.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractAddress, EtherToSendDecimal, 4, new BigInteger(45000)); //send money to the contract, everything is constant except the address and the amount - 1.00m = 1ether in decimal. the 2 after 1.00m is gas price which means the speed for mining.

            var wieEtherToSend = UnitConversion.Convert.ToWei(1); // = 1 ETH to send from the contract to someone

            var getBuyerFunction = ContractDeployedInstance.GetFunction("getBuyer");  //find the method of the contract 
            var payer = await getBuyerFunction.CallAsync<String>();


            //var getBalanceFunction = ContractDeployedInstance.GetFunction("getBalance"); //find the method of the contract 
            //var resultGetBalance2 = await getBalanceFunction.CallAsync<UInt64>(); // calling the method of the contract named -getBalance. Uint64 is the type that returns from the method
            var sendFunction = ContractDeployedInstance.GetFunction("send");  //find the method of the contract 

            //var gas = await sendFunction.EstimateGasAsync(senderAddress, null, null, recipientAddress, wieEtherToSend); //we calculates the gas for the send method - we must, because this method (send) changing the contract state
            //var receiptAmountSend = await sendFunction.SendTransactionAndWaitForReceiptAsync(senderAddress, gas, null, null, recipientAddress, wieEtherToSend); //here we call the send method, after the second null we put the parameters to the method

            var gas = await sendFunction.EstimateGasAsync(senderAddress, null, null, recipientAddress, wieEtherToSend); //we calculates the gas for the send method - we must, because this method (send) changing the contract state
            var receiptAmountSend = await sendFunction.SendTransactionAndWaitForReceiptAsync(senderAddress, gas, null, null, recipientAddress, wieEtherToSend); //here we call the send method, after the second null we put the parameters to the method

            int check = 1;

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


        async Task DeployAndSignSalesContract()
        {        
            var contractByteCode = "0x6080604052600b805464ffffffffff60a01b191690553480156200002257600080fd5b50604051620013d6380380620013d683398181016040526101008110156200004957600080fd5b815160208301516040808501805191519395929483019291846401000000008211156200007557600080fd5b9083019060208201858111156200008b57600080fd5b8251640100000000811182820188101715620000a657600080fd5b82525081516020918201929091019080838360005b83811015620000d5578181015183820152602001620000bb565b50505050905090810190601f168015620001035780820380516001836020036101000a031916815260200191505b506040818152602083015190830151606090930180519195939492939291846401000000008211156200013557600080fd5b9083019060208201858111156200014b57600080fd5b82516401000000008111828201881017156200016657600080fd5b82525081516020918201929091019080838360005b83811015620001955781810151838201526020016200017b565b50505050905090810190601f168015620001c35780820380516001836020036101000a031916815260200191505b50604090815260208201519101519092509050336001600160a01b03821614156200023a576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401808060200182810382526024815260200180620013b26024913960400191505060405180910390fd5b610e1088026001556040805160c0810182528881526020808201899052918101879052606081018690526080810185905260a0810184905260048981558851919290916200028f91600591908b01906200036f565b50604082015160028201556060820151600382015560808201518051620002c19160048401916020909101906200036f565b5060a09190910151600590910155600380546001600160a01b03199081163317909155600a80546001600160a01b03848116919093161790819055600b805460ff60a01b191674010000000000000000000000000000000000000000179055426000556040805130815291909216602082015281517ff9b1ce7940a665cc8c05b494835a3f22f87e0e601847e295e2aa576cc8556c6d929181900390910190a1505050505050505062000414565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f10620003b257805160ff1916838001178555620003e2565b82800160010185558215620003e2579182015b82811115620003e2578251825591602001919060010190620003c5565b50620003f0929150620003f4565b5090565b6200041191905b80821115620003f05760008155600101620003fb565b90565b610f8e80620004246000396000f3fe6080604052600436106100e85760003560e01c806384f12bfe1161008a578063a93942c411610059578063a93942c414610424578063bebdd82314610439578063db1f64b01461044e578063eba0941514610463576100e8565b806384f12bfe146103d05780638aa2964a146103e557806399c55789146103fa578063a86918ed1461040f576100e8565b806357807650116100c6578063578076501461022b5780635c5a0f931461026957806360af135b146103785780636f9fb98a146103a9576100e8565b8063202dd0b7146101ea578063204e735b146102015780632b68bb2d14610216575b600a546001600160a01b03163314610147576040805162461bcd60e51b815260206004820181905260248201527f4f6e6c79206275796572206d617920706179207468697320636f6e7472616374604482015290519081900360640190fd5b60095434146101875760405162461bcd60e51b815260040180806020018281038252602e815260200180610e72602e913960400191505060405180910390fd5b600b54600160b81b900460ff161515600114156101d55760405162461bcd60e51b8152600401808060200182810382526025815260200180610e4d6025913960400191505060405180910390fd5b600b805460ff60b81b1916600160b81b179055005b3480156101f657600080fd5b506101ff610478565b005b34801561020d57600080fd5b506101ff61056e565b34801561022257600080fd5b506101ff6105e1565b34801561023757600080fd5b506102556004803603602081101561024e57600080fd5b50356106e7565b604080519115158252519081900360200190f35b34801561027557600080fd5b5061027e61092f565b604051808781526020018060200186815260200185815260200180602001848152602001838103835288818151815260200191508051906020019080838360005b838110156102d75781810151838201526020016102bf565b50505050905090810190601f1680156103045780820380516001836020036101000a031916815260200191505b50838103825285518152855160209182019187019080838360005b8381101561033757818101518382015260200161031f565b50505050905090810190601f1680156103645780820380516001836020036101000a031916815260200191505b509850505050505050505060405180910390f35b34801561038457600080fd5b5061038d610a85565b604080516001600160a01b039092168252519081900360200190f35b3480156103b557600080fd5b506103be610a95565b60408051918252519081900360200190f35b3480156103dc57600080fd5b506101ff610a9a565b3480156103f157600080fd5b5061038d610b94565b34801561040657600080fd5b50610255610ba3565b34801561041b57600080fd5b50610255610bb3565b34801561043057600080fd5b506101ff610bc3565b34801561044557600080fd5b5061038d610d0a565b34801561045a57600080fd5b50610255610d19565b34801561046f57600080fd5b50610255610d3f565b600a546001600160a01b031633146104c15760405162461bcd60e51b8152600401808060200182810382526021815260200180610f126021913960400191505060405180910390fd5b600b54600160a81b900460ff1615156001141561050f5760405162461bcd60e51b815260040180806020018281038252602c815260200180610dc7602c913960400191505060405180910390fd5b600a54600354604080513081526001600160a01b039384166020820152919092168183015290517f60fd86fe4a800e746cd40800c4b85092b1036ec613534ae7fee8d9851dd517969181900360600190a1600a546001600160a01b0316ff5b33737988dfd8e9cecb888c1aea7cb416d44c6160ef80146105c05760405162461bcd60e51b8152600401808060200182810382526026815260200180610eec6026913960400191505060405180910390fd5b600b805460ff60c01b198116600160c01b9182900460ff1615909102179055565b737988dfd8e9cecb888c1aea7cb416d44c6160ef8033146106335760405162461bcd60e51b8152600401808060200182810382526026815260200180610eec6026913960400191505060405180910390fd5b600b54600160b01b900460ff161515600114156106815760405162461bcd60e51b815260040180806020018281038252602b815260200180610ec1602b913960400191505060405180910390fd5b600a54600354604080513081526001600160a01b03938416602082015291909216818301526001606082015290517f06af57916f4dcbcd64868bf1d68e647c602cf1b42d36da9d90d332b55eae43639181900360800190a1600a546001600160a01b0316ff5b600033737988dfd8e9cecb888c1aea7cb416d44c6160ef801461073b5760405162461bcd60e51b8152600401808060200182810382526026815260200180610eec6026913960400191505060405180910390fd5b610743610a95565b82106107805760405162461bcd60e51b8152600401808060200182810382526034815260200180610e196034913960400191505060405180910390fd5b600b54600160a81b900460ff1615156001146107cd5760405162461bcd60e51b815260040180806020018281038252602f815260200180610d77602f913960400191505060405180910390fd5b600b54600160b01b900460ff1615156001141561081b5760405162461bcd60e51b815260040180806020018281038252602b815260200180610ec1602b913960400191505060405180910390fd5b600282905560095460035460405191849003916001600160a01b039091169082156108fc029083906000818181858888f19350505050158015610862573d6000803e3d6000fd5b50600254604051737988dfd8e9cecb888c1aea7cb416d44c6160ef809180156108fc02916000818181858888f193505050501580156108a5573d6000803e3d6000fd5b50600a54600b805460ff60b01b196001600160a01b03199091166001600160a01b039384161716600160b01b17908190556003546040805130815291841660208301529190921682820152517f99da6eb11c504fc59be599d1e506e1f5e2b84cfce73f19c546700516ed1029b49181900360600190a15050600b54600160b01b900460ff16919050565b6004546006546007546009546005805460408051602060026001851615610100026000190190941693909304601f8101849004840282018401909252818152600097606097899788978a9789979496909593949293600893909187918301828280156109dc5780601f106109b1576101008083540402835291602001916109dc565b820191906000526020600020905b8154815290600101906020018083116109bf57829003601f168201915b5050855460408051602060026001851615610100026000190190941693909304601f8101849004840282018401909252818152959a5087945092508401905082828015610a6a5780601f10610a3f57610100808354040283529160200191610a6a565b820191906000526020600020905b815481529060010190602001808311610a4d57829003601f168201915b50505050509150955095509550955095509550909192939495565b6003546001600160a01b03165b90565b303190565b6003546001600160a01b03163314610ae35760405162461bcd60e51b8152600401808060200182810382526027815260200180610f336027913960400191505060405180910390fd5b600b54600160a81b900460ff16151560011415610b315760405162461bcd60e51b8152600401808060200182810382526021815260200180610da66021913960400191505060405180910390fd5b610b39610d19565b1515600114610b86576040805162461bcd60e51b8152602060048201526014602482015273151a5b5948191a591b981d081bdd995c881e595d60621b604482015290519081900360640190fd5b600a546001600160a01b0316ff5b600a546001600160a01b031690565b600b54600160a01b900460ff1690565b600b54600160c01b900460ff1690565b600a546001600160a01b03163314610c0c5760405162461bcd60e51b8152600401808060200182810382526021815260200180610ea06021913960400191505060405180910390fd5b600b54600160b81b900460ff161515600114610c595760405162461bcd60e51b8152600401808060200182810382526027815260200180610d506027913960400191505060405180910390fd5b600b54600160a81b900460ff16151560011415610ca75760405162461bcd60e51b8152600401808060200182810382526026815260200180610df36026913960400191505060405180910390fd5b600b805460ff60a81b1916600160a81b17905560408051308152737988dfd8e9cecb888c1aea7cb416d44c6160ef80602082015281517f56ced8198726b4967f1b6fd0dd9fc3e3e1b9a6273b4fb654135b45458715ba1c929181900390910190a1565b600b546001600160a01b031690565b60008060005442039050600154811115610d37576001915050610a92565b600091505090565b600b54600160a81b900460ff169056fe596f75206469646e60742070617965642074686520707269636520666f7220746865206465616c54686520636f6e7472616374206469646e60742072656369657665207468652062757965726073207369676e696e67427579657220616c7265616479207369676e65642074686520636f6e747261637442757965722063616e6e6f742064656e792074686520636f6e7472616374206166746572207369676e696e6754686520627579657220616c7265616479207369676e6564207468697320636f6e747261637454686520746178207061796d656e74206d757374206265206c6f776572207468616e20746865206275796572207061796d656e74427579657220616c726561647920706179656420666f72207468697320636f6e7472616374596f757220706179206973206e6f7420657175616c20746f207468652061737365746073207072696365207461674f6e6c79206275796572206d6179207369676e207468697320636f6e747261637454686520676f7672656e6d656e7420616c7265616479207369676e6564207468697320636f6e74726163744f6e6c7920676f7672656e6d656e74206d61792063616c6c20746869732066756e6374696f6e4f6e6c79206275796572206d61792063616c6c20746869732066756e6374696f6e4f6e6c79206173736574206f776e6572206d61792063616c6c20746869732066756e6374696f6ea265627a7a72315820ea3ced4cda62981bfde9e9548ddc105832fdef66b8d12bcab5b612c413081b8d64736f6c634300050b0032427579657220616e642073656c6c657220617265207468652073616d6520706572736f6e";
            var contractABI = @"[{""constant"":false,""inputs"":[],""name"":""denyContract"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""setIsExpired"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""cancelContract"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""uint256"",""name"":""_taxPay"",""type"":""uint256""}],""name"":""approveAndExcecuteContract"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getAssetDetails"",""outputs"":[{""internalType"":""uint256"",""name"":""AssetID"",""type"":""uint256""},{""internalType"":""string"",""name"":""AssetLoaction"",""type"":""string""},{""internalType"":""uint256"",""name"":""AssetRooms"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""AssetAreaIn"",""type"":""uint256""},{""internalType"":""string"",""name"":""AssetImageURL"",""type"":""string""},{""internalType"":""uint256"",""name"":""AssetPrice"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getOldAssetOwner"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getContractBalance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""abortContract"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getAssetBuyer"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getOwnerSigning"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getIsExpired"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""setBuyerSigning"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getNewAssetOwner"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""isTimerOver"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getBuyerSigning"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""_timeToBeOpen"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_Id"",""type"":""uint256""},{""internalType"":""string"",""name"":""_Loaction"",""type"":""string""},{""internalType"":""uint256"",""name"":""_Rooms"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_AreaIn"",""type"":""uint256""},{""internalType"":""string"",""name"":""_Image"",""type"":""string""},{""internalType"":""uint256"",""name"":""_price"",""type"":""uint256""},{""internalType"":""address payable"",""name"":""_buyer"",""type"":""address""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""payable"":true,""stateMutability"":""payable"",""type"":""fallback""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""fromContract"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toBuyer"",""type"":""address""}],""name"":""notifyNewOffer"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""fromContract"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toSeller"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toBuyer"",""type"":""address""}],""name"":""notifyContractApproved"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""fromContract"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toGovrenment"",""type"":""address""}],""name"":""notifyContractSigned"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""fromContract"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""fromBuyer"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toSeller"",""type"":""address""}],""name"":""notifyDenyOffer"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""fromContract"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toBuyer"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toSeller"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""codeAction"",""type"":""uint256""}],""name"":""notifyCancelOffer"",""type"":""event""}]";

            var sellerAddress = "0x9Bd6dc66e611Ae28344D52C4CF6167C98A1Aac43";
            var sellerPrivateKey = "8a24eeca6f3d9fc95b27b187c7240ae9b279ed73484fba3e486fcb4afc463121";
            var buyerAddress = "0xe9A7a73DdE51D08c62847c15A5eA6741F2a4f1D5";
            var buyerPrivateKey = "3a856ee64a38464eef3aa0cdd3e6e15e32ca3ac55c0e99467634221500504a64";
            var regulatorAddress = "0x7988dfD8E9ceCb888C1AeA7Cb416D44C6160Ef80";
            var regulatorPrivateKey = "ce155c9664386764ee49f72aa0e5d2820c7dee301154b545e26e69f6408f4d34";

            var infuraURL = "https://ropsten.infura.io/v3/4dc41c6f591d4d61a3a2e32a219c6635";
            var sellerAccount = new Nethereum.Web3.Accounts.Account(sellerPrivateKey); //login to ethereum account
            var BuyerAccount = new Nethereum.Web3.Accounts.Account(buyerPrivateKey); //login to ethereum account
            var RegulatorAccount = new Nethereum.Web3.Accounts.Account(regulatorPrivateKey); //login to ethereum account


            var web3Seller = new Web3(sellerAccount, infuraURL); //connection to ropsten blockchain
            var web3Buyer = new Web3(BuyerAccount, infuraURL); //connection to ropsten blockchain
            var web3Regulator = new Web3(RegulatorAccount, infuraURL); //connection to ropsten blockchain



            //-----------------Seller create a contract and sign it---------------------
            var _timeToBeOpen = 1;
            var _Id = 2232;
            var _Loaction = "Hadas 6 Haifa";
            var _Rooms = 3;
            var _AreaIn = 110;
            var _Image = "https://media.angieslist.com/s3fs-public/styles/vertical_large/public/colonial%20house%20for%20sale.jpeg?itok=HFDsNQyz";
            var _price = UnitConversion.Convert.ToWei(3);

            object[] contractParams;
            contractParams = new object[]
            {
                _timeToBeOpen,
                _Id,
                _Loaction,
                _Rooms,
                _AreaIn,
                _Image, 
                _price,
                buyerAddress
            };

            //_timeToBeOpen, _Id, _Loaction, _Rooms, _AreaIn, _Image, _price

            var estimateGasForDeploy = await web3Seller.Eth.DeployContract.EstimateGasAsync(contractABI, contractByteCode, sellerAddress, contractParams);
            var receiptSalesContract = await web3Seller.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(contractABI, contractByteCode, sellerAddress, estimateGasForDeploy, null, contractParams); //deploy the contract, 900000=time to deploy-constant, after null, we can add parameters to the constructor of the contract

            //-----------------Seller create a contract and sign it---------------------

            var salesContractAddress = receiptSalesContract.ContractAddress; //after deployment, we get contract address. 
            var etherscanURL = "https://ropsten.etherscan.io/address/" + salesContractAddress;

            //-----------------Buyer reads the deal---------------------
            var ContractDeployedInstanceAsBuyer = web3Buyer.Eth.GetContract(contractABI, salesContractAddress); //read instance of the contract
            var contractHandlerAsBuyer = web3Buyer.Eth.GetContractHandler(salesContractAddress);

            var getAssetBuyerFunctionAsBuyer = ContractDeployedInstanceAsBuyer.GetFunction("getAssetBuyer");   
            var getAssetBuyerAsBuyer = await getAssetBuyerFunctionAsBuyer.CallAsync<string> ();

            var getAssetDetailsOutputDTOAsBuyer = await contractHandlerAsBuyer.QueryDeserializingToObjectAsync<GetAssetDetailsFunction, GetAssetDetailsOutputDTO>();
            BigInteger assetID = getAssetDetailsOutputDTOAsBuyer.AssetID;
            string assetLoaction = getAssetDetailsOutputDTOAsBuyer.AssetLoaction;
            BigInteger assetRooms = getAssetDetailsOutputDTOAsBuyer.AssetRooms;
            BigInteger assetAreaIn = getAssetDetailsOutputDTOAsBuyer.AssetAreaIn;
            string assetImageUrl = getAssetDetailsOutputDTOAsBuyer.AssetImageURL;
            BigInteger assetPrice = getAssetDetailsOutputDTOAsBuyer.AssetPrice;
            string assetDetialsAllTogether = ""+ assetID+", "+ assetLoaction + ", "+ assetRooms+", "+ assetAreaIn+", "+ assetImageUrl+", "+ assetPrice;
            //-----------------Buyer reads the deal---------------------



            //-----------------Buyer send money and sign---------------------
            var getBuyerSigningFunctionAsBuyer = ContractDeployedInstanceAsBuyer.GetFunction("getBuyerSigning");
            var buyerSign = await getBuyerSigningFunctionAsBuyer.CallAsync<bool>();

            if(buyerSign==false)
            {
                double EtherToSend = 3.0;
                decimal EtherToPay = Convert.ToDecimal(EtherToSend);
                
                var payTransaction = await web3Buyer.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(salesContractAddress, EtherToPay, 4, new BigInteger(45000)); //send money to the contract, everything is constant except the address and the amount - 1.00m = 1ether in decimal. the 2 after 1.00m is gas price which means the speed for mining.
                var getContractBalanceFunctionAsBuyer = ContractDeployedInstanceAsBuyer.GetFunction("getContractBalance");
                var salesContractBalanceAsBuyer = await getContractBalanceFunctionAsBuyer.CallAsync<UInt64>();
                if(salesContractBalanceAsBuyer == UnitConversion.Convert.ToWei(EtherToPay))
                {
                    //first way -working
                    //var setBuyerSigningFunctionTxnReceipt = await contractHandlerAsBuyer.SendRequestAndWaitForReceiptAsync<SetBuyerSigningFunction>();

                    //second way -working
                     var setBuyerSigningFunction = ContractDeployedInstanceAsBuyer.GetFunction("setBuyerSigning");  //find the method of the contract 
                     var gasEstimationForBuyerSigning = await setBuyerSigningFunction.EstimateGasAsync(buyerAddress, null, null);
                     var receiptAmountSend = await setBuyerSigningFunction.SendTransactionAndWaitForReceiptAsync(buyerAddress, gasEstimationForBuyerSigning, null, null);


                }



            }

            //-----------------Buyer send money and sign---------------------

            //-----------------Government check legality ---------------------
            var ContractDeployedInstanceAsRegulator = web3Regulator.Eth.GetContract(contractABI, salesContractAddress); //read instance of the contract
            var contractHandlerAsRegulator = web3Regulator.Eth.GetContractHandler(salesContractAddress);
            var getBuyerSigningFunctionAsRegulator = ContractDeployedInstanceAsRegulator.GetFunction("getBuyerSigning");
            buyerSign = await getBuyerSigningFunctionAsRegulator.CallAsync<bool>();
            var getContractBalanceFunctionAsRegulator = ContractDeployedInstanceAsRegulator.GetFunction("getContractBalance");
            var salesContractBalanceAsRegulator = await getContractBalanceFunctionAsRegulator.CallAsync<UInt64>();
            double taxPercentage = 0.02;        
            double contractBalanceAsDouble = Convert.ToDouble(salesContractBalanceAsRegulator); ;
            double taxEtherAmountAsDouble = contractBalanceAsDouble* taxPercentage;
            //UInt64 taxEtherAmountAsWie = Convert.ToUInt64(taxEtherAmountAsDouble);
  
            if (buyerSign == true)
            {
                /* ------this is the first way to call approveAndExcecuteContract, and it`s work great!!!! ------
                var approveAndExcecuteContractFunction = new ApproveAndExcecuteContractFunction();
                approveAndExcecuteContractFunction.TaxPay = new BigInteger(taxEtherAmountAsDouble);
                var approveAndExcecuteContractFunctionTxnReceipt = await contractHandlerAsRegulator.SendRequestAndWaitForReceiptAsync(approveAndExcecuteContractFunction);
                */

                // -----this is a second way to call approveAndExcecuteContract------
                var approveAndExcecuteContractFunction = ContractDeployedInstanceAsRegulator.GetFunction("approveAndExcecuteContract");  //find the method of the contract 
                var gasEstimationForApproval = await approveAndExcecuteContractFunction.EstimateGasAsync(regulatorAddress, null, null, new BigInteger(taxEtherAmountAsDouble)); 
                var receiptAmountSend = await approveAndExcecuteContractFunction.SendTransactionAndWaitForReceiptAsync(regulatorAddress, gasEstimationForApproval, null, null, new BigInteger(taxEtherAmountAsDouble));
                
            }


            //-----------------Government check legality ---------------------

            int i = 1;
            i = 2;




        }

        
    }

    public partial class GetAssetDetailsFunction : GetAssetDetailsFunctionBase { }

    [Function("getAssetDetails", typeof(GetAssetDetailsOutputDTO))]
    public class GetAssetDetailsFunctionBase : FunctionMessage
    {

    }


    public partial class GetAssetDetailsOutputDTO : GetAssetDetailsOutputDTOBase { }


    [FunctionOutput]
    public class GetAssetDetailsOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "AssetID", 1)]
        public virtual BigInteger AssetID { get; set; }
        [Parameter("string", "AssetLoaction", 2)]
        public virtual string AssetLoaction { get; set; }
        [Parameter("uint256", "AssetRooms", 3)]
        public virtual BigInteger AssetRooms { get; set; }
        [Parameter("uint256", "AssetAreaIn", 4)]
        public virtual BigInteger AssetAreaIn { get; set; }
        [Parameter("string", "AssetImageURL", 5)]
        public virtual string AssetImageURL { get; set; }
        [Parameter("uint256", "AssetPrice", 6)]
        public virtual BigInteger AssetPrice { get; set; }
    }

    public partial class SetBuyerSigningFunction : SetBuyerSigningFunctionBase { }

    [Function("setBuyerSigning")]
    public class SetBuyerSigningFunctionBase : FunctionMessage
    {

    }
    /*
    public partial class ApproveAndExcecuteContractFunction : ApproveAndExcecuteContractFunctionBase { }

    [Function("approveAndExcecuteContract", "bool")]
    public class ApproveAndExcecuteContractFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_taxPay", 1)]
        public virtual BigInteger TaxPay { get; set; }
    }
    */

            }
