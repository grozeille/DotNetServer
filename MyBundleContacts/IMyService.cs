using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.DBus;

namespace MyBundleContacts
{
    [Interface("org.mathias.myservice")]
    public interface IMyService
    {
        string Say(Person prs);
    }
}
