using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;

namespace WpfApp1
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        NPC bot = new NPC();
        //Деревянный ящик
        Image woodenbox_obj = new Image { Name = "woodbox", Margin = new Thickness(0, 0, 0, -14) };
        //Игрок
        Canvas player = new Canvas();
        Image player_obj = new Image { Name = "dotnet" };
        //NPC
        Canvas player_npc = new Canvas();
        Image player_npc_obj = new Image { Name = "bot" };

        //Кнопка "Начать игру" в главном меню 
        Button play = new Button() { Name = "play" };
        //Счётик боеприпасов для оружия
        Label weapon_table = new Label { Name = "weapon_table" };
        //Панель сообщений
        Label chat = new Label { Name = "chat" };
        //Пуля
        Image bullet_obj = new Image { Name = "bullet" };
        //Оружие в руке
        Image weapon_obj = new Image { Name = "weapon" };

        public Weapon colt_45 = new Weapon();
        public Weapon m40a1 = new Weapon();



        public MainWindow()
        {
            InitializeComponent();

            IniFile config = new IniFile($"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}System.ini");
            player_obj.Height = Convert.ToInt32(config.Read("size", "player").ToString().Split(',')[0]);
            player_obj.Width = Convert.ToInt32(config.Read("size", "player").ToString().Split(',')[1]);
            player_npc_obj.Height = Convert.ToInt32(config.Read("size", "player_npc").ToString().Split(',')[0]);
            player_npc_obj.Width = Convert.ToInt32(config.Read("size", "player_npc").ToString().Split(',')[1]);
            player_obj.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{config.Read("visual", "player")}", UriKind.RelativeOrAbsolute));
            player_npc_obj.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{config.Read("visual", "player_npc")}", UriKind.RelativeOrAbsolute));
            Canvas.SetTop(player_obj, Convert.ToInt32(config.Read("position", "player").ToString().Split(',')[0]));
            Canvas.SetLeft(player_obj, Convert.ToInt32(config.Read("position", "player").ToString().Split(',')[1]));
            Canvas.SetTop(player_npc_obj, Convert.ToInt32(config.Read("position", "player_npc").ToString().Split(',')[0]));
            Canvas.SetLeft(player_npc_obj, Convert.ToInt32(config.Read("position", "player_npc").ToString().Split(',')[1]));



            //его "спавн"
            player.Children.Add(player_obj);
            player_npc.Children.Add(player_npc_obj);

            //Оружие в руках игрока
            weapon_obj.Height = Convert.ToInt32(config.Read("size", "weapon_obj").ToString().Split(',')[0]);
            weapon_obj.Width = Convert.ToInt32(config.Read("size", "weapon_obj").ToString().Split(',')[1]);
            //его расположение
            Canvas.SetTop(weapon_obj, Convert.ToInt32(config.Read("position", "weapon_obj").ToString().Split(',')[0]));
            Canvas.SetLeft(weapon_obj, Convert.ToInt32(config.Read("position", "weapon_obj").ToString().Split(',')[1]));

            //его "cпавн"
            player.Children.Add(weapon_obj);

            //Расположение счётика БП
            Grid.SetColumn(weapon_table, Convert.ToInt32(config.Read("cr_position", "weapon_table").ToString().Split(',')[0]));
            Grid.SetRow(weapon_table, Convert.ToInt32(config.Read("cr_position", "weapon_table").ToString().Split(',')[1]));
            weapon_table.Content = config.Read("content", "weapon_table");
            weapon_table.FontFamily = new FontFamily(config.Read("font", "weapon_table"));
            weapon_table.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString((config.Read("foreground", "weapon_table")));
            weapon_table.Background = (SolidColorBrush)new BrushConverter().ConvertFromString((config.Read("background", "weapon_table")));

            Canvas.SetTop(bullet_obj, Convert.ToInt32(config.Read("position", "bullet_obj").ToString().Split(',')[0]));
            Canvas.SetLeft(bullet_obj, Convert.ToInt32(config.Read("position", "bullet_obj").ToString().Split(',')[1]));
            bullet_obj.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{config.Read("visual", "bullet_obj")}", UriKind.RelativeOrAbsolute));
            //Расположение панели сообщений
            Grid.SetColumnSpan(chat, Convert.ToInt32(config.Read("cr_span", "chat").ToString().Split(',')[0]));
            Grid.SetRowSpan(chat, Convert.ToInt32(config.Read("cr_span", "chat").ToString().Split(',')[1]));
            chat.Content = config.Read("content", "chat");
            chat.FontFamily = new FontFamily(config.Read("font", "chat"));

            //его появление на экране
            gridok.Children.Add(chat);

            //Расположение деревянного ящика
            Grid.SetColumn(woodenbox_obj, Convert.ToInt32(config.Read("cr_position", "woodenbox_obj").ToString().Split(',')[0]));
            Grid.SetColumnSpan(woodenbox_obj, Convert.ToInt32(config.Read("cr_span", "woodenbox_obj").ToString().Split(',')[0]));
            Grid.SetRow(woodenbox_obj, Convert.ToInt32(config.Read("cr_position", "woodenbox_obj").ToString().Split(',')[1]));
            Grid.SetRowSpan(woodenbox_obj, Convert.ToInt32(config.Read("cr_span", "woodenbox_obj").ToString().Split(',')[1]));
            woodenbox_obj.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{config.Read("visual", "woodenbox_obj")}", UriKind.RelativeOrAbsolute));

            Player.general = player_obj; //Присваеваем тело игрока (его изображение) к классу Player
            Player.weapon = weapon_obj; //Присваеваем изображение орудия к классу игрока
            Player.weapon_table = weapon_table; //Присваеваем табло счётика БП к игроку (чтоб был виден классу weapon)


            bot.general = player_npc_obj;
            bot.chat = chat;


            //Расположение кнопки "Начать игру" в главном меню
            Grid.SetColumn(play, Convert.ToInt32(config.Read("cr_position", "play").ToString().Split(',')[0]));
            Grid.SetRow(play, Convert.ToInt32(config.Read("cr_position", "play").ToString().Split(',')[1]));
            play.Content = config.Read("content", "play");
            //её создание
            gridok.Children.Add(play);
            play.Click += Play_Click; //Событие клика по ней

            //Пистолет (Кольт M1911) 
            colt_45.Name = config.Read("name", "colt_45"); //Название оружия
            colt_45.cartridge_name = config.Read("cartridge_name", "colt_45"); //калибр боеприпасов к нему
            //Скин оружия
            colt_45.Skin = new BitmapImage[3] { new BitmapImage(new Uri("pack://application:,,,/weapons/colt_45.png")), new BitmapImage(new Uri("pack://application:,,,/weapons/colt_45_fire.png")), new BitmapImage(new Uri("pack://application:,,,/weapons/colt_45.png")) };
            //Звук выстрела
            colt_45.sound = new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}{config.Read("sound", "colt_45")}");
            colt_45.in_magazine = Convert.ToInt32(config.Read("in_magazine", "colt_45")); //Вместимость магазина
            colt_45.in_magazine_count = Convert.ToInt32(config.Read("in_magazine_count", "colt_45")); //Кол-во "маслят" в магазе
            colt_45.cartridge_count = Convert.ToInt32(config.Read("cartridge_count", "colt_45")); //Кол-во боеприпасов к оружию

            //Винтовка (Ремингтон M40A1)
            m40a1.Name = config.Read("name", "m40a1");
            m40a1.cartridge_name = config.Read("cartridge_name", "m40a1");
            m40a1.Skin = new BitmapImage[1] { new BitmapImage(new Uri("weapons/m40a1.png", UriKind.RelativeOrAbsolute)) };          
            m40a1.sound = new Uri($@"{System.IO.Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}{config.Read("sound", "m40a1")}");
            m40a1.in_magazine = Convert.ToInt32(config.Read("in_magazine", "m40a1"));
            m40a1.in_magazine_count = Convert.ToInt32(config.Read("in_magazine_count", "m40a1"));
            m40a1.cartridge_count = Convert.ToInt32(config.Read("cartridge_count", "m40a1"));

            this.KeyDown += Dotnet_KeyDown; //Присваевыем управление игре
           

using (StreamReader sr = new StreamReader($"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}user.cfg", System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    console.Text = line;
                    PlayCommand_Click(null, null);

                }
            }
        }

       
        string location = "level"; 
        int count = 0; //Кол-во хода
        int jump_count = 0; //Кол-во (высота) прыжка
        bool space_pressed = false; //Зажата ли Space
        //bool shift_pressed = false;

       
        TextBox console = new TextBox(); //Консолька
        Button console_button = new Button(); //Кнопка исполнения команды консоли

        //weapon
        

        public async void Play_Click(object sender, RoutedEventArgs e)
        {
            gridok.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/level.png"))); //Меняем бэкграунд на игровой фон

            //Спавним игрока           
            gridok.Children.Add(player);
            gridok.Children.Remove(play); //Удаляем кнопку
            gridok.Children.Add(weapon_table); //Отоброжаем счётик БП      
           
        }
        

            public async void PlayCommand_Click(object sender, RoutedEventArgs e)
        {
            chat.Content += "\n" + console.Text; //Добовляем сообщение на панель сообщение с консоли
            string user_command = console.Text.Split();
            switch (console.Text)
            {
                                 
                case "clear": //Очищаем панель сообщений
                    chat.ClearValue(ContentProperty);
                
                    break;
                case "exit":
                    this.Close();
                    break;
                case "start":
                    Play_Click(null, null);
                    break;
                case "help":
                    chat.Content += "\nstart - Начать игру\nexit - Выйти\nclear - очистить консоль";
                        break;
                case "location":
                    chat.Content += $"\nLocation: {location}"; //Сообщение о смене локации
                    break;
                case "fullscreen":
                    switch (user_command[1])
                    {
                        case "on":
                            this.WindowStyle = WindowStyle.None;
                            this.WindowState = WindowState.Maximized;
                            break;
                        case "off":
                            this.WindowStyle = WindowStyle.SingleBorderWindow;
                            this.WindowState = WindowState.Normal;
                            break;
                    }
                   
                    break;             
                
                
            }
            console.Clear();
            gridok.Children.Remove(console);
            gridok.Children.Remove(console_button);
        }
public async void Dotnet_KeyDown(object sender, KeyEventArgs e)
        {
            //Будем поворачивать игрока с его орудием, когда будет нужно
            ScaleTransform flipTrans = new ScaleTransform();
            ScaleTransform flipTrans2 = new ScaleTransform();
            Player.general.RenderTransformOrigin = new Point(0.5, 0.5);
            Player.weapon.RenderTransformOrigin = new Point(0.5, 0.5);
            bot.general.RenderTransformOrigin = new Point(0.5, 0.5);
            Player.general.RenderTransform = flipTrans;
            Player.weapon.RenderTransform = flipTrans;
            bot.general.RenderTransform = flipTrans2;
            flipTrans.ScaleX = Player.player_flip;
            flipTrans2.ScaleX = bot.player_flip;


            switch (e.Key)
            {
                case Key.D: //Движение направо
                    //Поворачиваем игрока направо
                    flipTrans.ScaleX = 1;
                    Player.player_flip = 1;

                    Player.Walking(); //Запуск анимации хождения
                    count += Player.walk_speed; //Приписываем кол-ву хода скорсть хождения игрока
                    Canvas.SetLeft(Player.weapon, count + 82); //Движение орудия
                    Canvas.SetLeft(Player.general, count); //Движение игрока                  

                    //Cмена локаций
                    if (Canvas.GetLeft(Player.general) == 650 && location != "level_2")
                    { //Переход на вторую локацию с "бункером"

                        location = "level_2"; //Меняем локацию    
                        
                        gridok.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/level_2.png")));  //Отображаем другой бэкграунд
                        Canvas.SetLeft(Player.general, 0); //Устанавливаем позицию игрока на место
                        count = 0; //Обнуляем кол-во хода
                        gridok.Children.Add(woodenbox_obj); //Cпавним ящик
                        
                        gridok.Children.Add(player_npc);
                       
                        flipTrans.ScaleX = -1;
                        bot.player_flip = -1;
                        //Пересоздаём табло
                        gridok.Children.Remove(weapon_table); 
                        gridok.Children.Add(weapon_table);                       
                        bot.Say("Здраствуй, брат!\nЕсли ты хочешь\nвыжить в этой зоне\nтебе необходима\nбоевая подготовка\nНу как берёшся?");
                        while (true)
                        {
                            await Task.Delay(1);
                            if (chat.Content.ToString().Contains("да") || chat.Content.ToString().Contains("OK"))
                            {
                                bot.Say("Тогда держи ствол и стрельни\nв этот ящик");
                                Player.weapons[1] = colt_45;
                                Grid.SetRow(woodenbox_obj, 1);
                                break;

                            };
                        } 
                        
                    }
                    break;

                case Key.A: //Движение налево

                    flipTrans.ScaleX = -1;
                    Player.player_flip = -1;
                    Player.Walking();
                    count -= Player.walk_speed;

                    Canvas.SetLeft(Player.general, count);

                    Canvas.SetLeft(Player.weapon, count - 41);
                    if (Canvas.GetLeft(Player.general) == -5 && location != "level")
                    { //Переход обратно
                        location = "level";

                        gridok.Background = gridok.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/level.png")));
                        console.Text += "clear";
                        PlayCommand_Click(null, null);
                        Canvas.SetLeft(Player.general, 650);
                        count = 650;
                        gridok.Children.Remove(woodenbox_obj);
                        gridok.Children.Remove(player_npc);
                    }                   
                    break;

                case Key.Space: //Прыжок
                    flipTrans.ScaleX = Player.player_flip;

                    if (space_pressed == false)
                    {
                        space_pressed = true;
                        Canvas.SetTop(Player.general, jump_count);

                        Canvas.SetTop(Player.weapon, jump_count + 194);
                        Canvas.SetTop(bullet_obj, jump_count + 194);
                        jump_count += 50;
                        Canvas.SetTop(Player.general, jump_count);

                        Canvas.SetTop(Player.weapon, jump_count + 194);
                        Canvas.SetTop(bullet_obj, jump_count + 194);

                        for (int i = 0; i < 40; i++) //Анимация падения
                        {
                            await Task.Delay(1);
                            jump_count += 2;
                            Canvas.SetTop(Player.general, jump_count);

                            Canvas.SetTop(Player.weapon, jump_count + 70);
                            Canvas.SetTop(bullet_obj, jump_count + 70);
                        }

                        jump_count = 0;
                        space_pressed = false;
                    }

                    //x_coords.Content += "\n" + Canvas.GetTop(Player.general);

                    break;

                case Key.D2: //Клавиша "2" (переключение на пистолет)
                    flipTrans.ScaleX = Player.player_flip;                   
                    
                    Player.player_weapon = Player.weapons[1]; //Оружие игрока - пистолет
                    Player.weapon.Source = Player.player_weapon.Skin[0]; //его изображение                  
                    //Отображение счётика БП для данного орудия
                    weapon_table.Content = $"{Player.player_weapon.cartridge_name}\n--------------\n{Player.player_weapon.in_magazine_count} / {Player.player_weapon.cartridge_count}";
                    if (Player.weapon.Source == null)
                    {
                        weapon_table.Content = "";
                    }
                    break;
                case Key.D1:  //Клавиша "2" (переключение на винтовку)
                    flipTrans.ScaleX = Player.player_flip;
                    Player.player_weapon = Player.weapons[0];
                    Player.weapon.Source = Player.player_weapon.Skin[0];                 
                    weapon_table.Content = $"{Player.player_weapon.cartridge_name}\n--------------\n{Player.player_weapon.in_magazine_count} / {Player.player_weapon.cartridge_count}";
                    if (Player.weapon.Source == null)
                    {
                        weapon_table.Content = "";
                    }
                    break;
                case Key.Back: //Клавиша "Backspace" (оружие убрал!)
                    flipTrans.ScaleX = Player.player_flip;
                    Player.weapon.Source = null;
                    weapon_table.Content = "";
                    break;

                case Key.Enter:
                    //int side = 0;
                    flipTrans.ScaleX = Player.player_flip;
                    if (Player.weapon.Source != null) //Только если есть волына
                    {
                        Player.player_weapon.Fire(); //Огонь!
                        //Если кончились патроны стрелять не должно
                        if (Player.player_weapon.in_magazine_count != 0 && Player.player_weapon.in_magazine_count < Player.player_weapon.in_magazine) 
                        {
                            player.Children.Add(bullet_obj); //Спавн маслины                        
                                for (int i = 0; i < 5000; i += 500) //её полёт (пока только в одну сторону)
                                {
                                if(i > 1000)
                                {
                                    gridok.Children.Remove(woodenbox_obj);
                                    bot.Say("Ого! Держи патроны");                                  
                                    colt_45.cartridge_count += 20;
                                    break;
                                }
                                    await Task.Delay(1); //Прошла типа одна секунда                         
                                    Canvas.SetLeft(bullet_obj, count + 82 + i); //И быстро полетела (кто не поймал - я не виноват)                                  
                                }
                                
                            player.Children.Remove(bullet_obj); //Приземлилась
                        }                                              
                    }
                    break;
                case Key.OemTilde:
                    gridok.Children.Add(console); //Отображаем консольку
                    gridok.Children.Add(console_button); //Кнопку исполнения
                    Grid.SetColumnSpan(console, 5); //Расположение консоли
                    Grid.SetColumn(console_button, 6); //Расположение её кнопки
                    console_button.Content = "Play"; //Играть!
                    console_button.Click += PlayCommand_Click; //Подписываем на исполнение
                    console.Background = Brushes.Black; //Делаем брутально-чёрной
                    console.FontFamily = Fonts.SystemFontFamilies.First(x => x.ToString() == "Consolas"); //Харатерно консольный шрифт
                    console.Foreground = Brushes.White; 
                    console.Focus();  //Cтавим её на фокус                 
                    break;                                                      
            }
        }
    }
       class Player 
    {
        public static List<Weapon> weapons = new List<Weapon>() { new Weapon() { Skin = new BitmapImage[1] { null } }, new Weapon() { Skin = new BitmapImage[1] { null } } };
        public static Image general; //Тело игрока
        public static Image weapon; //его орудие (изображение)
        public static Weapon player_weapon; //Оружие игрока
        public static Label weapon_table { get; set; } //Счётик БП
        public static int player_flip = 0; //Сторона в которую повёрнут игрок
        public static int walk_speed = 5; //Скорость хождения

        public static async void  Walking() //Анимация хождения (смена картинок через определёное время)
        {
            
            general.Source = new BitmapImage(new Uri("pack://application:,,,/player_skin/player_walking/player_walking_1.png")); 
            await Task.Delay(20);
            general.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{new IniFile($"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}System.ini").Read("visual", "player")}", UriKind.RelativeOrAbsolute));
            await Task.Delay(5);
            general.Source = new BitmapImage(new Uri("pack://application:,,,/player_skin/player_walking/player_walking_2.png"));
            await Task.Delay(20);
            general.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{new IniFile($"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}System.ini").Read("visual", "player")}", UriKind.RelativeOrAbsolute));
        }       
    }
    class NPC : Player
    {
        public Image general;
        public Image weapon;
        public Weapon player_weapon;
        public Label chat;
        public int player_flip;
        public int walk_speed = 5;       

        public void Say(string text)
        {
            chat.Content += "\n" + text;
            
        }
        public async void Walking() //Анимация хождения (смена картинок через определёное время)
        {

            general.Source = new BitmapImage(new Uri("pack://application:,,,/player_skin/player_walking/player_armored_walking_1.png"));
            await Task.Delay(20);
            general.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{new IniFile($"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}System.ini").Read("visual", "player_npc")}", UriKind.RelativeOrAbsolute));
            await Task.Delay(5);
            general.Source = new BitmapImage(new Uri("pack://application:,,,/player_skin/player_walking/player_armored_walking_2.png"));
            await Task.Delay(20);
            general.Source = new BitmapImage(new Uri($@"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}/{new IniFile($"{Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}System.ini").Read("visual", "player_npc")}", UriKind.RelativeOrAbsolute));
        }

    }
   public class Weapon
    {
        public string Name; //Название оружия
        public BitmapImage[] Skin; //Его скин
        public Uri sound; //Его звук
        public int in_magazine;
        public int in_magazine_count;
        public string cartridge_name;
        public int cartridge_count;
        public  void Fire() //Огонь из оружия
        {
            if (Player.weapon.Source != null)
            {
                if (in_magazine_count == 0 && cartridge_count == 0)
                {
                    MediaPlayer sp = new MediaPlayer();
                    sp.Open(new Uri($@"{System.IO.Directory.GetCurrentDirectory().Replace(@"bin\Debug", "")}weapons\sounds\block.ogg"));
                    sp.Play();
                }
                if (in_magazine_count == 0)
                {
                    if (cartridge_count != 0)
                    {
                        cartridge_count -= in_magazine;
                        in_magazine_count += in_magazine;
                    }
                    Player.weapon_table.Content = $"{cartridge_name}\n--------------\n{in_magazine_count} / {cartridge_count}";
                }
                else
                {
                    in_magazine_count -= 1;
                    Player.weapon_table.Content = $"{cartridge_name}\n--------------\n{in_magazine_count} / {cartridge_count}";
                    Fire_animation();
                    MediaPlayer sp = new MediaPlayer();
                    sp.Open(sound);
                    sp.Play();
                }
            }
        }
        public async void Fire_animation() //Анимация огня
        {
            foreach (var item in Skin)
            {
                Player.weapon.Source = item;
                await Task.Delay(50);
            }
        }
    }
   
}
        


  

