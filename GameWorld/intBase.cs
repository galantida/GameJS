using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace GameWorld
{
    public interface intBase
    {
        bool populate(MySqlDataReader sqlReader);
        int save();
        int save(bool children);
    }
}
