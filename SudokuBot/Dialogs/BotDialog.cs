using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.IO;
using SudokuLibrary;
using System.Drawing;

namespace SudokuBot
{
    [Serializable]
    public class BotDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            var replyMessage = context.MakeMessage();
            replyMessage.Text = "Hi. Just send me image with game=)";

            foreach (var item in message.Attachments)
            {
                if (!item.ContentType.StartsWith("image"))
                    continue;

                try
                {
                    // Download attached image
                    var connector = new ConnectorClient(new Uri(message.ServiceUrl));
                    var content = connector.HttpClient.GetStreamAsync(item.ContentUrl).Result;
                    var memoryStream = new MemoryStream();
                    content.CopyTo(memoryStream);

                    var bmp = new Bitmap(memoryStream);


                    // Processing image
                    var resImg = new Sudoku(bmp).GetLightResult();

                    // Add result image to attachment
                    var converter = new ImageConverter();
                    var b = (byte[])converter.ConvertTo(resImg, typeof(byte[]));
                    var imageData = Convert.ToBase64String(b);

                    replyMessage.Attachments.Add(new Attachment
                    {
                        Name = "Result",
                        ContentType = "image/png",
                        ContentUrl = $"data:{item.ContentType};base64,{imageData}"
                    });

                    replyMessage.Text = "It`s your answer";
                }
                catch (Exception)
                {
                    await context.PostAsync("Sorry, There`s some problem.\n Try send another one image");
                }
            }

            // Sending message to user
            await context.PostAsync(replyMessage);
            context.Wait(MessageReceivedAsync);
        }
    }
}