
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace DarlingDb.Models.FindDarling
//{
//    public class Darling
//    {
//        public ulong Id { get; set; }
//        public ulong UsersId { get; set; }
//        public Users Users { get; set; }
//        public string Name { get; set; }
//        public string ImageUrl { get; set; }
//        public sbyte Year { get; set; }
//        public string Description { get; set; }
//        public bool ReportPermission { get; set; }
//        public Gender Sex { get; set; }
//        public enum Gender
//        {
//            none,
//            Man,
//            Woman
//        }
//        public Gender SexSearch { get; set; }
//        public ICollection<Darling_LikeDisLike> MyLikeDisLike { get; set; } = new List<Darling_LikeDisLike>();
//        public ICollection<Darling_LikeDisLike> MeLikeDisLikeTwo { get; set; } = new List<Darling_LikeDisLike>();
//        [NotMapped]
//        public bool AnketeWork
//        {
//            get
//            {
//                bool State = true;

//                if (Year == 0 || !string.IsNullOrWhiteSpace(Name) || !Visible || !AcceptRules || !Blocked)
//                    State = false;

//                return State;
//            }
//        }
//        public bool AcceptRules { get; set; }
//        public State State { get; set; }
//        public bool Visible { get; set; }
//        public bool Blocked { get; set; }
//        public string LastMessage { get; set; }
//        public ulong? ReportLastId { get; set; }
//        public Darling_Reports ReportLast { get; set; }
//        public ulong? DarlingLastId { get; set; }
//        public Darling DarlingLast { get; set; }
//        public ICollection<Darling_Hobbies> Hobbies { get; set; }
//    }
//}
