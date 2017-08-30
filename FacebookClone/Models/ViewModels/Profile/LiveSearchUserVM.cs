using FacebookClone.Models.Data;

namespace FacebookClone.Models.ViewModels.Profile
{
    public class LiveSearchUserVM
    {
        public LiveSearchUserVM()
        {
            
        }

        public LiveSearchUserVM(UserDTO row)
        {
            UserId = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            Username = row.Username;
        }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }
}