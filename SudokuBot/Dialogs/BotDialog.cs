using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.IO;
using SudokuLibrary;
using System.Drawing;
using SudokuLibrary.ComputerVision;

namespace SudokuBot
{
    [Serializable]
    public class BotDialog : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            // TODO
            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }

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

                    var img = new Bitmap(memoryStream);


                    // Processing image
                    var gameField = GameFieldRecognizer.Recognize(img);
                    var resImg = new Sudoku(gameField).GetLightResultImage().Bitmap;

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
                catch (Exception e)
                {
                    await context.PostAsync("Sorry, There`s some problem.\n Try to send another image");
                    await context.PostAsync($"Error. Source: { e.Source}, Message: { e.Message}");
                }
            }

            // Sending message to user
            await context.PostAsync(replyMessage);
            context.Wait(MessageReceivedAsync);
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}