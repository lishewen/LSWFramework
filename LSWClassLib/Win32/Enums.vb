Imports System.Runtime.InteropServices

Namespace Win32
    ''' <summary>
    ''' Windows 消息列表
    ''' </summary>
    Public Enum WindowsMessage As Integer
        ''' <summary>
        ''' 
        ''' </summary>
        WM_NULL = &H0
        ''' <summary>
        ''' 应用程序创建一个窗口 
        ''' </summary>
        WM_CREATE = &H1
        ''' <summary>
        ''' 一个窗口被销毁 
        ''' </summary>
        WM_DESTROY = &H2
        ''' <summary>
        ''' 移动一个窗口
        ''' </summary>
        WM_MOVE = &H3
        ''' <summary>
        ''' 改变一个窗口的大小
        ''' </summary>
        WM_SIZE = &H5
        ''' <summary>
        ''' 一个窗口被激活或失去激活状态；
        ''' </summary>
        WM_ACTIVATE = &H6
        ''' <summary>
        ''' 获得焦点后 
        ''' </summary>
        WM_SETFOCUS = &H7
        ''' <summary>
        ''' 失去焦点
        ''' </summary>
        WM_KILLFOCUS = &H8
        ''' <summary>
        ''' 改变enable状态
        ''' </summary>
        WM_ENABLE = &HA
        ''' <summary>
        ''' 设置窗口是否能重画
        ''' </summary>
        WM_SETREDRAW = &HB
        ''' <summary>
        ''' 应用程序发送此消息来设置一个窗口的文本
        ''' </summary>
        WM_SETTEXT = &HC
        ''' <summary>
        ''' 应用程序发送此消息来复制对应窗口的文本到缓冲区
        ''' </summary>
        WM_GETTEXT = &HD
        ''' <summary>
        ''' 得到与一个窗口有关的文本的长度（不包含空字符）
        ''' </summary>
        WM_GETTEXTLENGTH = &HE
        ''' <summary>
        ''' 要求一个窗口重画自己
        ''' </summary>
        WM_PAINT = &HF
        ''' <summary>
        ''' 当一个窗口或应用程序要关闭时发送一个信号
        ''' </summary>
        WM_CLOSE = &H10
        ''' <summary>
        ''' 当用户选择结束对话框或程序自己调用ExitWindows函数
        ''' </summary>
        WM_QUERYENDSESSION = &H11
        ''' <summary>
        ''' 用来结束程序运行或当程序调用postquitmessage函数 
        ''' </summary>
        WM_QUIT = &H12
        ''' <summary>
        ''' 当用户窗口恢复以前的大小位置时，把此消息发送给某个图标
        ''' </summary>
        WM_QUERYOPEN = &H13
        ''' <summary>
        ''' 当窗口背景必须被擦除时（例在窗口改变大小时）
        ''' </summary>
        WM_ERASEBKGND = &H14
        ''' <summary>
        ''' 当系统颜色改变时，发送此消息给所有顶级窗口
        ''' </summary>
        WM_SYSCOLORCHANGE = &H15
        ''' <summary>
        ''' 当系统进程发出WM_QUERYENDSESSION消息后，此消息发送给应用程序，通知它对话是否结束 
        ''' </summary>
        WM_ENDSESSION = &H16
        ''' <summary>
        ''' 
        ''' </summary>
        WM_SYSTEMERROR = &H17
        ''' <summary>
        ''' 当隐藏或显示窗口是发送此消息给这个窗口 
        ''' </summary>
        WM_SHOWWINDOW = &H18
        ''' <summary>
        ''' 发此消息给应用程序哪个窗口是激活的，哪个是非激活的；
        ''' </summary>
        WM_ACTIVATEAPP = &H1C
        ''' <summary>
        ''' 当系统的字体资源库变化时发送此消息给所有顶级窗口 
        ''' </summary>
        WM_FONTCHANGE = &H1D
        ''' <summary>
        ''' 当系统的时间变化时发送此消息给所有顶级窗口
        ''' </summary>
        WM_TIMECHANGE = &H1E
        ''' <summary>
        ''' 发送此消息来取消某种正在进行的摸态（操作） 
        ''' </summary>
        WM_CANCELMODE = &H1F
        ''' <summary>
        ''' 如果鼠标引起光标在某个窗口中移动且鼠标输入没有被捕获时，就发消息给某个窗口 
        ''' </summary>
        WM_SETCURSOR = &H20
        ''' <summary>
        ''' 当光标在某个非激活的窗口中而用户正按着鼠标的某个键发送此消息给当前窗口 
        ''' </summary>
        WM_MOUSEACTIVATE = &H21
        ''' <summary>
        ''' 发送此消息给MDI子窗口当用户点击此窗口的标题栏，或当窗口被激活，移动，改变大小 
        ''' </summary>
        WM_CHILDACTIVATE = &H22
        ''' <summary>
        ''' 此消息由基于计算机的训练程序发送，通过WH_JOURNALPALYBACK的hook程序分离出用户输入消息
        ''' </summary>
        WM_QUEUESYNC = &H23
        ''' <summary>
        ''' 此消息发送给窗口当它将要改变大小或位置； 
        ''' </summary>
        WM_GETMINMAXINFO = &H24
        ''' <summary>
        ''' 发送给最小化窗口当它图标将要被重画 
        ''' </summary>
        WM_PAINTICON = &H26
        ''' <summary>
        ''' 此消息发送给某个最小化窗口，仅当它在画图标前它的背景必须被重画
        ''' </summary>
        WM_ICONERASEBKGND = &H27
        ''' <summary>
        ''' 发送此消息给一个对话框程序去更改焦点位置
        ''' </summary>
        WM_NEXTDLGCTL = &H28
        ''' <summary>
        ''' 每当打印管理列队增加或减少一条作业时发出此消息 
        ''' </summary>
        WM_SPOOLERSTATUS = &H2A
        ''' <summary>
        ''' 当button，combobox，listbox，menu的可视外观改变时发送此消息给这些空件的所有者
        ''' </summary>
        WM_DRAWITEM = &H2B
        ''' <summary>
        ''' 当button, combo box, list box, list view control, or menu item 被创建时 
        ''' 发送此消息给控件的所有者 
        ''' </summary>
        WM_MEASUREITEM = &H2C
        ''' <summary>
        ''' 当the list box 或 combo box 被销毁 或 当某些项被删除通过LB_DELETESTRING, LB_RESETCONTENT, 
        ''' CB_DELETESTRING, or CB_RESETCONTENT 消息 
        ''' </summary>
        WM_DELETEITEM = &H2D
        ''' <summary>
        ''' 此消息有一个LBS_WANTKEYBOARDINPUT风格的发出给它的所有者来响应WM_KEYDOWN消息 
        ''' </summary>
        WM_VKEYTOITEM = &H2E
        ''' <summary>
        ''' 此消息由一个LBS_WANTKEYBOARDINPUT风格的列表框发送给他的所有者来响应WM_CHAR消息
        ''' </summary>
        WM_CHARTOITEM = &H2F
        ''' <summary>
        ''' 当绘制文本时程序发送此消息得到控件要用的颜色
        ''' </summary>
        WM_SETFONT = &H30
        ''' <summary>
        ''' 应用程序发送此消息得到当前控件绘制文本的字体 
        ''' </summary>
        WM_GETFONT = &H31
        ''' <summary>
        ''' 应用程序发送此消息让一个窗口与一个热键相关连 
        ''' </summary>
        WM_SETHOTKEY = &H32
        ''' <summary>
        ''' 应用程序发送此消息来判断热键与某个窗口是否有关联
        ''' </summary>
        WM_GETHOTKEY = &H33
        ''' <summary>
        ''' 此消息发送给最小化窗口，当此窗口将要被拖放而它的类中没有定义图标，应用程序能 
        ''' 返回一个图标或光标的句柄，当用户拖放图标时系统显示这个图标或光标 
        ''' </summary>
        WM_QUERYDRAGICON = &H37
        ''' <summary>
        ''' 发送此消息来判定combobox或listbox新增加的项的相对位置 
        ''' </summary>
        WM_COMPAREITEM = &H39
        ''' <summary>
        ''' 
        ''' </summary>
        WM_GETOBJECT = &H3D
        ''' <summary>
        ''' 显示内存已经很少了
        ''' </summary>
        WM_COMPACTING = &H41
        ''' <summary>
        ''' 发送此消息给那个窗口的大小和位置将要被改变时，来调用setwindowpos函数或其它窗口管理函数 
        ''' </summary>
        WM_WINDOWPOSCHANGING = &H46
        ''' <summary>
        ''' 发送此消息给那个窗口的大小和位置已经被改变时，来调用setwindowpos函数或其它窗口管理函数 
        ''' </summary>
        WM_WINDOWPOSCHANGED = &H47
        ''' <summary>
        ''' 当系统将要进入暂停状态时发送此消息 （适用于16位的windows）
        ''' </summary>
        WM_POWER = &H48
        ''' <summary>
        ''' 当一个应用程序传递数据给另一个应用程序时发送此消息
        ''' </summary>
        WM_COPYDATA = &H4A
        ''' <summary>
        ''' 当某个用户取消程序日志激活状态，提交此消息给程序
        ''' </summary>
        WM_CANCELJOURNAL = &H4B
        ''' <summary>
        ''' 当某个控件的某个事件已经发生或这个控件需要得到一些信息时，发送此消息给它的父窗口 
        ''' </summary>
        WM_NOTIFY = &H4E
        ''' <summary>
        ''' 当用户选择某种输入语言，或输入语言的热键改变
        ''' </summary>
        WM_INPUTLANGCHANGEREQUEST = &H50
        ''' <summary>
        ''' 当平台现场已经被改变后发送此消息给受影响的最顶级窗口
        ''' </summary>
        WM_INPUTLANGCHANGE = &H51
        ''' <summary>
        ''' 当程序已经初始化windows帮助例程时发送此消息给应用程序
        ''' </summary>
        WM_TCARD = &H52
        ''' <summary>
        ''' 此消息显示用户按下了F1，如果某个菜单是激活的，就发送此消息个此窗口关联的菜单，否则就 
        ''' 发送给有焦点的窗口，如果当前都没有焦点，就把此消息发送给当前激活的窗口 
        ''' </summary>
        WM_HELP = &H53
        ''' <summary>
        ''' 当用户已经登入或退出后发送此消息给所有的窗口，当用户登入或退出时系统更新用户的具体 
        ''' 设置信息，在用户更新设置时系统马上发送此消息； 
        ''' </summary>
        WM_USERCHANGED = &H54
        ''' <summary>
        ''' 公用控件，自定义控件和他们的父窗口通过此消息来判断控件是使用ANSI还是UNI CODE结构 
        ''' 在WM_NOTIFY消息，使用此控件能使某个控件与它的父控件之间进行相互通信 
        ''' </summary>
        WM_NOTIFYformAT = &H55
        ''' <summary>
        ''' 当用户某个窗口中点击了一下右键就发送此消息给这个窗口 
        ''' </summary>
        WM_CONTEXTMENU = &H7B
        ''' <summary>
        ''' 当调用SETWINDOWLONG函数将要改变一个或多个 窗口的风格时发送此消息给那个窗口 
        ''' </summary>
        WM_styleCHANGING = &H7C
        ''' <summary>
        ''' 当调用SETWINDOWLONG函数一个或多个 窗口的风格后发送此消息给那个窗口 
        ''' </summary>
        WM_styleCHANGED = &H7D
        ''' <summary>
        ''' 当显示器的分辨率改变后发送此消息给所有的窗口 
        ''' </summary>
        WM_DISPLAYCHANGE = &H7E
        ''' <summary>
        ''' 此消息发送给某个窗口来返回与某个窗口有关连的大图标或小图标的句柄； 
        ''' </summary>
        WM_GETICON = &H7F
        ''' <summary>
        ''' 程序发送此消息让一个新的大图标或小图标与某个窗口关联； 
        ''' </summary>
        WM_SETICON = &H80
        ''' <summary>
        ''' 当某个窗口第一次被创建时，此消息在WM_CREATE消息发送前发送；
        ''' </summary>
        WM_NCCREATE = &H81
        ''' <summary>
        ''' 此消息通知某个窗口，非客户区正在销毁 
        ''' </summary>
        WM_NCDESTROY = &H82
        ''' <summary>
        '''  当某个窗口的客户区域必须被核算时发送此消息 
        ''' </summary>
        WM_NCCALCSIZE = &H83
        ''' <summary>
        ''' 移动鼠标，按住或释放鼠标时发生 
        ''' </summary>
        WM_NCHITTEST = &H84
        ''' <summary>
        ''' 程序发送此消息给某个窗口当它（窗口）的框架必须被绘制时；
        ''' </summary>
        WM_NCPAINT = &H85
        ''' <summary>
        ''' 此消息发送给某个窗口 仅当它的非客户区需要被改变来显示是激活还是非激活状态； 
        ''' </summary>
        WM_NCACTIVATE = &H86
        ''' <summary>
        ''' 发送此消息给某个与对话框程序关联的控件，widdows控制方位键和TAB键使输入进入此控件 
        ''' 通过响应WM_GETDLGCODE消息，应用程序可以把他当成一个特殊的输入控件并能处理它 
        ''' </summary>
        WM_GETDLGCODE = &H87
        ''' <summary>
        ''' 当光标在一个窗口的非客户区内移动时发送此消息给这个窗口(非客户区 为：窗体的标题栏及窗的边框体)
        ''' </summary>
        WM_NCMOUSEMOVE = &HA0
        ''' <summary>
        ''' 当光标在一个窗口的非客户区同时按下鼠标左键时提交此消息
        ''' </summary>
        WM_NCLBUTTONDOWN = &HA1
        ''' <summary>
        ''' 当用户释放鼠标左键同时光标某个窗口在非客户区十发送此消息； 
        ''' </summary>
        WM_NCLBUTTONUP = &HA2
        ''' <summary>
        ''' 当用户双击鼠标左键同时光标某个窗口在非客户区十发送此消息 
        ''' </summary>
        WM_NCLBUTTONDBLCLK = &HA3
        ''' <summary>
        ''' 当用户按下鼠标右键同时光标又在窗口的非客户区时发送此消息 
        ''' </summary>
        WM_NCRBUTTONDOWN = &HA4
        ''' <summary>
        ''' 当用户释放鼠标右键同时光标又在窗口的非客户区时发送此消息 
        ''' </summary>
        WM_NCRBUTTONUP = &HA5
        ''' <summary>
        ''' 当用户双击鼠标右键同时光标某个窗口在非客户区十发送此消息 
        ''' </summary>
        WM_NCRBUTTONDBLCLK = &HA6
        ''' <summary>
        ''' 当用户按下鼠标中键同时光标又在窗口的非客户区时发送此消息 
        ''' </summary>
        WM_NCMBUTTONDOWN = &HA7
        ''' <summary>
        ''' 当用户释放鼠标中键同时光标又在窗口的非客户区时发送此消息 
        ''' </summary>
        WM_NCMBUTTONUP = &HA8
        ''' <summary>
        ''' 当用户双击鼠标中键同时光标又在窗口的非客户区时发送此消息 
        ''' </summary>
        WM_NCMBUTTONDBLCLK = &HA9
        ''' <summary>
        ''' 
        ''' </summary>
        WM_KEYFIRST = &H100
        ''' <summary>
        ''' 按下一个键 
        ''' </summary>
        WM_KEYDOWN = &H100
        ''' <summary>
        ''' 释放一个键
        ''' </summary>
        WM_KEYUP = &H101
        ''' <summary>
        ''' 按下某键，并已发出WM_KEYDOWN， WM_KEYUP消息 
        ''' </summary>
        WM_CHAR = &H102
        ''' <summary>
        ''' 当用translatemessage函数翻译WM_KEYUP消息时发送此消息给拥有焦点的窗口 
        ''' </summary>
        WM_DEADCHAR = &H103
        ''' <summary>
        ''' 当用户按住ALT键同时按下其它键时提交此消息给拥有焦点的窗口； 
        ''' </summary>
        WM_SYSKEYDOWN = &H104
        ''' <summary>
        ''' 当用户释放一个键同时ALT 键还按着时提交此消息给拥有焦点的窗口 
        ''' </summary>
        WM_SYSKEYUP = &H105
        ''' <summary>
        ''' 当WM_SYSKEYDOWN消息被TRANSLATEMESSAGE函数翻译后提交此消息给拥有焦点的窗口 
        ''' </summary>
        WM_SYSCHAR = &H106
        ''' <summary>
        ''' 当WM_SYSKEYDOWN消息被TRANSLATEMESSAGE函数翻译后发送此消息给拥有焦点的窗口 
        ''' </summary>
        WM_SYSDEADCHAR = &H107
        ''' <summary>
        ''' 
        ''' </summary>
        WM_KEYLAST = &H108
        ''' <summary>
        ''' 在一个对话框程序被显示前发送此消息给它，通常用此消息初始化控件和执行其它任务 
        ''' </summary>
        WM_INITDIALOG = &H110
        ''' <summary>
        ''' 当用户选择一条菜单命令项或当某个控件发送一条消息给它的父窗口，一个快捷键被翻译 
        ''' </summary>
        WM_COMMAND = &H111
        ''' <summary>
        ''' 当用户选择窗口菜单的一条命令或当用户选择最大化或最小化时那个窗口会收到此消息 
        ''' </summary>
        WM_SYSCOMMAND = &H112
        ''' <summary>
        ''' 发生了定时器事件
        ''' </summary>
        WM_TIMER = &H113
        ''' <summary>
        ''' 当一个窗口标准水平滚动条产生一个滚动事件时发送此消息给那个窗口，也发送给拥有它的控件 
        ''' </summary>
        WM_HSCROLL = &H114
        ''' <summary>
        ''' 当一个窗口标准垂直滚动条产生一个滚动事件时发送此消息给那个窗口，发送给拥有它的控件
        ''' </summary>
        WM_VSCROLL = &H115
        ''' <summary>
        ''' 当一个菜单将要被激活时发送此消息，它发生在用户菜单条中的某项或按下某个菜单键，它允许程序在显示前更改菜单 
        ''' </summary>
        wm_initmenu = &H116
        ''' <summary>
        ''' 当一个下拉菜单或子菜单将要被激活时发送此消息，它允许程序在它显示前更改菜单，而不要改变全部
        ''' </summary>
        WM_INITMENUPOPUP = &H117
        ''' <summary>
        ''' 当用户选择一条菜单项时发送此消息给菜单的所有者（一般是窗口） 
        ''' </summary>
        WM_MENUSELECT = &H11F
        ''' <summary>
        ''' 当菜单已被激活用户按下了某个键（不同于加速键），发送此消息给菜单的所有者；
        ''' </summary>
        WM_MENUCHAR = &H120
        ''' <summary>
        ''' 当一个模态对话框或菜单进入空载状态时发送此消息给它的所有者，一个模态对话框或菜单进入空载
        ''' 状态就是在处理完一条或几条先前的消息后没有消息它的列队中等待
        ''' </summary>
        WM_ENTERIDLE = &H121
        ''' <summary>
        ''' 
        ''' </summary>
        WM_MENURBUTTONUP = &H122
        ''' <summary>
        ''' 
        ''' </summary>
        WM_MENUDRAG = &H123
        ''' <summary>
        ''' 
        ''' </summary>
        WM_MENUGETOBJECT = &H124
        ''' <summary>
        ''' 
        ''' </summary>
        WM_UNINITMENUPOPUP = &H125
        ''' <summary>
        ''' 
        ''' </summary>
        WM_MENUCOMMAND = &H126
        ''' <summary>
        ''' 
        ''' </summary>
        WM_CHANGEUISTATE = &H127
        ''' <summary>
        ''' 
        ''' </summary>
        WM_UPDATEUISTATE = &H128
        ''' <summary>
        ''' 
        ''' </summary>
        WM_QUERYUISTATE = &H129
        ''' <summary>
        ''' 在windows绘制消息框前发送此消息给消息框的所有者窗口，通过响应这条消息， 所有者窗口可以 
        ''' 通过使用给定的相关显示设备的句柄来设置消息框的文本和背景颜色 
        ''' </summary>
        WM_CTLCOLORMSGBOX = &H132
        ''' <summary>
        ''' 当一个编辑型控件将要被绘制时发送此消息给它的父窗口；通过响应这条消息，所有者窗口可以 
        ''' 通过使用给定的相关显示设备的句柄来设置编辑框的文本和背景颜色 
        ''' </summary>
        WM_CTLCOLOREDIT = &H133
        ''' <summary>
        ''' 当一个列表框控件将要被绘制前发送此消息给它的父窗口；通过响应这条消息，所有者窗口可以 
        ''' 通过使用给定的相关显示设备的句柄来设置列表框的文本和背景颜色 
        ''' </summary>
        WM_CTLCOLORLISTBOX = &H134
        ''' <summary>
        ''' 当一个按钮控件将要被绘制时发送此消息给它的父窗口；通过响应这条消息，所有者窗口可以 
        ''' 通过使用给定的相关显示设备的句柄来设置按纽的文本和背景颜色 
        ''' </summary>
        WM_CTLCOLORBTN = &H135
        ''' <summary>
        ''' 当一个对话框控件将要被绘制前发送此消息给它的父窗口；通过响应这条消息，所有者窗口可以 
        ''' 通过使用给定的相关显示设备的句柄来设置对话框的文本背景颜色 
        ''' </summary>
        WM_CTLCOLORDLG = &H136
        ''' <summary>
        ''' 当一个滚动条控件将要被绘制时发送此消息给它的父窗口；通过响应这条消息，所有者窗口可以 
        ''' 通过使用给定的相关显示设备的句柄来设置滚动条的背景颜色 
        ''' </summary>
        WM_CTLCOLORSCROLLBAR = &H137
        ''' <summary>
        ''' 当一个静态控件将要被绘制时发送此消息给它的父窗口；通过响应这条消息，所有者窗口可以 
        ''' 通过使用给定的相关显示设备的句柄来设置静态控件的文本和背景颜色 
        ''' </summary>
        WM_CTLCOLORSTATIC = &H138
        ''' <summary>
        ''' 
        ''' </summary>
        WM_MOUSEFIRST = &H200
        ''' <summary>
        ''' 移动鼠标
        ''' </summary>
        WM_MOUSEMOVE = &H200
        ''' <summary>
        ''' 按下鼠标左键
        ''' </summary>
        WM_LBUTTONDOWN = &H201
        ''' <summary>
        ''' 释放鼠标左键 
        ''' </summary>
        WM_LBUTTONUP = &H202
        ''' <summary>
        ''' 双击鼠标左键 
        ''' </summary>
        WM_LBUTTONDBLCLK = &H203
        ''' <summary>
        ''' 按下鼠标右键 
        ''' </summary>
        WM_RBUTTONDOWN = &H204
        ''' <summary>
        ''' 释放鼠标右键
        ''' </summary>
        WM_RBUTTONUP = &H205
        ''' <summary>
        ''' 双击鼠标右键
        ''' </summary>
        WM_RBUTTONDBLCLK = &H206
        ''' <summary>
        ''' 按下鼠标中键
        ''' </summary>
        WM_MBUTTONDOWN = &H207
        ''' <summary>
        ''' 释放鼠标中键 
        ''' </summary>
        WM_MBUTTONUP = &H208
        ''' <summary>
        ''' 双击鼠标中键 
        ''' </summary>
        WM_MBUTTONDBLCLK = &H209
        ''' <summary>
        ''' 当鼠标轮子转动时发送此消息给当前有焦点的控件 
        ''' </summary>
        WM_MOUSEWHEEL = &H20A
        ''' <summary>
        ''' 
        ''' </summary>
        WM_MOUSELAST = &H20A
        ''' <summary>
        ''' 当MDI子窗口被创建或被销毁，或用户按了一下鼠标键而光标在子窗口上时发送此消息给它的父窗口 
        ''' </summary>
        WM_PARENTNOTIFY = &H210
        ''' <summary>
        ''' 发送此消息通知应用程序的主窗口that已经进入了菜单循环模式 
        ''' </summary>
        WM_ENTERMENULOOP = &H211
        ''' <summary>
        ''' 发送此消息通知应用程序的主窗口that已退出了菜单循环模式 
        ''' </summary>
        WM_EXITMENULOOP = &H212
        ''' <summary>
        ''' 
        ''' </summary>
        WM_NEXTMENU = &H213
        ''' <summary>
        ''' 当用户正在调整窗口大小时发送此消息给窗口；通过此消息应用程序可以监视窗口大小和位置也可以修改他们 
        ''' </summary>
        WM_SIZING = &H214
        ''' <summary>
        ''' 发送此消息 给窗口当它失去捕获的鼠标时；
        ''' </summary>
        WM_CAPTURECHANGED = &H215
        ''' <summary>
        ''' 当用户在移动窗口时发送此消息，通过此消息应用程序可以监视窗口大小和位置也可以修改他们； 
        ''' </summary>
        WM_MOVING = &H216
        ''' <summary>
        ''' 此消息发送给应用程序来通知它有关电源管理事件； 
        ''' </summary>
        WM_POWERBROADCAST = &H218
        ''' <summary>
        ''' 当设备的硬件配置改变时发送此消息给应用程序或设备驱动程序 
        ''' </summary>
        WM_DEVICECHANGE = &H219
        ''' <summary>
        ''' 
        ''' </summary>
        WM_IME_STARTCOMPOSITION = &H10D
        ''' <summary>
        ''' 
        ''' </summary>
        WM_IME_ENDCOMPOSITION = &H10E
        ''' <summary>
        ''' 
        ''' </summary>
        WM_IME_COMPOSITION = &H10F
        ''' <summary>
        ''' 
        ''' </summary>
        WM_IME_KEYLAST = &H10F
        ''' <summary>
        ''' 
        ''' </summary>
        WM_IME_SETCONTEXT = &H281
        ''' <summary>
        ''' 
        ''' </summary>
        WM_IME_NOTIFY = &H282
        ''' <summary>
        ''' 
        ''' </summary>
        WM_IME_CONTROL = &H283
        ''' <summary>
        ''' 
        ''' </summary>
        WM_IME_COMPOSITIONFULL = &H284
        ''' <summary>
        ''' 
        ''' </summary>
        WM_IME_SELECT = &H285
        ''' <summary>
        ''' 
        ''' </summary>
        WM_IME_CHAR = &H286
        ''' <summary>
        ''' 
        ''' </summary>
        WM_IME_REQUEST = &H288
        ''' <summary>
        ''' 
        ''' </summary>
        WM_IME_KEYDOWN = &H290
        ''' <summary>
        ''' 
        ''' </summary>
        WM_IME_KEYUP = &H291
        ''' <summary>
        ''' 应用程序发送此消息给多文档的客户窗口来创建一个MDI 子窗口 
        ''' </summary>
        WM_MDICREATE = &H220
        ''' <summary>
        ''' 应用程序发送此消息给多文档的客户窗口来关闭一个MDI 子窗口 
        ''' </summary>
        WM_MDIDESTROY = &H221
        ''' <summary>
        ''' 应用程序发送此消息给多文档的客户窗口通知客户窗口激活另一个MDI子窗口，当客户窗口收到 
        ''' 此消息后，它发出WM_MDIACTIVE消息给MDI子窗口（未激活）激活它； 
        ''' </summary>
        WM_MDIACTIVATE = &H222
        ''' <summary>
        ''' 程序 发送此消息给MDI客户窗口让子窗口从最大最小化恢复到原来大小 
        ''' </summary>
        WM_MDIRESTORE = &H223
        ''' <summary>
        ''' 程序 发送此消息给MDI客户窗口激活下一个或前一个窗口 
        ''' </summary>
        WM_MDINEXT = &H224
        ''' <summary>
        ''' 程序发送此消息给MDI客户窗口来最大化一个MDI子窗口； 
        ''' </summary>
        WM_MDIMAXIMIZE = &H225
        ''' <summary>
        ''' 程序 发送此消息给MDI客户窗口以平铺方式重新排列所有MDI子窗口 
        ''' </summary>
        WM_MDITILE = &H226
        ''' <summary>
        ''' 程序 发送此消息给MDI客户窗口以层叠方式重新排列所有MDI子窗口 
        ''' </summary>
        WM_MDICASCADE = &H227
        ''' <summary>
        ''' 程序 发送此消息给MDI客户窗口重新排列所有最小化的MDI子窗口 
        ''' </summary>
        WM_MDIICONARRANGE = &H228
        ''' <summary>
        ''' 程序 发送此消息给MDI客户窗口来找到激活的子窗口的句柄 
        ''' </summary>
        WM_MDIGETACTIVE = &H229
        ''' <summary>
        ''' 程序 发送此消息给MDI客户窗口用MDI菜单代替子窗口的菜单 
        ''' </summary>
        WM_MDISETMENU = &H230
        ''' <summary>
        ''' 
        ''' </summary>
        WM_ENTERSIZEMOVE = &H231
        ''' <summary>
        ''' 
        ''' </summary>
        WM_EXITSIZEMOVE = &H232
        ''' <summary>
        ''' 
        ''' </summary>
        WM_DROPFILES = &H233
        ''' <summary>
        ''' 
        ''' </summary>
        WM_MDIREFRESHMENU = &H234
        ''' <summary>
        ''' 
        ''' </summary>
        WM_MOUSEHOVER = &H2A1
        ''' <summary>
        ''' 
        ''' </summary>
        WM_MOUSELEAVE = &H2A3
        ''' <summary>
        ''' 程序发送此消息给一个编辑框或combobox来删除当前选择的文本 
        ''' </summary>
        WM_CUT = &H300
        ''' <summary>
        ''' 程序发送此消息给一个编辑框或combobox来复制当前选择的文本到剪贴板 
        ''' </summary>
        WM_COPY = &H301
        ''' <summary>
        ''' 程序发送此消息给editcontrol或combobox从剪贴板中得到数据 
        ''' </summary>
        WM_PASTE = &H302
        ''' <summary>
        ''' 程序发送此消息给editcontrol或combobox清除当前选择的内容；
        ''' </summary>
        WM_CLEAR = &H303
        ''' <summary>
        ''' 程序发送此消息给editcontrol或combobox撤消最后一次操作
        ''' </summary>
        WM_UNDO = &H304
        ''' <summary>
        ''' 
        ''' </summary>
        WM_RENDERformAT = &H305
        ''' <summary>
        ''' 
        ''' </summary>
        WM_RENDERALLformATS = &H306
        ''' <summary>
        ''' 当调用ENPTYCLIPBOARD函数时 发送此消息给剪贴板的所有者 
        ''' </summary>
        WM_DESTROYCLIPBOARD = &H307
        ''' <summary>
        ''' 当剪贴板的内容变化时发送此消息给剪贴板观察链的第一个窗口；它允许用剪贴板观察窗口来显示剪贴板的新内容； 
        ''' </summary>
        WM_DRAWCLIPBOARD = &H308
        ''' <summary>
        ''' 当剪贴板包含CF_OWNERDIPLAY格式的数据并且剪贴板观察窗口的客户区需要重画； 
        ''' </summary>
        WM_PAINTCLIPBOARD = &H309
        ''' <summary>
        ''' 
        ''' </summary>
        WM_VSCROLLCLIPBOARD = &H30A
        ''' <summary>
        ''' 当剪贴板包含CF_OWNERDIPLAY格式的数据并且剪贴板观察窗口的客户区域的大小已经改变是此消息通过剪
        ''' 贴板观察窗口发送给剪贴板的所有者； 
        ''' </summary>
        WM_SIZECLIPBOARD = &H30B
        ''' <summary>
        ''' 通过剪贴板观察窗口发送此消息给剪贴板的所有者来请求一个CF_OWNERDISPLAY格式的剪贴板的名字 
        ''' </summary>
        WM_ASKCBformATNAME = &H30C
        ''' <summary>
        ''' 当一个窗口从剪贴板观察链中移去时发送此消息给剪贴板观察链的第一个窗口；
        ''' </summary>
        WM_CHANGECBCHAIN = &H30D
        ''' <summary>
        ''' 此消息通过一个剪贴板观察窗口发送给剪贴板的所有者 ；它发生在当剪贴板包含CFOWNERDISPALY格式的数据
        ''' 并且有个事件在剪贴板观察窗的水平滚动条上；所有者应滚动剪贴板图象并更新滚动条的值； 
        ''' </summary>
        WM_HSCROLLCLIPBOARD = &H30E
        ''' <summary>
        ''' 此消息发送给将要收到焦点的窗口，此消息能使窗口在收到焦点时同时有机会实现他的逻辑调色板 
        ''' </summary>
        WM_QUERYNEWPALETTE = &H30F
        ''' <summary>
        ''' 当一个应用程序正要实现它的逻辑调色板时发此消息通知所有的应用程序 
        ''' </summary>
        WM_PALETTEISCHANGING = &H310
        ''' <summary>
        ''' 此消息在一个拥有焦点的窗口实现它的逻辑调色板后发送此消息给所有顶级并重叠的窗口，以此来改变系统调色板 
        ''' </summary>
        WM_PALETTECHANGED = &H311
        ''' <summary>
        ''' 当用户按下由REGISTERHOTKEY函数注册的热键时提交此消息 
        ''' </summary>
        WM_HOTKEY = &H312
        ''' <summary>
        ''' 应用程序发送此消息仅当WINDOWS或其它应用程序发出一个请求要求绘制一个应用程序的一部分； 
        ''' </summary>
        WM_PRINT = &H317
        ''' <summary>
        ''' 
        ''' </summary>
        WM_PRINTCLIENT = &H318
        ''' <summary>
        ''' 
        ''' </summary>
        WM_HANDHELDFIRST = &H358
        ''' <summary>
        ''' 
        ''' </summary>
        WM_HANDHELDLAST = &H35F
        ''' <summary>
        ''' 
        ''' </summary>
        WM_PENWINFIRST = &H380
        ''' <summary>
        ''' 
        ''' </summary>
        WM_PENWINLAST = &H38F
        ''' <summary>
        ''' 
        ''' </summary>
        WM_COALESCE_FIRST = &H390
        ''' <summary>
        ''' 
        ''' </summary>
        WM_COALESCE_LAST = &H39F
        ''' <summary>
        ''' 
        ''' </summary>
        WM_DDE_FIRST = &H3E0
        ''' <summary>
        ''' 
        ''' </summary>
        WM_THEMECHNAGED = &H31A
    End Enum

    ''' <summary>
    ''' 发送到一个窗口，以确定鼠标在窗口的哪一部分，对应于一个特定的屏幕坐标
    ''' </summary>
    Public Enum WM_NCHITTEST As Integer
        ''' <summary>
        ''' 在屏幕背景或窗口之间的分界线
        ''' </summary>
        HTERROR = -2
        ''' <summary>
        ''' 在目前一个窗口，其他窗口覆盖在同一个线程
        ''' （该消息将被发送到相关窗口在同一个线程，直到其中一个返回一个代码，是不是HTTRANSPARENT）
        ''' </summary>
        HTTRANSPARENT = -1
        ''' <summary>
        ''' 在屏幕背景或窗口之间的分界线上
        ''' </summary>
        HTNOWHERE = 0
        ''' <summary>
        ''' 在客户端区域
        ''' </summary>
        HTCLIENT = 1
        ''' <summary>
        ''' 在标题栏
        ''' </summary>
        HTCAPTION = 2
        ''' <summary>
        ''' 在窗口菜单中，或在一个子窗口的关闭按钮
        ''' </summary>
        HTSYSMENU = 3
        ''' <summary>
        ''' 在大小框（与HTGROWBO相同）
        ''' </summary>
        HTSIZE = 4
        ''' <summary>
        ''' 在大小框（与HTSIZE相同）
        ''' </summary>
        HTGROWBOX = 4
        ''' <summary>
        ''' 在一个菜单
        ''' </summary>
        HTMENU = 5
        ''' <summary>
        ''' 在水平滚动条
        ''' </summary>
        HTHSCROLL = 6
        ''' <summary>
        ''' 在垂直滚动条
        ''' </summary>
        HTVSCROLL = 7
        ''' <summary>
        ''' 在最小化按钮
        ''' </summary>
        HTREDUCE = 8
        ''' <summary>
        ''' 在最小化按钮
        ''' </summary>
        HTMINBUTTON = 8
        ''' <summary>
        ''' 在最大化按钮
        ''' </summary>
        HTMAXBUTTON = 9
        ''' <summary>
        ''' 在最大化按钮
        ''' </summary>
        HTZOOM = 9
        ''' <summary>
        ''' 在左边框可调整大小的窗口
        ''' </summary>
        HTLEFT = 10
        ''' <summary>
        ''' 在一个可调整大小的窗口的右边框
        ''' </summary>
        HTRIGHT = 11
        ''' <summary>
        ''' 在窗口的上边框水平线上
        ''' </summary>
        HTTOP = 12
        ''' <summary>
        ''' 在窗口的左上边框
        ''' </summary>
        HTTOPLEFT = 13
        ''' <summary>
        ''' 在窗口的右上边框
        ''' </summary>
        HTTOPRIGHT = 14
        ''' <summary>
        ''' （用户可以在较低的水平边界可调整大小的窗口单击鼠标，改变窗口的垂直大小）
        ''' </summary>
        HTBOTTOM = 15
        ''' <summary>
        ''' 在左下角的边框可调整大小的窗口（用户可以通过点击鼠标来调整窗口的大小，对角）
        ''' </summary>
        HTBOTTOMLEFT = 16
        ''' <summary>
        ''' 在右下角的边框可调整大小的窗口（用户可以通过点击鼠标来调整窗口的大小，对角）
        ''' </summary>
        HTBOTTOMRIGHT = 17
        ''' <summary>
        ''' 在一个不具有缩放边框的窗口
        ''' </summary>
        HTBORDER = 18
        ''' <summary>
        ''' 在关闭按钮
        ''' </summary>
        HTCLOSE = 20
        ''' <summary>
        ''' 在帮助按钮
        ''' </summary>
        HTHELP = 21
    End Enum

    <Flags> _
    Public Enum AllocationType
        ' Fields
        Commit = &H1000
        Decommit = &H4000
        LargePages = &H20000000
        Physical = &H400000
        Release = &H8000
        Reserve = &H2000
        Reset = &H80000
        TopDown = &H100000
        WriteWatch = &H200000
    End Enum

    <Flags> _
    Public Enum MemoryProtection
        ' Fields
        Execute = &H10
        ExecuteRead = &H20
        ExecuteReadWrite = &H40
        ExecuteWriteCopy = &H80
        GuardModifierflag = &H100
        NoAccess = 1
        NoCacheModifierflag = &H200
        [ReadOnly] = 2
        ReadWrite = 4
        WriteCombineModifierflag = &H400
        WriteCopy = 8
    End Enum
End Namespace