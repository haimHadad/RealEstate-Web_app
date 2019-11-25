﻿using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;

namespace RealEstate_Web_app.Models
{
    public class DappAccount //this class i a general class of user ! ! ! from this class the regulator and buyer/seller will will be inherited
    {
        public string Address { get; set; }

        public string Password { get; set; }

        public Boolean IsValidated { get; set; }

        public String AccountNetwork { get; set; }
 
        public Web3 Blockchain { get; set; }

        
        public Nethereum.Web3.Accounts.Account BlockchainAcount;


        public DappAccount(String _publicKey, String _privateKey) //password = public key
        {
            try
            {
                BlockchainAcount = new Nethereum.Web3.Accounts.Account(_privateKey);
            }
            catch (Exception e)
            {
                IsValidated = false;
                return;
            }

            String declaredAddress = _publicKey;                         //address from son = textbox of our web dApp = Account class that Haim created
            String realAddress = BlockchainAcount.Address;               //BlockchainAcount.Address is the real address of the private key  
            if (declaredAddress.Equals(realAddress))                     //If the addresses are matched => Login details are correct = > dApp will show wallt(account) content in the next view 
                IsValidated = true;
            else
            {
                IsValidated = false;
                return;
            }
            Address = _publicKey;
            Password = _privateKey;
            var infuraURL = "https://ropsten.infura.io/v3/4dc41c6f591d4d61a3a2e32a219c6635";
            Blockchain = new Web3(BlockchainAcount, infuraURL);
        }



        public async Task<double> get_ETH_Balance()
        {
            double balance = await this.get_ETH_BalanceOfAnyAccount(Address);
            return balance;
        }





        public async Task<double> get_ETH_BalanceOfAnyAccount(String AccountAddress)
        {
            if (AccountAddress == null || IsValidated == false)
                return -1;
            var balance = await Blockchain.Eth.GetBalance.SendRequestAsync(AccountAddress);
            var etherAmount = Web3.Convert.FromWei(balance.Value);
            double tempBalance = (double)etherAmount;
            tempBalance = Math.Truncate(tempBalance * 10000) / 10000; //make the double number to be with 3 digits after dot
            return tempBalance;
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
                catch (Exception e)
                {
                    exchangeRate = -1;
                }
            }
            return exchangeRate;


        }

        public async Task<double> get_ILS_BalanceOfAnyAccount(String AccountAddress)
        {
            double exchangeRate = this.getExchangeRate_ETH_To_ILS();
            double tempBalance = await this.get_ETH_BalanceOfAnyAccount(AccountAddress);
            return (exchangeRate * tempBalance);

        }

        public async Task<double> get_ILS_Balance()
        {    
            double tempBalance = await this.get_ILS_BalanceOfAnyAccount(this.Address);
            return tempBalance;

        }
    }
}
