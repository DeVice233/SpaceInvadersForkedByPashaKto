using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp25
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool goLeft, goRight;
        bool goLeftFrnd, goRightFrnd;
        List<Rectangle> itemsToRemove = new List<Rectangle>();
        int ImagesOfEnemy = 0;
        int bulletTimer = 0;
        int bulletTimerLimit = 90;
        int totalEnemies = 0;
        int totalFriends = 1;
        int enemySpeed = 6;
        bool gameOver = false;
        DispatcherTimer gameTimer = new DispatcherTimer();
        ImageBrush playerSkin = new ImageBrush();
        ImageBrush friendSkin = new ImageBrush();

        public MainWindow()
        {
            InitializeComponent();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(30);
            gameTimer.Start();
            playerSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Pasha (1).png"));
            friendSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player.png"));
            player.Fill = playerSkin;
            friend.Fill = friendSkin;
            myCanvas.Focus();
            MakeEnemies(40);
        }

        private void GameLoop(object sender, EventArgs e) // начало игрового цикла
        {
            Rect playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            Rect friendHitBox = new Rect(Canvas.GetLeft(friend), Canvas.GetTop(friend), friend.Width, friend.Height);
            enemiesLeft.Content = "Осталось всего пришельцев: " + totalEnemies;

            if (goLeft == true && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);
            }

            if (goLeftFrnd == true && Canvas.GetLeft(friend) > 0)
            {
                Canvas.SetLeft(friend, Canvas.GetLeft(friend) - 10);
            }

            if (goRightFrnd == true && Canvas.GetLeft(friend) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(friend, Canvas.GetLeft(friend) + 10);
            }

            if (goRight == true && Canvas.GetLeft(player) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
            }
            bulletTimer -= 3;
            if (bulletTimer < 0)
            {
                EnemyBulletMaker(Canvas.GetLeft(player) + 20, 10);
                EnemyBulletMaker(Canvas.GetLeft(friend) + 20, 10);
                Random random = new Random();
                EnemyBulletMaker(random.Next(0, 500), random.Next(3, 10));
                EnemyBulletMaker(random.Next(0 , 500), random.Next(3, 10));
                EnemyBulletMaker(random.Next(0, 500), random.Next(3,10));
                bulletTimer = bulletTimerLimit;
            }
            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet")
                {

                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemsToRemove.Add(x);
                    }

                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bullet.IntersectsWith(enemyHit))
                            {
                                itemsToRemove.Add(x);
                                itemsToRemove.Add(y);
                                totalEnemies -= 1;
                            }
                        }
                    }
                }
                if (x is Rectangle && (string)x.Tag == "bulletFrnd")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);
                    if (Canvas.GetTop(x) < 10)
                    {
                        itemsToRemove.Add(x);
                    }
                    Rect bulletFrnd = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bulletFrnd.IntersectsWith(enemyHit))
                            {
                                itemsToRemove.Add(x);
                                itemsToRemove.Add(y);
                                totalEnemies -= 1;
                            }
                        }
                    }
                }
                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + enemySpeed);

                    if (Canvas.GetLeft(x) > 820)
                    {
                        Canvas.SetLeft(x, -80);
                        Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));
                    }
                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (playerHitBox.IntersectsWith(enemyHitBox) || friendHitBox.IntersectsWith(enemyHitBox))
                    {
                        ShowGameOver("Пришельцы захватили мир!!");
                    }
                }
                if (x is Rectangle && (string)x.Tag == "enemyBullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 10);
                    if (Canvas.GetTop(x) > 480)
                    {
                        itemsToRemove.Add(x);
                    }
                    Rect enemyBulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), 20, 30);
                    if (playerHitBox.IntersectsWith(enemyBulletHitBox) || friendHitBox.IntersectsWith(enemyBulletHitBox))
                    {
                        ShowGameOver("Пришельцы испепелили вас!!");
                    }
                }
            }
            foreach (Rectangle i in itemsToRemove)
            {
                myCanvas.Children.Remove(i);
            }

            if (totalEnemies < 30)
            {
                enemySpeed = 8;
            }

            if(totalEnemies < totalEnemies / 100 * 50)
            {
                enemySpeed = 30;
            }

            if (totalEnemies < totalEnemies / 100 * 30)
            {
                enemySpeed = 12;
            }        

            if (totalEnemies < totalEnemies / 100 * 10)
            {
                enemySpeed = 16;
            }

            if (totalEnemies < 1)
            {
                ShowGameOver("Поздравляю вы выиграли!");
            }
        } //конец цикла
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = true;
            }

            if (e.Key == Key.A)
            {
                goLeftFrnd = true;
            }

            if (e.Key == Key.D)
            {
                goRightFrnd = true;
            }

            if (e.Key == Key.Right)
            {
                goRight = true;
            }
        }
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = false;
            }

            if (e.Key == Key.A)
            {
                goLeftFrnd = false;
            }

            if (e.Key == Key.D)
            {
                goRightFrnd = false;
            }

            if (e.Key == Key.Right)
            {
                goRight = false;
            }

            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle { Tag = "bullet",Height = 20,Width = 5, Fill = Brushes.Orange,  Stroke = Brushes.Red };
                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);
                myCanvas.Children.Add(newBullet);
            }

            if (e.Key == Key.LeftCtrl)
            {
                Rectangle newBulletFrnd = new Rectangle { Tag = "bulletFrnd", Height = 20, Width = 5, Fill = Brushes.Orange, Stroke = Brushes.Red };
                Canvas.SetTop(newBulletFrnd, Canvas.GetTop(friend) - newBulletFrnd.Height);
                Canvas.SetLeft(newBulletFrnd, Canvas.GetLeft(friend) + friend.Width / 2);
                myCanvas.Children.Add(newBulletFrnd);
            }

            if (e.Key == Key.Enter && gameOver == true)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void EnemyBulletMaker(double x, double y)
        {
            Rectangle enemyBullet = new Rectangle { Tag = "enemyBullet", Height = 40, Width = 15, Fill = Brushes.Red, Stroke = Brushes.OrangeRed, StrokeThickness = 5 };
            Canvas.SetTop(enemyBullet, y);
            Canvas.SetLeft(enemyBullet, x);
            myCanvas.Children.Add(enemyBullet);
        }

        private void makeFriend()
        {
            int right = 0;
            totalFriends = 1;
            Rectangle newFriend = new Rectangle { Tag = "friend", Height = 65, Width = 50,Fill = friendSkin };
        }

        private void MakeEnemies(int limit)
        {
            int left = 0;
            totalEnemies = limit;
            for (int i = 0; i < limit; i++)
            {
                ImageBrush enemySkin = new ImageBrush();
                Rectangle newEnemy = new Rectangle { Tag = "enemy", Height = 45, Width = 45, Fill = enemySkin };
                Canvas.SetTop(newEnemy, 0);
                Canvas.SetLeft(newEnemy, left);
                myCanvas.Children.Add(newEnemy);
                left -= 60;
                ImagesOfEnemy++;

                if (ImagesOfEnemy > 4)
                {
                    ImagesOfEnemy = 1;
                }
                switch (ImagesOfEnemy)
                {
                    case 1:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Pasha1.png"));
                        break;
                    case 2:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Pasha21.png"));
                        break;
                    case 3:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Pasha3.png"));
                        break;
                    case 4:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Pasha_4.png"));
                        break;
                    default:
                        MessageBox.Show("jgcjfchjg");
                        break;
                }
            }
        }
        private void ShowGameOver(string message)
        {
            gameOver = true;
            gameTimer.Stop();
            enemiesLeft.Content =  message + " Нажмите Enter чтобы снова играть";
        }
    }
}

