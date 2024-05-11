using System;
using System.Windows;
using ApplicationManager.Views;

namespace ApplicationManager.Utils
{
    public class LoadingDialogHub
    {
        #region 懒汉单例模式

        private static readonly Lazy<LoadingDialogHub> Lazy = new Lazy<LoadingDialogHub>(() => new LoadingDialogHub());

        public static LoadingDialogHub Get => Lazy.Value;


        private LoadingDialogHub()
        {
        }

        #endregion
        
        private LoadingDialog _loadingDialog;

        public void ShowLoadingDialog(Window owner, string message)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                _loadingDialog = new LoadingDialog(message)
                {
                    Owner = owner
                };
                _loadingDialog.Show();
            });
        }

        public void DismissLoadingDialog()
        {
            Application.Current.Dispatcher.Invoke(delegate { _loadingDialog?.Close(); });
        }
    }
}