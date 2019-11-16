using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using ActressMas;
namespace multi_agent_load_balancer.Agents.Base
{
    public abstract class ExtendedConcurrentAgent : ConcurrentAgent
    {

        public T ParseMessage<T>(string json) where T: class
        {
            T obj = null;
            try
            {
                var parsed = JsonSerializer.Deserialize<T>(json);
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
    }
}
