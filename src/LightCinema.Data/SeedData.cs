using LightCinema.Data.Entities;

namespace LightCinema.Data;

public static class SeedData
{
    public static async Task Seed(ApplicationDbContext dbContext)
    {
        var random = new Random();

        var users = new List<User>
        {
            new()
            {
                Login = "user1",
                Password = "test"
            },
            new()
            {
                Login = "user2",
                Password = "test"
            }
        };

        var hallCount = 2;
        var seatRows = 5;
        var seatNumbers = 5;
        var seats = new List<Seat>(hallCount * seatRows * seatNumbers);

        for (var i = 0; i < hallCount; i++)
        {
            for (var j = 0; j < seatRows; j++)
            {
                for (var k = 0; k < seatNumbers; k++)
                {
                    seats.Add(new Seat
                    {
                        Hall = i,
                        Row = j,
                        Number = k,
                        IsIncreasedPrice = j == seatRows - 1
                    });
                }
            }
        }

        var countries = new List<Country>
        {
            new() { Name = "Россия" },
            new() { Name = "США" },
            new() { Name = "Великобритания" },
            new() { Name = "Германия" },
            new() { Name = "Франция" }
        };

        var genres = new List<Genre>
        {
            new() { Name = "криминал" },
            new() { Name = "драма" },
            new() { Name = "триллер" },
            new() { Name = "ужасы" },
            new() { Name = "романтика" }
        };

        var movies = new List<Movie>
        {
            new()
            {
                Name = "Драйв",
                Descriptions =
                    "Молчаливый водитель спасает девушку от гангстеров. Неонуар с Райаном Гослингом и пульсирующим саундтреком",
                Genres = genres.Take(3).ToList(),
                Countries = new List<Country> { countries[1] },
                Year = 2011,
                AgeLimit = 16,
                PosterLink = "https://avatars.mds.yandex.net/get-kinopoisk-image/1704946/66b27e0c-9f85-424c-bfb0-415bd8475bc8/1920x",
                ImageLink = "https://avatars.mds.yandex.net/get-kinopoisk-image/1946459/af69a221-5921-4186-b700-2197d39e8362/1920x",
            },
            new()
            {
                Name = "Зелёный слоник",
                Descriptions =
                    "Два младших офицера, сидя в одной камере на гауптвахте, вынуждены решать острые социальные и психологические вопросы в небольшом пространстве.",
                Genres = new List<Genre> { genres[2], genres[3] },
                Countries = new List<Country> { countries[0] },
                Year = 1999,
                AgeLimit = 16,
                PosterLink = "https://www.kinopoisk.ru/BS30p1339/ef5848sTERG/KbQz5zYtTFyb17k4p4_4hJZKgzpNWRVMicfW8ufqmCcYhTZaFF1-TScY2oFCu3Mj203Alkab8w7kqemQvssXVqmx5zDk_hkLzEVqLbAYrJSE3Jc_CssA3Y6DAaQwIVQtFA9q05uXdkY_SUrQhSwkdYtJg5SWzSGfYbVzFXcTQAjt-bMoez6GX1YEdH6mj3hHBoOBMl0aWo3bxhi_5axX4VCN4FKTH3LMtg2HnZCvLfVNAwRUUoMx1y16csX7mkTKZfr4b7D9h5BbRry54gU3xIBFz71dVV4M2gWccmkg0efdTaNS0xx3zLKKB55apSf3ToaBXhNJ-1lne7qF9FoOhe388iA-v4Nd1MD_8C_C_U9Oiwi7WJwZS1cNmvugolGmnc5v3BfecQzwBoyflWuv90XBixDUBPNYrHHygfXTj4_ocXWnezpOENUPcrxlTDkIzodEfdIbHgxcDhT6JKOa590OqRrcWXYHP41NkBIkpLyFx0VUXsy_1mJ3tcUyEkxAYPL143b-DNuVR3-77ET5SYBIyT8cGxBF0IDS-60nGi-WhmCc0Vl3RnZFDllTpKe_hY3L0pkDuVNi-7xJPRyFw-r1N-2w_YXeXky-8qyFs0pICoH53x1Zh5wGkrTmLFAnGcIpkxmUN84_z47X0uQq8oaNBtBQBbKQ4LL_jzkeBwCtcv6o8_XDVJYJfvxuRXSJTEFMO5ien8UWhZd0Im1arB0OIFySHfTNcoCCFt3l5fQJwATfGkW3Xyp_-c-5n8hNLDc3Lru9hRxVjP-8Yg1_gcWBSfNW2Z4KU88UfK0in6URAW1Y3N7-Tf3CBlZeYue9Ts3M0pdDt1Dgv70GPhIFjqzztaowOgPVFI28c65HsQQOCUQ53tCYg1sFWLtnp5ZumUIt0RYVssh8gc-ZUCcufsgLhJwYDLASIfCySzWTQkBheDGgcn9NVh2AfHLlx_qJiIlI95rZ2MleBFU44W2Rb15OrxWVWz4BfU5JGJojL_uFj8gT3so6kO9ztIE1XgWC5XA6Zfh6zdQaD3Ewoo97y8fLD_MVUd9C08BS8O-lViHQjO5ZUpn1xzZBRZ6XL2_9AYwF3xJMsB4uv7XEcRfCQOewOmj_NcbWlka3_uJGdQsBQo-3HlBVSdvGmb9tZ1AvlozpkV5WPMa4jodZGu9rdMEHRhtYTHeXZ_56zDpUjkZhcnFm8D8PW5XGsf3sTTAMwUoAPlbWXMKbAxd9YSYRLF5Cpx3TX_uJf41OXZvj5bsIDIqYl465EO7xOc46n8jC6Dt6KTm6zpRZAL05L4j6jIyLSvcYHVSJ1oZW8akqXaCSzu7b21Q_TT3AhZwUaGP8AszGFlcNulZvczGE_1WATqx59ir5tAdf3k11daEPNYIOg0qylB3RgBDJUHnt61bjWQ6kkhPXP0jwQc2c2CroNUNMDBCTRvkdrXt9T3pVwUelsrlm-_dGFR6Kvjimgj3Hj0rHOxQR0cfRSB495uMarhmDKRgcF71PP8DBWN-nZ32Fho1S0kX-Uy058Ec-1MXC4jt-5vP8Ap9Xzn1zKApzwI-Ni7eanN4NXM4c-uTsWylQxGAd0py7j72JSleQ5-vzQ40CUFbOe1Ekef8MfBfJgKo8sWfw9MIfVo9yeqHMcUMADwC_1BwQyJ-AEPzs5FkhlM6mUlVYdYD0SMyT3-2rNkPOQZPZi7BeZng-Df_fSkahcvMi9_eFXJ_JOnZnj3qGwU9O8loa1s8czFX64GtfLFwGptNcEfvAOAaGWBolpPQLQISTVwT2WGB2-Iz7WQkHZ31z6Xg1BB0ejv67rUW0zkzIR7rUU5lAGACd-m5omiWQgaDYmVm8DzyMA5-XpS-4hMcMW5FOcxissjwJdVbGAOd6vqt-9AeZHsa9O6jOcQTBBQn_G1ZVydYIVrBvq1pmGIuvWVLZMwQ4BgoWUiZgvAnABdrayXvWYTT3C_2cjMIlOP_tNLcMkNKMdvlshXkJw0hGvlfS0Q0Zw5A9p2NWb19D5h7bEfYM_AjJUduubPWDg4TU0ocx0yEzPAc-ks7J6HX5prR8TNATzLl9bIpyg0SMg31T3VAIHoDccy7kEqaQyqddX5T2QPmBjBhUJW20AgMLkhgGeJIo9zgB_F5IymC3MWL5N03XUwmy-uVN94JExEt01NmdiByN1b1m6hZu34eplNpY9w94zASYUqhnMEWEzlCRjHda4To_Tr6RxUHtuXMpMLiCUdbO8f6nTbdJwUMOftfTmcwaA5_6r-RYJpAC5NQX1fKKPg8O0VAiKvbFgIOc00F63-S88YS0lgHJo_S_oji1i9hVjXN0JIe1QguMzv0Smh8BWcNXdahmWicQxK6UUZi6CH6IjZWVJyP0TczNU9xEudovN_jPdhpJSmU9Nm9_PAXXk4--e-_HMgeJQUg3F1bZwlYEV_iuox2nUYzu3NbctUp6TwpY1eDnM0RIjRCfzLYSpfh4yPUWT4MlejCrMXeMUBkOvjBlivtGCUDMctdaVcwYht5xoOSXbFTJ7tOSFP4EvA7JGZ1iojoFQ4RaFs930e43ectzUkqArHBxazn3xpYZCjZ16QM9QY2NSjZckdqLH0cXsaBo3mFXi6NbGxF3yHQOjp-X7OL3hEnFV1QNMpKlt3gAdl4ET6U8fa47toLb3I9-ceREuUYOC4f1VdteT9iOmX3sq5qmVIRpER-WMwX-wQxb1Oun9ojFxRxRAfYZ5rDwALVVRUglMrims_5GGxdMvzFmy_jLAAMJfttSWg_SDhG6J-2Sb57L7F4UlDwGvckFENXlL7wIBUxeX8c7XWR-uI89X0RIqzA_bbS2i9ERRj02oQo_y8kBSrlXUhIEl8OY_OXn3atVQyTUFtY-RjZGSx9cLCQ8SE8BldhO_B2lsD1NvNPExmAxsOP_PEkW3IX-dmWGMkNIwcs1l5oYCBhLVbRho57vWIZkE5-Rco11DY6UFSIrs0lOShXXj7nSKE",
                ImageLink = "https://www.kinopoisk.ru/BS30p1339/ef5848sTERG/KbQz5zYtTFyb17k4p4_4hJZKgzpNWRVMicfW8ufqmCcYhTZcVNn6nzaPzsaC-3MinJvVxQePJprwun1FqcoFFuilpuPyKxoInlH-7HHbORbTnJb_zU6BnZyQQaVr4VOtFMgu1N1U9cfxCUrG1GyltcyOQhIQ3HaeqzVszfNbj0ehcbhqcreN1J9POTplj3xBz0pPfNwWUEjZiRow5uKf6ZZCr9xVUH9OtcQH01dlJXOGCYLWXsw-0mQ0vEF6X03Pojr_obG2Qtndh_5-78D_xwkEwHyTEtFJGg3XvGpkmWRWAa-cVlV_SjJEBBldbydwA4yIl5QGsJhl_PxOuhUCR6QwsC_zso9WW8S3sygKdAnHw8Z5WlWWxBILXnXo5Ngk1c0hWJRTvwi-zwXWk-cne0SGxlDZDrFTb7T4TzOUCEIptzdqdn_CV5RJ-_mmzjOJy48A891S0c8RhV_x6S-ZZBUL55MTVLTHNQ4KUdzsbLtCSILaEUI_nWnzvIjyV8fKqjdzZ7I9CRfcRPxwrg5yxwQCQj3dXJhDn0NeeG2vUS-dwmGeE1X1jv1NwxBc72-7CMYEHd5Eup3l-jCH_J5EQK31faGxtAzc14W1MG9EcQ9GSoT-2xVaDxkDETNm5Vmg2YtuVt4Vfcd3zU2RHGIiuAgLBtTYT3kfrLn2g_4chwcqPDjiufKGFJJFu_Kvg7ILDYdGuVjTGIWaBtH3J-_SpBWCod1X1n6KOMGMnh2tJDdFCQmemEq21WG_tgN_08qGb_W-qvG0ztcXxPv-54i6gs2Cjncf0tfA0IXZeGgq26gaz6WTlNz-BXpFzB2ar21wSMEEE55KuR-h-3-E8h4JBqt3OiF2MgeWFoc0Mq1GP0lFj0T_FtReyBrJHrLtIxAgWY8sWV-Qe4Q5jAMT32au9o6JSpzRTfve7vQyj3NZx8sg8zBjM3yEnxtHNXktDbLPxYOKux-UFM0bxJ00JyQR51UN6NoRHLKF9gqC2dtnK7sKxcVaF8d5EG3y-I--HgVPKPj16Tb8BpiUSnc-ZYzwgIfEjjSXk59A38NVOu_jX2mXTKQd09d0zvkGBNTXJy0_CQgJlpFN99Gh873L99nHTej4-O559wQU3Yy5fqyCMEYORMo_lhmUSNkIGrgt5VEvl0tsERwedUA2xMNZFyOk_4JLzdyRin6Y4Dy1gLSVwcsqs_bhcz6JF12KunCnxzeGBstDdxAQHwgchti0bKRS51kF4JwV2TqHNQ3H2ButazaJh04TU0T5Ee9_t4B_00VCY7i5KPb_Rtubhn6zYg23y8eBijnbGFRFmcdUfGDo3ivVTCaUHh3-xXjGBleQKyw8ScvA09BHv5Btd_1FtZvJBiE0uuj4No1c1k4yPeXCuUnPgc-1251dg9bB3Dih453gFQZvXJ0d-wj5jgab0qDlfckBxhebBPRSZTs2wLXawA_qe_bqu3fHnBGFfzpoyvzIBgxGNdedGkJXj5gzqa_QoJiL5VNdn_zHeILCnF8vrbsDgIRWmAO60ie2PoQ03kVIY7x24rAzTdVVRjS04IT7yMFAyrtaktDP0Y1fMabuV-nfwuCd1pk8RTEJzdMfoyN9CA-G0hOGuNtnuXXG99IHAGRz9-G4883UFEk9PSaGeEdDy8L12lwVDJ-BWTmu7F8t1QSvGhJXMwzwjwmcFePmfUtMRV1WTbeZZnh0RT9RwQsqMbLmu7SOHVIBMftljb2GA4WPe9yaEo_TxF81IepS5R0ELhNb2XPAvsXCWd3sJDXFiUXT2Quxn2i-9UG5EoDNJbF5aXk1z5wVxfwxr0P1C4SMx_WV1Z2LHwxfuyIvWymaAiXWE568xDRABdRdZ2i6QgGNFZOO8VOsenDPtt2HTSJ8O2-4NkucXYZ8NCSGP4ZJwoI6kBkURRfHFbrh7xihkA2kHZMRt8C-SYwR3ihsN0UIDF4Uhj-eKrFyR3yXRY9gPX0l-z1CUBdNvvBvjjKEBI3DdhSd0IrcAZhyKeMR5lhE45Rb1L8EsIrLmFYkJb0GiQJWWsw63i16foRy1U5CLTs2pTB9ApFXgjrwYIW4A8BIAHIbHNWNn03W-66n2CnRBaAQ3tTzATnPghfdJWQ8hgZEnNuFe9fpfnhGvlNNyu_z8uh7fAXRkom9eacAuQOIgAn1H9FVj5JEGLOgoxBmnAtplRLVvIB0RwIRUC_gewHDhhVRirMeJHk3BHHexkfhsbkh9LODVFXKuTunQHKGD8UD9hXVEYkcDl96ru1YKRlGKVif0DnGt01LE9piJvsFjkpXnIc2G6K3_Q52Gk4JrH0yKfm6CtcWSDO4bUJ5TMAFgDNcU9zK3MbQfSzvWanfDGke0pi7hjDOD9bfayRzScCFWJlEM9ApvrbM-lLNz2X0_25wNAURFIU8cy3FPM4Ng0o2kJUfxRvGXXvpqNnol0whmZaX-YL3ScKWGK_jes2AxhsRS_ta5j6xT_ZUBI8i8jsgO72Cm5WFd_lgDH1ODAcP9pwZEYuZT9R1riIS7dJMLt1e3LdEtoqD3prq6jvGiYySEoo4ESk_ssmyUQcGKLP7KLv3RJuRDTJ16cp6ysGBS31XllaMWIYUdSJrH-6QAaZUW1V7jLbNBdQUqie6zMiB0NDPe1qpPnnMvh_ID2S_Pir6swleFEU2eK5OfUlHTIh0HRKSS5EI2DnhL9jtn8vsUNwRtgZ5T8GXE-8mtkDIytXcC_AZrrZ5D7Vez49qejaisnfJldeEdvohD_BHT8ID-pQW0kERgB_ypycRJ9BOo1veHrVFcUaKlh1nbDaAQYjbGsa0m2D-9oe_X88BaP39pfq6A5PdBnE94Mjwjk2BxHaUXtkE3AlZMK1o1exYhilZnBz1zv4IhR_UbOx2ygxDXJMB9FquezQGM99Bymlyc-5weMReHsUx-WzFeA-NAEi2XFTVi1TEEbTpK5Hhncbu0NtQPo21zQ5W2mNjd8tHw1NSRDvXYA",
            }
        };

        var sessions = new List<Session>
        {
            new()
            {
                Movie = movies[1],
                Hall = 0,
                Price = 1000,
                IncreasedPrice = 99999,
                Start = DateTimeOffset.UtcNow.AddHours(4).AddHours(3)
            },
            new()
            {
                Movie = movies[1],
                Hall = 0,
                Price = 1000,
                IncreasedPrice = 99999,
                Start = DateTimeOffset.UtcNow.AddHours(4).AddDays(1)
            },
            new()
            {
                Movie = movies[0],
                Hall = 1,
                Price = 1000,
                IncreasedPrice = 99999,
                Start = DateTimeOffset.UtcNow.AddHours(4).AddHours(2)
            },
            new()
            {
                Movie = movies[0],
                Hall = 1,
                Price = 20,
                IncreasedPrice = 500,
                Start = DateTimeOffset.UtcNow.AddHours(4).AddDays(1).AddHours(2)
            },
            new()
            {
                Movie = movies[1],
                Hall = 1,
                Price = 1,
                IncreasedPrice = 99999,
                Start = DateTimeOffset.UtcNow.AddHours(4).AddDays(15)
            }
        };

        var reservations = new List<Reservation>
        {
            new()
            {
                User = users[0],
                Session = sessions[0],
                Seat = seats[0]
            },
            new()
            {
                User = users[0],
                Session = sessions[0],
                Seat = seats[1]
            },
            new()
            {
                User = users[1],
                Session = sessions[0],
                Seat = seats[2]
            }
        };

        await dbContext.Users.AddRangeAsync(users);
        await dbContext.Seats.AddRangeAsync(seats);
        await dbContext.Countries.AddRangeAsync(countries);
        await dbContext.Genres.AddRangeAsync(genres);
        await dbContext.Movies.AddRangeAsync(movies);
        await dbContext.Sessions.AddRangeAsync(sessions);
        await dbContext.Reservations.AddRangeAsync(reservations);

        await dbContext.SaveChangesAsync();
    }
}