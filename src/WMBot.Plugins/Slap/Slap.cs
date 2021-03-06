//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or   
//  (at your option) version 3.                                         

//  This program is distributed in the hope that it will be useful,     
//  but WITHOUT ANY WARRANTY; without even the implied warranty of      
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       
//  GNU General Public License for more details.                        

//  You should have received a copy of the GNU General Public License   
//  along with this program; if not, write to the                       
//  Free Software Foundation, Inc.,                                     
//  51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

namespace wmib.Extensions
{
    public class Slap : wmib.Module
    {
        public override bool Construct()
        {
            HasSeparateThreadInstance = false;
            Version = new System.Version(1, 0);
            return true;
        }

        public override void Hook_PRIV(Channel channel, libirc.UserInfo invoker, string message)
        {
            if (!message.StartsWith(Configuration.System.CommandPrefix) && GetConfig(channel, "Slap.Enabled", false))
            {
                string ms = message.Trim();
                ms = ms.Replace("!", "");
                ms = ms.Replace("?", "");
                ms = ms.ToLower();
                if (ms.StartsWith("hi "))
                    ms = ms.Substring(3);
                if (ms.StartsWith("hi, "))
                    ms = ms.Substring(4);
                if (ms.StartsWith("hello "))
                    ms = ms.Substring(6);
                if (ms.StartsWith("hello, "))
                    ms = ms.Substring(7);
                if (ms.EndsWith(":ping") || ms.EndsWith(": ping"))
                {
                    string target = message.Substring(0, message.IndexOf(":"));
                    if (GetConfig(channel, "Slap.Ping." + target, false))
                    {
                        IRC.DeliverMessage("Hi " + invoker.Nick + ", you just managed to say pointless nick: ping. Now please try again with some proper meaning of your request, something like nick: I need this and that. Or don't do that at all, it's very annoying. Thank you", channel);
                        return;
                    }
                }

                if (!channel.SystemUsers.IsKnown(invoker))
                {
                    if (ms == "i have a question" || ms == "can i ask a question" || ms == "is anyone willing to help" || ms == "can i ask" || ms == "i got a question" || ms == "can i have a question" || ms == "can someone help me" || ms == "i need help")
                    {
                        IRC.DeliverMessage("Hi " + invoker.Nick + ", just ask! There is no need to ask if you can ask, if you already asked the question please wait for someone to respond", channel);
                        return;
                    }

                    if (ms == "is anyone here" || ms == "is anybody here" || ms == "is anybody there" || ms == "is anyone there" || ms == "is some one there" || ms == "is someone there" || ms == "is someone here" || ms == "anyone here" || ms == "someone here")
                    {
                        IRC.DeliverMessage("Hi " + invoker.Nick + ", I am here, if you need anything, please ask, otherwise no one is going to help you... Thank you", channel);
                        return;
                    }
                }
            }

            if (!message.StartsWith(Configuration.System.CommandPrefix) && GetConfig(channel, "Welcoming.Enabled", false))
            {
                if (!channel.SystemUsers.IsKnown(invoker))
                {
                    string ms = message.Trim().ToLower();

                    if (ms == "hello?" || ms == "hi?")
                    {
                        IRC.DeliverMessage("Hi " + invoker.Nick + ", if you need any help, please state your question and please wait for someone to answer it.", channel);
                        return;
                    }
                }
            }

            if (message == Configuration.System.CommandPrefix + "slap" ||
                message == Configuration.System.CommandPrefix + "slap-on")
            {
                if (channel.SystemUsers.IsApproved(invoker, "admin"))
                {
                    SetConfig(channel, "Slap.Enabled", true);
                    IRC.DeliverMessage("I will be slapping annoying people since now", channel);
                    channel.SaveConfig();
                    return;
                }
                if (!channel.SuppressWarnings)
                    IRC.DeliverMessage("Permission denied", channel);
            }

            if (message == Configuration.System.CommandPrefix + "noslap" ||
                message == Configuration.System.CommandPrefix + "slap-off")
            {
                if (channel.SystemUsers.IsApproved(invoker, "admin"))
                {
                    SetConfig(channel, "Slap.Enabled", false);
                    IRC.DeliverMessage("I will not be slapping people since now", channel);
                    channel.SaveConfig();
                    return;
                }
                if (!channel.SuppressWarnings)
                    IRC.DeliverMessage("Permission denied", channel);
            }

            if (message == Configuration.System.CommandPrefix + "nopingslap")
            {
                if (channel.SystemUsers.IsApproved(invoker, "trust"))
                {
                    SetConfig(channel, "Slap.Ping." + invoker.Nick.ToLower(), false);
                    IRC.DeliverMessage("I will not be slapping people who slap you now", channel);
                    channel.SaveConfig();
                    return;
                }
                if (!channel.SuppressWarnings)
                    IRC.DeliverMessage("Permission denied", channel);
            }

            if (message == Configuration.System.CommandPrefix + "pingslap")
            {
                if (channel.SystemUsers.IsApproved(invoker, "trust"))
                {
                    SetConfig(channel, "Slap.Ping." + invoker.Nick.ToLower(), true);
                    IRC.DeliverMessage("I will be slapping people who ping you now", channel);
                    channel.SaveConfig();
                    return;
                }
                if (!channel.SuppressWarnings)
                    IRC.DeliverMessage("Permission denied", channel);
            }
        }
    }
}
