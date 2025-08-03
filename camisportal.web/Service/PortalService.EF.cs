using intaps.camisPortal.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace intaps.camisPortal.Service
{
    public partial class PortalService
    {
        CamisPortalContext context=null;
        public void ConnectDB()
        {
            if(context==null)
                context = new CamisPortalContext();
        }

        internal AddressService GetAddressService()
        {
            ConnectDB();
            return new AddressService(context);
        }

        internal Entities.Regions GetRegion(string code)
        {
            ConnectDB();
            var res= context.Regions.Where(x => x.Code.Equals(code));
            if (res.Any())
                return res.First();
            return null;
        }

        internal String GetCamisList(string key)
        {
            ConnectDB();
            var res = context.CamisList.Where(x => x.Key.Equals(key));
            if (res.Any())
                return res.First().ListData;
            throw new InvalidOperationException($"List {key} is not found");
        }
    }
}
