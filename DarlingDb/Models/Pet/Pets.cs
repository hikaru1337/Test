using DarlingDb.Models.Pet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static DarlingDb.Enums;

namespace DarlingDb.Models
{
    public class Pets
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public string Status
        {
            get
            {
                if (SleepNow)
                    return "Питомец спит. Не будите его!";
                else if(HP < 50)
                {
                    if (EAT < 50)
                        return "Питомца нужно покормить!";
                    else if (MOOD < 50)
                        return "Питомец нуждается в заботе!";
                    else
                        return "Питомец нуждается в помощи";
                }
                else
                {
                    if (EAT < 75)
                        return "Питомца скоро нужно покормить";
                    else if (MOOD < 75)
                        return "Питомца скоро нужно потискать";
                    else if (SLEEP < 75)
                        return "Питомец скоро устанет";
                    else
                        return "Питомец в норме";
                }
            }
        }
        public ulong UserId { get; set; }
        public Users User { get; set; }

        [NotMapped]
        public bool OnControl { get; set; }

        public bool RegenController
        {
            get
            {
                if(!Die && !OnControl)
                {
                    OnControl = true;
                    Regens(RegenType.Eat);
                    Regens(RegenType.Mood);
                    Regens(RegenType.Sleep);
                    OnControl = false;
                } 
                return false;
            }
            set { }
        }


        [NotMapped]
        public byte MaxHaracter => Convert.ToByte(127 + Level);
        public uint XP { get; set; }
        public byte Level
        {
            get
            {
                uint MaxXP = 1290320;
                if (XP > MaxXP)
                    XP = MaxXP;

                return (byte)Math.Sqrt(XP / 80);
            }
        }

        private enum RegenType
        {
            Eat,
            Mood,
            Sleep
        }
        private void Regens(RegenType type)
        {
            using (db _db = new())
            {
                var Time1 = DateTime.Now;
                byte Value1 = 0;
                int MinuteToAction = 12;

                switch (type)
                {
                    case RegenType.Eat:
                        if (Time1.Year != 1)
                            Time1 = LastEat;
                        if (EAT > MaxHaracter)
                            EAT = MaxHaracter;
                        Value1 = EAT;
                        break;
                    case RegenType.Mood:
                        if (Time1.Year != 1)
                            Time1 = LastMood;
                        if (MOOD > MaxHaracter)
                            MOOD = MaxHaracter;
                        Value1 = MOOD;
                        break;
                    case RegenType.Sleep:
                        MinuteToAction = 3;
                        if (Time1.Year != 1)
                            Time1 = LastSleep;

                        if (SLEEP > MaxHaracter)
                            SLEEP = MaxHaracter;

                        Value1 = SLEEP;

                        if (Value1 < 50)
                            SleepNow = true;
                        else if (Value1 == MaxHaracter)
                            SleepNow = false;

                        break;
                }


                var MinuteHave = (DateTime.Now - Time1).TotalMinutes;
                if (MinuteHave >= MinuteToAction)
                {
                    var CountAdd = Math.Truncate(MinuteHave / MinuteToAction);
                    for (int i = 0; i < CountAdd; i++)
                    {
                        Time1 = Time1.AddMinutes(MinuteToAction);
                        if (type == RegenType.Sleep && SleepNow)
                        {
                            if (Value1 != MaxHaracter)
                                Value1++;
                            else
                            {
                                Time1.AddMinutes(MinuteToAction * ((CountAdd - 1) - i));
                                SleepNow = false;
                                break;
                            }
                                
                        }
                        else
                        {
                            if (Value1 > 0)
                            {
                                if (type == RegenType.Sleep && Value1 < 50)
                                    SleepNow = true;
                                else
                                    Value1--;

                            }  
                            else
                            {
                                if (type != RegenType.Sleep)
                                    Die = true;
                                break;
                            }
                        }
                    }

                    switch (type)
                    {
                        case RegenType.Eat:
                            LastEat = Time1;
                            EAT = Value1;
                            break;
                        case RegenType.Mood:
                            LastMood = Time1;
                            MOOD = Value1;
                            break;
                        case RegenType.Sleep:
                            LastSleep = Time1;
                            SLEEP = Value1;
                            break;
                    }

                    
                }

                _db.Pets.Update(this);
                _db.SaveChangesAsync();

            }
        }


        public DateTime LastEat { get; set; }
        public DateTime LastMood { get; set; }
        public DateTime LastSleep { get; set; }
        public bool SleepNow { get; set; }

        public ICollection<Items> Items { get; set; }
        public PetTypesEnum PetType { get; set; }
        public DateTime DateOfBirth { get; set; }
        public byte HP
        {
            get
            {
                const int MaxTwo = 100;
                double Max = 1.27 + (Level / 100);

                double _a = 35 * (MOOD / Max) / MaxTwo;
                double _b = 57 * (EAT / Max) / MaxTwo;
                double _c = 35 * (SLEEP / Max) / MaxTwo;

                return (byte)(_a + _b + _c);
            }
        }
        public byte MOOD { get; set; }
        public byte EAT { get; set; }
        public byte SLEEP { get; set; }
        public bool Die { get; set; }


    }
}
