using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using FirstFloor.ModernUI.Presentation;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace FirstFloor.ModernUI.App
{
    public class CTModernWindow : ModernWindow, IContent
    {
        public ContentLoaderCallback.delegateLoadContent OnLoadContent {
            get { return this.ContentLoader.OnLoadContent; }
            set { this.ContentLoader.OnLoadContent = value; } }

        public event EventHandler<NavigationEventArgs>          NavigatedFrom;
        public event EventHandler<NavigationEventArgs>          NavigatedTo;
        public event EventHandler<FragmentNavigationEventArgs>  FragmentNavigation;
        public event EventHandler<NavigatingCancelEventArgs>    NavigatingFrom;

        /// <summary>
        /// Called when navigation to a content fragment begins.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        public void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
            if (FragmentNavigation != null)
            {
                FragmentNavigation.Invoke(this, e);
            }
        }
        /// <summary>
        /// Called when this instance is no longer the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (NavigatedFrom != null)
            {
                NavigatedFrom.Invoke(this, e);
            }
        }
        /// <summary>
        /// Called when a this instance becomes the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        public void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigatedTo != null)
            {
                NavigatedTo.Invoke(this, e);
            }
        }
        /// <summary>
        /// Called just before this instance is no longer the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        /// <remarks>The method is also invoked when parent frames are about to navigate.</remarks>
        public void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (NavigatingFrom != null)
            {
                NavigatingFrom.Invoke(this, e);
            }
        }
    }

    public class CTUserContorl : UserControl, IContent
    {
        /*
        public ContentLoaderCallback.delegateLoadContent OnLoadContent
        {
            get { return this.ContentLoader.OnLoadContent; }
            set { this.ContentLoader.OnLoadContent = value; }
        }
        */
        public event EventHandler<NavigationEventArgs> NavigatedFrom;
        public event EventHandler<NavigationEventArgs> NavigatedTo;
        public event EventHandler<FragmentNavigationEventArgs> FragmentNavigation;
        public event EventHandler<NavigatingCancelEventArgs> NavigatingFrom;
        
        /// <summary>
        /// Called when navigation to a content fragment begins.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        public void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
            if (FragmentNavigation != null)
            {
                FragmentNavigation.Invoke(this, e);
            }
        }
        /// <summary>
        /// Called when this instance is no longer the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (NavigatedFrom != null)
            {
                NavigatedFrom.Invoke(this, e);
            }
        }
        /// <summary>
        /// Called when a this instance becomes the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        public void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigatedTo != null)
            {
                NavigatedTo.Invoke(this, e);
            }
        }
        /// <summary>
        /// Called just before this instance is no longer the active content in a frame.
        /// </summary>
        /// <param name="e">An object that contains the navigation data.</param>
        /// <remarks>The method is also invoked when parent frames are about to navigate.</remarks>
        public void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (NavigatingFrom != null)
            {
                NavigatingFrom.Invoke(this, e);
            }
        }

        /*
        /// <summary>
        /// Defines the ContentSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentSourceProperty = DependencyProperty.Register("ContentSource", typeof(Uri), typeof(ModernWindow));
        /// <summary>
        /// Identifies the ContentLoader dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentLoaderProperty = DependencyProperty.Register("ContentLoader", typeof(IContentLoader), typeof(ModernWindow), new PropertyMetadata(new DefaultContentLoader()));
        /// <summary>
        /// Identifies the LinkNavigator dependency property.
        /// </summary>
        /// <summary>
        /// Identifies the LinkNavigator dependency property.
        /// </summary>
        public static DependencyProperty LinkNavigatorProperty = DependencyProperty.Register("LinkNavigator", typeof(ILinkNavigator), typeof(ModernWindow), new PropertyMetadata(new DefaultLinkNavigator()));


        /// <summary>
        /// Gets or sets the source uri of the current content.
        /// </summary>
        public Uri ContentSource
        {
            get { return (Uri)GetValue(ContentSourceProperty); }
            set { SetValue(ContentSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content loader.
        /// </summary>
        public IContentLoader ContentLoader
        {
            get { return (IContentLoader)GetValue(ContentLoaderProperty); }
            set { SetValue(ContentLoaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the link navigator.
        /// </summary>
        /// <value>The link navigator.</value>
        public ILinkNavigator LinkNavigator
        {
            get { return (ILinkNavigator)GetValue(LinkNavigatorProperty); }
            set { SetValue(LinkNavigatorProperty, value); }
        }
        */
    }

    public static class CTUtils
    {
        public static void SetUserControlItems_LoadContent(UserControl ctrl, ContentLoaderCallback.delegateLoadContent callback)
        {
            List<object> ModernTab_items = GetControlChildObjects_byTypeName(ctrl, "FirstFloor.ModernUI.Windows.Controls.ModernTab");
            List<object> ModernFrame_items = GetControlChildObjects_byTypeName(ctrl, "FirstFloor.ModernUI.Windows.Controls.ModernFrame");
            List<object> ModernWindow_items = GetControlChildObjects_byTypeName(ctrl, "FirstFloor.ModernUI.Windows.Controls.ModernWindow");
            List<object> CTModernWindow_items = GetControlChildObjects_byTypeName(ctrl, "FirstFloor.ModernUI.App.CTModernWindow");
            //List<object> CTUserContorl_items = GetControlChildObjects_byTypeName(ctrl, "FirstFloor.ModernUI.App.CTUserContorl");
            ModernTab_items.ForEach(delegate (object o) { ((ModernTab)o).ContentLoader.OnLoadContent = callback; });
            ModernFrame_items.ForEach(delegate (object o) { ((ModernFrame)o).ContentLoader.OnLoadContent = callback; });
            ModernWindow_items.ForEach(delegate (object o) { ((ModernWindow)o).ContentLoader.OnLoadContent = callback; });
            CTModernWindow_items.ForEach(delegate (object o) { ((CTModernWindow)o).ContentLoader.OnLoadContent = callback; });
            //CTUserContorl_items.ForEach(delegate (object o) { ((CTUserContorl)o).ContentLoader.OnLoadContent = callback; });
        }
        //一、查找某种类型的子控件，并返回一个List集合
        public static List<T> GetChildObjects_byType<T>(DependencyObject obj, Type type) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).GetType() == type))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects_byType<T>(child, type));
            }
            return childList;
        }

        public static List<object> GetChildObjects_byTypeName(DependencyObject obj, string Typename)
        {
            DependencyObject child = null;
            List<object> childList = new List<object>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (((child).GetType().ToString() == Typename))
                {
                    childList.Add(child);
                }
                childList.AddRange(GetChildObjects_byTypeName(child, Typename));
            }
            return childList;
        }
        //调用：List<Button> listButtons = GetChildObjects<Button>(parentPanel, typeof(Button));

        public static List<object> GetControlChildObjects_byTypeName(UserControl ctrl, string Typename)
        {
            //DependencyObject obj = null;
            if (!(ctrl.Content is DependencyObject)) return new List<object>();
            return GetChildObjects_byTypeName((DependencyObject)ctrl.Content, Typename);
        }

        //二、通过名称查找子控件，并返回一个List集合
        public static List<T> GetChildObjects_byName<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (child.GetType().ToString() == name | string.IsNullOrEmpty(name)))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects_byName<T>(child, name));
            }
            return childList;
        }
        //调用：List<Button> listButtons = GetChildObjects<Button>(parentPanel, "button1");

        //三、通过名称查找某子控件：
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
        }
        //调用：StackPanel sp = GetChildObject<StackPanel>(this.LayoutRoot, "spDemoPanel");
        public static Control GetChildControl(DependencyObject obj, string name) //where T : FrameworkElement
        {
            DependencyObject child = null;
            Control grandChild = null;

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is Control && (((Control)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (Control)child;
                }
                else
                {
                    grandChild = GetChildControl(child, name);
                    if (grandChild != null)
                    {
                        return grandChild;
                    }
                }
            }
            return null;
        }

        public static LinkGroup MenuAddGroup(LinkGroupCollection groups, string display)
        {
            LinkGroup group = new LinkGroup();
            group.DisplayName = display;
            return group;
        }

        public static void MenuAddLink(LinkGroup group, string display, string source)
        {
            Link link = new Link();
            link.DisplayName = display;
            link.Source = new Uri(source, System.UriKind.Relative);
            group.Links.Add(link);
        }

        public static void TabAddLink(ModernTab tab, string display, string source)
        {
            Link link = new Link();
            link.DisplayName = display;
            link.Source = new Uri(source, System.UriKind.Relative);
            tab.Links.Add(link);
        }

    }

    public class CTObservableCollection : ObservableCollection<object>
    {

    }
}
