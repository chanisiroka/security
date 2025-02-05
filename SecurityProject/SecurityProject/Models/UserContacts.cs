using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityProject.Models
{
    public class UserContacts
    {
        public static List<UserModel> Db = new List<UserModel>()
        {
             new UserModel{ UserName="jason_admin",Password="myPasswORD",
            Email="json.admin@gmail.com",SurName="britan",GivenName="jason",Role="administartor"},

        new UserModel{ UserName="elysa_seller",Password="myPassw1ORD",
            Email="elysa.admin@gmail.com",SurName="lambert",GivenName="elysa",Role="seller"}




        };
       
    }
}
