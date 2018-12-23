using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace AP_CW_Sokoban_KuryshevRV
{
    [DataContract]
    public class User
    {
        public User()
        {
            datas = new List<UserData>();
        }

        public User(string name, UserData userData)
        {
            UserName = name;
            datas.Add(userData);
        }
        [DataMember]
        public string UserName { get; set; }
        
        public class UserData
        {
            public int CompletedLVL { get; set; }//пройденный уровень

            public int AttemptedCount { get; set; }//попытки

            public int StepsNum { get; set; }//гиги

            public UserData(){}

            public UserData(int attemptedCount, int stepsNum, int level)
            {
                AttemptedCount = attemptedCount;
                StepsNum = stepsNum;
                CompletedLVL = level;
            }
        }
        [DataMember]
        public List<UserData> datas { get; set; }

    }
}
