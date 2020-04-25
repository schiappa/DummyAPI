using DummyAPI.Database;
using DummyAPI.Models;
using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DummyAPI.Repositories
{
    public static class UsersRepository
    {
        public static List<UserDTO> GetUsers(string search)
        {
            return DBManager.GetUsers(search);
        }

        public static int Create(UserDTO user, out string errorMsg)
        {
            if (user == null)
            {
                errorMsg = "El usuario no puede ser nulo";
                return -1;
            }
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                errorMsg = "El nombre de usuario y la contraseña son campos obligatorios";
                return -1;
            }
            user.PasswordHash = SessionRepository.GeneratePasswordHash(user.Password);
            return DBManager.CreateUser(user, out errorMsg);
        }

        public static UserDTO GetById(int id)
        {
            return DBManager.GetUser(id);
        }

        public static bool Delete(int id)
        {
            return DBManager.DeleteUser(id);
        }

        public static bool Edit(int id, UserDTO user, out string errorMsg)
        {
            if (user == null)
            {
                errorMsg = "El usuario no puede ser nulo";
                return false;
            }
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                errorMsg = "El nombre de usuario y la contraseña son campos obligatorios";
                return false;
            }
            if(!string.IsNullOrWhiteSpace(user.Password))
                user.PasswordHash = user.PasswordHash = SessionRepository.GeneratePasswordHash(user.Password); ;
            return DBManager.Edit(id, user, out errorMsg);
        }

        #region DIFF
        internal static string Diff()
        {
            UserDTO a1 = new UserDTO()
            {
                Active = true,
                Email = "admin@admin.com",
                Id = 1,
                Lastname = "chiappa",
                Name = "santi",
                Permits = new Dictionary<int, List<int>>(),
                Products = new List<int>() { 1, 2, 3, 4 },
                Username = "admin",
                Items = new List<Item>()
                {
                    new Item()
                    {
                        Id = 2,
                        Name = "Item Dos",
                        Active = true
                    },
                    new Item()
                    {
                        Id = 10,
                        Name = "Item Diez",
                        Active = false
                    },
                }
            };
            a1.Permits.Add(1, new List<int>() { 1, 2, 3 });
            a1.Permits.Add(2, new List<int>(){1, 2, 3});
            a1.Permits.Add(3, new List<int>(){3, 2, 5});
            UserDTO a2 = new UserDTO()
            {
                Active = true,
                Email = "admin@admin.com",
                Id = 1,
                Lastname = "Chiappa",
                Name = "Santiago",
                Permits = new Dictionary<int, List<int>>(),
                Products = new List<int>() { 1, 3, 4 },
                Username = "admin",
                Items = new List<Item>()
                {
                    new Item()
                    {
                        Id = 2,
                        Name = "Item Dos",
                        Active = true
                    },
                    new Item()
                    {
                        Id = 9,
                        Name = "Item Nueve",
                        Active = false
                    },
                }
            };
            a2.Permits.Add(1, new List<int>() { 1, 2, 3,9 });
            a2.Permits.Add(2, new List<int>() { 3,2,1 });
            a2.Permits.Add(3, new List<int>() { 3, 2, 4 });
            //CompareLogic compareLogic = new CompareLogic( new ComparisonConfig() 
            //{
            //CompareStaticFields = true,
            //CompareChildren = true,
            // CompareFields = true,
            // ComparePrivateFields = true,
            // ComparePrivateProperties = true,
            // CompareProperties = true,
            // CompareStaticProperties = true,
            // CaseSensitive = false,
            // IgnoreCollectionOrder = true,
            // IgnoreObjectTypes=true,
            // MaxDifferences = int.MaxValue,
            // MaxMillisecondsDateDifference = 999,
            // MaxStructDepth = 5,
            // ShowBreadcrumb = false, //¿?
            // TreatStringEmptyAndNullTheSame = true

            //});
            //ComparisonResult result = compareLogic.Compare(a1, a2);
            //DifferencesToString(result);
            //return result.AreEqual ? "" : result.DifferencesString;

            //var result = a1.DetailedCompare(a2);
            //return Newtonsoft.Json.JsonConvert.SerializeObject(result);


            var compareLogic = new CompareLogic();
            compareLogic.Config.IgnoreCollectionOrder = true;
            compareLogic.Config.IgnoreObjectTypes = true;
            compareLogic.Config.TreatStringEmptyAndNullTheSame = true;
            compareLogic.Config.MaxDifferences = int.MaxValue;
            compareLogic.Config.IgnoreStringLeadingTrailingWhitespace = true;
            compareLogic.Config.CaseSensitive = false;
            compareLogic.Config.IgnoreCollectionOrder = true;

            ComparisonResult result = compareLogic.Compare(a1, a2);

            var r = new ObjectsComparisonResult
            {
                AreEqual = result.AreEqual,
                PropertyDifferences = result.AreEqual ?
                    Enumerable.Empty<PropertyDifference>() :
                    result.Differences.Select(x => new PropertyDifference
                    {
                        PropertyName = x.PropertyName,
                        OriginalValue = x.Object1Value,
                        NewValue = x.Object2Value
                    })
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(r);

        }

        public static string DifferencesToString(ComparisonResult diff)
        {
            string result = "";
            if(diff != null && !diff.AreEqual)
            {
                List<Variance> list = new List<Variance>();
                foreach (var item in diff.Differences)
                {
                    list.Add(new Variance() 
                    {
                        Prop = item.PropertyName.TrimStart('.'),
                        valA = item.Object1Value,
                        valB = item.Object2Value
                    });
                    string coso0 = $"{item.PropertyName.TrimStart('.')}";
                    var coso1 = item.GetShortItem();
                    var coso2 = item.GetWhatIsCompared();
                    string itemStr = item.ToString();
                }
                result = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            }
            return result;
        }

        public static List<Variance> DetailedCompare<T>(this T val1, T val2)
        {
            try
            {
                List<Variance> variances = new List<Variance>();
                PropertyInfo[] fi = val1.GetType().GetProperties(BindingFlags.Instance |
                           BindingFlags.Static |
                           BindingFlags.NonPublic |
                           BindingFlags.Public);
                foreach (PropertyInfo f in fi)
                {
                    Variance v = new Variance();
                    v.Prop = f.Name;
                    v.valA = f.GetValue(val1);
                    v.valB = f.GetValue(val2);
                    if (!Equals(v.valA, v.valB))
                        variances.Add(v);

                }
                return variances;
            }
            catch (Exception ex)
            {
                string a = ex.ToString();
            }
            return null;
        }
        #endregion DIFF
    }
}
