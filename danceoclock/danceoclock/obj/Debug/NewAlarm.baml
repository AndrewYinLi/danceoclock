   M S B A M L   `   `   `  ����F  Bdanceoclock, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:danceoclockdanceoclock  5   	        danceoclock.NewAlarmWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:danceoclock     5   	     � Title$  	New Alarm��5   	   $	��264��6   $��380.444��6(   .��5	      � $��4,0,6,10.889q�6   $��Center=�6!   $	��366��6>   .+�5   
   �� -   5         ��Name$ label��5      $��Music:��6   $
��Left=�60   $��	10,12,0,0q�6K   $	��Top=�6^   �� 5   
   -   5         ��Name$ browseMusicButton��5      $��	Browse...��6,   $
��Left=�6@   $��
272,14,0,0q�6[   $	��Top=�6o   $��75��6�   $��22��6�   6�   �� 5   
   -   5         ��Name$ musicPathTextBox��5      $
��Left=�6,   $��22��6G   $��	59,14,0,0q�6S    �� TextWrapping$
 Wrap=�6f   $
��None��6z   $	��Top=�6�   $	��203��6�   4  .System.Windows.Automation.AutomationProperties   Name$ b��6�   	� 5   
   -   5         	�Name$ amButton��5      $��AM��6(   $
��Left=�65   $��120,119,0,0q�6P   $	��Top=�6e    d� 	IsChecked$
 TrueY�6}   	� 5   
   -   5        $ pmButton��5      $��PM��6(   $
��Left=�65   $��163,119,0,0q�6P   $	��Top=�6e    J� RenderTransformOrigin$ -1.531,3.988&�6}   �� 5   
   -   5        $ label1��5      $��Time:��6    $
��Left=�60   $��
10,112,0,0q�6K   $	��Top=�6_   $ -0.147,0.558&�6w   $��2��6�   (  "System.Windows.Controls.DatePicker  5   
   -   5        	  Name$	 alarmDatePicker��5      $
��Left=�6.   $��	16,80,0,0q�6I   $	��Top=�6\   $	��331��6t   �� 5   
   -   5        $ hoursTextBox��5      $
��Left=�6(   $��20��6C   $��
50,116,0,0q�6O   $
 Wrap=�6c   $��h��6w   $	��Top=�6�   $��27��6�   6�   �� 5   
   -	   5        $ minutesTextBox��5      $
��Left=�6*   $��20��6E   $��
88,116,0,0q�6Q   $
 Wrap=�6e   $��m��6y   $	��Top=�6�   $��27��6�   6�   �� 5   
   -
   5        $ label2��5      $��:��6    $
��Left=�6,   $��
75,111,0,0q�6G   $	��Top=�6[   $��2��6s   �� 5   
   -   5        $ actionTextBox��5      $
��Left=�6)   $��22��6D   $��	59,48,0,0q�6P   $
��None��6c   $	��Top=�6o   $	��203��6�   $ b��6�   �� 5   
   -   5        $ 
label_Copy��5      $��Action:��6$   $
��Left=�66   $��	10,45,0,0q�6Q   $	��Top=�6d   �� 5   
   -   5        $ browseActionTextBox��5      $��	Browse...��6.   $
��Left=�6B   $��
272,48,0,0q�6]   $	��Top=�6q   $��75��6�   $��22��6�   6�   �� 5   
   -   5        $ label_Copy2��5      $��Repetitions:��6%   $
��Left=�6<   $��232,112,0,0q�6W   $	��Top=�6l   
 � ToolTip$<
 6Number of times that the movement routine is repeated.��6�   $��2��6�   �� 5   
   -   5        $ RepBox��5      $
��Left=�6"   $��23��6=   $
 Wrap=�6I   $	��Top=�6]   $��43��6u   $��304,113,0,0q�6�   $��1��6�   �� 5   
   -   5        $ createAlarmButton��5      $��Create��6,   $
��Left=�6=   $��16,178.222,0,0q�6X   $	��Top=�6p   $	��331��6�   $��39��6�   �� 5   
   -   5        $ label_Copy1��5      $��Tolerance (degrees):��6%   $
��Left=�6D   $��10,143.221,0,0q�6_   $	��Top=�6w   $Z
 TTolerance for matching movements, in terms of the angles between joints in the body.��6�   �� 5   
   -   5        $ ToleranceBox��5      $
��Left=�6(   $��22��6C   $
 Wrap=�6O   $	��Top=�6c   $��32��6{   $��126,147.222,0,0q�6�   $��35��6�   �� 5   
   -   5        $ 
MaxtimeBox��5      $
��Left=�6&   $��23��6A   $
 Wrap=�6M   $	��Top=�6a   $��32��6y   $��294,146.222,0,0q�6�   $��5��6�   �� 5   
   -   5        $ label_Copy3��5      $��Maximum Frame Time:��6%   $
��Left=�6C   $��163,143.221,0,0q�6^   $	��Top=�6w   $X
 RMaximum amount of time spent on matching each movement frame before it is skipped.��6�   �� 5   
   -   5        $ label_Copy4��5      $	��sec��6%   $
��Left=�63   $��325,143.221,-9,0q�6N   $	��Top=�6h   $
  ��6�   $��34��6�   5!      5"      