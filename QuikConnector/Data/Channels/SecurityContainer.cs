using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.Data.Channels
{
    public class SecurityContainer
    {
        private Security _value;

        public Security Value 
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                OnUpdated(this, _value);
            }
        }

        public event EventHandler<Security> Updated;

        protected virtual void OnUpdated(object sender, Security security)
        {
            if (Updated != null) Updated(this, security);
        }
    }
}
