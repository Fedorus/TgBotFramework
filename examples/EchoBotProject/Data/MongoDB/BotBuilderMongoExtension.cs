using System.Collections.Generic;
using EchoBotProject.StateMachineBoilerplate;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using TgBotFramework;

namespace EchoBotProject.Data.MongoDB
{
    public static class BotBuilderMongoExtension
    {
        public static IBotFrameworkBuilder<TContext> UseMongoDb<TContext>(this IBotFrameworkBuilder<TContext> builder, MongoUrl address) where TContext : IUpdateContext
        {
            var db = new MongoClient(address).GetDatabase(address.DatabaseName);
            
            var userModel = db.GetCollection<UserModel>("Framework.UserModel");
            HandleUserModelCreation(userModel);
            builder.Services.AddSingleton<IMongoDatabase>(sp => new MongoClient(address).GetDatabase(address.DatabaseName));
            
            
            builder.Services.AddScoped<UserStateMapper<TContext>>();
            builder.Services.AddScoped<UserStageManager>();
            builder.UpdatePipelineSettings.Middlewares.Add(typeof(UserStateMapper<TContext>));
            /*var updateModel = db.GetCollection<UpdateModel>("Framework.UpdateModel");
            HandleUpdateModel(updateModel);*/
            
            return builder;
        }

        //NotUsed
        private static void HandleUpdateModel(IMongoCollection<UpdateModel> updateModel)
        {
            var indexes = new List<CreateIndexModel<UpdateModel>>()
            {
                //Consider making this unique... , new CreateIndexOptions(){Unique = true}
                new CreateIndexModel<UpdateModel>(Builders<UpdateModel>.IndexKeys.Hashed(x => x.UpdateId)),
                new CreateIndexModel<UpdateModel>(Builders<UpdateModel>.IndexKeys.Hashed(x => x.ChatId)),
                new CreateIndexModel<UpdateModel>(Builders<UpdateModel>.IndexKeys.Hashed(x => x.UserId)),
                //Builders<UpdateModel>.IndexKeys.Ascending(x => x.Id)
            };

            updateModel.Indexes.CreateMany(indexes);
        }
        
        private static void HandleUserModelCreation(IMongoCollection<UserModel> model)
        {
          /*  var index = Builders<UserModel>.IndexKeys.Ascending(x => x.Id);
            model.Indexes.CreateOne(new CreateIndexModel<UserModel>(index,
                new CreateIndexOptions() { Unique = true }
                )
            );*/
        }
    }
}