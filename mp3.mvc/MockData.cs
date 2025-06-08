using App.Common.Helpers;
using App.Domain.Entities;
using mp3.mvc.Enums;

namespace mp3.mvc
{
    public static class MockData
    {
        public static List<User> UserData = new List<User>()
        {
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-61446981112f"),
                Username = "anonymous",
                DisplayName = "Anonymous",
                Gender = (int)GenderType.male,
                Address = "Thạch Thất, Hà Nội",
                AvatarUrl = "/images/avatars/empty.jpg",
                Dob = DateTime.Now,
                PhoneNumber = "",
                Email = "",
                IsAdmin = false,
                IsLocked = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Password = ""
            },
            new User
            {
                Id = Guid.Parse("44db2929-809b-41f8-9ce4-61446981112f"),
                Username = "anhcanviet",
                DisplayName = "Cấn Việt Anh",
                Gender = (int)GenderType.male,
                Address = "Thạch Thất, Hà Nội",
                AvatarUrl = "/images/avatars/user1.jpg",
                Dob = DateTime.Now,
                PhoneNumber = "0123456111",
                Email = "anhcanviet@gmail.com",
                IsAdmin = false,
                IsLocked = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Password = TokenHelper.md5_hash("abc1234")
            },
            new User
            {
                Id = Guid.Parse("6dbdf1c9-5f97-41bf-876b-712d2143ab44"),
                Username = "hieulevan",
                DisplayName = "Lê Văn Hiếu",
                Gender = (int)GenderType.male,
                Address = "Cẩm Giàng, Hải Dương",
                AvatarUrl = "/images/avatars/empty.jpg",
                Dob = DateTime.Now,
                PhoneNumber = "0123456222",
                Email = "hieulevan@gmail.com",
                IsAdmin = false,
                IsLocked = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Password = TokenHelper.md5_hash("abc1234")
            },
            new User
            {
                Id = Guid.Parse("fd998c87-d3db-4703-abcb-02463aaf9cd2"),
                Username = "bnguyenthi",
                DisplayName = "Nguyễn Thị B",
                Gender = (int)GenderType.female,
                Address = "Thạch Thất, Hà Nội",
                AvatarUrl ="/images/avatars/empty.jpg",
                Dob = DateTime.Now,
                PhoneNumber = "0123456333",
                Email = "bnguyenthi@gmail.com",
                IsAdmin = false,
                IsLocked = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Password = TokenHelper.md5_hash("abc1234")
            },
            new User
            {
                Id = Guid.Parse("800eac18-f5c3-4f83-a64b-92f1b92c5afb"),
                Username = "admin",
                DisplayName = "Admin",
                Gender = (int)GenderType.male,
                Address = "Thạch Thất, Hà Nội",
                AvatarUrl = "/images/avatars/empty.jpg",
                Dob = DateTime.Now,
                PhoneNumber = "0123456444",
                Email = "admin@gmail.com",
                IsAdmin = true,
                IsLocked = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Password = TokenHelper.md5_hash("abc1234")
            }
        };
        public static List<Author> AuthorData = new List<Author>()
        {
            new Author()
            {
                Id = Guid.Parse("19898cd2-75b4-4d3c-86bd-0d111392ae62"),
                Name = "anonymous",
            },
            new Author()
            {
                Id = Guid.Parse("19898cd2-75b4-4d3c-86bd-0d111392ae63"),
                Name = "Sơn Tùng MTP",
            },
            new Author()
            {
                Id = Guid.Parse("19898cd2-75b4-4d3c-86bd-0d111392ae64"),
                Name = "Đen Vâu",
            },
            new Author()
            {
                Id = Guid.Parse("a54c0c25-6172-43ee-8d5f-e1be77d21204"),
                Name = "BigBang",
            },
            new Author()
            {
                Id = Guid.Parse("a54c0c25-6172-43ee-8d5f-e1be77d21205"),
                Name = "Nguyễn Hải Phong",
            },
        };
        public static List<Category> CategoryData = new List<Category>()
        {
            new Category
            {
                Id = Guid.Parse("fe37b02d-ef94-4e3b-b685-46bb8f7b57dc"),
                Name = "Unknown"
            },
            new Category
            {
                Id = Guid.Parse("7fdbc6fa-c648-4944-be6a-00c9a6785fce"),
                Name = "Pop"
            },
            new Category
            {
                Id = Guid.Parse("d7d58d7b-f11c-4ec0-8409-939914aa9c1a"),
                Name = "Folk"
            },
            new Category
            {
                Id = Guid.Parse("ea367dd6-f8d7-4e08-b4f2-d537763cb64e"),
                Name = "Country"
            },
            new Category
            {
                Id = Guid.Parse("6d23da42-426f-4572-a7e1-baace7460841"),
                Name = "Rap"
            },
        };
        public static List<Media> MediaData = new List<Media>()
        {
            new Media
            {
                Id = Guid.Parse("e0f37b00-d617-4122-ae63-f766c91ea64e"),
                UserId = UserData[0].Id,
                User = UserData[0],
                Name = "Chúng ta của hiện tại",
                MediaContent = new List<MediaContent>
                {
                    new MediaContent
                    {
                        Id = Guid.Parse("d235b516-fe6d-44cc-86c6-24bac034b588"),
                        MediaId = Guid.Parse("e0f37b00-d617-4122-ae63-f766c91ea64e"),
                        Value = "/images/media/chung_ta_cua_hien_tai.jpg",
                        Type = (int) MediaContentType.image,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    }
                },
                Price = 1000000,
                Type = (int)MediaType.audio,
                ContentUrl = "",
                Description = "",
                IsHidden = false,
                IsLocked = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AuthorId = AuthorData[1].Id,
                Author = AuthorData[1],
                CategoryId = CategoryData[1].Id,
                Category = CategoryData[1],
            },
            new Media
            {
                Id = Guid.Parse("bfac4cfe-1ad6-4b8e-9c22-45d51acd20fb"),
                UserId = UserData[0].Id,
                User = UserData[0],
                Name = "Chắc ai đó sẽ về",
                MediaContent = new List<MediaContent>
                {
                    new MediaContent
                    {
                        Id = Guid.Parse("98f5ca24-1d91-40e5-b0ea-9abf908d24cb"),
                        MediaId = Guid.Parse("bfac4cfe-1ad6-4b8e-9c22-45d51acd20fb"),
                        Value = "/images/media/chac_ai_do_se_ve.jpg",
                        Type = (int) MediaContentType.image,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    }
                },
                Price = 2000000,
                Type = (int)MediaType.audio,
                ContentUrl = "",
                Description = "",
                IsHidden = false,
                IsLocked = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AuthorId = AuthorData[1].Id,
                Author = AuthorData[1],
                CategoryId = CategoryData[1].Id,
                Category = CategoryData[1],
            },
            new Media
            {
                Id = Guid.Parse("f7450d9a-ab05-4b02-9bbe-473e334b06fd"),
                UserId = UserData[0].Id,
                User = UserData[0],
                Name = "Blue",
                MediaContent = new List<MediaContent>
                {
                    new MediaContent
                    {
                        Id = Guid.Parse("0cf89168-d600-435e-80c1-09f1b3035d20"),
                        MediaId = Guid.Parse("f7450d9a-ab05-4b02-9bbe-473e334b06fd"),
                        Value = "/images/media/blue.jpg",
                        Type = (int) MediaContentType.image,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    }
                },
                Price = 0,
                Type = (int)MediaType.audio,
                ContentUrl = "/media/audio/blue.mp3",
                Description = "",
                IsHidden = false,
                IsLocked = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AuthorId = AuthorData[3].Id,
                Author = AuthorData[3],
                CategoryId = CategoryData[4].Id,
                Category = CategoryData[4],
            },
            new Media
            {
                Id = Guid.Parse("5087667c-2f59-473a-9e78-9e871cb6b3b2"),
                UserId = UserData[1].Id,
                User = UserData[1],
                Name = "Let not fall in love",
                MediaContent = new List<MediaContent>
                {
                    new MediaContent
                    {
                        Id = Guid.Parse("4ae773b0-8db3-4dc8-b371-832af3613ee9"),
                        MediaId = Guid.Parse("5087667c-2f59-473a-9e78-9e871cb6b3b2"),
                        Value = "/images/media/let_not_fall_in_love.jpg",
                        Type = (int) MediaContentType.image,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    }
                },
                Price = 2000000,
                Type = (int)MediaType.audio,
                ContentUrl = "",
                Description = "",
                IsHidden = false,
                IsLocked = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AuthorId = AuthorData[3].Id,
                Author = AuthorData[3],
                CategoryId = CategoryData[4].Id,
                Category = CategoryData[4],
            },
            new Media
            {
                Id = Guid.Parse("315dc472-f790-47f3-9473-79edcc4ca060"),
                UserId = UserData[2].Id,
                User = UserData[2],
                Name = "Chạy ngay đi",
                MediaContent = new List<MediaContent>
                {
                    new MediaContent
                    {
                        Id = Guid.Parse("142ad6ec-a913-4771-9df3-20f1d3ed8390"),
                        MediaId = Guid.Parse("315dc472-f790-47f3-9473-79edcc4ca060"),
                        Value = "/images/media/chay_ngay_di.jpg",
                        Type = (int) MediaContentType.image,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    }
                },
                Price = 500000,
                Type = (int)MediaType.audio,
                ContentUrl = "/media/audio/blue.mp3",
                Description = "",
                IsHidden = false,
                IsLocked = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AuthorId = AuthorData[1].Id,
                Author = AuthorData[1],
                CategoryId = CategoryData[1].Id,
                Category = CategoryData[1],
            },
            new Media
            {
                Id = Guid.Parse("315dc472-f790-47f3-9473-79edcc4ca061"),
                UserId = UserData[2].Id,
                User = UserData[2],
                Name = "Ba kể con nghe",
                MediaContent = new List<MediaContent>
                {
                    new MediaContent
                    {
                        Id = Guid.Parse("142ad6ec-a913-4771-9df3-20f1d3ed8390"),
                        MediaId = Guid.Parse("315dc472-f790-47f3-9473-79edcc4ca060"),
                        Value = "/images/media/chay_ngay_di.jpg",
                        Type = (int) MediaContentType.image,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    }
                },
                Price = 500000,
                Type = (int)MediaType.audio,
                ContentUrl = "/media/audio/blue.mp3",
                Description = "",
                IsHidden = false,
                IsLocked = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AuthorId = AuthorData[1].Id,
                Author = AuthorData[1],
                CategoryId = CategoryData[1].Id,
                Category = CategoryData[1],
            },
            new Media
            {
                Id = Guid.Parse("315dc472-f790-47f3-9473-79edcc4ca062"),
                UserId = UserData[2].Id,
                User = UserData[2],
                Name = "Ba kể con nghe",
                MediaContent = new List<MediaContent>
                {
                    new MediaContent
                    {
                        Id = Guid.Parse("142ad6ec-a913-4771-9df3-20f1d3ed8390"),
                        MediaId = Guid.Parse("315dc472-f790-47f3-9473-79edcc4ca060"),
                        Value = "/images/media/chay_ngay_di.jpg",
                        Type = (int) MediaContentType.image,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    }
                },
                Price = 500000,
                Type = (int)MediaType.audio,
                ContentUrl = "/media/audio/blue.mp3",
                Description = "",
                IsHidden = false,
                IsLocked = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AuthorId = AuthorData[1].Id,
                Author = AuthorData[1],
                CategoryId = CategoryData[1].Id,
                Category = CategoryData[1],
            },
        };
    }
}
