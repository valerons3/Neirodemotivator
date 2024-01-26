using System;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
            try
            {
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


                        string destinationFilePath = @"D:\CheckFolder\1.jpg";
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

                        string hash = Guid.NewGuid().ToString().Remove(5);
                        string destinationFilePath = @$"D:\CheckFolder\{hash}.jpg";
                        await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                        await botClient.DownloadFileAsync(filePath, fileStream);
                        fileStream.Close();
                        dt.DestinationPhoto = destinationFilePath;
                        //логика демотиватора

                        string backgroundImagePath = @"D:\CheckFolder\sample.jpg";
                        string overlayImagePath = dt.DestinationPhoto;
                        int rectangleX = 77;
                        int rectangleY = 50;
                        int rectangleWidth = 652;
                        int rectangleHeight = 437;

                        string hash2 = Guid.NewGuid().ToString().Remove(5);
                        
                        Bitmap backgroundImage = new Bitmap(backgroundImagePath);
                        Bitmap overlayImage = new Bitmap(overlayImagePath);
                        using (Graphics g = Graphics.FromImage(backgroundImage))
                        {
                            g.DrawImage(overlayImage, new Rectangle(rectangleX, rectangleY, rectangleWidth, rectangleHeight));
                        }
                        string outputImagePath = @$"D:\CheckFolder\{hash2}.jpg";
                        backgroundImage.Save(outputImagePath, ImageFormat.Jpeg);


                        PointF firstLocation = new PointF(100, 510);

                        string imageFilePath = outputImagePath;
                        Bitmap bitmap = (Bitmap)Image.FromFile(imageFilePath);

                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            using (Font arialFont = new Font("Arial", 50))
                            {
                                graphics.DrawString(dt.Text, arialFont, Brushes.White, firstLocation);
                            }
                        }

                        string hash3 = Guid.NewGuid().ToString().Remove(5);
                        string resultPhotoPath = @$"D:\CheckFolder\{hash3}.jpg";
                        bitmap.Save(resultPhotoPath);


                        await using Stream stream = System.IO.File.OpenRead(resultPhotoPath);
                        await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, "3.jpg"));


                        dt = new DataNeiro();
                        return;
                    }
                }
                if (message.Text != null)
                {
                    if (message.Text == "/start")
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Привет! Это бот демотиватор\nСначала отправь картинку или текст, а потом следуй инструкциям");
                        return;
                    }
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
                        string backgroundImagePath = @"D:\CheckFolder\sample.jpg";
                        string overlayImagePath = dt.DestinationPhoto;
                        int rectangleX = 77;
                        int rectangleY = 50;
                        int rectangleWidth = 652;
                        int rectangleHeight = 437;

                        string hash2 = Guid.NewGuid().ToString().Remove(5);

                        Bitmap backgroundImage = new Bitmap(backgroundImagePath);
                        Bitmap overlayImage = new Bitmap(overlayImagePath);
                        using (Graphics g = Graphics.FromImage(backgroundImage))
                        {
                            g.DrawImage(overlayImage, new Rectangle(rectangleX, rectangleY, rectangleWidth, rectangleHeight));
                        }
                        string outputImagePath = @$"D:\CheckFolder\{hash2}.jpg";
                        backgroundImage.Save(outputImagePath, ImageFormat.Jpeg);


                        PointF firstLocation = new PointF(100, 510);

                        string imageFilePath = outputImagePath;
                        Bitmap bitmap = (Bitmap)Image.FromFile(imageFilePath);

                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            using (Font arialFont = new Font("Arial", 45))
                            {
                                graphics.DrawString(dt.Text, arialFont, Brushes.White, firstLocation);
                            }
                        }

                        string hash3 = Guid.NewGuid().ToString().Remove(5);
                        string resultPhotoPath = @$"D:\CheckFolder\{hash3}.jpg";
                        bitmap.Save(resultPhotoPath);


                        await using Stream stream = System.IO.File.OpenRead(resultPhotoPath);
                        await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, "3.jpg"));

                        dt = new DataNeiro();
                        return;
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Exception in Update method: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                await botClient.SendTextMessageAsync(message.Chat.Id, "Попробуйте повторить попытку через 10 секунд");
                
            }
        }
    }
    private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
    { throw new NotImplementedException(); }
}