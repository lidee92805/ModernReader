﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OKr.Common.Storage;
using OKr.Win8Book.Client.Core;
using OKr.Win8Book.Client.Core.Builder;
using OKr.Win8Book.Client.Core.Context;
using OKr.Win8Book.Client.Core.Data;
using Sina.View.Common.Toast;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OKr.Win8Book.Client.View
{
    public sealed partial class Viewer : Windows.UI.Xaml.Controls.Page
    {
        public Viewer()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var category = e.Parameter as Chapter;
            if (e.NavigationMode == NavigationMode.New)
            {
                this.pageTitle.Text = category.Title;
                this.currentChapter = category.ChapterNo;

                this.chapter = category;
                this.current = 0;

                this.book = await bc.Load();
                this.chapter = await LoadData(this.currentChapter);
                chapter.CurrentPage = chapter.Pages[this.current];

                this.DataContext = chapter;

                this.mark = await mc.Load();
                this.progress = await pc.Load();
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }

        private void bodyGrid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            //double x = e.Position.X;
            //double y = e.Position.Y;

            //if (x < 200.0)
            //{
            //    if (this.current > 0)
            //    {
            //        this.current--;
            //        this.ChangePage();
            //        //this.pageno1.Text = this.pageno2.Text;
            //        //this.pageno2.Text = string.Concat(new object[] { this.index + 1, "/", this.chapter.PageNum, OkrConstant.PAGE });
            //        this.SetPage();
            //    }
            //    else if (this.currentChapter >= 1)
            //    {
            //        this.currentChapter--;
            //        //this.title1.Text = this.title2.Text;
            //        //this.LoadData(this.currentChapter);
            //        this.current = this.chapter.PageNum - 1;
            //        this.ChangePage();
            //        //this.pageno1.Text = this.pageno2.Text;
            //        //this.pageno2.Text = string.Concat(new object[] { this.index + 1, "/", this.chapter.PageNum, OkrConstant.PAGE });
            //        this.SetChapter();                    
            //    }
            //    else
            //    {
            //        //MessageBox.Show(OkrConstant.FIRSTPAGEERR);
            //    }
            //}
            //else if (x > 280.0)
            //{
            //    if (this.current <= (this.chapter.PageNum - 2))
            //    {
            //        this.current++;
            //        this.ChangePage();
            //        //this.pageno1.Text = this.pageno2.Text;
            //        //this.pageno2.Text = string.Concat(new object[] { this.index + 1, "/", this.chapter.PageNum, OkrConstant.PAGE });
            //        this.SetPage();
            //    }
            //    else if (this.currentChapter < (this.book.Chapters.Count - 1))
            //    {
            //        this.currentChapter++;
            //        this.current = 0;
            //        //this.pageno1.Text = this.pageno2.Text;
            //        //this.title1.Text = this.title2.Text;
            //        //this.LoadData(this.currentChapter);
            //        this.ChangePage();
            //        //this.pageno2.Text = string.Concat(new object[] { this.index + 1, "/", this.chapter.PageNum, OkrConstant.PAGE });
            //        //this.SetChapter();
            //    }
            //    else
            //    {
            //        // MessageBox.Show(OkrConstant.LASTPAGEERR);
            //    }
            //}
        }

        private void ChangePage()
        {
        }

        private async void SetChapter()
        {
            this.progress.Chapter = this.currentChapter;
            this.progress.Page = this.current;
            this.progress.Percent = 0.0;

            await pc.Save(this.progress);
        }

        private void bodyGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

        }

        private void bodyGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Velocities.Linear.X > 2)
            {
                PrePage();
            }
            else if (e.Velocities.Linear.X < -2)
            {
                NextPage();
            }

            this.SetPage();

        }

        private void OnFontChange(object sender, RoutedEventArgs e)
        {

        }

        private void OnMark(object sender, RoutedEventArgs e)
        {
            Mark m = this.chapter.Mark;

            if (m == null)
            {
                m = new Mark();
                this.chapter.Mark = m;
            }

            ChapterMark item = m.Marks.FirstOrDefault(x => x.Current == this.current);

            if (item == null)
            {
                item = new ChapterMark();
                item.ChapterNo = this.currentChapter;
                item.Title = this.chapter.Title;
                item.Date = DateTime.Now.ToString("yyyy/mm/dd hh:MM:ss");
                item.Current = this.current;
                item.Percent = ((double)this.current) / ((double)this.chapter.PageNum);
                // At.Okr.Client.Core.Data.Page setting = this.chapter.Pages[this.index];
                // item.Content = setting.Row[0].Trim() + setting.Row[1].Trim();
                this.mark.Marks.Add(item);
                m.Marks.Add(item);
            }
            else
            {
                int chapterNo = item.ChapterNo;
                List<ChapterMark> list = new List<ChapterMark>();
                for (int i = 0; i < this.mark.Marks.Count; i++)
                {
                    if ((this.mark.Marks[i].ChapterNo != chapterNo) || (this.mark.Marks[i].Current != this.current))
                    {
                        list.Add(this.mark.Marks[i]);
                    }
                }
                this.mark.Marks.Clear();
                this.mark.Marks = list;
                List<ChapterMark> list2 = new List<ChapterMark>();
                for (int j = 0; j < m.Marks.Count; j++)
                {
                    if (m.Marks[j].Current != this.current)
                    {
                        list2.Add(m.Marks[j]);
                    }
                }
                this.chapter.Mark.Marks.Clear();
                this.chapter.Mark.Marks = list2;
            }

            // IsolatedStorageSettings.ApplicationSettings["marks"] = this.mark;
            // IsolatedStorageSettings.ApplicationSettings["bookinfo"] = this.book;

            this.ShowMark();

            this.mc.Save(this.mark);
        }

        private async Task<Chapter> LoadData(int index)
        {
            int[] count = this.GetCounts(this.fontsize);

            OKrStorage storage = new OKrStorage();

            Windows.ApplicationModel.Package package = Windows.ApplicationModel.Package.Current;
            Windows.Storage.StorageFolder installedLocation = package.InstalledLocation;

            this.chapter = this.book.Chapters[index];

            var file = await StorageFile.GetFileFromPathAsync(Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, @"Assets\Data\book\" + this.chapter.FileName + ".txt"));

            var content = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);

            Chapter chapter = null;
            if (content != null)
            {
                chapter = TextParser.GetChapter(content, count);
            }

            return chapter;
        }

        private int[] GetCounts(int fontSize)
        {
            int[] numArray = new int[2];
            int num = 0;
            int num2 = 0;
            int num3 = (fontSize * 0x20) / 100;
            num2 = 460 / (fontSize - num3);
            num = this.height / (fontSize + this.lineHeight);
            numArray[0] = num;
            numArray[1] = num2;
            return numArray;
        }

        private void NextPage()
        {
            if (this.current >= this.chapter.Pages.Count - 1)
            {
                OKrToast.Show("已经是最后一页了。");
            }
            else
            {
                this.current++;

                this.chapter.CurrentPage = this.chapter.Pages[this.current];
            }            
        }

        private void PrePage()
        {
            if (this.current <= 0)
            {
                OKrToast.Show("已经是第一页了。");
            }
            else
            {
                this.current--;

                this.chapter.CurrentPage = this.chapter.Pages[this.current];
            }         
        }

        private void ShowMark()
        {
            Mark mark = this.chapter.Mark;
            ChapterMark currentMark = null;
            if (mark != null)
            {
                foreach (var item in mark.Marks)
                {
                    if (item.Current == this.current)
                    {
                        currentMark = item;
                        break;
                    }
                }
            }

            //ImageBrush brush = new ImageBrush();
            //if (currentMark != null)
            //{
            //    brush.ImageSource = new BitmapImage(new Uri("/_static/img/okr-marked.png", UriKind.Relative));
            //}
            //else
            //{
            //    brush.ImageSource = new BitmapImage(new Uri("/_static/img/okr-marking.png", UriKind.Relative));
            //}

            //Dispatcher.BeginInvoke(delegate
            //{
            //    this.markbtn.Source = brush.ImageSource;
            //});
        }

        private void SetPage()
        {
            this.progress.Page = this.current;
            this.progress.Percent = ((double)this.current) / ((double)this.chapter.PageNum);
            pc.Save(this.progress);
        }

        private int fontsize = OKrBookConfig.DEFALUTFONTSIZE;
        private int height = OKrBookConfig.HEIGHT; //762;
        private int lineHeight = OKrBookConfig.LINEHEIGHT; //0x10;
        private int currentChapter;

        MarkContext mc = new MarkContext();
        ProgressContext pc = new ProgressContext();
        BookContext bc = new BookContext();

        private Progress progress;
        private Book book;
        private Mark mark;
        private Chapter chapter;

        private int current;
    }
}