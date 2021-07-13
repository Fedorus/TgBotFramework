# TgBotFramework

Make [Telegram.Bot.Framework](https://github.com/TelegramBots/Telegram.Bot.Framework) great again!

# Early state
there is nothing that is guarantied to work, but something definitely works ;)

# Whats new?

This project targets .NET 5.0+ and there won`t be any support for Framework. So keep your stack updated =)

In this implementation you can get (at least) same pipeline experience as in [Telegram.Bot.Framework](https://github.com/TelegramBots/Telegram.Bot.Framework) but enchanted with:


#Short-term goals
- [x] Same pipeline processing for Longpolling, Webhook and testing.
- [x] Longpolling
- [x] **Middlewares** - special concept to separate update handling from updateContext configuration.
- [x] **Attributes** and some reflection magic to handle states and command handlers
- [ ] Webhook
- [ ] **DB integration**
- [ ] **Stages** (stage and step) to describe user`s state. (yeah, state machine out of the box, at least in plans)
- [ ] **Roles** - it is always needed to separate bot owner from others, right?

  
#Long-term goals
- [ ] **Dashboard** 
- [ ] Better **Logging**
- [ ] Fully tested (I wouldn't count on that one 😭) 
