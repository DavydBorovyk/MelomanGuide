using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using System.Linq;

namespace MelomanGuide
{
  
    /// Основна форма застосунку для пошуку музики та управління улюбленими піснями.
    public partial class Form1 : Form
    {
        /// Шлях до локальної бази даних.
        private string dbPath = "songs.db";

        /// Шлях до файлу збереження улюблених пісень.
        string savePath = Path.Combine(Application.StartupPath, "loved_songs.txt");

        /// Зберігає улюблені пісні у файл.
        private void SaveLovedSongs()
        {
            var cleanList = lstLovedSongs.Items.Cast<string>()
                .Select(item =>
                {
                    int dotIndex = item.IndexOf('.');
                    return dotIndex != -1 ? item.Substring(dotIndex + 1).Trim() : item;
                });
            File.WriteAllLines(savePath, cleanList);
        }

        /// Завантажує улюблені пісні з файлу.
        private void LoadLovedSongs()
        {
            if (File.Exists(savePath))
            {
                string[] lines = File.ReadAllLines(savePath);
                lstLovedSongs.Items.Clear();
                for (int i = 0; i < lines.Length; i++)
                {
                    lstLovedSongs.Items.Add($"{i + 1}. {lines[i]}");
                }
            }
        }

        /// Рядок з'єднання з базою даних SQLite.
        private string connStr => $"Data Source={dbPath};Version=3;";

        /// Поточні результати пошуку пісень.
        private List<Song> currentSearchResults = new List<Song>();

        /// Конструктор форми.
        public Form1()
        {
            InitializeComponent();

            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);

            this.BackColor = Color.FromArgb(40, 40, 40);
            this.Font = new Font("Segoe UI", 10);

            Search.FlatStyle = FlatStyle.Flat;
            Search.FlatAppearance.BorderSize = 0;
            Search.BackColor = Color.MediumSlateBlue;
            Search.ForeColor = Color.White;

            lstLovedSongs.BackColor = Color.FromArgb(60, 60, 60);
            lstLovedSongs.ForeColor = Color.White;

            lstSongs.BackColor = Color.FromArgb(60, 60, 60);
            lstSongs.ForeColor = Color.White;

            txtIndexOfSong.BackColor = Color.FromArgb(80, 80, 80);
            txtIndexOfSong.ForeColor = Color.White;
        }

        /// Обробник події завантаження форми.
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadLovedSongs();
        }

        /// Створює базу даних та заповнює її, якщо вона порожня.
        private void InitializeDatabase()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                string createTableSql = @"CREATE TABLE IF NOT EXISTS Songs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Artist TEXT NOT NULL,
                    Album TEXT NOT NULL)";

                using (var cmd = new SQLiteCommand(createTableSql, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                string countSql = "SELECT COUNT(*) FROM Songs";
                using (var cmd = new SQLiteCommand(countSql, conn))
                {
                    long count = (long)cmd.ExecuteScalar();
                    if (count == 0)
                    {
                        string insertSql = @"INSERT INTO Songs (Title, Artist, Album) VALUES 
                            ('Imagine', 'John Lennon', 'Imagine'),
                            ('Bohemian Rhapsody', 'Queen', 'A Night at the Opera'),
                            ('Nothing Else Matters', 'Metallica', 'Metallica'),
                            ('Smells Like Teen Spirit', 'Nirvana', 'Nevermind'),
                            ('Shape of You', 'Ed Sheeran', 'Divide')";

                        using (var insertCmd = new SQLiteCommand(insertSql, conn))
                        {
                            insertCmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Базу даних створено та заповнено!");
                    }
                }
            }
        }

        /// Виконує локальний пошук пісень за ключовим словом.
        private List<Song> SearchSongsLocal(string query)
        {
            var result = new List<Song>();
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT * FROM Songs WHERE Title LIKE @query OR Artist LIKE @query OR Album LIKE @query";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@query", $"%{query}%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Song
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = reader["Title"].ToString(),
                                Artist = reader["Artist"].ToString(),
                                Album = reader["Album"].ToString()
                            });
                        }
                    }
                }
            }
            return result;
        }

        /// Виконує онлайн-пошук пісень через Deezer API.
        private async Task<List<Song>> SearchSongsOnline(string keyword)
        {
            var results = new List<Song>();
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://api.deezer.com/search?q={Uri.EscapeDataString(keyword)}";
                try
                {
                    string json = await client.GetStringAsync(url);
                    JObject obj = JObject.Parse(json);
                    foreach (var item in obj["data"])
                    {
                        results.Add(new Song
                        {
                            Title = item["title"].ToString(),
                            Artist = item["artist"]["name"].ToString(),
                            Album = item["album"]["title"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка під час онлайн-пошуку: {ex.Message}");
                }
            }
            return results;
        }

        /// Обробник кнопки "Шукати". Виконує пошук пісень та оновлює інтерфейс.
        private async void Search_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            lstSongs.Items.Clear();

            var songs = SearchSongsLocal(keyword);

            if (songs.Count == 0)
            {
                songs = await SearchSongsOnline(keyword);
            }

            currentSearchResults = songs;

            for (int i = 0; i < songs.Count; i++)
            {
                var song = songs[i];
                lstSongs.Items.Add($"{i + 1}. {song}");
            }

            if (songs.Count == 0)
            {
                MessageBox.Show("Нічого не знайдено.");
            }
        }

        /// Показує інформацію про вибрану пісню у MessageBox.
        private void lstSongs_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lstSongs.SelectedIndex;
            if (index >= 0 && index < currentSearchResults.Count)
            {
                var selected = currentSearchResults[index];
                MessageBox.Show($"Назва: {selected.Title}\nВиконавець: {selected.Artist}\nАльбом: {selected.Album}",
                    "Інформація про пісню", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// Додає вибрану пісню до списку улюблених, якщо її там ще немає.
        private void btnAddToLoved_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtIndexOfSong.Text.Trim(), out int index))
            {
                if (index >= 1 && index <= currentSearchResults.Count)
                {
                    var song = currentSearchResults[index - 1];

                    bool alreadyExists = false;
                    foreach (var item in lstLovedSongs.Items)
                    {
                        if (item.ToString().Contains(song.ToString()))
                        {
                            alreadyExists = true;
                            break;
                        }
                    }

                    if (!alreadyExists)
                    {
                        string songText = $"{lstLovedSongs.Items.Count + 1}. {song}";
                        lstLovedSongs.Items.Add(songText);
                        MessageBox.Show("Пісню додано до улюблених!");
                    }
                    else
                    {
                        MessageBox.Show("Ця пісня вже є в списку улюблених.");
                    }
                }
                else
                {
                    MessageBox.Show("Неправильний номер пісні.");
                }
            }
            else
            {
                MessageBox.Show("Введіть коректний номер.");
            }
        }

        /// Видаляє пісню зі списку улюблених за індексом та оновлює номери.
        private void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtIndexOfLoved.Text.Trim(), out int index))
            {
                if (index >= 1 && index <= lstLovedSongs.Items.Count)
                {
                    lstLovedSongs.Items.RemoveAt(index - 1);

                    for (int i = 0; i < lstLovedSongs.Items.Count; i++)
                    {
                        string itemText = lstLovedSongs.Items[i].ToString();
                        int dotIndex = itemText.IndexOf('.');
                        if (dotIndex != -1)
                        {
                            itemText = itemText.Substring(dotIndex + 1).Trim();
                        }
                        lstLovedSongs.Items[i] = $"{i + 1}. {itemText}";
                    }

                    MessageBox.Show("Пісню видалено зі списку улюблених.");
                }
                else
                {
                    MessageBox.Show("Невірний номер пісні.");
                }
            }
            else
            {
                MessageBox.Show("Введіть коректний номер.");
            }
        }

        private void txtIndexOfSong_TextChanged(object sender, EventArgs e) { }

        /// Обробка зміни тексту індексу пісні в списку улюблених.
        private void txtIndexOfLoved_TextChanged(object sender, EventArgs e)
        {
            
        }

        /// Завантажує улюблені пісні у файл Excel.
        private void downloadToExcel_Click(object sender, EventArgs e)
        {
            if (lstLovedSongs.Items.Count == 0)
            {
                MessageBox.Show("Список улюблених пісень порожній.");
                return;
            }

            var excelApp = new Excel.Application();
            excelApp.Workbooks.Add();
            Excel._Worksheet workSheet = (Excel._Worksheet)excelApp.ActiveSheet;

            workSheet.Cells[1, "A"] = "Улюблені пісні";

            for (int i = 0; i < lstLovedSongs.Items.Count; i++)
            {
                workSheet.Cells[i + 2, "A"] = lstLovedSongs.Items[i].ToString();
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Excel файли (*.xlsx)|*.xlsx",
                Title = "Зберегти улюблені пісні"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    workSheet.SaveAs(saveDialog.FileName);
                    MessageBox.Show("Файл успішно збережено!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка при збереженні: " + ex.Message);
                }
                finally
                {
                    excelApp.Quit();
                }
            }
            else
            {
                excelApp.Quit();
            }
        }

        /// Обробник події закриття форми. Зберігає улюблені пісні.
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveLovedSongs();
        }
    }
}
