﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Services.Utilities
{
    public static class Messages
    {
        public static class General
        {
            public static string ValidationError()
            {
                return "Bir veya daha fazla validasyon hatası ile karşılaşıldı";
            }
        }
        public static class Category
        {
            public static string NotFound(bool isPlural)
            {
                return isPlural ? "Hiçbir Kategori bulunamadı" : "Böyle bir kategori bulunamadı.";
            }

            public static string NotFoundById(int categoryId)
            {
                return $"{categoryId} Id'li kategori bulunamadı.";
            }

            public static string Add(string categoryName)
            {
                return $"{categoryName} adlı Kategori eklendi";
            }

            public static string Update(string categoryName)
            {
                return $"{categoryName} adlı Kategori güncellendi";
            }

            public static string Delete(string categoryName)
            {
                return $"{categoryName} adlı Kategori silindi";
            }

            public static string HardDelete(string categoryName)
            {
                return $"{categoryName} adlı Kategori veritabanından silindi";
            }

            public static string UndoDelete(string categoryName)
            {
                return $"{categoryName} adlı Kategori arşivden geri getirilmiştir.";
            }
        }

        public static class Article
        {
            public static string NotFound(bool isPlural)
            {
                if (isPlural) return "Makaleler bulunamadı.";
                return "Böyle bir makale bulunamadı.";
            }

            public static string NotFoundById(int articleId)
            {
                return $"{articleId} Id'li makale bulunamadı.";
            }

            public static string Add(string articleTitle)
            {
                return $"{articleTitle} başlıklı makale başarıyla eklenmiştir.";
            }

            public static string Update(string articleTitle)
            {
                return $"{articleTitle} başlıklı makale başarıyla güncellenmiştir.";
            }
            public static string Delete(string articleTitle)
            {
                return $"{articleTitle} başlıklı makale başarıyla silinmiştir.";
            }
            public static string HardDelete(string articleTitle)
            {
                return $"{articleTitle} başlıklı makale başarıyla veritabanından silinmiştir.";
            }

            public static string UndoDelete(string articleTitle)
            {
                return $"{articleTitle} başlıklı makale arşivden geri getirilmiştir.";
            }

            public static string IncreaseViewCount(string articleTitle)
            {
                return $"{articleTitle} başlıklı makalenin okunma sayısı arttırılmıştır";
            }
        }

        public static class Comment
        {
            public static string NotFound(bool isPlural)
            {
                if (isPlural) return "Hiç bir yorum bulunamadı.";
                return "Böyle bir yorum bulunamadı.";
            }

            public static string Add(string createdByName)
            {
                return $"Sayın {createdByName}, yorumunuz başarıyla eklenmiştir.";
            }

            public static string Aprove(int commentId)
            {
                return $"Sayın {commentId}, nolu yorum başarıyla onaylanmıştır";
            }

            public static string Update(string createdByName)
            {
                return $"{createdByName} tarafından eklenen yorum başarıyla güncellenmiştir.";
            }
            public static string Delete(string createdByName)
            {
                return $"{createdByName} tarafından eklenen yorum başarıyla silinmiştir.";
            }
            public static string HardDelete(string createdByName)
            {
                return $"{createdByName} tarafından eklenen yorum başarıyla veritabanından silinmiştir.";
            }

            public static string UndoDelete(string createdByName)
            {
                return $"{createdByName} tarafından eklenen yorum arşivden geri getirilmiştir.";
            }
        }

        public static class User
        {
            public static string NotFound(bool isPlural)
            {
                return isPlural ? "Hiçbir Kullanıcı bulunamadı" : "Böyle bir kullanıcı bulunamadı.";
            }

            public static string NotFoundById(int userId)
            {
                return $"{userId} Id'li kullanıcı bulunamadı.";
            }

            //public static string Add(string categoryName)
            //{
            //    return $"{categoryName} adlı Kategori eklendi";
            //}

            //public static string Update(string categoryName)
            //{
            //    return $"{categoryName} adlı Kategori güncellendi";
            //}

            //public static string Delete(string categoryName)
            //{
            //    return $"{categoryName} adlı Kategori silindi";
            //}

            //public static string HardDelete(string categoryName)
            //{
            //    return $"{categoryName} adlı Kategori veritabanından silindi";
            //}

            //public static string UndoDelete(string categoryName)
            //{
            //    return $"{categoryName} adlı Kategori arşivden geri getirilmiştir.";
            //}
        }
    }
}
