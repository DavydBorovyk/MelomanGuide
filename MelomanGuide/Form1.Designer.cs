namespace MelomanGuide
{
    /// Частковий клас, який містить автоматично згенерований код дизайнера форми.
    partial class Form1
    {
        /// Змінна, необхідна для відстеження компонентів форми.
        private System.ComponentModel.IContainer components = null;

        /// Очищає ресурси, що використовуються компонентами.
        /// <param name="disposing">true, якщо керовані ресурси слід утилізувати; інакше — false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Ініціалізація компонентів форми.
        /// </summary>
        private void InitializeComponent()
        {
            // Текстове поле для введення запиту пошуку
            this.txtSearch = new System.Windows.Forms.TextBox();

            // Кнопка для виконання пошуку пісень
            this.Search = new System.Windows.Forms.Button();

            // Список знайдених пісень
            this.lstSongs = new System.Windows.Forms.ListBox();

            // Кнопка для додавання обраної пісні до улюблених
            this.btnAddToLoved = new System.Windows.Forms.Button();

            // Поле для введення індексу пісні зі списку знайдених
            this.txtIndexOfSong = new System.Windows.Forms.TextBox();

            // Підпис для поля індексу пісні
            this.label1 = new System.Windows.Forms.Label();

            // Список улюблених пісень
            this.lstLovedSongs = new System.Windows.Forms.ListBox();

            // Підпис над списком улюблених пісень
            this.label2 = new System.Windows.Forms.Label();

            // Кнопка для видалення пісні з улюблених
            this.btnDelete = new System.Windows.Forms.Button();

            // Поле для введення індексу пісні, яку слід видалити з улюблених
            this.txtIndexOfLoved = new System.Windows.Forms.TextBox();

            // Підпис для поля індексу улюбленої пісні
            this.label3 = new System.Windows.Forms.Label();

            // Кнопка для вивантаження улюблених пісень в Excel
            this.downloadToExcel = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(434, 178);
            this.txtSearch.Multiline = true;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(281, 58);
            this.txtSearch.TabIndex = 0;

            // Search
            this.Search.Location = new System.Drawing.Point(733, 178);
            this.Search.Name = "Search";
            this.Search.Size = new System.Drawing.Size(121, 41);
            this.Search.TabIndex = 1;
            this.Search.Text = "Шукати...";
            this.Search.UseVisualStyleBackColor = true;
            this.Search.Click += new System.EventHandler(this.Search_Click);

            // lstSongs
            this.lstSongs.FormattingEnabled = true;
            this.lstSongs.ItemHeight = 20;
            this.lstSongs.Location = new System.Drawing.Point(325, 449);
            this.lstSongs.Name = "lstSongs";
            this.lstSongs.Size = new System.Drawing.Size(513, 164);
            this.lstSongs.TabIndex = 2;
            this.lstSongs.SelectedIndexChanged += new System.EventHandler(this.lstSongs_SelectedIndexChanged);

            // btnAddToLoved
            this.btnAddToLoved.Location = new System.Drawing.Point(868, 449);
            this.btnAddToLoved.Name = "btnAddToLoved";
            this.btnAddToLoved.Size = new System.Drawing.Size(110, 57);
            this.btnAddToLoved.TabIndex = 3;
            this.btnAddToLoved.Text = "Додати до улюблених";
            this.btnAddToLoved.UseVisualStyleBackColor = true;
            this.btnAddToLoved.Click += new System.EventHandler(this.btnAddToLoved_Click);

            // txtIndexOfSong
            this.txtIndexOfSong.Location = new System.Drawing.Point(1012, 449);
            this.txtIndexOfSong.Name = "txtIndexOfSong";
            this.txtIndexOfSong.Size = new System.Drawing.Size(100, 26);
            this.txtIndexOfSong.TabIndex = 4;
            this.txtIndexOfSong.TextChanged += new System.EventHandler(this.txtIndexOfSong_TextChanged);

            // label1
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Info;
            this.label1.Location = new System.Drawing.Point(990, 426);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Введіть номер пісні";

            // lstLovedSongs
            this.lstLovedSongs.FormattingEnabled = true;
            this.lstLovedSongs.ItemHeight = 20;
            this.lstLovedSongs.Location = new System.Drawing.Point(49, 87);
            this.lstLovedSongs.Name = "lstLovedSongs";
            this.lstLovedSongs.Size = new System.Drawing.Size(210, 544);
            this.lstLovedSongs.TabIndex = 6;

            // label2
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Info;
            this.label2.Location = new System.Drawing.Point(67, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Вподобані пісні";

            // btnDelete
            this.btnDelete.Location = new System.Drawing.Point(287, 175);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(129, 33);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "Видалити";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.button1_Click);

            // txtIndexOfLoved
            this.txtIndexOfLoved.Location = new System.Drawing.Point(300, 123);
            this.txtIndexOfLoved.Name = "txtIndexOfLoved";
            this.txtIndexOfLoved.Size = new System.Drawing.Size(100, 26);
            this.txtIndexOfLoved.TabIndex = 9;
            this.txtIndexOfLoved.TextChanged += new System.EventHandler(this.txtIndexOfLoved_TextChanged);

            // label3
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Info;
            this.label3.Location = new System.Drawing.Point(272, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "Введіть номер пісні";

            // downloadToExcel
            this.downloadToExcel.Location = new System.Drawing.Point(287, 231);
            this.downloadToExcel.Name = "downloadToExcel";
            this.downloadToExcel.Size = new System.Drawing.Size(129, 32);
            this.downloadToExcel.TabIndex = 11;
            this.downloadToExcel.Text = "Вивантажити";
            this.downloadToExcel.UseVisualStyleBackColor = true;
            this.downloadToExcel.Click += new System.EventHandler(this.downloadToExcel_Click);

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1228, 687);
            this.Controls.Add(this.downloadToExcel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtIndexOfLoved);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstLovedSongs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtIndexOfSong);
            this.Controls.Add(this.btnAddToLoved);
            this.Controls.Add(this.lstSongs);
            this.Controls.Add(this.Search);
            this.Controls.Add(this.txtSearch);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

      
        /// Поле для введення пошукового запиту.
        private System.Windows.Forms.TextBox txtSearch;

        /// Кнопка для запуску пошуку.
        private System.Windows.Forms.Button Search;

        /// Список знайдених пісень.
        private System.Windows.Forms.ListBox lstSongs;

        /// Кнопка додавання пісні до улюблених.
        private System.Windows.Forms.Button btnAddToLoved;

        /// Поле введення індексу пісні для додавання.
        private System.Windows.Forms.TextBox txtIndexOfSong;

        /// Підпис до поля індексу пісні для додавання.
        private System.Windows.Forms.Label label1;

        /// Список улюблених пісень.
        private System.Windows.Forms.ListBox lstLovedSongs;

        /// Підпис до списку улюблених пісень.
        private System.Windows.Forms.Label label2;

        /// Кнопка видалення пісні з улюблених.
        private System.Windows.Forms.Button btnDelete;

        /// Поле введення індексу пісні для видалення.
        private System.Windows.Forms.TextBox txtIndexOfLoved;

        /// Підпис до поля індексу пісні для видалення.
        private System.Windows.Forms.Label label3;

        
        /// Кнопка для експорту улюблених пісень у Excel.
        private System.Windows.Forms.Button downloadToExcel;
    }
}

