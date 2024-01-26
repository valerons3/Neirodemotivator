using System;
using System.Threading.Tasks;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Neirodemotivator;

public class Program
{
    private static string _token = "6530487214:AAHgWGBKKdEU2IBHv0Yh72Ms4ZfkXuTjt5A";
    public static TelegramBotClient BotClient;
    public static int ControlVariable = 0;
    public static DataNeiro dt;
    public static Chat HelpBot = new Chat();
    public static void Main(string[] args)
    {
        HelpBot.Id = 1016409811;
        BotClient = new TelegramBotClient(_token);
        BotClient.StartReceiving(Update, Error);
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

                    await using Stream stream = System.IO.File.OpenRead(dt.DestinationPhoto);
                    await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, "1.png"),
                        caption: dt.Text);

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
                    await using Stream stream = System.IO.File.OpenRead(dt.DestinationPhoto);
                    await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, "1.png"),
                        caption: dt.Text);

                    dt = new DataNeiro();
                    return;
                }
            }
        }
    }
    private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
    { throw new NotImplementedException(); }
}