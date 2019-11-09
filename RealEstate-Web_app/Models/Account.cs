using Microsoft.EntityFrameworkCore.Internal;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace RealEstate_Web_app.Models
{
    public class Account : Nethereum.Web3.Accounts.Account
    {
        [Required]
        public String AccountAddress { get; set; }
        [Required]
        public String AccountPassword { get; set; }

        public double AccountBalance { get; set; }

        public Boolean IsValidated { get; set; }

        [Required]
        public String AccountNetwork { get; set; }

        public String InfuraApiKey { get; set; }

        public List<Asset> Account_RE_Assets { get; set; }


        public Account(String _address, String _password, String _network) : base(_password)
        {
            
            AccountAddress = _address;
            AccountPassword = _password;
            InfuraApiKey = "4dc41c6f591d4d61a3a2e32a219c6635";
            AccountBalance = 0;
            Account_RE_Assets = new List<Asset>();
            IsValidated = false;
            switch (_network)
            {
                case "MAINNET":
                    AccountNetwork = "https://mainnet.infura.io/v3/";
                    break;

                case "TESTNET":
                    AccountNetwork = "https://ropsten.infura.io/v3/";
                    break;

                    default:
                    AccountNetwork= "https://ropsten.infura.io/v3/";
                    break;
            }
        }


       
        public Account(String _address, String _password, uint _balance) : base(_password)
        {
            AccountAddress = _address;
            AccountPassword = _password;
            AccountBalance = _balance;
            InfuraApiKey = "4dc41c6f591d4d61a3a2e32a219c6635";
        }

        public void  addAsset(Asset _asset)
        {
            if( !Account_RE_Assets.Contains(_asset) )
            {
                Account_RE_Assets.Add(_asset);
            }
            
        }

        public void removeAsset(Asset _asset)
        {
            if (Account_RE_Assets.Contains(_asset))
            {
                Account_RE_Assets.Remove(_asset);
            }

        }


        public static void ValidateAddress(String addresss)
        {
            Regex regex = new Regex("^[a-zA-Z0-9]{42}$");

            if (!regex.IsMatch(addresss))
                throw new WrongAccountDetails("Account address is illegal"); 

            if (!(addresss[0].Equals('0')) || (!addresss[1].Equals('x'))) //if address is not in pattern 
                throw new WrongAccountDetails("Account address is illegal");

        }

        public async Task<double> GetAccountBalance()
        {
            String Url = this.AccountNetwork + "" + this.InfuraApiKey;
            

            var web3 = new Web3(Url);
            var balance = await web3.Eth.GetBalance.SendRequestAsync(this.AccountAddress);
            var value = Web3.Convert.FromWei(balance.Value);
            this.AccountBalance = (double)value;
            return this.AccountBalance;
        }

        public float getExchangeRate_ETH_To_ILS()
        {
            float exchangeRate;
            string[] words;
            int indexOfExchangeRate;

            using (var client = new WebClient())
            {
                try
                {
                    var htmlPage = client.DownloadString("https://coinyep.com/he/rss/ETH-ILS.xml");
                    words = htmlPage.Split(' ');
                    indexOfExchangeRate = words.IndexOf("ILS");
                    exchangeRate = (float)Convert.ToDouble(words[indexOfExchangeRate - 1]);
                    
                }
                 catch(Exception e)
                 {                        
                    exchangeRate = -1;
                 }
            }
            return exchangeRate;
            
        }


      
       
    }





    [Serializable]
    class WrongAccountDetails : Exception
    {
        public WrongAccountDetails()
        {

        }

        public WrongAccountDetails(String Message)
            : base(String.Format(""+ Message))
        {

        }

    

    
    }
}
