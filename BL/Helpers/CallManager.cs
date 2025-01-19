using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers;

internal static class CallManager
{
    private static IDal _dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new();

}
