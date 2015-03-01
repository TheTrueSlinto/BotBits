﻿using System;
using System.Diagnostics;
using System.Threading;
using BotBits.Events;
using BotBits.SendMessages;

namespace BotBits.Demo
{
    class Program
    {
        static BotBitsClient bot = new BotBitsClient();

        static void Main()
        {
            EventLoader
                .Of(bot)
                .LoadStatic<Program>();

            ConnectionManager
                .Of(bot)
                .EmailLoginAsync("guest", "guest")
                .CreateJoinRoomAsync("PWAARLDluVa0I").Wait();
            
            Thread.Sleep(-1);
        }


        [EventListener]
        static void OnJoin(JoinEvent e)
        {    
        }
    }
}