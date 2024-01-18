using System;
using System.Threading.Tasks;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Neirodemotivator;

public class Program
{
    private static string _token = "6530487214:AAHgWGBKKdEU2IBHv0Yh72Ms4ZfkXuTjt5A";
    public static TelegramBotClient _botClient;
    public static int ControlVariable = 0;
    public static DataNeiro dt;
    public static void Main(string[] args)
    {
        _botClient = new TelegramBotClient(_token);
        _botClient.StartReceiving(Update, Error);
        Console.ReadLine();
    }
    
    async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        if (update.Message != null)
        {
            var message = update.Message;
            if (message.Photo != null)
            {
                if (ControlVariable == 0)
                {
                    dt = new DataNeiro();
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Теперь отправь текст");
                    ControlVariable = 1;

                    var field = update.Message.Photo.Last().FileId;
                    var fileInfo = await botClient.GetFileAsync(field);
                    var filePath = fileInfo.FilePath;

                    string destinationFilePath = @"D:\CheckFolder\1.png";
                    await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                    await botClient.DownloadFileAsync(filePath, fileStream);
                    fileStream.Close();
                    dt.DestinationPhoto = destinationFilePath;

                    return;
                }
                else
                {
                    ControlVariable = 0;

                    var field = update.Message.Photo.Last().FileId;
                    var fileInfo = await botClient.GetFileAsync(field);
                    var filePath = fileInfo.FilePath;

                    string destinationFilePath = @$"D:\CheckFolder\1.png";
                    await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                    await botClient.DownloadFileAsync(filePath, fileStream);
                    fileStream.Close();
                    dt.DestinationPhoto = destinationFilePath;
                    //логика демотиватора
                    await botClient.SendTextMessageAsync(message.Chat.Id, "проверка что всё работает");

                    dt = new DataNeiro();
                    return;
                }
            }
            if (message.Text != null)
            {
                if (ControlVariable == 0)
                {
                    dt = new DataNeiro();
                    dt.Text = message.Text;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Теперь отправь картинку");
                    ControlVariable = 1;
                    return;
                }
                else
                {
                    ControlVariable = 0;
                    dt.Text = message.Text;

                    //логика демотиватора
                    await botClient.SendTextMessageAsync(message.Chat.Id, "проверка что всё работает");

                    dt = new DataNeiro();
                    return;
                }
            }
        }
    }
    private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
    { throw new NotImplementedException(); }
}