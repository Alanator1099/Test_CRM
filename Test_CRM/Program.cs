using Microsoft.VisualBasic;
using System;
using System.ComponentModel.Design;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Dadata;
using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dadata.Model;
using System.Text.RegularExpressions;
using System.IO;

namespace Test_CRM
{
    
    
    internal class Program
    {
        delegate Task LastFunction(TelegramBotClient client, Update update);
        static LastFunction lastFunction = null;
        static bool MessegeIsInnList = false;
        static string[] token = null;
        static void Main(string[] args)
        {
            token = System.IO.File.ReadAllText("C:/configs/apiKeys.txt").Split("\r\n");
            //API телеграмм
            var client = new TelegramBotClient(token[0]);
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        async static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine(exception);
        }

        async static Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            
            var message = update.Message;
            if (message.Text != null)
            {
               
                    switch (message.Text)
                    {
                        case "/start":
                            await Start((TelegramBotClient)client, update);
                            lastFunction = Start;
                            MessegeIsInnList = false;
                            break;
                        case "/help":
                            await Help((TelegramBotClient)client, update);
                            lastFunction = Help;
                            MessegeIsInnList = false;
                            break;

                        case "/hello":
                            await Hello((TelegramBotClient)client, update);
                            lastFunction = Hello;
                            MessegeIsInnList = false;
                            break;
                        case "/inn":
                            await Inn((TelegramBotClient)client, update);
                            lastFunction = Inn;
                            break;
                        case "/last":
                            await Last((TelegramBotClient)client, update);
                            MessegeIsInnList = false;
                            break;
                        default:
                        if (!MessegeIsInnList)
                        {
                            await client.SendTextMessageAsync(update.Message.Chat.Id, "Простите я не отвечаю на нетипизированые запросы, вы можете обратиться ко мне при помощи команд.\n/help - вывести список команд");
                        }
                        else
                        {
                            var innList = message.Text.Split("\n");
                            for (int i = 0; i < innList.Length; i++)
                            {
                                FindPartyByInn((TelegramBotClient)client, update, innList[i]);
                            }
                        }
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
            await client.SendTextMessageAsync(update.Message.Chat.Id, "Режим поиска по ИНН включён, он автоматически выключиться если использовать любую другую команду.");
            await client.SendTextMessageAsync(update.Message.Chat.Id, "Пришлите список ИНН в следующем формате\r\n{ВАШ_НОМЕР_ИНН1}\r\n{ВАШ_НОМЕР_ИНН2}\r\n.\r\n.\r\n.\r\nВот пример правильного запроса\r\n7707049388\r\n7707049388");
            MessegeIsInnList = true;
        }

        private static async Task Hello(TelegramBotClient client, Update update)
        {
            await client.SendTextMessageAsync(update.Message.Chat.Id, "Айрапетян Александр\r\noladij@bk,ru\r\nhttps://github.com/Alanator1099/Test_CRM");
        }

        private static async Task Start(TelegramBotClient client, Update update)
        {
            await client.SendTextMessageAsync(update.Message.Chat.Id, "Привет, я бот который поможет тепе с поиском компании по его ИНН.\r\nДля того чтобы узнать что я умею введи /help.");
        }

        async static Task Help(TelegramBotClient client, Update update) 
        {
            await client.SendTextMessageAsync(update.Message.Chat.Id, "/start - начать общение с ботом.\r\n" +
                        "/help - вывести справку о доступных командах.\r\n" +
                        "/hello - вывести ваше имя и фамилию, ваш email и ссылку на github.\r\n" +
                        "/inn - получить наименования и адреса компаний по ИНН.\r\n" +
                        "/last - повторить последнее действие бота.");
        }

        private static async Task FindPartyByInn(TelegramBotClient client, Update update, string inn) 
        {
            if ((inn.Length != 10)||(!IsNumeric(inn)))
            {
                await client.SendTextMessageAsync(update.Message.Chat.Id, "ИНН: " + inn + " введён неравильно. Проверте правильность номера и попробуйте снова");
            }
            else
            {
                var api = new SuggestClientAsync(token[1]);

                var result = await api.FindParty(inn);
                if (result.suggestions.Count > 0)
                {
                    var party = result.suggestions[0].data;

                    await client.SendTextMessageAsync(update.Message.Chat.Id, "Наименование организации: " + party.name.full_with_opf + "\r\n" + "Адреcc: " + party.address.value);
                }
                else
                {
                    await client.SendTextMessageAsync(update.Message.Chat.Id, "Организация с номером ИНН "+inn+" не числиться в нашей базе");
                }
            }
        }
        private static bool IsNumeric(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsDigit(str[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
