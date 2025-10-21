*==============================================================================
* PROGRAMA: OrdersGeneral.prg
* DESCRIPCIÓN: CRUD de Orders con encabezado y detalle
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

*-- Lanzar el formulario
LOCAL oForm
oForm = NEWOBJECT("frmOrdersGeneral")
oForm.Show()
READ EVENTS
RETURN

*==============================================================================
DEFINE CLASS frmOrdersGeneral AS Form
    Height = 700
    Width = 1200
    AutoCenter = .T.
    Caption = "Sistema OC2 - Gestión de Orders"
    BorderStyle = 3
    ShowWindow = 2
    MaxButton = .T.
    MinButton = .T.
    BackColor = RGB(241,244,252)
    
    *-- Propiedades personalizadas
    nCurrentOrderId = 0
    lEditMode = .F.
    
    *-- Fondo principal
    ADD OBJECT shpBackground AS Shape WITH ;
        Top = 0, Left = 0, Height = 700, Width = 1200, ;
        BackColor = RGB(241,244,252), BorderWidth = 0
    
    *-- Header del formulario
    ADD OBJECT shpHeader AS Shape WITH ;
        Top = 10, Left = 10, Height = 60, Width = 1180, ;
        BackColor = RGB(33,43,54), BorderWidth = 0, ;
        SpecialEffect = 1, Curvature = 8
    
    ADD OBJECT lblTitle AS Label WITH ;
        Caption = "Gestión de Orders", ;
        FontName = "Segoe UI", FontSize = 18, FontBold = .T., ;
        ForeColor = RGB(255,255,255), Top = 25, Left = 30, ;
        Width = 300, Height = 30, BackStyle = 0
    
    ADD OBJECT lblUser AS Label WITH ;
        Caption = "Usuario: " + gcUserEmail, ;
        FontName = "Segoe UI", FontSize = 10, ;
        ForeColor = RGB(200,210,225), Top = 30, Left = 900, ;
        Width = 250, Height = 20, BackStyle = 0
    
    *-- Área de encabezado de Order
    ADD OBJECT shpHeaderArea AS Shape WITH ;
        Top = 80, Left = 10, Height = 180, Width = 1180, ;
        BackColor = RGB(255,255,255), BorderColor = RGB(220,224,236), ;
        BorderWidth = 1, SpecialEffect = 1, Curvature = 8
    
    ADD OBJECT lblHeaderTitle AS Label WITH ;
        Caption = "Encabezado de Order", ;
        FontName = "Segoe UI", FontSize = 14, FontBold = .T., ;
        ForeColor = RGB(33,63,125), Top = 95, Left = 30, ;
        Width = 200, Height = 25, BackStyle = 0
    
    *-- Campos del encabezado
    ADD OBJECT lblCustomerId AS Label WITH ;
        Caption = "Customer ID:", FontName = "Segoe UI", FontSize = 10, ;
        ForeColor = RGB(84,96,130), Top = 130, Left = 30, ;
        Width = 100, Height = 20, BackStyle = 0
    
    ADD OBJECT txtCustomerId AS TextBox WITH ;
        Top = 150, Left = 30, Width = 150, Height = 26, ;
        FontName = "Segoe UI", FontSize = 10, Value = 0, ;
        BorderColor = RGB(180,189,214), SpecialEffect = 0, ;
        InputMask = "999999"
    
    ADD OBJECT lblSellerId AS Label WITH ;
        Caption = "Seller ID:", FontName = "Segoe UI", FontSize = 10, ;
        ForeColor = RGB(84,96,130), Top = 130, Left = 200, ;
        Width = 100, Height = 20, BackStyle = 0
    
    ADD OBJECT txtSellerId AS TextBox WITH ;
        Top = 150, Left = 200, Width = 150, Height = 26, ;
        FontName = "Segoe UI", FontSize = 10, Value = 0, ;
        BorderColor = RGB(180,189,214), SpecialEffect = 0, ;
        InputMask = "999999"
    
    ADD OBJECT lblNotes AS Label WITH ;
        Caption = "Notas:", FontName = "Segoe UI", FontSize = 10, ;
        ForeColor = RGB(84,96,130), Top = 130, Left = 370, ;
        Width = 100, Height = 20, BackStyle = 0
    
    ADD OBJECT txtNotes AS TextBox WITH ;
        Top = 150, Left = 370, Width = 400, Height = 26, ;
        FontName = "Segoe UI", FontSize = 10, ;
        BorderColor = RGB(180,189,214), SpecialEffect = 0
    
    *-- Botones del encabezado
    ADD OBJECT cmdNew AS CommandButton WITH ;
        Caption = "Nuevo", Top = 190, Left = 30, Width = 80, Height = 30, ;
        FontName = "Segoe UI", FontSize = 10, ;
        BackColor = RGB(37,92,221), ForeColor = RGB(255,255,255), ;
        SpecialEffect = 2
    
    ADD OBJECT cmdSave AS CommandButton WITH ;
        Caption = "Guardar", Top = 190, Left = 120, Width = 80, Height = 30, ;
        FontName = "Segoe UI", FontSize = 10, ;
        BackColor = RGB(40,140,90), ForeColor = RGB(255,255,255), ;
        SpecialEffect = 2, Enabled = .F.
    
    ADD OBJECT cmdEdit AS CommandButton WITH ;
        Caption = "Editar", Top = 190, Left = 210, Width = 80, Height = 30, ;
        FontName = "Segoe UI", FontSize = 10, ;
        BackColor = RGB(255,165,0), ForeColor = RGB(255,255,255), ;
        SpecialEffect = 2, Enabled = .F.
    
    ADD OBJECT cmdDelete AS CommandButton WITH ;
        Caption = "Eliminar", Top = 190, Left = 300, Width = 80, Height = 30, ;
        FontName = "Segoe UI", FontSize = 10, ;
        BackColor = RGB(195,64,64), ForeColor = RGB(255,255,255), ;
        SpecialEffect = 2, Enabled = .F.
    
    ADD OBJECT cmdCancel AS CommandButton WITH ;
        Caption = "Cancelar", Top = 190, Left = 390, Width = 80, Height = 30, ;
        FontName = "Segoe UI", FontSize = 10, ;
        BackColor = RGB(108,117,125), ForeColor = RGB(255,255,255), ;
        SpecialEffect = 2, Enabled = .F.
    
    *-- Área de detalle (Order Lines)
    ADD OBJECT shpDetailArea AS Shape WITH ;
        Top = 270, Left = 10, Height = 350, Width = 1180, ;
        BackColor = RGB(255,255,255), BorderColor = RGB(220,224,236), ;
        BorderWidth = 1, SpecialEffect = 1, Curvature = 8
    
    ADD OBJECT lblDetailTitle AS Label WITH ;
        Caption = "Detalle de Order Lines", ;
        FontName = "Segoe UI", FontSize = 14, FontBold = .T., ;
        ForeColor = RGB(33,63,125), Top = 285, Left = 30, ;
        Width = 200, Height = 25, BackStyle = 0
    
    *-- Grid para las líneas de order
    ADD OBJECT grdOrderLines AS Grid WITH ;
        Top = 320, Left = 30, Width = 1140, Height = 250, ;
        FontName = "Segoe UI", FontSize = 9, ;
        BackColor = RGB(255,255,255), ;
        GridLineColor = RGB(220,224,236), ;
        HighlightBackColor = RGB(37,92,221), ;
        HighlightForeColor = RGB(255,255,255), ;
        RecordSource = "curOrderLines", ;
        AllowAddNew = .T., AllowDelete = .T.
    
    *-- Botones del detalle
    ADD OBJECT cmdAddLine AS CommandButton WITH ;
        Caption = "Agregar Línea", Top = 580, Left = 30, Width = 120, Height = 30, ;
        FontName = "Segoe UI", FontSize = 10, ;
        BackColor = RGB(37,92,221), ForeColor = RGB(255,255,255), ;
        SpecialEffect = 2, Enabled = .F.
    
    ADD OBJECT cmdDeleteLine AS CommandButton WITH ;
        Caption = "Eliminar Línea", Top = 580, Left = 160, Width = 120, Height = 30, ;
        FontName = "Segoe UI", FontSize = 10, ;
        BackColor = RGB(195,64,64), ForeColor = RGB(255,255,255), ;
        SpecialEffect = 2, Enabled = .F.
    
    *-- Botón cerrar
    ADD OBJECT cmdClose AS CommandButton WITH ;
        Caption = "Cerrar", Top = 630, Left = 1100, Width = 80, Height = 30, ;
        FontName = "Segoe UI", FontSize = 10, ;
        BackColor = RGB(108,117,125), ForeColor = RGB(255,255,255), ;
        SpecialEffect = 2
    
    *-- Status bar
    ADD OBJECT lblStatus AS Label WITH ;
        Caption = "Listo", FontName = "Segoe UI", FontSize = 9, ;
        ForeColor = RGB(84,96,130), Top = 670, Left = 20, ;
        Width = 1160, Height = 20, BackStyle = 0
    
    PROCEDURE Init
        THIS.CreateCursor()
        THIS.SetupGrid()
        THIS.LoadOrders()
    ENDPROC
    
    PROCEDURE CreateCursor
        IF USED("curOrderLines")
            USE IN curOrderLines
        ENDIF
        
        CREATE CURSOR curOrderLines ( ;
            productId I, ;
            qty I, ;
            productName C(100) ;
        )
    ENDPROC
    
    PROCEDURE SetupGrid
        WITH THIS.grdOrderLines
            .ColumnCount = 3
            
            * Columna Product ID
            .Column1.Header1.Caption = "Product ID"
            .Column1.Width = 100
            .Column1.ControlSource = "curOrderLines.productId"
            .Column1.Text1.InputMask = "999999"
            
            * Columna Quantity
            .Column2.Header1.Caption = "Cantidad"
            .Column2.Width = 100
            .Column2.ControlSource = "curOrderLines.qty"
            .Column2.Text1.InputMask = "999999"
            
            * Columna Product Name (solo lectura)
            .Column3.Header1.Caption = "Nombre del Producto"
            .Column3.Width = 300
            .Column3.ControlSource = "curOrderLines.productName"
            .Column3.ReadOnly = .T.
            .Column3.BackColor = RGB(248,249,250)
        ENDWITH
    ENDPROC
    
    PROCEDURE LoadOrders
        THIS.lblStatus.Caption = "Cargando orders..."
        * Aquí se cargaría la lista de orders desde la API
        THIS.lblStatus.Caption = "Listo"
    ENDPROC
    
    PROCEDURE cmdNew.Click
        THISFORM.NewOrder()
    ENDPROC
    
    PROCEDURE cmdSave.Click
        THISFORM.SaveOrder()
    ENDPROC
    
    PROCEDURE cmdEdit.Click
        THISFORM.EditOrder()
    ENDPROC
    
    PROCEDURE cmdDelete.Click
        THISFORM.DeleteOrder()
    ENDPROC
    
    PROCEDURE cmdCancel.Click
        THISFORM.CancelOrder()
    ENDPROC
    
    PROCEDURE cmdAddLine.Click
        THISFORM.AddOrderLine()
    ENDPROC
    
    PROCEDURE cmdDeleteLine.Click
        THISFORM.DeleteOrderLine()
    ENDPROC
    
    PROCEDURE cmdClose.Click
        THISFORM.Release()
    ENDPROC
    
    PROCEDURE NewOrder
        THIS.ClearForm()
        THIS.lEditMode = .T.
        THIS.nCurrentOrderId = 0
        THIS.EnableControls(.T.)
        THIS.txtCustomerId.SetFocus()
        THIS.lblStatus.Caption = "Modo: Nuevo Order"
    ENDPROC
    
    PROCEDURE SaveOrder
        LOCAL lcJSON, lcResponse, loHTTP
        
        IF THIS.ValidateOrder()
            lcJSON = THIS.BuildOrderJSON()
            
            IF THIS.nCurrentOrderId = 0
                * Crear nuevo order
                lcResponse = THIS.APIPost("/Orders", lcJSON)
            ELSE
                * Actualizar order existente
                lcResponse = THIS.APIPut("/Orders/" + TRANSFORM(THIS.nCurrentOrderId), lcJSON)
            ENDIF
            
            IF !EMPTY(lcResponse)
                THIS.lEditMode = .F.
                THIS.EnableControls(.F.)
                THIS.lblStatus.Caption = "Order guardado exitosamente"
                MESSAGEBOX("Order guardado correctamente", 64, "Éxito")
            ENDIF
        ENDIF
    ENDPROC
    
    PROCEDURE EditOrder
        IF THIS.nCurrentOrderId > 0
            THIS.lEditMode = .T.
            THIS.EnableControls(.T.)
            THIS.lblStatus.Caption = "Modo: Editando Order #" + TRANSFORM(THIS.nCurrentOrderId)
        ELSE
            MESSAGEBOX("Seleccione un order para editar", 48, "Advertencia")
        ENDIF
    ENDPROC
    
    PROCEDURE DeleteOrder
        IF THIS.nCurrentOrderId > 0
            IF MESSAGEBOX("¿Está seguro de eliminar este order?", 4+32, "Confirmar") = 6
                LOCAL lcResponse
                lcResponse = THIS.APIDelete("/Orders/" + TRANSFORM(THIS.nCurrentOrderId))
                IF !EMPTY(lcResponse)
                    THIS.ClearForm()
                    THIS.lblStatus.Caption = "Order eliminado exitosamente"
                    MESSAGEBOX("Order eliminado correctamente", 64, "Éxito")
                ENDIF
            ENDIF
        ELSE
            MESSAGEBOX("Seleccione un order para eliminar", 48, "Advertencia")
        ENDIF
    ENDPROC
    
    PROCEDURE CancelOrder
        THIS.lEditMode = .F.
        THIS.EnableControls(.F.)
        THIS.lblStatus.Caption = "Operación cancelada"
    ENDPROC
    
    PROCEDURE AddOrderLine
        IF THIS.lEditMode
            INSERT INTO curOrderLines (productId, qty, productName) VALUES (0, 1, "")
            THIS.grdOrderLines.Refresh()
            THIS.lblStatus.Caption = "Línea agregada"
        ENDIF
    ENDPROC
    
    PROCEDURE DeleteOrderLine
        IF THIS.lEditMode AND RECCOUNT("curOrderLines") > 0
            DELETE IN curOrderLines
            PACK
            THIS.grdOrderLines.Refresh()
            THIS.lblStatus.Caption = "Línea eliminada"
        ENDIF
    ENDPROC
    
    PROCEDURE ClearForm
        THIS.txtCustomerId.Value = 0
        THIS.txtSellerId.Value = 0
        THIS.txtNotes.Value = ""
        ZAP IN curOrderLines
        THIS.grdOrderLines.Refresh()
    ENDPROC
    
    PROCEDURE EnableControls
        LPARAMETERS llEnable
        
        THIS.txtCustomerId.Enabled = llEnable
        THIS.txtSellerId.Enabled = llEnable
        THIS.txtNotes.Enabled = llEnable
        THIS.grdOrderLines.Enabled = llEnable
        
        THIS.cmdSave.Enabled = llEnable
        THIS.cmdCancel.Enabled = llEnable
        THIS.cmdAddLine.Enabled = llEnable
        THIS.cmdDeleteLine.Enabled = llEnable
        
        THIS.cmdNew.Enabled = !llEnable
        THIS.cmdEdit.Enabled = !llEnable AND THIS.nCurrentOrderId > 0
        THIS.cmdDelete.Enabled = !llEnable AND THIS.nCurrentOrderId > 0
    ENDPROC
    
    PROCEDURE ValidateOrder
        IF THIS.txtCustomerId.Value <= 0
            MESSAGEBOX("Debe especificar un Customer ID válido", 48, "Error")
            THIS.txtCustomerId.SetFocus()
            RETURN .F.
        ENDIF
        
        IF THIS.txtSellerId.Value <= 0
            MESSAGEBOX("Debe especificar un Seller ID válido", 48, "Error")
            THIS.txtSellerId.SetFocus()
            RETURN .F.
        ENDIF
        
        IF RECCOUNT("curOrderLines") = 0
            MESSAGEBOX("Debe agregar al menos una línea de order", 48, "Error")
            RETURN .F.
        ENDIF
        
        * Validar que todas las líneas tengan datos válidos
        SELECT curOrderLines
        GO TOP
        SCAN
            IF productId <= 0 OR qty <= 0
                MESSAGEBOX("Todas las líneas deben tener Product ID y Cantidad válidos", 48, "Error")
                RETURN .F.
            ENDIF
        ENDSCAN
        
        RETURN .T.
    ENDPROC
    
    PROCEDURE BuildOrderJSON
        LOCAL lcJSON, lcLines
        
        lcJSON = '{'
        lcJSON = lcJSON + '"customerId":' + TRANSFORM(THIS.txtCustomerId.Value) + ','
        lcJSON = lcJSON + '"sellerId":' + TRANSFORM(THIS.txtSellerId.Value) + ','
        lcJSON = lcJSON + '"notes":"' + THIS.SanitizeJson(ALLTRIM(THIS.txtNotes.Value)) + '",'
        lcJSON = lcJSON + '"orderLines":['
        
        SELECT curOrderLines
        GO TOP
        lcLines = ""
        SCAN
            IF !EMPTY(lcLines)
                lcLines = lcLines + ","
            ENDIF
            lcLines = lcLines + '{"productId":' + TRANSFORM(productId) + ',"qty":' + TRANSFORM(qty) + '}'
        ENDSCAN
        
        lcJSON = lcJSON + lcLines + ']}'
        
        RETURN lcJSON
    ENDPROC
    
    PROCEDURE APIPost
        LPARAMETERS tcEndpoint, tcJSON
        RETURN THIS.APIRequest("POST", tcEndpoint, tcJSON)
    ENDPROC
    
    PROCEDURE APIPut
        LPARAMETERS tcEndpoint, tcJSON
        RETURN THIS.APIRequest("PUT", tcEndpoint, tcJSON)
    ENDPROC
    
    PROCEDURE APIDelete
        LPARAMETERS tcEndpoint
        RETURN THIS.APIRequest("DELETE", tcEndpoint, "")
    ENDPROC
    
    PROCEDURE APIRequest
        LPARAMETERS tcMethod, tcEndpoint, tcJSON
        LOCAL loHTTP, lcResponse, lcURL, llError
        
        lcURL = gcAPIBaseURL + tcEndpoint
        llError = .F.
        
        TRY
            loHTTP = CREATEOBJECT("WinHttp.WinHttpRequest.5.1")
        CATCH
            THIS.lblStatus.Caption = "Error: No se pudo crear el componente WinHTTP"
            RETURN ""
        ENDTRY
        
        TRY
            loHTTP.Open(tcMethod, lcURL, .F.)
            loHTTP.SetRequestHeader("Authorization", "Bearer " + gcToken)
            loHTTP.SetRequestHeader("Content-Type", "application/json")
            
            IF !EMPTY(tcJSON)
                loHTTP.Send(tcJSON)
            ELSE
                loHTTP.Send()
            ENDIF
            
            lcResponse = loHTTP.ResponseText
        CATCH TO loEx
            THIS.lblStatus.Caption = "Error de conexión: " + NVL(loEx.Message, "Desconocido")
            RETURN ""
        ENDTRY
        
        IF loHTTP.Status >= 200 AND loHTTP.Status < 300
            RETURN lcResponse
        ELSE
            THIS.lblStatus.Caption = "Error HTTP " + TRANSFORM(loHTTP.Status) + ": " + loHTTP.StatusText
            MESSAGEBOX("Error en la operación: " + TRANSFORM(loHTTP.Status) + " - " + loHTTP.StatusText, 16, "Error")
            RETURN ""
        ENDIF
    ENDPROC
    
    PROCEDURE SanitizeJson
        LPARAMETERS lcValue
        lcValue = STRTRAN(lcValue, "\", "\\")
        lcValue = STRTRAN(lcValue, '"', '\"')
        lcValue = STRTRAN(lcValue, CHR(13), "")
        lcValue = STRTRAN(lcValue, CHR(10), "")
        RETURN lcValue
    ENDPROC
    
    PROCEDURE Destroy
        IF USED("curOrderLines")
            USE IN curOrderLines
        ENDIF
        CLEAR EVENTS
    ENDPROC
    
ENDDEFINE
