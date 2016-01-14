using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GameJS
{
    public interface intBase
    {
        bool populate(MySqlDataReader sqlReader);
    }
}
