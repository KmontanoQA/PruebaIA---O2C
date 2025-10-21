*==============================================================================
* PROGRAMA: main_menu.prg
* DESCRIPCIÓN: Menú principal del sistema
* SISTEMA: ERP O2C Supermercado
*==============================================================================

*-- Asegurar que las variables públicas existan
IF TYPE("gcUserEmail") = "U"
    PUBLIC gcUserEmail, gcUserRole, gcToken, gcAPIBaseURL
    gcUserEmail = ""
    gcUserRole = ""
    gcToken = ""
    gcAPIBaseURL = "https://localhost:7109/api/v1"
ENDIF

DEFINE CLASS frmMain AS Form
    Height = 500
    Width = 700
    AutoCenter = .T.
    Caption = "Sistema ERP O2C - Menú Principal"
    
    ADD OBJECT lblWelcome AS Label WITH ;
        Caption = "Bienvenido: " + gcUserEmail + " (" + gcUserRole + ")", ;
        FontSize = 10, ;
        FontBold = .T., ;
        Top = 10, ;
        Left = 20, ;
        Width = 660, ;
        Height = 25
    
    ADD OBJECT lblToken AS Label WITH ;
        Caption = "Token: " + LEFT(gcToken, 50) + "...", ;
        FontSize = 8, ;
        Top = 35, ;
        Left = 20, ;
        Width = 660, ;
        Height = 20
    
    * Botones del menú
    ADD OBJECT cmdCustomers AS CommandButton WITH ;
        Caption = "Clientes", ;
        Top = 80, ;
        Left = 50, ;
        Width = 150, ;
        Height = 40
    
    ADD OBJECT cmdProducts AS CommandButton WITH ;
        Caption = "Productos", ;
        Top = 80, ;
        Left = 220, ;
        Width = 150, ;
        Height = 40
    
    ADD OBJECT cmdOrders AS CommandButton WITH ;
        Caption = "Pedidos", ;
        Top = 80, ;
        Left = 390, ;
        Width = 150, ;
        Height = 40
    
    ADD OBJECT cmdInvoices AS CommandButton WITH ;
        Caption = "Facturas", ;
        Top = 140, ;
        Left = 50, ;
        Width = 150, ;
        Height = 40
    
    ADD OBJECT cmdReports AS CommandButton WITH ;
        Caption = "Reportes", ;
        Top = 140, ;
        Left = 220, ;
        Width = 150, ;
        Height = 40
    
    ADD OBJECT cmdLogout AS CommandButton WITH ;
        Caption = "Cerrar Sesión", ;
        Top = 140, ;
        Left = 390, ;
        Width = 150, ;
        Height = 40
    
    ADD OBJECT edtConsole AS EditBox WITH ;
        Top = 200, ;
        Left = 20, ;
        Width = 660, ;
        Height = 280, ;
        ReadOnly = .T., ;
        ScrollBars = 2
    
    PROCEDURE Init
        This.edtConsole.Value = "Sistema iniciado correctamente" + CHR(13) + CHR(10)
    ENDPROC
    
    PROCEDURE cmdCustomers.Click
        ThisForm.LoadCustomers()
    ENDPROC
    
    PROCEDURE cmdProducts.Click
        ThisForm.LoadProducts()
    ENDPROC
    
    PROCEDURE cmdOrders.Click
        ThisForm.LoadOrders()
    ENDPROC
    
    PROCEDURE cmdInvoices.Click
        ThisForm.LoadInvoices()
    ENDPROC
    
    PROCEDURE cmdReports.Click
        ThisForm.ShowReports()
    ENDPROC
    
    PROCEDURE cmdLogout.Click
        gcToken = ""
        gcUserEmail = ""
        gcUserRole = ""
        ThisForm.Release()
        DO login.prg
    ENDPROC
    
    * Método para hacer peticiones GET
    PROCEDURE APIGet(tcEndpoint)
        LOCAL loHTTP, lcResponse, lcURL
        
        lcURL = gcAPIBaseURL + tcEndpoint
        
        TRY
            loHTTP = CREATEOBJECT("WinHttp.WinHttpRequest.5.1")
            loHTTP.Open("GET", lcURL, .F.)
            loHTTP.SetRequestHeader("Authorization", "Bearer " + gcToken)
            loHTTP.SetRequestHeader("Content-Type", "application/json")
            loHTTP.Send()
            
            IF loHTTP.Status = 200
                lcResponse = loHTTP.ResponseText
                RETURN lcResponse
            ELSE
                This.LogMessage("Error HTTP " + TRANSFORM(loHTTP.Status) + ": " + loHTTP.StatusText)
                RETURN ""
            ENDIF
            
        CATCH TO loEx
            This.LogMessage("Error: " + loEx.Message)
            RETURN ""
        ENDTRY
    ENDPROC
    
    * Método para registrar mensajes
    PROCEDURE LogMessage(tcMessage)
        This.edtConsole.Value = This.edtConsole.Value + ;
            TTOC(DATETIME()) + " - " + tcMessage + CHR(13) + CHR(10)
        This.edtConsole.SelStart = LEN(This.edtConsole.Value)
    ENDPROC
    
    * Cargar clientes
    PROCEDURE LoadCustomers
        LOCAL lcResponse
        
        This.LogMessage("Cargando clientes...")
        lcResponse = This.APIGet("/customers")
        
        IF !EMPTY(lcResponse)
            This.LogMessage("Clientes cargados: " + TRANSFORM(OCCURS('"id":', lcResponse)) + " registros")
            This.LogMessage(LEFT(lcResponse, 500))
        ENDIF
    ENDPROC
    
    * Cargar productos
    PROCEDURE LoadProducts
        LOCAL lcResponse
        
        This.LogMessage("Cargando productos...")
        lcResponse = This.APIGet("/products")
        
        IF !EMPTY(lcResponse)
            This.LogMessage("Productos cargados: " + TRANSFORM(OCCURS('"id":', lcResponse)) + " registros")
            This.LogMessage(LEFT(lcResponse, 500))
        ENDIF
    ENDPROC
    
    * Cargar pedidos
    PROCEDURE LoadOrders
        LOCAL lcResponse
        
        This.LogMessage("Cargando pedidos...")
        lcResponse = This.APIGet("/orders?page=1&pageSize=10")
        
        IF !EMPTY(lcResponse)
            This.LogMessage("Pedidos cargados exitosamente")
            This.LogMessage(LEFT(lcResponse, 500))
        ENDIF
    ENDPROC
    
    * Cargar facturas
    PROCEDURE LoadInvoices
        LOCAL lcResponse
        
        This.LogMessage("Cargando facturas...")
        lcResponse = This.APIGet("/invoices?page=1&pageSize=10")
        
        IF !EMPTY(lcResponse)
            This.LogMessage("Facturas cargadas exitosamente")
            This.LogMessage(LEFT(lcResponse, 500))
        ENDIF
    ENDPROC
    
    * Mostrar reportes
    PROCEDURE ShowReports
        LOCAL lcResponse, ldDateFrom, ldDateTo
        
        ldDateFrom = DATE() - 30
        ldDateTo = DATE()
        
        This.LogMessage("Generando reporte de ventas...")
        lcResponse = This.APIGet("/reports/sales?dateFrom=" + ;
            TTOC(ldDateFrom, 1) + "&dateTo=" + TTOC(ldDateTo, 1))
        
        IF !EMPTY(lcResponse)
            This.LogMessage("Reporte generado exitosamente")
            This.LogMessage(LEFT(lcResponse, 500))
        ENDIF
    ENDPROC
    
ENDDEFINE
