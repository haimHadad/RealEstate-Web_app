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
                string[] numerology = new string[26] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
                string colFrom = numerology[from - 1];
                string colTO = numerology[to - 1];
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





        public static bool isAssetAvailable(string assetID)
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


        public static bool InsertNewEntryToOpenContractsTable(string AssetID, string ContractAddress, string  Seller, string Buyer)
        {
            try 
            {
                if (isAssetAvailable(AssetID) == false)
                    return false;

                if (isAssetExist(AssetID) == false)
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

       
    }
}
