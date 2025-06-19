using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace MelomanGuide
{
    public partial class Form1 : Form
    {
        private string dbPath = "songs.db";
        private string savePath = Path.Combine(Application.StartupPath, "loved_songs.txt");
        private string connStr => $"Data Source={dbPath};Version=3;";

        private List<Song> currentSearchResults = new List<Song>();
        private List<Song> lovedSongs = new List<Song>();

        private bool suppressInfoPopup = false; // <- Правильне місце

        private void RefreshLovedSongsList()
        {
            lstLovedSongs.Items.Clear();

            for (int i = 0; i < lovedSongs.Count; i++)
            {
                // Додаємо текст зі сформованою нумерацією
                lstLovedSongs.Items.Add($"{i + 1}. {lovedSongs[i]}");
            }
        }



        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_FormClosing;
            this.Load += Form1_Load;
            lstLovedSongs.Click += lstLovedSongs_Click;

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

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadLovedSongs();
        }

        private void lstLovedSongs_Click(object sender, EventArgs e)
        {
            if (suppressInfoPopup)
            {
                suppressInfoPopup = false;
                return;
            }

            int index = lstLovedSongs.SelectedIndex;
            if (index >= 0 && index < lovedSongs.Count)
            {
                var song = lovedSongs[index];
                MessageBox.Show($"Назва: {song.Title}\nВиконавець: {song.Artist}\nАльбом: {song.Album}",
                    "Інформація про улюблену пісню", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadLovedSongs()
        {
            lovedSongs.Clear();
            lstLovedSongs.Items.Clear();

            if (File.Exists(savePath))
            {
                string[] lines = File.ReadAllLines(savePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        lovedSongs.Add(new Song { Title = parts[0], Artist = parts[1], Album = parts[2] });
                    }
                }
                RefreshLovedSongsList();
            }
        }

        private void SaveLovedSongs()
        {
            var lines = lovedSongs.Select(s => $"{s.Title}|{s.Artist}|{s.Album}");
            File.WriteAllLines(savePath, lines);
        }

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
                lstSongs.Items.Add($"{i + 1}. {songs[i]}");
            }

            if (songs.Count == 0)
            {
                MessageBox.Show("Нічого не знайдено.");
            }
        }

        private void btnAddToLoved_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtIndexOfSong.Text.Trim(), out int index))
            {
                if (index >= 1 && index <= currentSearchResults.Count)
                {
                    var song = currentSearchResults[index - 1];
                    if (!lovedSongs.Any(s => s.Title == song.Title && s.Artist == song.Artist && s.Album == song.Album))
                    {
                        lovedSongs.Add(song);
                        RefreshLovedSongsList();
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtIndexOfLoved.Text.Trim(), out int index))
            {
                if (index >= 1 && index <= lovedSongs.Count)
                {
                    lovedSongs.RemoveAt(index - 1);
                    RefreshLovedSongsList();
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

        private void downloadToExcel_Click(object sender, EventArgs e)
        {
            if (lovedSongs.Count == 0)
            {
                MessageBox.Show("Список улюблених пісень порожній.");
                return;
            }

            var excelApp = new Excel.Application();
            excelApp.Workbooks.Add();
            Excel._Worksheet workSheet = (Excel._Worksheet)excelApp.ActiveSheet;

            workSheet.Cells[1, "A"] = "Назва";
            workSheet.Cells[1, "B"] = "Виконавець";
            workSheet.Cells[1, "C"] = "Альбом";

            for (int i = 0; i < lovedSongs.Count; i++)
            {
                workSheet.Cells[i + 2, "A"] = lovedSongs[i].Title;
                workSheet.Cells[i + 2, "B"] = lovedSongs[i].Artist;
                workSheet.Cells[i + 2, "C"] = lovedSongs[i].Album;
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveLovedSongs();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtIndexOfLoved.Text.Trim(), out int userIndex))
            {
                int index = userIndex - 1; // перетворюємо в 0-based

                if (index > 0 && index < lovedSongs.Count)
                {
                    var temp = lovedSongs[index];
                    lovedSongs[index] = lovedSongs[index - 1];
                    lovedSongs[index - 1] = temp;

                    RefreshLovedSongsList();
                    lstLovedSongs.SelectedIndex = index - 1;
                    txtIndexOfLoved.Text = (index).ToString(); // оновити текстове поле
                    SaveLovedSongs();
                }
                else
                {
                    MessageBox.Show("Неможливо підняти цю пісню вище.");
                }
            }
            else
            {
                MessageBox.Show("Введіть коректний номер пісні.");
            }
        }


        private void btnDown_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtIndexOfLoved.Text.Trim(), out int userIndex))
            {
                int index = userIndex - 1;

                if (index >= 0 && index < lovedSongs.Count - 1)
                {
                    var temp = lovedSongs[index];
                    lovedSongs[index] = lovedSongs[index + 1];
                    lovedSongs[index + 1] = temp;

                    RefreshLovedSongsList();
                    lstLovedSongs.SelectedIndex = index + 1;
                    txtIndexOfLoved.Text = (index + 2).ToString();
                    SaveLovedSongs();
                }
                else
                {
                    MessageBox.Show("Неможливо опустити цю пісню нижче.");
                }
            }
            else
            {
                MessageBox.Show("Введіть коректний номер пісні.");
            }
        }

    }
}
    
        