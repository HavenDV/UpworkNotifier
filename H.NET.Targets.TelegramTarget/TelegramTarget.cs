using H.NET.Core;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace H.NET.Targets.TelegramTarget
{
    public class TelegramTarget : ITarget
    {
        #region Properties

        private TelegramBotClient Client { get; }
        private ChatId ChatId { get; }

        #endregion

        #region Constructors

        public TelegramTarget(string token, int userId)
        {
            Client = new TelegramBotClient(token);
            ChatId = new ChatId(userId);
        }

        #endregion

        #region ITarget

        public async void SendMessage(string text) => await Client.SendTextMessageAsync(ChatId, text);

        #endregion
    }
}
