using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace CK3_Save_Cleaner
{
    public partial class Form1 : Form
    {
        private Button btnOpenFile;
        private Button btnProcess;
        private Button btnHelp;
        private Button btnInstructions;
        private Button btnAbout;
        private Button btnLanguageRU;
        private Button btnLanguageEN;
        private ProgressBar progressBar;
        private Label statusLabel;

        // Чекбоксы
        private CheckBox chkCreateBackup;
        private CheckBox chkDescription;
        private CheckBox chkAcceptanceChanges;
        private CheckBox chkHouseUnityChanges;
        private CheckBox chkCharacterMemoryManager;
        private CheckBox chkHistory;
        private CheckBox chkHistorical;
        private CheckBox chkDeadUnprunable;
        private CheckBox chkDeadPrunable;
        private CheckBox chkFamilyDataChildren;
        private CheckBox chkDynnDynasty;
        private CheckBox chkKeyOnly;

        // Подсказки
        private ToolTip toolTip;

        // Пояснения цветов
        private Label lblColorInfo;

        // Локализация
        private enum Language { Russian, English }
        private Language currentLanguage = Language.Russian;

        // Словари для локализации
        private Dictionary<string, string> russianTexts = new Dictionary<string, string>();
        private Dictionary<string, string> englishTexts = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();
            InitializeLocalization();
            InitializeCustomComponents();
        }

        private void InitializeLocalization()
        {
            // Русские тексты
            russianTexts["FormTitle"] = "Очистка сохранений Crusader Kings 3";
            russianTexts["SelectFile"] = "Выбрать файл";
            russianTexts["Process"] = "Обработать";
            russianTexts["Help"] = "Справка";
            russianTexts["Instructions"] = "Инструкции";
            russianTexts["About"] = "О программе";
            russianTexts["StatusReady"] = "Готово";
            russianTexts["StatusProcessing"] = "Обработка...";
            russianTexts["StatusFileSelected"] = "Выбран файл: {0}";

            russianTexts["Backup"] = "Создать бэкап перед обработкой";
            russianTexts["BackupTooltip"] = "Создает резервную копию файла с расширением .backup перед внесением изменений. Рекомендуется всегда оставлять включенным.";
            russianTexts["Description"] = "Очистить все текстовые описания (description)";
            russianTexts["DescriptionTooltip"] = "Заменяет description=\"любой текст\" на description=\"\". Безопасная операция.";
            russianTexts["AcceptanceChanges"] = "Очистить историю изменений терпимости культур";
            russianTexts["AcceptanceChangesTooltip"] = "Очищает блоки acceptance_changes культур. Безопасная операция, не влияет на игровой процесс.";
            russianTexts["HouseUnityChanges"] = "Очистить историю изменений единства домов";
            russianTexts["HouseUnityChangesTooltip"] = "Очищает блоки changes внутри house_unity. Безопасная операция, не влияет на игровой процесс.";
            russianTexts["CharacterMemoryManager"] = "Очистить воспоминания персонажей";
            russianTexts["CharacterMemoryManagerTooltip"] = "Очищает блок character_memory_manager. Безопасная операция, не влияет на игровой процесс.";
            russianTexts["History"] = "Очистить историю титулов, артефактов и ситуаций";
            russianTexts["HistoryTooltip"] = "Очищает блоки history. Довольно безопасная операция, но может удалить некоторую историю мира.";
            russianTexts["Historical"] = "Очистить историю династий";
            russianTexts["HistoricalTooltip"] = "Очищает блоки historical внутри династий. Довременно безопасная операция.";
            russianTexts["DeadUnprunable"] = "Очистить мертвых персонажей, часть 1";
            russianTexts["DeadUnprunableTooltip"] = "Очищает блок dead_unprunable. Применяйте с осторожностью - может вызвать нестабильность игры.";
            russianTexts["DeadPrunable"] = "Очистить мертвых персонажей, часть 2";
            russianTexts["DeadPrunableTooltip"] = "Очищает блок dead_prunable. Применяйте с осторожностью - может вызвать нестабильность игры.";
            russianTexts["FamilyDataChildren"] = "Очистить родственные связи живых персонажей с умершими детьми";
            russianTexts["FamilyDataChildrenTooltip"] = "Удаляет умерших детей из family_data персонажей. Применяйте с осторожностью - может вызвать проблемы с семейными древами. Крайне рекомендуется использовать вместе с очисткой мертвых персонажей, так как это очищает связи с несуществующими объектами и позволяет решить часть проблем с вылетом игры.";
            russianTexts["DynnDynasty"] = "Очистить мертвые дома";
            russianTexts["DynnDynastyTooltip"] = "Удаляет дома без главы дома, что логически означает его вымирание. Применяйте с осторожностью - может вызвать краши игры.";
            russianTexts["KeyOnly"] = "Очистить мертвые династии";
            russianTexts["KeyOnlyTooltip"] = "Удаляет династии без главы династии, что логически означает её вымирание. Применяйте с осторожностью - может вызвать краши игры. Не рекомендуется применять, если вы не очистили мертвые дома.";

            russianTexts["ColorInfo"] = "Справка:\n" +
                               "• Настройки, отмеченные зеленым, были проверены и могут быть безопасны. Чем ниже настройка в списке, тем более рискованны настройки.\n" +
                               "• Оранжевые настройки могут нести в себе серьёзные риски в виде отложенных крашей каждые 1-10 игровых лет или иные проблемы. \n" +
                               "• Всегда будьте бдительны и сохраняйте игру не реже 1 раза в игровой год. Если вы столкнулись с крашем, не паникуйте, загрузите последнее сохранение. Это может быть просто проблема игры, которая больше не повторится. Если же нет, используйте более безопасные настройки в следующий раз. Самые безопасные - первые три зеленые настройки.";

            // Результаты операций
            russianTexts["OperationCompleted"] = "Операции завершены:";
            russianTexts["BackupCreated"] = "Создан бэкап файла";
            russianTexts["DescriptionsCleared"] = "Текстовые описания очищены";
            russianTexts["AcceptanceChangesCleared"] = "История изменений терпимости культур очищена";
            russianTexts["HouseUnityChangesCleared"] = "История изменений единства домов очищена";
            russianTexts["CharacterMemoryManagerCleared"] = "Воспоминания персонажей очищены";
            russianTexts["HistoryCleared"] = "История титулов и артефактов очищена";
            russianTexts["HistoricalCleared"] = "История династий очищена";
            russianTexts["DeadUnprunableCleared"] = "Мёртвые персонажи (часть 1) очищены";
            russianTexts["DeadPrunableCleared"] = "Мёртвые персонажи (часть 2) очищены";
            russianTexts["FamilyDataChildrenFiltered"] = "Родственные связи с умершими детьми очищены";
            russianTexts["DynnDynastyCleared"] = "Мёртвые дома очищены";
            russianTexts["KeyOnlyCleared"] = "Мёртвые династии очищены";
            russianTexts["Done"] = "Готово";

            // Английские тексты
            englishTexts["FormTitle"] = "Crusader Kings 3 Save Cleaner";
            englishTexts["SelectFile"] = "Select file";
            englishTexts["Process"] = "Process";
            englishTexts["Help"] = "Help";
            englishTexts["Instructions"] = "Instructions";
            englishTexts["About"] = "About";
            englishTexts["StatusReady"] = "Ready";
            englishTexts["StatusProcessing"] = "Processing...";
            englishTexts["StatusFileSelected"] = "Selected file: {0}";

            englishTexts["Backup"] = "Create backup before processing";
            englishTexts["BackupTooltip"] = "Creates a backup copy of the file with .backup extension before making changes. Recommended to always keep enabled.";
            englishTexts["Description"] = "Clear all text descriptions (description)";
            englishTexts["DescriptionTooltip"] = "Replaces description=\"any text\" with description=\"\". Safe operation.";
            englishTexts["AcceptanceChanges"] = "Clear cultural acceptance changes history";
            englishTexts["AcceptanceChangesTooltip"] = "Clears acceptance_changes blocks of cultures. Safe operation, does not affect gameplay.";
            englishTexts["HouseUnityChanges"] = "Clear house unity changes history";
            englishTexts["HouseUnityChangesTooltip"] = "Clears changes blocks inside house_unity. Safe operation, does not affect gameplay.";
            englishTexts["CharacterMemoryManager"] = "Clear character memories";
            englishTexts["CharacterMemoryManagerTooltip"] = "Clears character_memory_manager block. Safe operation, does not affect gameplay.";
            englishTexts["History"] = "Clear title, artifact and situation history";
            englishTexts["HistoryTooltip"] = "Clears history blocks. Fairly safe operation, but may remove some world history.";
            englishTexts["Historical"] = "Clear dynasty history";
            englishTexts["HistoricalTooltip"] = "Clears historical blocks inside dynasties. Fairly safe operation.";
            englishTexts["DeadUnprunable"] = "Clear dead characters, part 1";
            englishTexts["DeadUnprunableTooltip"] = "Clears dead_unprunable block. Use with caution - may cause game instability.";
            englishTexts["DeadPrunable"] = "Clear dead characters, part 2";
            englishTexts["DeadPrunableTooltip"] = "Clears dead_prunable block. Use with caution - may cause game instability.";
            englishTexts["FamilyDataChildren"] = "Clear family connections of living characters with dead children";
            englishTexts["FamilyDataChildrenTooltip"] = "Removes dead children from characters' family_data. Use with caution - may cause problems with family trees. Highly recommended to use together with dead character cleanup, as it clears connections to non-existent objects and helps solve some crash problems.";
            englishTexts["DynnDynasty"] = "Clear dead houses";
            englishTexts["DynnDynastyTooltip"] = "Removes houses without head of house, which logically means its extinction. Use with caution - may cause game crashes.";
            englishTexts["KeyOnly"] = "Clear dead dynasties";
            englishTexts["KeyOnlyTooltip"] = "Removes dynasties without dynasty head, which logically means its extinction. Use with caution - may cause game crashes. Not recommended to use if you haven't cleared dead houses.";

            englishTexts["ColorInfo"] = "Information:\n" +
                               "• Settings marked in green have been tested and may be safe. The lower the setting in the list, the more risky the settings are.\n" +
                               "• Orange settings may carry serious risks in the form of delayed crashes every 1-10 game years or other problems. \n" +
                               "• Always be vigilant and save the game at least once per game year. If you encounter a crash, don't panic, load the last save. It may just be a game problem that won't happen again. If not, use safer settings next time. The safest are the first three green settings.";

            // Результаты операций
            englishTexts["OperationCompleted"] = "Operations completed:";
            englishTexts["BackupCreated"] = "Backup file created";
            englishTexts["DescriptionsCleared"] = "Text descriptions cleared";
            englishTexts["AcceptanceChangesCleared"] = "Cultural acceptance changes history cleared";
            englishTexts["HouseUnityChangesCleared"] = "House unity changes history cleared";
            englishTexts["CharacterMemoryManagerCleared"] = "Character memories cleared";
            englishTexts["HistoryCleared"] = "Title and artifact history cleared";
            englishTexts["HistoricalCleared"] = "Dynasty history cleared";
            englishTexts["DeadUnprunableCleared"] = "Dead characters (part 1) cleared";
            englishTexts["DeadPrunableCleared"] = "Dead characters (part 2) cleared";
            englishTexts["FamilyDataChildrenFiltered"] = "Family connections with dead children cleared";
            englishTexts["DynnDynastyCleared"] = "Dead houses cleared";
            englishTexts["KeyOnlyCleared"] = "Dead dynasties cleared";
            englishTexts["Done"] = "Done";
        }

        private void InitializeCustomComponents()
        {
            this.Size = new System.Drawing.Size(700, 620);
            this.Text = GetText("FormTitle");

            // Загружаем иконку по умолчанию
            LoadDefaultIcon();

            // Инициализация ToolTip
            toolTip = new ToolTip();
            toolTip.AutoPopDelay = 15000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay = 100;
            toolTip.ShowAlways = true;

            // Создание и настройка кнопок языка
            btnLanguageRU = new Button();
            btnLanguageRU.Text = "RU";
            btnLanguageRU.Size = new System.Drawing.Size(40, 30);
            btnLanguageRU.Location = new System.Drawing.Point(20, 20);
            btnLanguageRU.Click += new EventHandler(btnLanguageRU_Click);
            btnLanguageRU.BackColor = Color.LightGray;
            toolTip.SetToolTip(btnLanguageRU, "Русский язык");

            btnLanguageEN = new Button();
            btnLanguageEN.Text = "EN";
            btnLanguageEN.Size = new System.Drawing.Size(40, 30);
            btnLanguageEN.Location = new System.Drawing.Point(70, 20);
            btnLanguageEN.Click += new EventHandler(btnLanguageEN_Click);
            toolTip.SetToolTip(btnLanguageEN, "English language");

            // Создание и настройка кнопки выбора файла
            btnOpenFile = new Button();
            btnOpenFile.Text = GetText("SelectFile");
            btnOpenFile.Size = new System.Drawing.Size(100, 30);
            btnOpenFile.Location = new System.Drawing.Point(120, 20);
            btnOpenFile.Click += new EventHandler(btnOpenFile_Click);

            // Создание и настройка кнопки обработки
            btnProcess = new Button();
            btnProcess.Text = GetText("Process");
            btnProcess.Size = new System.Drawing.Size(100, 30);
            btnProcess.Location = new System.Drawing.Point(230, 20);
            btnProcess.Click += new EventHandler(btnProcess_Click);
            btnProcess.Enabled = false;

            // Кнопка справки
            btnHelp = new Button();
            btnHelp.Text = GetText("Help");
            btnHelp.Size = new System.Drawing.Size(100, 30);
            btnHelp.Location = new System.Drawing.Point(340, 20);
            btnHelp.Click += new EventHandler(btnHelp_Click);

            // Кнопка инструкций
            btnInstructions = new Button();
            btnInstructions.Text = GetText("Instructions");
            btnInstructions.Size = new System.Drawing.Size(100, 30);
            btnInstructions.Location = new System.Drawing.Point(450, 20);
            btnInstructions.Click += new EventHandler(btnInstructions_Click);

            // Кнопка "О программе"
            btnAbout = new Button();
            btnAbout.Text = GetText("About");
            btnAbout.Size = new System.Drawing.Size(100, 30);
            btnAbout.Location = new System.Drawing.Point(560, 20);
            btnAbout.Click += new EventHandler(btnAbout_Click);
            toolTip.SetToolTip(btnAbout, GetText("About"));

            // Создание и настройка прогресс-бара
            progressBar = new ProgressBar();
            progressBar.Location = new System.Drawing.Point(20, 60);
            progressBar.Size = new System.Drawing.Size(660, 20);
            progressBar.Visible = false;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Style = ProgressBarStyle.Marquee;

            // Создание и настройка метки статуса
            statusLabel = new Label();
            statusLabel.Location = new System.Drawing.Point(20, 85);
            statusLabel.Size = new System.Drawing.Size(660, 20);
            statusLabel.Text = "";

            int yPos = 115;

            // 1. Создать бэкап (черный)
            chkCreateBackup = CreateCheckBox(GetText("Backup"), yPos, Color.Black);
            toolTip.SetToolTip(chkCreateBackup, GetText("BackupTooltip"));
            yPos += 25;

            // 2. description="***"
            chkDescription = CreateCheckBox(GetText("Description"), yPos, Color.DarkGreen);
            toolTip.SetToolTip(chkDescription, GetText("DescriptionTooltip"));
            yPos += 25;

            // 3. acceptance_changes={ }
            chkAcceptanceChanges = CreateCheckBox(GetText("AcceptanceChanges"), yPos, Color.DarkGreen);
            toolTip.SetToolTip(chkAcceptanceChanges, GetText("AcceptanceChangesTooltip"));
            yPos += 25;

            // 4. Очистить changes={ } внутри house_unity
            chkHouseUnityChanges = CreateCheckBox(GetText("HouseUnityChanges"), yPos, Color.DarkGreen);
            toolTip.SetToolTip(chkHouseUnityChanges, GetText("HouseUnityChangesTooltip"));
            yPos += 25;

            // 5. character_memory_manager={ }
            chkCharacterMemoryManager = CreateCheckBox(GetText("CharacterMemoryManager"), yPos, Color.DarkGreen);
            toolTip.SetToolTip(chkCharacterMemoryManager, GetText("CharacterMemoryManagerTooltip"));
            yPos += 25;

            // 6. history={ }
            chkHistory = CreateCheckBox(GetText("History"), yPos, Color.Green);
            toolTip.SetToolTip(chkHistory, GetText("HistoryTooltip"));
            yPos += 25;

            // 7. historical={ }
            chkHistorical = CreateCheckBox(GetText("Historical"), yPos, Color.Green);
            toolTip.SetToolTip(chkHistorical, GetText("HistoricalTooltip"));
            yPos += 25;

            // 8. dead_unprunable={ }
            chkDeadUnprunable = CreateCheckBox(GetText("DeadUnprunable"), yPos, Color.Orange);
            toolTip.SetToolTip(chkDeadUnprunable, GetText("DeadUnprunableTooltip"));
            yPos += 25;

            // 9. dead_prunable={ }
            chkDeadPrunable = CreateCheckBox(GetText("DeadPrunable"), yPos, Color.Orange);
            toolTip.SetToolTip(chkDeadPrunable, GetText("DeadPrunableTooltip"));
            yPos += 25;

            // 10. Фильтровать детей в family_data по living
            chkFamilyDataChildren = CreateCheckBox(GetText("FamilyDataChildren"), yPos, Color.Orange);
            toolTip.SetToolTip(chkFamilyDataChildren, GetText("FamilyDataChildrenTooltip"));
            yPos += 25;

            // 11. Блоки с name=\"dynn_\" и dynasty= без head_of_house
            chkDynnDynasty = CreateCheckBox(GetText("DynnDynasty"), yPos, Color.Orange);
            toolTip.SetToolTip(chkDynnDynasty, GetText("DynnDynastyTooltip"));
            yPos += 25;

            // 12. Блоки только с key= (и допустимыми атрибутами)
            chkKeyOnly = CreateCheckBox(GetText("KeyOnly"), yPos, Color.Orange);
            toolTip.SetToolTip(chkKeyOnly, GetText("KeyOnlyTooltip"));
            yPos += 30;

            // Пояснения цветов
            lblColorInfo = new Label();
            lblColorInfo.Location = new System.Drawing.Point(20, yPos);
            lblColorInfo.Size = new System.Drawing.Size(660, 150);
            lblColorInfo.Text = GetText("ColorInfo");
            lblColorInfo.Font = new Font(lblColorInfo.Font, FontStyle.Bold);

            // Добавление всех элементов на форму
            this.Controls.Add(btnLanguageRU);
            this.Controls.Add(btnLanguageEN);
            this.Controls.Add(btnOpenFile);
            this.Controls.Add(btnProcess);
            this.Controls.Add(btnHelp);
            this.Controls.Add(btnInstructions);
            this.Controls.Add(btnAbout);
            this.Controls.Add(progressBar);
            this.Controls.Add(statusLabel);
            this.Controls.Add(chkCreateBackup);
            this.Controls.Add(chkDescription);
            this.Controls.Add(chkAcceptanceChanges);
            this.Controls.Add(chkHouseUnityChanges);
            this.Controls.Add(chkCharacterMemoryManager);
            this.Controls.Add(chkHistory);
            this.Controls.Add(chkHistorical);
            this.Controls.Add(chkDeadUnprunable);
            this.Controls.Add(chkDeadPrunable);
            this.Controls.Add(chkFamilyDataChildren);
            this.Controls.Add(chkDynnDynasty);
            this.Controls.Add(chkKeyOnly);
            this.Controls.Add(lblColorInfo);
        }

        private CheckBox CreateCheckBox(string text, int yPos, Color color)
        {
            var checkBox = new CheckBox();
            checkBox.Text = "? " + text;
            checkBox.Size = new System.Drawing.Size(660, 20);
            checkBox.Location = new System.Drawing.Point(20, yPos);
            checkBox.Checked = true;
            checkBox.ForeColor = color;
            return checkBox;
        }

        // Метод для получения текста в зависимости от языка
        private string GetText(string key)
        {
            if (currentLanguage == Language.Russian && russianTexts.ContainsKey(key))
                return russianTexts[key];
            else if (currentLanguage == Language.English && englishTexts.ContainsKey(key))
                return englishTexts[key];
            return key; // Возвращаем ключ, если перевод не найден
        }

        // Метод для загрузки иконки по умолчанию
        private void LoadDefaultIcon()
        {
            try
            {
                // Попробуем найти иконку в папке с программой
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string iconPath = Path.Combine(appDirectory, "icon.ico");

                if (File.Exists(iconPath))
                {
                    // Загружаем иконку из файла
                    this.Icon = new Icon(iconPath);
                }
                else
                {
                    // Создаем простую иконку с буквой "C" (для CK3 Cleaner)
                    CreateDefaultIcon();
                }
            }
            catch
            {
                // В случае ошибки создаем иконку по умолчанию
                CreateDefaultIcon();
            }
        }

        // Метод для создания иконки по умолчанию
        private void CreateDefaultIcon()
        {
            // Создаем простую иконку с буквой "C" (для CK3 Cleaner)
            Bitmap bmp = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                using (Font font = new Font("Arial", 14, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.Blue))
                {
                    g.DrawString("C", font, brush, new PointF(8, 6));
                }
            }

            // Конвертируем Bitmap в Icon
            this.Icon = Icon.FromHandle(bmp.GetHicon());
        }

        // Обработчики для кнопок языка
        private void btnLanguageRU_Click(object sender, EventArgs e)
        {
            if (currentLanguage != Language.Russian)
            {
                currentLanguage = Language.Russian;
                UpdateLanguage();
                btnLanguageRU.BackColor = Color.LightGray;
                btnLanguageEN.BackColor = SystemColors.Control;
            }
        }

        private void btnLanguageEN_Click(object sender, EventArgs e)
        {
            if (currentLanguage != Language.English)
            {
                currentLanguage = Language.English;
                UpdateLanguage();
                btnLanguageEN.BackColor = Color.LightGray;
                btnLanguageRU.BackColor = SystemColors.Control;
            }
        }

        // Метод для обновления языка интерфейса
        private void UpdateLanguage()
        {
            this.Text = GetText("FormTitle");
            btnOpenFile.Text = GetText("SelectFile");
            btnProcess.Text = GetText("Process");
            btnHelp.Text = GetText("Help");
            btnInstructions.Text = GetText("Instructions");
            btnAbout.Text = GetText("About");
            toolTip.SetToolTip(btnAbout, GetText("About"));
            toolTip.SetToolTip(btnLanguageRU, currentLanguage == Language.Russian ? "Русский язык" : "Russian language");
            toolTip.SetToolTip(btnLanguageEN, currentLanguage == Language.Russian ? "Английский язык" : "English language");

            // Обновляем текст чекбоксов
            chkCreateBackup.Text = "? " + GetText("Backup");
            toolTip.SetToolTip(chkCreateBackup, GetText("BackupTooltip"));

            chkDescription.Text = "? " + GetText("Description");
            toolTip.SetToolTip(chkDescription, GetText("DescriptionTooltip"));

            chkAcceptanceChanges.Text = "? " + GetText("AcceptanceChanges");
            toolTip.SetToolTip(chkAcceptanceChanges, GetText("AcceptanceChangesTooltip"));

            chkHouseUnityChanges.Text = "? " + GetText("HouseUnityChanges");
            toolTip.SetToolTip(chkHouseUnityChanges, GetText("HouseUnityChangesTooltip"));

            chkCharacterMemoryManager.Text = "? " + GetText("CharacterMemoryManager");
            toolTip.SetToolTip(chkCharacterMemoryManager, GetText("CharacterMemoryManagerTooltip"));

            chkHistory.Text = "? " + GetText("History");
            toolTip.SetToolTip(chkHistory, GetText("HistoryTooltip"));

            chkHistorical.Text = "? " + GetText("Historical");
            toolTip.SetToolTip(chkHistorical, GetText("HistoricalTooltip"));

            chkDeadUnprunable.Text = "? " + GetText("DeadUnprunable");
            toolTip.SetToolTip(chkDeadUnprunable, GetText("DeadUnprunableTooltip"));

            chkDeadPrunable.Text = "? " + GetText("DeadPrunable");
            toolTip.SetToolTip(chkDeadPrunable, GetText("DeadPrunableTooltip"));

            chkFamilyDataChildren.Text = "? " + GetText("FamilyDataChildren");
            toolTip.SetToolTip(chkFamilyDataChildren, GetText("FamilyDataChildrenTooltip"));

            chkDynnDynasty.Text = "? " + GetText("DynnDynasty");
            toolTip.SetToolTip(chkDynnDynasty, GetText("DynnDynastyTooltip"));

            chkKeyOnly.Text = "? " + GetText("KeyOnly");
            toolTip.SetToolTip(chkKeyOnly, GetText("KeyOnlyTooltip"));

            lblColorInfo.Text = GetText("ColorInfo");
        }

        // Обработчик для кнопки "О программе"
        private void btnAbout_Click(object sender, EventArgs e)
        {
            ShowAboutInfo();
        }

        // Метод для отображения информации о программе
        private void ShowAboutInfo()
        {
            string aboutText = currentLanguage == Language.Russian ?
                @"CK3 Save Cleaner - Программа для очистки сохранений Crusader Kings 3

Ковровский государственный технологический университет имени В.А. Дегтярева
Кафедра прикладной математики и систем автоматизированного проектирования
Разработчик: ст. гр. И-122 Святов М. И.
Руководитель: Пронин С. Р.
Версия: 1.0.0
Дата сборки: " + DateTime.Now.ToString("dd-MM-yyyy") + @"

Функции:
• Очистка данных сохранений
• Уменьшение размера файлов сохранений
• Улучшение производительности игры
• Создание резервных копий" :
                @"CK3 Save Cleaner - Program for cleaning Crusader Kings 3 saves

The Kovrov State Technological University named after V.A. Degtyarev
Department of Applied Mathematics and CAD Systems
Developer: group I-122 Svyatov M. I.
Supervisor: Pronin S. R.
Version: 1.0.0
Build date: " + DateTime.Now.ToString("dd-MM-yyyy") + @"

Functions:
• Cleaning save data
• Reducing save file size
• Improving game performance
• Creating backup copies";

            MessageBox.Show(aboutText, GetText("About"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ОБРАБОТЧИКИ СУЩЕСТВУЮЩИХ КНОПОК
        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpText = currentLanguage == Language.Russian ?
                @"СПРАВКА ПО ИСПОЛЬЗОВАНИЮ ПРОГРАММЫ

1. Выберите файл сохранения (.ck3)
Обычный путь: Documents/Paradox Interactive/Crusader Kings III/save games
2. Отметьте нужные опции очистки
3. Нажмите 'Обработать'
4. Проверьте результат в игре

Рекомендации:
- Всегда оставляйте включенным 'Создать бэкап'
- Начинайте с зеленых опций
- Тестируйте сохранение после каждой очистки" :
                @"PROGRAM HELP

1. Select save file (.ck3)
Default path: Documents/Paradox Interactive/Crusader Kings III/save games
2. Select desired cleaning options
3. Click 'Process'
4. Check the result in the game

Recommendations:
- Always keep 'Create backup' enabled
- Start with green options
- Test the save after each cleaning";

            MessageBox.Show(helpText, GetText("Help"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnInstructions_Click(object sender, EventArgs e)
        {
            string instructionsText = currentLanguage == Language.Russian ?
                @"ИНСТРУКЦИИ ПО ПОДГОТОВКЕ СОХРАНЕНИЯ

1. Запуск игры в режиме отладки:
   - Измените свойства ярлыка на рабочем столе, дописав в строке 'Объект:' в конце ' -debug_mode'
   - Если у вас есть лицензия Steam, то зайдите в свою библиотеку Steam и кликните правой кнопкой мыши по Crusader Kings 3, Выберите пункт 'Свойства', затем выберите пункт 'Установить параметры запуска' и в появившемся окне вводим: -debug_mode
   - Вы также можете открыть лаунчер игры, в лаунчере перейти в 'Настройки игры' и прокрутить вниз до раздела «Открыть игру в режиме отладки» и нажать Запустить.
   - Либо используйте сторонние модификации
   - Иные способы и подробности есть в сети Интернет

2. В игре:
   - Загрузите требуемое для очистки сохранение
   - Нажмите кнопку на клавиатуре [ ~ ], после чего открывается консоль
   - В окне консоли введите 'save SaveName', что создаст сохранение в полностью несжатом виде, необходимом для работы программы (вместо SaveName можете ввести любое другое название)
   - Выйдите из игры полностью, чтобы освободить ресурсы ОЗУ для работы программы (рекомендуется для пользователей с объёмом ОЗУ менее 32 гб)" :
                @"INSTRUCTIONS FOR PREPARING THE SAVE

1. Launch the game in debug mode:
   - Modify the desktop shortcut properties by adding ' -debug_mode' at the end of the 'Object:' line
   - If you have a Steam license, go to your Steam library, right-click on Crusader Kings 3, select 'Properties', then select 'Set launch options' and enter: -debug_mode
   - You can also open the game launcher, go to 'Game Settings' and scroll down to the 'Launch game in debug mode' section and click Launch.
   - Or use third-party modifications
   - Other methods and details are available on the Internet

2. In the game:
   - Load the save you want to clean
   - Press the [ ~ ] key on the keyboard to open the console
   - In the console window, type 'save SaveName' which will create a save in completely uncompressed format required for the program (you can use any name instead of SaveName)
   - Exit the game completely to free up RAM resources for the program (recommended for users with less than 32 GB RAM)";

            MessageBox.Show(instructionsText, GetText("Instructions"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string currentFilePath = "";

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = currentLanguage == Language.Russian ?
                    "Файлы сохранений CK3 (*.ck3)|*.ck3|Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*" :
                    "CK3 Save files (*.ck3)|*.ck3|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.Title = currentLanguage == Language.Russian ?
                    "Выберите файл сохранения для очистки" : "Select save file for cleaning";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = openFileDialog.FileName;
                    btnProcess.Enabled = true;
                    statusLabel.Text = string.Format(GetText("StatusFileSelected"), Path.GetFileName(currentFilePath));
                }
            }
        }

        private async void btnProcess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                MessageBox.Show(
                    currentLanguage == Language.Russian ? "Сначала выберите файл!" : "First select a file!",
                    currentLanguage == Language.Russian ? "Ошибка" : "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(currentFilePath))
            {
                MessageBox.Show(
                    currentLanguage == Language.Russian ? "Выбранный файл не существует!" : "The selected file does not exist!",
                    currentLanguage == Language.Russian ? "Ошибка" : "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверяем, что выбран хотя бы один метод очистки
            bool anyOptionSelected = chkCreateBackup.Checked || chkDescription.Checked ||
                                   chkAcceptanceChanges.Checked || chkHouseUnityChanges.Checked ||
                                   chkCharacterMemoryManager.Checked || chkHistory.Checked ||
                                   chkHistorical.Checked || chkDeadUnprunable.Checked ||
                                   chkDeadPrunable.Checked || chkFamilyDataChildren.Checked ||
                                   chkDynnDynasty.Checked || chkKeyOnly.Checked;

            if (!anyOptionSelected)
            {
                MessageBox.Show(
                    currentLanguage == Language.Russian ? "Выберите хотя бы один метод очистки!" : "Select at least one cleaning method!",
                    currentLanguage == Language.Russian ? "Ошибка" : "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnOpenFile.Enabled = false;
            btnProcess.Enabled = false;
            btnHelp.Enabled = false;
            btnInstructions.Enabled = false;
            btnAbout.Enabled = false;
            btnLanguageRU.Enabled = false;
            btnLanguageEN.Enabled = false;
            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Marquee;
            statusLabel.Text = GetText("StatusProcessing");

            try
            {
                await ProcessFileWithOptions(currentFilePath);

                // Создаем понятное для пользователя сообщение
                string message = GetText("OperationCompleted");
                if (chkCreateBackup.Checked) message += "\n- " + GetText("BackupCreated");
                if (chkDescription.Checked) message += "\n- " + GetText("DescriptionsCleared");
                if (chkAcceptanceChanges.Checked) message += "\n- " + GetText("AcceptanceChangesCleared");
                if (chkHouseUnityChanges.Checked) message += "\n- " + GetText("HouseUnityChangesCleared");
                if (chkCharacterMemoryManager.Checked) message += "\n- " + GetText("CharacterMemoryManagerCleared");
                if (chkHistory.Checked) message += "\n- " + GetText("HistoryCleared");
                if (chkHistorical.Checked) message += "\n- " + GetText("HistoricalCleared");
                if (chkDeadUnprunable.Checked) message += "\n- " + GetText("DeadUnprunableCleared");
                if (chkDeadPrunable.Checked) message += "\n- " + GetText("DeadPrunableCleared");
                if (chkFamilyDataChildren.Checked) message += "\n- " + GetText("FamilyDataChildrenFiltered");
                if (chkDynnDynasty.Checked) message += "\n- " + GetText("DynnDynastyCleared");
                if (chkKeyOnly.Checked) message += "\n- " + GetText("KeyOnlyCleared");

                MessageBox.Show(message, GetText("Done"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    (currentLanguage == Language.Russian ? "Ошибка: " : "Error: ") + ex.Message,
                    currentLanguage == Language.Russian ? "Ошибка" : "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnOpenFile.Enabled = true;
                btnProcess.Enabled = true;
                btnHelp.Enabled = true;
                btnInstructions.Enabled = true;
                btnAbout.Enabled = true;
                btnLanguageRU.Enabled = true;
                btnLanguageEN.Enabled = true;
                progressBar.Visible = false;
                progressBar.Style = ProgressBarStyle.Blocks;
                statusLabel.Text = GetText("StatusReady");
            }
        }

        private async Task ProcessFileWithOptions(string filePath)
        {
            // Устанавливаем высокий приоритет процессу
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            string content = File.ReadAllText(filePath);

            // Создаем бэкап если выбрано
            if (chkCreateBackup.Checked)
            {
                string backupPath = filePath + ".backup";
                File.Copy(filePath, backupPath, true);
            }

            // Извлекаем ID из блока living (нужно для фильтрации детей)
            HashSet<int> livingIds = new HashSet<int>();
            if (chkFamilyDataChildren.Checked)
            {
                livingIds = await Task.Run(() => ExtractLivingIds(content));
            }

            // ПРИМЕНЯЕМ ВЫБРАННЫЕ МЕТОДЫ ОЧИСТКИ В ТОЧНОМ ПОРЯДКЕ
            // Сначала description
            if (chkDescription.Checked)
            {
                content = await Task.Run(() => ClearDescriptions(content));
            }

            // Затем остальные блоки
            if (chkAcceptanceChanges.Checked || chkCharacterMemoryManager.Checked ||
                chkHistory.Checked || chkHistorical.Checked ||
                chkDeadUnprunable.Checked || chkDeadPrunable.Checked)
            {
                content = await Task.Run(() => ClearSelectedBlocks(content));
            }

            if (chkHouseUnityChanges.Checked)
            {
                content = await Task.Run(() => ClearHouseUnityChanges(content));
            }

            if (chkDynnDynasty.Checked)
            {
                content = await Task.Run(() => RemoveDynnDynastyBlocks(content));
            }

            if (chkKeyOnly.Checked)
            {
                content = await Task.Run(() => RemoveKeyOnlyBlocks(content));
            }

            if (chkFamilyDataChildren.Checked)
            {
                content = await Task.Run(() => FilterFamilyDataChildren(content, livingIds));
            }

            File.WriteAllText(filePath, content);
        }

        // Метод для очистки description="***"
        private string ClearDescriptions(string content)
        {
            StringBuilder result = new StringBuilder();
            int i = 0;
            int len = content.Length;

            while (i < len)
            {
                // Ищем подстроку "description="
                if (i < len - 13 && content[i] == 'd')
                {
                    // Быстрая проверка
                    bool isDescription = true;
                    string descriptionStr = "description=\"";

                    for (int j = 0; j < descriptionStr.Length; j++)
                    {
                        if (i + j >= len || content[i + j] != descriptionStr[j])
                        {
                            isDescription = false;
                            break;
                        }
                    }

                    if (isDescription)
                    {
                        // Нашли description=", добавляем его в результат
                        result.Append("description=\"");
                        i += 13; // Пропускаем description="

                        // Пропускаем весь текст внутри кавычек
                        bool escaped = false;
                        while (i < len && (content[i] != '"' || escaped))
                        {
                            if (content[i] == '\\' && i + 1 < len)
                            {
                                escaped = true;
                                i++;
                            }
                            else
                            {
                                escaped = false;
                                i++;
                            }
                        }

                        // Добавляем закрывающую кавычку
                        if (i < len && content[i] == '"')
                        {
                            result.Append("\"");
                            i++;
                        }
                        continue;
                    }
                }

                // Просто копируем символ
                result.Append(content[i]);
                i++;
            }

            return result.ToString();
        }

        // ОСТАЛЬНЫЕ МЕТОДЫ
        private string ClearSelectedBlocks(string content)
        {
            StringBuilder result = new StringBuilder(content.Length);
            int i = 0;
            int len = content.Length;

            while (i < len)
            {
                bool blockProcessed = false;

                // Проверяем выбранные блоки в порядке приоритета
                if (chkDeadUnprunable.Checked && i < len - 12 && content[i] == 'd' && StartsWith(content, i, "dead_unprunable={"))
                {
                    i = ClearBlock(content, i, "dead_unprunable={", result);
                    blockProcessed = true;
                }
                else if (chkDeadPrunable.Checked && i < len - 12 && content[i] == 'd' && StartsWith(content, i, "dead_prunable={"))
                {
                    i = ClearBlock(content, i, "dead_prunable={", result);
                    blockProcessed = true;
                }
                else if (chkHistory.Checked && i < len - 8 && content[i] == 'h' && StartsWith(content, i, "history={"))
                {
                    i = ClearBlock(content, i, "history={", result);
                    blockProcessed = true;
                }
                else if (chkHistorical.Checked && i < len - 8 && content[i] == 'h' && StartsWith(content, i, "historical={"))
                {
                    i = ClearBlock(content, i, "historical={", result);
                    blockProcessed = true;
                }
                else if (chkAcceptanceChanges.Checked && i < len - 18 && content[i] == 'a' && StartsWith(content, i, "acceptance_changes={"))
                {
                    i = ClearBlock(content, i, "acceptance_changes={", result);
                    blockProcessed = true;
                }
                else if (chkCharacterMemoryManager.Checked && i < len - 24 && content[i] == 'c' && StartsWith(content, i, "character_memory_manager={"))
                {
                    i = ClearBlock(content, i, "character_memory_manager={", result);
                    blockProcessed = true;
                }

                if (blockProcessed)
                {
                    continue;
                }

                // Если не целевой блок, просто добавляем символ
                result.Append(content[i]);
                i++;
            }

            return result.ToString();
        }

        private HashSet<int> ExtractLivingIds(string content)
        {
            var livingIds = new HashSet<int>();
            int i = content.IndexOf("living={");
            if (i == -1) return livingIds;

            i += 8; // Длина "living={"
            int braceCount = 1;
            int len = content.Length;
            int blockStart = i;

            // Собираем весь блок living
            while (braceCount > 0 && i < len)
            {
                if (content[i] == '{')
                    braceCount++;
                else if (content[i] == '}')
                    braceCount--;
                i++;
            }

            // Теперь внутри блока living ищем все ID персонажей
            string livingBlock = content.Substring(blockStart, i - blockStart - 1); // -1 чтобы убрать закрывающую скобку
            int j = 0;
            while (j < livingBlock.Length)
            {
                if (char.IsDigit(livingBlock[j]))
                {
                    int idStart = j;
                    // Читаем все цифры
                    while (j < livingBlock.Length && char.IsDigit(livingBlock[j])) j++;
                    string idStr = livingBlock.Substring(idStart, j - idStart);

                    // Проверяем, что после цифр идет "={"
                    if (j < livingBlock.Length - 1 && livingBlock[j] == '=' && livingBlock[j + 1] == '{')
                    {
                        if (int.TryParse(idStr, out int id))
                        {
                            livingIds.Add(id);
                        }
                    }
                }
                else
                {
                    j++;
                }
            }

            return livingIds;
        }

        private string FilterFamilyDataChildren(string content, HashSet<int> livingIds)
        {
            StringBuilder result = new StringBuilder(content.Length);
            int i = 0;
            int len = content.Length;

            while (i < len)
            {
                // Ищем family_data={
                if (i < len - 13 && content[i] == 'f' && StartsWith(content, i, "family_data={"))
                {
                    int familyDataStart = i;
                    result.Append("family_data={");

                    i += 13; // Пропускаем "family_data={"
                    int braceCount = 1;

                    // Обрабатываем содержимое блока family_data
                    while (braceCount > 0 && i < len)
                    {
                        // Если нашли child={
                        if (i < len - 7 && content[i] == 'c' && StartsWith(content, i, "child={"))
                        {
                            int childStart = i;
                            result.Append("child={");

                            i += 7; // Пропускаем "child={"
                            int childBraceCount = 1;

                            // Обрабатываем содержимое блока child
                            while (childBraceCount > 0 && i < len)
                            {
                                // Если на уровне child (childBraceCount == 1) и нашли цифру
                                if (childBraceCount == 1 && char.IsDigit(content[i]))
                                {
                                    int idStart = i;
                                    // Читаем все цифры
                                    while (i < len && char.IsDigit(content[i])) i++;
                                    string idStr = content.Substring(idStart, i - idStart);

                                    if (int.TryParse(idStr, out int childId))
                                    {
                                        // Проверяем, есть ли этот ID в livingIds
                                        if (livingIds.Contains(childId))
                                        {
                                            result.Append(idStr);
                                            result.Append(" ");
                                        }
                                        // Если нет, просто пропускаем этот ID
                                    }

                                    // Пропускаем пробелы после ID
                                    while (i < len && char.IsWhiteSpace(content[i])) i++;
                                }
                                else
                                {
                                    // Если не цифра, просто копируем символ
                                    if (content[i] == '{')
                                        childBraceCount++;
                                    else if (content[i] == '}')
                                        childBraceCount--;

                                    // Добавляем символ только если это не закрывающая скобка верхнего уровня
                                    if (!(childBraceCount == 0 && content[i] == '}'))
                                        result.Append(content[i]);

                                    i++;
                                }
                            }

                            // Добавляем закрывающую скобку child
                            result.Append("}");
                        }
                        else
                        {
                            // Если не child, просто копируем содержимое
                            if (content[i] == '{')
                                braceCount++;
                            else if (content[i] == '}')
                                braceCount--;

                            // Добавляем символ только если это не закрывающая скобка верхнего уровня
                            if (!(braceCount == 0 && content[i] == '}'))
                                result.Append(content[i]);

                            i++;
                        }
                    }

                    // Добавляем закрывающую скобку family_data
                    result.Append("}");
                }
                else
                {
                    result.Append(content[i]);
                    i++;
                }
            }

            return result.ToString();
        }

        private string ClearHouseUnityChanges(string content)
        {
            StringBuilder result = new StringBuilder(content.Length);
            int i = 0;
            int len = content.Length;

            while (i < len)
            {
                // Ищем house_unity={
                if (i < len - 13 && content[i] == 'h' && StartsWith(content, i, "house_unity={"))
                {
                    int houseUnityStart = i;
                    result.Append("house_unity={");

                    i += 13; // Пропускаем "house_unity={"
                    int braceCount = 1;

                    // Обрабатываем содержимое блока house_unity
                    while (braceCount > 0 && i < len)
                    {
                        // Если нашли changes={
                        if (i < len - 9 && content[i] == 'c' && StartsWith(content, i, "changes={"))
                        {
                            int changesStart = i;
                            result.Append("changes={");

                            i += 9; // Пропускаем "changes={"
                            int changesBraceCount = 1;

                            // Пропускаем содержимое блока changes
                            while (changesBraceCount > 0 && i < len)
                            {
                                if (content[i] == '{')
                                    changesBraceCount++;
                                else if (content[i] == '}')
                                    changesBraceCount--;

                                i++;
                            }

                            // Добавляем закрывающую скобку changes с пробелом
                            result.Append(" }");
                        }
                        else
                        {
                            // Если не changes, просто копируем содержимое
                            if (content[i] == '{')
                                braceCount++;
                            else if (content[i] == '}')
                                braceCount--;

                            // Добавляем символ только если это не закрывающую скобку верхнего уровня
                            if (!(braceCount == 0 && content[i] == '}'))
                                result.Append(content[i]);

                            i++;
                        }
                    }

                    // Добавляем закрывающую скобку house_unity
                    result.Append("}");
                }
                else
                {
                    result.Append(content[i]);
                    i++;
                }
            }

            return result.ToString();
        }

        private string RemoveDynnDynastyBlocks(string content)
        {
            StringBuilder result = new StringBuilder(content.Length);
            int i = 0;
            int len = content.Length;

            while (i < len)
            {
                // Ищем начало блока (одна или несколько цифр и "={")
                if (char.IsDigit(content[i]))
                {
                    int blockStart = i;

                    // Пропускаем все цифры ID
                    while (i < len && char.IsDigit(content[i])) i++;

                    // Проверяем, что после цифр идет "={"
                    if (i < len - 2 && content[i] == '=' && content[i + 1] == '{')
                    {
                        bool hasDynnName = false;
                        bool hasDynasty = false;
                        bool hasHeadOfHouse = false;

                        // Переходим к содержимому блока
                        i += 2; // Пропускаем "={"
                        int braceCount = 1;

                        // Анализируем содержимое блока
                        while (braceCount > 0 && i < len)
                        {
                            // Проверяем наличие нужных строк
                            if (!hasDynnName && i < len - 10 && StartsWith(content, i, "name=\"dynn_"))
                            {
                                hasDynnName = true;
                            }

                            if (!hasDynasty && i < len - 8 && StartsWith(content, i, "dynasty="))
                            {
                                hasDynasty = true;
                            }

                            if (!hasHeadOfHouse && i < len - 13 && StartsWith(content, i, "head_of_house"))
                            {
                                hasHeadOfHouse = true;
                            }

                            // Отслеживаем уровень вложенности скобок
                            if (content[i] == '{')
                                braceCount++;
                            else if (content[i] == '}')
                                braceCount--;

                            i++;
                        }

                        // Проверяем условия: есть dynn_name и dynasty, но нет head_of_house
                        if (hasDynnName && hasDynasty && !hasHeadOfHouse)
                        {
                            // Пропускаем этот блок (не добавляем его в результат)
                            continue;
                        }
                        else
                        {
                            // Добавляем весь блок в результат
                            result.Append(content, blockStart, i - blockStart);
                        }
                    }
                    else
                    {
                        // Это не блок, добавляем цифры как есть
                        result.Append(content, blockStart, i - blockStart);
                    }
                }
                else
                {
                    result.Append(content[i]);
                    i++;
                }
            }

            return result.ToString();
        }

        private string RemoveKeyOnlyBlocks(string content)
        {
            StringBuilder result = new StringBuilder(content.Length);
            int i = 0;
            int len = content.Length;

            while (i < len)
            {
                // Ищем начало блока (одна или несколько цифр и "={")
                if (char.IsDigit(content[i]))
                {
                    int blockStart = i;

                    // Пропускаем все цифры ID
                    while (i < len && char.IsDigit(content[i])) i++;

                    // Проверяем, что после цифр идет "={"
                    if (i < len - 2 && content[i] == '=' && content[i + 1] == '{')
                    {
                        int contentStart = i + 2; // Начало содержимого блока (после "{")
                        HashSet<string> attributes = new HashSet<string>();
                        bool hasOtherAttributes = false;

                        // Переходим к содержимому блока
                        i += 2; // Пропускаем "={"
                        int braceCount = 1;

                        // Анализируем содержимое блока
                        while (braceCount > 0 && i < len)
                        {
                            // Ищем атрибуты (слова перед "=")
                            if (i < len - 1 && char.IsLetter(content[i]) && content[i] != '=' && content[i] != '{' && content[i] != '}')
                            {
                                int attrStart = i;
                                // Читаем имя атрибута
                                while (i < len && (char.IsLetterOrDigit(content[i]) || content[i] == '_')) i++;

                                string attribute = content.Substring(attrStart, i - attrStart);

                                // Пропускаем пробелы
                                while (i < len && char.IsWhiteSpace(content[i])) i++;

                                // Проверяем, что после имени атрибута идет "="
                                if (i < len && content[i] == '=')
                                {
                                    attributes.Add(attribute);

                                    // Проверяем, является ли атрибут допустимым
                                    if (attribute != "key" && attribute != "coat_of_arms_id" && attribute != "good_for_realm_name")
                                    {
                                        hasOtherAttributes = true;
                                    }
                                }
                            }
                            else
                            {
                                // Отслеживаем уровень вложенности скобок
                                if (content[i] == '{')
                                    braceCount++;
                                else if (content[i] == '}')
                                    braceCount--;

                                i++;
                            }
                        }

                        // Проверяем условия: есть key=, нет других атрибутов кроме допустимых
                        if (attributes.Contains("key") && !hasOtherAttributes)
                        {
                            // Пропускаем этот блок (не добавляем его в результат)
                            continue;
                        }
                        else
                        {
                            // Добавляем весь блок в результат
                            result.Append(content, blockStart, i - blockStart);
                        }
                    }
                    else
                    {
                        // Это не блок, добавляем цифры как есть
                        result.Append(content, blockStart, i - blockStart);
                    }
                }
                else
                {
                    result.Append(content[i]);
                    i++;
                }
            }

            return result.ToString();
        }

        private int ClearBlock(string content, int startIndex, string blockStart, StringBuilder result)
        {
            // Добавляем начало блока
            result.Append(blockStart);

            int i = startIndex + blockStart.Length;
            int braceCount = 1;
            int len = content.Length;

            // Пропускаем содержимое блока, отслеживая скобки
            while (braceCount > 0 && i < len)
            {
                if (content[i] == '{')
                    braceCount++;
                else if (content[i] == '}')
                    braceCount--;

                i++;
            }

            // Если нашли закрывающую скобку, добавляем пробел и закрывающую скобку
            if (braceCount == 0)
            {
                result.Append(" }");
            }
            else
            {
                // Если что-то пошло не так, оставляем оригинальное содержимое
                result.Append(content, startIndex + blockStart.Length, i - (startIndex + blockStart.Length));
            }

            return i;
        }

        private bool StartsWith(string content, int startIndex, string value)
        {
            if (startIndex + value.Length > content.Length)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (content[startIndex + i] != value[i])
                    return false;
            }

            return true;
        }
    }
}