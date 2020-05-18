using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ItViteaYahtzee
{
    public class ScoreBarDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is ScoreBar)
            {
                ScoreBar baritem = item as ScoreBar;

                if (baritem.AllowClick == false)
                    return
                        element.FindResource("DataTGridBar2") as DataTemplate;
                else
                {
                    //Return Yahtzee GridBar for index 14.
                    if (baritem.Index == 14)
                        return
                            element.FindResource("DataTGridBar3") as DataTemplate;
                    else
                        return 
                            element.FindResource("DataTGridBar1") as DataTemplate;
                }
                   
            }

            return null;
        }
        
    }
}
