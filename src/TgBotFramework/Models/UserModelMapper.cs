namespace TgBotFramework.Models
{
    public static class UserModelMapper
    {
        public static void MapModelToState(UserState state, UserModel model)
        {
            state.Role = model.Role;
            state.Stage = model.Stage;
            state.Step = model.Step;
            state.LanguageCode = model.LanguageCode;
        }
        
        public static bool MapStateToModel(UserState contextUserState, UserModel userDbObject)
        {
            bool result = false;
            if (userDbObject == null)
            {
                return false;
            }

            if (contextUserState.Role != userDbObject.Role)
            {
                userDbObject.Role = contextUserState.Role;
                result = true;
            }

            if (contextUserState.Stage != userDbObject.Stage)
            {
                userDbObject.Stage = contextUserState.Stage;
                result = true;
            }
            
            if (contextUserState.Step != userDbObject.Step)
            {
                userDbObject.Step = contextUserState.Step;
                result = true;
            }

            if (contextUserState.LanguageCode != userDbObject.LanguageCode)
            {
                userDbObject.LanguageCode = contextUserState.LanguageCode;
                result = true;
            }

            return result;
        }
    }
}