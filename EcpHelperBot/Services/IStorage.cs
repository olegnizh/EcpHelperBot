﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcpHelperBot.Models;

namespace EcpHelperBot.Services
{
    public interface IStorage
    {
        Session GetSession(long chatId);
    }

}
