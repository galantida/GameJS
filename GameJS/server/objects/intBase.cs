using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace GameJS
{
    public interface intBase
    {
        bool populate(MySqlDataReader sqlReader);
        int save();
        int save(bool children);
    }
}
