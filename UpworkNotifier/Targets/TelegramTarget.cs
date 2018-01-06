using Telegram.Bot;
using Telegram.Bot.Types;

namespace UpworkNotifier.Targets
{
    public class TelegramTarget : ITarget
    {
        #region Properties

        public TelegramBotClient Client { get; }
        public ChatId ChatId { get; }

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
