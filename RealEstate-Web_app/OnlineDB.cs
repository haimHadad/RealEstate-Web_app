using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Google.Apis.Sheets.v4.Data;

namespace RealEstate_Web_app
{
    public class OnlineDB
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };

        static readonly string ApplicationName = "OnlineDB";

        static readonly string spreadSheetID = "1yoSOZPUto61qwcoD9ApkbyZiOLtkhdcJT47ToIB6DJY";

        static readonly string [] Sheet = { "Assets", "OpenContracts" , "ApprovedContracts" , "RejectedContracts" };

        static SheetsService service;


        public OnlineDB()
        {
            GoogleCredential credential;
            using(var stream = new FileStream("client_secrets.json", FileMode.Open,FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }
            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            int k = 3;
            k = 2;
        }

        public static string getLetterByNumber(int num)
        {
            if (num<1 || num>26)
                return null;
            string[] numerology = new string[26] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            return numerology[num - 1];
            
        }


    
        public static List<List<string>> getTable(int tableNO)
        {
            List<List<String>> table = new List<List<String>>();
            try 
            {
                if (tableNO > (Sheet.Length - 1) || tableNO < 0)
                    return null;
                var range = $"{Sheet[tableNO]}";
                var request = service.Spreadsheets.Values.Get(spreadSheetID, range);
                var response = request.Execute();
                var values = response.Values;

                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        List<String> rowTable = new List<String>();
                        foreach (var cell in row)
                        {
                            rowTable.Add("" + cell);

                        }
                        table.Add(rowTable);

                    }
                }

                else
                {
                    return null;
                }
            }
            catch(Exception e) 
            {
                return null;
            }

            return table;
        }

        public static List<List<string>> getColAtTable(int tableNO, int from, int to)
        {
            List<List<String>> table = new List<List<String>>();
            try
            {
                if (tableNO > (Sheet.Length - 1) || tableNO < 0 || from < 1 || from > to || to < 1 || to > 26)
                    return null;
                string colFrom = getLetterByNumber(from);
                string colTO = getLetterByNumber(to);
                var range = $"{Sheet[tableNO]}!{colFrom}:{colTO}";
                var request = service.Spreadsheets.Values.Get(spreadSheetID, range);
                var response = request.Execute();
                var values = response.Values;

                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        List<String> rowTable = new List<String>();
                        foreach (var cell in row)
                        {
                            rowTable.Add("" + cell);

                        }
                        table.Add(rowTable);

                    }
                }

                else
                {
                    return null;
                }

            }
            catch(Exception e)
            {
                return null;
            }
            

            return table;
        }

        public static bool isAssetExist(string assetID)
        {
            List<List<String>> colOfAssetID = getColAtTable(0, 1, 1);
            if (colOfAssetID == null)
                throw new Exception("Failed to read table");

            foreach (var item in colOfAssetID)
            {
                if (item.Contains(assetID))
                    return true;
            }
            return false;
        }





        public static bool isAssetAvailableForSale(string assetID)
        {
            List<List<String>> colOfAssetID = getColAtTable(1, 1, 1);

            if (colOfAssetID == null)
                throw new Exception("Failed to read table");

            foreach ( var item in colOfAssetID)
            {
                if (item.Contains(assetID))
                    return false;
            }
            return true;
        }

        public static bool InsertNewEntryToAssetsTable(string PrivateKey , string AssetID, string Loaction, string AreaIn, string Rooms, string Image, string Owner)
        {
            try
            {
                var RegulatorAccount = new Nethereum.Web3.Accounts.Account(PrivateKey);
                string PublicKeyRegulaotr = RegulatorAccount.Address;
                if ( !PublicKeyRegulaotr.Equals("0x7988dfD8E9ceCb888C1AeA7Cb416D44C6160Ef80") )
                    return false;
                PrivateKey = null;
                RegulatorAccount = null;
                if (isAssetExist(AssetID) == true)
                    return false;

                var range = $"{Sheet[0]}";
                var valueRange = new ValueRange();
                var objectList = new List<object>
                {
                AssetID,
                Loaction,
                AreaIn,
                Rooms,
                Image,
                Owner
                };
                valueRange.Values = new List<IList<object>> { objectList };
                var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadSheetID, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.Execute();

            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public static bool InsertNewEntryToOpenContractsTable(string AssetID, string ContractAddress, string  Seller, string Buyer)
        {
            try 
            {
                if (isAssetAvailableForSale(AssetID) == false)
                    return false;

                var range = $"{Sheet[1]}";
                var valueRange = new ValueRange();
                var objectList = new List<object>
                {
                AssetID,
                ContractAddress,
                Seller,
                Buyer
                };
                valueRange.Values = new List<IList<object>> { objectList };
                var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadSheetID, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.Execute();
                
            }
            catch(Exception e)
            {
                return false;
            }

            
            return true;
        }

        public static bool InsertNewEntryToApprovedContracts(string PrivateKey , string AssetID, string ContractAddress, string Seller, string Buyer)
        {
            try
            {
                var RegulatorAccount = new Nethereum.Web3.Accounts.Account(PrivateKey);
                string PublicKeyRegulaotr = RegulatorAccount.Address;
                if (!PublicKeyRegulaotr.Equals("0x7988dfD8E9ceCb888C1AeA7Cb416D44C6160Ef80"))
                    return false;
                PrivateKey = null;
                RegulatorAccount = null;               

                var range = $"{Sheet[2]}";
                var valueRange = new ValueRange();
                var objectList = new List<object>
                {
                AssetID,
                ContractAddress,  
                Seller,
                Buyer
                };
                valueRange.Values = new List<IList<object>> { objectList };
                var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadSheetID, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.Execute();

            }
            catch (Exception e)
            {
                return false;
            }


            return true;
        }

        public static bool InsertNewEntryToRejectedContracts(string AssetID, string ContractAddress, int RejectedByCode, string Seller, string Buyer)
        {
            if (RejectedByCode != 0 && RejectedByCode != 1)
                return false;
            string[] RejectionParty = new string[2] { "Buyer", "Regulator" };
            try
            {
                var range = $"{Sheet[3]}";
                var valueRange = new ValueRange();
                var objectList = new List<object>
                {
                AssetID,
                ContractAddress,
                RejectionParty[RejectedByCode],
                Seller,
                Buyer
                };
                valueRange.Values = new List<IList<object>> { objectList };
                var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadSheetID, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.Execute();

            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public static bool UpdateNewAssetOwner(string PrivateKey, string AssetID, string OldOwner, string NewOwner)
        {
            try
            {
                var RegulatorAccount = new Nethereum.Web3.Accounts.Account(PrivateKey);
                string PublicKeyRegulaotr = RegulatorAccount.Address;
                if (!PublicKeyRegulaotr.Equals("0x7988dfD8E9ceCb888C1AeA7Cb416D44C6160Ef80"))
                    return false;
                PrivateKey = null;
                RegulatorAccount = null;
                if (isAssetExist(AssetID) == false)
                    return false;
                List<List<String>> colOfAssetID = getColAtTable(0, 1, 1);
                if (colOfAssetID == null)
                    return false;
                int targetRowNum=-1;

                for(int i =0; i< colOfAssetID.Count; i++)
                {
                    if(colOfAssetID.ElementAt(i).Contains(AssetID))
                    {
                        targetRowNum = i + 1;
                        break;
                    }
                }

                List<List<String>> colOfOwners = getColAtTable(0, 6, 6);
                if (colOfAssetID == null)
                    return false;

                if (colOfOwners.ElementAt(targetRowNum).Contains(OldOwner) == false)
                    return false;

                string TargetRowStr = ""+ targetRowNum;

                var range = $"{Sheet[0]}!F{TargetRowStr}";
                var valueRange = new ValueRange();
                var objectList = new List<object>{ NewOwner };
                valueRange.Values = new List<IList<object>> { objectList };

                var updateRequest = service.Spreadsheets.Values.Update(valueRange, spreadSheetID, range);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = updateRequest.Execute();
                
            }
            catch (Exception e)
            {
                return false;
            }
            

            return true;
        }


    }
}
