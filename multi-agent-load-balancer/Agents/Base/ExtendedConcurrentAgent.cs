using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using ActressMas;
namespace multi_agent_load_balancer.Agents.Base
{
    public abstract class ExtendedConcurrentAgent : ConcurrentAgent
    {
        public ExtendedConcurrentAgent(string name)
        {
            Name = name;
        }
        public abstract AgentType AgentType { get; }
        public T ParseMessage<T>(string json) where T: class
        {
            T obj = null;
            try
            {
                obj = JsonSerializer.Deserialize<T>(json);
            }catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return obj;
        }
        public void Send<T>(string receiver, T content, string conversationId = "") where T:class
        {
            string toSend = null;
            try
            {
                toSend = JsonSerializer.Serialize(content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            base.Send(receiver, toSend, conversationId);

        }
        public void Broadcast<T>(T content, bool includeSender = false, string conversationId = "")
        {
            string toSend = null;
            try
            {
                toSend = JsonSerializer.Serialize(content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            base.Broadcast(toSend, includeSender, conversationId);
        }
        public string GetConversationId(Messaging.MessagingConversationId id)
        {
            return ((int)id).ToString();
        }

        public void Log(string message, string filePath, ConsoleColor consoleColor = ConsoleColor.White)
        {
            var fileName = Path.GetFileName(filePath);
            if(consoleColor != Console.ForegroundColor)
            {
                Console.ForegroundColor = consoleColor;
            }

            Console.WriteLine($"[{AgentType}]\t {message} - {fileName}");
            if(Console.ForegroundColor != ConsoleColor.White)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
