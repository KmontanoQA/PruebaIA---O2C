*==============================================================================
* PROGRAMA : PanelPrincipal.prg
* SISTEMA  : Sistema Gestion OC2 - Panel Principal
*==============================================================================

*-- Lanzar el formulario principal
PUBLIC  oForm as Form
oForm = NEWOBJECT("frmMain")
oForm.WindowType = 1

oForm.Show()

READ EVENTS
RETURN

*==============================================================================
DEFINE CLASS frmMain AS Form
    Height = 700
    Width = 1200
    AutoCenter = .T.
    Caption = "Sistema Gestion OC2 - Panel Principal"
    BorderStyle = 3
    ShowWindow = 2
    MaxButton = .T.
    MinButton = .T.
    BackColor = RGB(241,244,252)
    
    
    *-- Propiedades personalizadas
    cCurrentModule = ""
    
    *-- Fondo principal
    ADD OBJECT shpBackground AS Shape WITH ;
        Top = 0, Left = 0, Height = 700, Width = 1200, ;
        BackColor = RGB(241,244,252), BorderWidth = 0
    
    *-- Navbar lateral (sidebar)
    ADD OBJECT shpSidebar AS Shape WITH ;
        Top = 0, Left = 0, Height = 700, Width = 220, ;
        BackColor = RGB(33,43,54), BorderWidth = 0
    
    *-- Logo y titulo en sidebar
    ADD OBJECT lblLogo AS Label WITH ;
        Caption = "@ OC2 System", FontName = "Segoe UI", FontSize = 14, FontBold = .T., ;
        ForeColor = RGB(255,255,255), Top = 20, Left = 20, ;
        Width = 180, Height = 30, BackStyle = 0
    
    ADD OBJECT lineSeparator AS Line WITH ;
        X1 = 20, Y1 = 60, X2 = 200, Y2 = 60, ;
        BorderColor = RGB(60,70,85), BorderWidth = 1
    
    *-- Botones del menu (navbar)
    ADD OBJECT btnCustomers AS CommandButton WITH ;
        Caption = CHR(9) + "Customers", Top = 90, Left = 10, Width = 200, Height = 40, ;
        FontName = "Segoe UI", FontSize = 10, FontBold = .F., ;
        BackColor = RGB(45,55,70), ForeColor = RGB(200,210,225), ;
        SpecialEffect = 0, Alignment = 0
    
    ADD OBJECT btnProducts AS CommandButton WITH ;
        Caption = CHR(9) + "Products", Top = 135, Left = 10, Width = 200, Height = 40, ;
        FontName = "Segoe UI", FontSize = 10, FontBold = .F., ;
        BackColor = RGB(45,55,70), ForeColor = RGB(200,210,225), ;
        SpecialEffect = 0, Alignment = 0
    
    ADD OBJECT btnOrders AS CommandButton WITH ;
        Caption = CHR(9) + "Orders", Top = 180, Left = 10, Width = 200, Height = 40, ;
        FontName = "Segoe UI", FontSize = 10, FontBold = .F., ;
        BackColor = RGB(45,55,70), ForeColor = RGB(200,210,225), ;
        SpecialEffect = 0, Alignment = 0
    
    ADD OBJECT btnInvoices AS CommandButton WITH ;
        Caption = CHR(9) + "Invoices", Top = 225, Left = 10, Width = 200, Height = 40, ;
        FontName = "Segoe UI", FontSize = 10, FontBold = .F., ;
        BackColor = RGB(45,55,70), ForeColor = RGB(200,210,225), ;
        SpecialEffect = 0, Alignment = 0
    
    ADD OBJECT btnReports AS CommandButton WITH ;
        Caption = CHR(9) + "Reports", Top = 270, Left = 10, Width = 200, Height = 40, ;
        FontName = "Segoe UI", FontSize = 10, FontBold = .F., ;
        BackColor = RGB(45,55,70), ForeColor = RGB(200,210,225), ;
        SpecialEffect = 0, Alignment = 0
    
    ADD OBJECT btnRoles AS CommandButton WITH ;
        Caption = CHR(9) + "Roles", Top = 315, Left = 10, Width = 200, Height = 40, ;
        FontName = "Segoe UI", FontSize = 10, FontBold = .F., ;
        BackColor = RGB(45,55,70), ForeColor = RGB(200,210,225), ;
        SpecialEffect = 0, Alignment = 0
    
    ADD OBJECT btnCategories AS CommandButton WITH ;
        Caption = CHR(9) + "Categories", Top = 360, Left = 10, Width = 200, Height = 40, ;
        FontName = "Segoe UI", FontSize = 10, FontBold = .F., ;
        BackColor = RGB(45,55,70), ForeColor = RGB(200,210,225), ;
        SpecialEffect = 0, Alignment = 0
    
    *-- Separador y boton de salir
    ADD OBJECT lineSeparator2 AS Line WITH ;
        X1 = 20, Y1 = 620, X2 = 200, Y2 = 620, ;
        BorderColor = RGB(60,70,85), BorderWidth = 1
    
    ADD OBJECT btnLogout AS CommandButton WITH ;
        Caption = CHR(9) + "Logout", Top = 640, Left = 10, Width = 200, Height = 40, ;
        FontName = "Segoe UI", FontSize = 10, FontBold = .F., ;
        BackColor = RGB(195,64,64), ForeColor = RGB(255,255,255), ;
        SpecialEffect = 0, Alignment = 0
    
    *-- Area de contenido principal
    ADD OBJECT shpContentArea AS Shape WITH ;
        Top = 10, Left = 230, Height = 680, Width = 960, ;
        BackColor = RGB(255,255,255), BorderColor = RGB(220,224,236), ;
        BorderWidth = 1, SpecialEffect = 1, Curvature = 8
    
    *-- Header del contenido
    ADD OBJECT lblWelcome AS Label WITH ;
        Caption = "Welcome back, " + gcUserEmail, ;
        FontName = "Segoe UI", FontSize = 18, FontBold = .T., ;
        ForeColor = RGB(33,63,125), Top = 30, Left = 250, ;
        Width = 600, Height = 30, BackStyle = 0
    
    ADD OBJECT lblSubtitle AS Label WITH ;
        Caption = "Here are today's stats from your online store!", ;
        FontName = "Segoe UI", FontSize = 10, ;
        ForeColor = RGB(120,132,166), Top = 62, Left = 250, ;
        Width = 600, Height = 20, BackStyle = 0
    
    *-- Cards de estadisticas
    ADD OBJECT shpCard1 AS Shape WITH ;
        Top = 100, Left = 250, Height = 120, Width = 280, ;
        BackColor = RGB(37,92,221), BorderWidth = 0, ;
        SpecialEffect = 1, Curvature = 12
    
    ADD OBJECT lblCard1Title AS Label WITH ;
        Caption = "Total Sales", FontName = "Segoe UI", FontSize = 11, ;
        ForeColor = RGB(255,255,255), Top = 115, Left = 270, ;
        Width = 240, Height = 20, BackStyle = 0
    
    ADD OBJECT lblCard1Value AS Label WITH ;
        Caption = "$9,328.55", FontName = "Segoe UI", FontSize = 22, FontBold = .T., ;
        ForeColor = RGB(255,255,255), Top = 145, Left = 270, ;
        Width = 240, Height = 35, BackStyle = 0
    
    ADD OBJECT lblCard1Detail AS Label WITH ;
        Caption = "731 Orders  |  +15.6%  +1.4k this week", ;
        FontName = "Segoe UI", FontSize = 8, ;
        ForeColor = RGB(200,215,255), Top = 185, Left = 270, ;
        Width = 240, Height = 18, BackStyle = 0
    
    ADD OBJECT shpCard2 AS Shape WITH ;
        Top = 100, Left = 550, Height = 120, Width = 280, ;
        BackColor = RGB(245,247,252), BorderColor = RGB(220,224,236), ;
        BorderWidth = 1, SpecialEffect = 1, Curvature = 12
    
    ADD OBJECT lblCard2Title AS Label WITH ;
        Caption = "Visitors", FontName = "Segoe UI", FontSize = 11, ;
        ForeColor = RGB(84,96,130), Top = 115, Left = 570, ;
        Width = 240, Height = 20, BackStyle = 0
    
    ADD OBJECT lblCard2Value AS Label WITH ;
        Caption = "12,302", FontName = "Segoe UI", FontSize = 22, FontBold = .T., ;
        ForeColor = RGB(33,63,125), Top = 145, Left = 570, ;
        Width = 240, Height = 35, BackStyle = 0
    
    ADD OBJECT lblCard2Detail AS Label WITH ;
        Caption = "Avg. time: 4:30m  |  +12.7%  +1.2k this week", ;
        FontName = "Segoe UI", FontSize = 8, ;
        ForeColor = RGB(120,132,166), Top = 185, Left = 570, ;
        Width = 240, Height = 18, BackStyle = 0
    
    ADD OBJECT shpCard3 AS Shape WITH ;
        Top = 100, Left = 850, Height = 120, Width = 280, ;
        BackColor = RGB(245,247,252), BorderColor = RGB(220,224,236), ;
        BorderWidth = 1, SpecialEffect = 1, Curvature = 12
    
    ADD OBJECT lblCard3Title AS Label WITH ;
        Caption = "Refunds", FontName = "Segoe UI", FontSize = 11, ;
        ForeColor = RGB(84,96,130), Top = 115, Left = 870, ;
        Width = 240, Height = 20, BackStyle = 0
    
    ADD OBJECT lblCard3Value AS Label WITH ;
        Caption = "963", FontName = "Segoe UI", FontSize = 22, FontBold = .T., ;
        ForeColor = RGB(33,63,125), Top = 145, Left = 870, ;
        Width = 240, Height = 35, BackStyle = 0
    
    ADD OBJECT lblCard3Detail AS Label WITH ;
        Caption = "2 Disputed  |  -12.7%  -213", ;
        FontName = "Segoe UI", FontSize = 8, ;
        ForeColor = RGB(195,64,64), Top = 185, Left = 870, ;
        Width = 240, Height = 18, BackStyle = 0
    
    *-- Area de modulos
    ADD OBJECT lblModuleTitle AS Label WITH ;
        Caption = "Select a module from the sidebar to get started", ;
        FontName = "Segoe UI", FontSize = 12, ;
        ForeColor = RGB(120,132,166), Alignment = 2, ;
        Top = 350, Left = 250, Width = 880, Height = 30, BackStyle = 0
    
    *-- Footer
    ADD OBJECT lblFooter AS Label WITH ;
        Caption = "(c) 2025 Sistema Gestion OC2. Todos los derechos reservados.", ;
        FontName = "Segoe UI", FontSize = 8, ForeColor = RGB(150,160,190), ;
        Alignment = 2, Top = 675, Left = 230, Width = 960, Height = 20, ;
        BackStyle = 0
    
    *-- Metodos
    PROCEDURE Init
        THIS.HighlightButton(THIS.btnCustomers)
    ENDPROC
    
    PROCEDURE btnCustomers.Click
        THISFORM.LoadModule("Customers")
    ENDPROC
    
    PROCEDURE btnProducts.Click
        THISFORM.LoadModule("Products")
    ENDPROC
    
    PROCEDURE btnOrders.Click
        THISFORM.LoadModule("Orders")
        * Abrir el formulario de gestión de orders
        DO OrdersGeneral.prg
    ENDPROC
    
    PROCEDURE btnInvoices.Click
        THISFORM.LoadModule("Invoices")
    ENDPROC
    
    PROCEDURE btnReports.Click
        THISFORM.LoadModule("Reports")
    ENDPROC
    
    PROCEDURE btnRoles.Click
        THISFORM.LoadModule("Roles")
    ENDPROC
    
    PROCEDURE btnCategories.Click
        THISFORM.LoadModule("Categories")
    ENDPROC
    
    PROCEDURE btnLogout.Click
        IF MESSAGEBOX("Are you sure you want to logout?", 4+32, "Confirm Logout") = 6
            gcToken = ""
            gcUserEmail = ""
            gcUserRole = ""
            THISFORM.Release()
        ENDIF
    ENDPROC
    
    PROCEDURE Destroy
        CLEAR EVENTS
    ENDPROC
    
    PROCEDURE LoadModule
        LPARAMETERS lcModule
        THIS.cCurrentModule = lcModule
        THIS.ResetButtons()
        
        DO CASE
            CASE lcModule = "Customers"
                THIS.HighlightButton(THIS.btnCustomers)
                THIS.lblModuleTitle.Caption = "Customers Module - Coming Soon"
                
            CASE lcModule = "Products"
                THIS.HighlightButton(THIS.btnProducts)
                THIS.lblModuleTitle.Caption = "Products Module - Coming Soon"
                
            CASE lcModule = "Orders"
                THIS.HighlightButton(THIS.btnOrders)
                THIS.lblModuleTitle.Caption = "Orders Module - Coming Soon"
                
            CASE lcModule = "Invoices"
                THIS.HighlightButton(THIS.btnInvoices)
                THIS.lblModuleTitle.Caption = "Invoices Module - Coming Soon"
                
            CASE lcModule = "Reports"
                THIS.HighlightButton(THIS.btnReports)
                THIS.lblModuleTitle.Caption = "Reports Module - Coming Soon"
                
            CASE lcModule = "Roles"
                THIS.HighlightButton(THIS.btnRoles)
                THIS.lblModuleTitle.Caption = "Roles Module - Coming Soon"
                
            CASE lcModule = "Categories"
                THIS.HighlightButton(THIS.btnCategories)
                THIS.lblModuleTitle.Caption = "Categories Module - Coming Soon"
        ENDCASE
    ENDPROC
    
    PROCEDURE ResetButtons
        THIS.btnCustomers.BackColor = RGB(45,55,70)
        THIS.btnProducts.BackColor = RGB(45,55,70)
        THIS.btnOrders.BackColor = RGB(45,55,70)
        THIS.btnInvoices.BackColor = RGB(45,55,70)
        THIS.btnReports.BackColor = RGB(45,55,70)
        THIS.btnRoles.BackColor = RGB(45,55,70)
        THIS.btnCategories.BackColor = RGB(45,55,70)
    ENDPROC
    
    PROCEDURE HighlightButton
        LPARAMETERS loButton
        loButton.BackColor = RGB(37,92,221)
        loButton.ForeColor = RGB(255,255,255)
        loButton.FontBold = .T.
    ENDPROC
ENDDEFINE