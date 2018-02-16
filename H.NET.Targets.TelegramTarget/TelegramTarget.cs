using H.NET.Core;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace H.NET.Targets
{
    public class TelegramTarget : Module, ITarget
    {
        #region Properties

        private int _userId;
        public int UserId
        {
            get => _userId;
            set {
                _userId = value;

                if (!IsValid())
                {
                    return;
                }

                ChatId = new ChatId(value);
            }
        }

        private string _token;
        public string Token
        {
            get => _token;
            set {
                _token = value;

                if (!IsValid())
                {
                    return;
                }

                Client = new TelegramBotClient(value);
            }
        }

        private TelegramBotClient Client { get; set; }
        private ChatId ChatId { get; set; }

        #endregion

        #region Constructors

        public TelegramTarget()
        {
            AddSetting("Token", o => Token = (string)o, o => o is string, string.Empty);
            AddSetting("UserId", o => UserId = (int)o, o => o is int, 0);
        }

        public override bool IsValid() => base.IsValid() && !string.IsNullOrWhiteSpace(Token) && UserId > 0;

        #endregion

        #region ITarget

        public async void SendMessage(string text) => await Client.SendTextMessageAsync(ChatId, text);

        #endregion
    }
}
