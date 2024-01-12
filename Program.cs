using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;

class Program
{
    static async Task Main(string[] args)
    {
        int blockLevel = 0;
        bool messDeleted = false;
        string[] badwords = new string[] { "bad word", "badword" };
        string[] veryBadWords = new string[] { "very bad word", "verybadword" };

        var botClient = new TelegramBotClient("6813168768:AAGla6FIp-sM4mhp-V0I7JxZK_Yf71WOM88");

        int year;
        int month;
        int day;
        int hour;
        int minute;
        int second;

        long chadID = 0;
        string messageText;
        int messageID;
        string firtname;
        string lastname;
        long ID;

        Message sentMessage;

        year = int.Parse(DateTime.UtcNow.Year.ToString());
        month = int.Parse(DateTime.UtcNow.Month.ToString());
        day = int.Parse(DateTime.UtcNow.Day.ToString());
        hour = int.Parse(DateTime.UtcNow.Hour.ToString());
        minute = int.Parse(DateTime.UtcNow.Minute.ToString());
        second = int.Parse(DateTime.UtcNow.Second.ToString());
        Console.WriteLine("Data: " + year + "/" + month + "/" + day);
        Console.WriteLine("Time: " + hour + "/" + minute + "/" + second);

        using var cts = new CancellationTokenSource();

        botClient.StartReceiving(
            
            HandleUpdateAsync,
            HandleErrorAsync,
            cancellationToken: cts.Token);
        
        var me = await botClient.GetMeAsync();

        Console.WriteLine($"\nHello! I'm {me.Username} and I'm your Bot!");

        Console.ReadKey();
        cts.Cancel();

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }
            if (update.Message!.Type != MessageType.Text)
            {
                return;
            }

            chadID = update.Message.Chat.Id;
            messageText = update.Message.Text;
            messageID = update.Message.MessageId;
            firtname = update.Message.From.FirstName;
            lastname = update.Message.From.LastName;
            ID = update.Message.From.Id;
            year = update.Message.Date.Year;
            month = update.Message.Date.Month;
            day = update.Message.Date.Day;
            hour = update.Message.Date.Hour;
            minute = update.Message.Date.Minute;
            second = update.Message.Date.Second;

            Console.WriteLine("\nData message --> " + year + "/" + month + "/" + day + " - " + hour + ":" + minute + ":" + second);

            Console.WriteLine($"Received: a '{messageText}' message in chat {chadID} from user:\n" + firtname + " - " + lastname);

            messageText = messageText.ToLower();

            if (messageText != null && int.Parse(day.ToString()) >= day && int.Parse(hour.ToString()) >= hour && int.Parse(minute.ToString()) >= minute && int.Parse(second.ToString()) >= second - 10)
            {
                if (messageText == "/Greeting")
                {
                    switch (blockLevel)
                    {
                        case 0:
                            blockLevel = 1;
                            await botClient.SendTextMessageAsync(
                                chatId: chadID,
                                text: "Greeting: \"good block\".",
                                cancellationToken: cancellationToken
                            );
                            return;
                        case 1:
                            blockLevel = 2;
                            await botClient.SendTextMessageAsync(
                                chatId: chadID,
                                text: "Greeting: \"splendid block\".",
                                cancellationToken: cancellationToken
                            );
                            return;
                        case 2:
                            blockLevel = 0;
                            await botClient.SendTextMessageAsync(
                                chatId: chadID,
                                text: "Greeting: \"resetting block\".",
                                cancellationToken: cancellationToken
                            );
                            return;
                    }
                }

                for (int x = 0; x < badwords.Length; x++)
                {
                    if (messageText.Contains(badwords[x]) && blockLevel == 2 && !messDeleted)
                    {
                        messDeleted = true;
                        await botClient.DeleteMessageAsync(chadID, messageID);
                    }
                }

                for (int x = 0; x < veryBadWords.Length; x++)
                {
                    if (messageText.Contains(veryBadWords[x]) && (blockLevel == 2 || blockLevel == 1) && !messDeleted)
                    {
                        messDeleted = true;
                        await botClient.DeleteMessageAsync(chadID, messageID);
                    }
                }
                messDeleted = false;

                if (messageText == "Hello" || messageText == "hello")
                {
                    sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chadID,
                        text: "Hello" + firtname + " " + lastname + " ",
                        cancellationToken: cancellationToken);
                }
            }
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n [{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
