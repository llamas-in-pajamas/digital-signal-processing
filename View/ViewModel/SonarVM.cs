namespace View.ViewModel
{
    public class SonarVM
    {

        #region Props

        public MainWindowVM Parent { get; set; }

        #endregion
        

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="parent"></param>
        public SonarVM(MainWindowVM parent)
        {
            Parent = parent;
        }

        #region Methods

        

        #endregion

    }
}