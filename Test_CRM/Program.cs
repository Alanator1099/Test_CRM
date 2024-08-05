using Microsoft.VisualBasic;
using System;
using System.ComponentModel.Design;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Test_CRM
{
    
    
    internal class Program
    {
        delegate Task LastFunction(TelegramBotClient client, Update update);
        static LastFunction lastFunction = null;
        static void Main(string[] args)
        {
            var client = new TelegramBotClient("7134466368:AAFK86rL0fn1Mpkvo7mrMZMazCp7sbOcns4");
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        async static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
        }

        async static Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            
            var message = update.Message;
            if (message.Text != null)
            {
                switch (message.Text)
                {
                    case "/help":
                        await Help((TelegramBotClient)client, update);
                        lastFunction = Help;
                        break;

                    case "/hello":
                        await Hello((TelegramBotClient)client, update);
                        lastFunction = Hello;
                        break;
                    case "/inn":
                        await Inn((TelegramBotClient)client, update);
                        lastFunction = Inn;
                        break;
                    case "/last":
                        await Last((TelegramBotClient)client, update);
                        break;
                }
            }
        }

        private static async Task Last(TelegramBotClient client, Update update)
        {
            await lastFunction((TelegramBotClient)client, update);
        }

        private static async Task Inn(TelegramBotClient client, Update update)
        {
            await client.SendTextMessageAsync(update.Message.Chat.Id, "Напишу его завтра");
        }

        private static async Task Hello(TelegramBotClient client, Update update)
        {
            await client.SendTextMessageAsync(update.Message.Chat.Id, "Айрапетян Александр");
        }

        async static Task Help(TelegramBotClient client, Update update) 
        {
            await client.SendTextMessageAsync(update.Message.Chat.Id, "/start - начать общение с ботом.\r\n" +
                        "/help - вывести справку о доступных командах.\r\n" +
                        "/hello - вывести ваше имя и фамилию, ваш email и ссылку на github.\r\n" +
                        "/inn - получить наименования и адреса компаний по ИНН.\r\n" +
                        "/last - повторить последнее действие бота.");
        }


    }
}
