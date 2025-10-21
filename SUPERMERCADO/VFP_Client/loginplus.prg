*==============================================================================
* PROGRAMA : loginPLUS.prg
* SISTEMA  : Sistema Gestion OC2 - Cliente Visual FoxPro
*==============================================================================

CLEAR
SET TALK OFF
SET CENTURY ON

PUBLIC gcAPIBaseURL, gcToken, gcUserEmail, gcUserRole
IF TYPE('gcAPIBaseURL') # 'C' OR EMPTY(gcAPIBaseURL)
    gcAPIBaseURL = "https://localhost:7109/api/v1"
ENDIF
IF TYPE('gcToken') # 'C'
    gcToken = ""
ENDIF
IF TYPE('gcUserEmail') # 'C'
    gcUserEmail = ""
ENDIF
IF TYPE('gcUserRole') # 'C'
    gcUserRole = ""
ENDIF

LOCAL loForm
loForm = NEWOBJECT('frmLogin', 'loginPLUS.prg')
loForm.Show()
READ EVENTS
RETURN

*==============================================================================
DEFINE CLASS frmLogin AS Form
    Height = 420
    Width = 760
    AutoCenter = .T.
    Caption = "Sistema Gestion OC2 - Acceso"
    BorderStyle = 3
    ShowWindow = 2
    MaxButton = .F.
    MinButton = .F.
    BackColor = RGB(241,244,252)

    ADD OBJECT shpMainCard AS Shape WITH ;
        Top = 20, Left = 20, Height = 380, Width = 720, ;
        BackColor = RGB(255,255,255), BorderColor = RGB(220,224,236), ;
        BorderWidth = 1, SpecialEffect = 1, Curvature = 12

    ADD OBJECT shpLeftBackground AS Shape WITH ;
        Top = 40, Left = 40, Height = 340, Width = 320, ;
        BackColor = RGB(34,83,195), BorderColor = RGB(34,83,195), ;
        BorderWidth = 0, Curvature = 18

    ADD OBJECT lblLeftMonogram AS Label WITH ;
        Caption = "OC2", FontName = "Segoe UI", FontSize = 26, FontBold = .T., ;
        ForeColor = RGB(255,255,255), Alignment = 2, Top = 70, Left = 40, ;
        Width = 320, Height = 40, BackStyle = 0

    ADD OBJECT lblLeftTitle AS Label WITH ;
        Caption = "Sistema de Gestion" + CHR(13) + "Administracion OC2", ;
        FontName = "Segoe UI", FontSize = 18, FontBold = .T., ;
        ForeColor = RGB(255,255,255), Alignment = 2, Top = 120, Left = 40, ;
        Width = 320, Height = 60, BackStyle = 0

    ADD OBJECT lblLeftDescription AS Label WITH ;
        Caption = "Gestiona ordenes, facturacion e inventario desde cualquier cliente.", ;
        FontName = "Segoe UI", FontSize = 10, ForeColor = RGB(220,229,255), ;
        Alignment = 2, Top = 190, Left = 60, Width = 280, Height = 80, ;
        WordWrap = .T., BackStyle = 0

    ADD OBJECT lblRightHeader AS Label WITH ;
        Caption = "Bienvenido", FontName = "Segoe UI", FontSize = 20, FontBold = .T., ;
        ForeColor = RGB(33,63,125), Alignment = 2, Top = 70, Left = 380, ;
        Width = 340, Height = 30, BackStyle = 0

    ADD OBJECT lblRightSubtitle AS Label WITH ;
        Caption = "Acceso al Portal OC2", FontName = "Segoe UI", FontSize = 10, ;
        ForeColor = RGB(120,132,166), Alignment = 2, Top = 105, Left = 380, ;
        Width = 340, Height = 18, BackStyle = 0

    ADD OBJECT lblEmail AS Label WITH ;
        Caption = "Correo electronico", FontName = "Segoe UI", FontSize = 9, ;
        ForeColor = RGB(84,96,130), Top = 150, Left = 400, ;
        Width = 300, Height = 16, BackStyle = 0

    ADD OBJECT txtEmail AS TextBox WITH ;
        Top = 167, Left = 400, Width = 300, Height = 26, FontName = "Segoe UI", ;
        FontSize = 10, Value = "admin@supermercado.com", ;
        BorderColor = RGB(180,189,214), SpecialEffect = 0

    ADD OBJECT lblPassword AS Label WITH ;
        Caption = "Contrasena", FontName = "Segoe UI", FontSize = 9, ;
        ForeColor = RGB(84,96,130), Top = 201, Left = 400, ;
        Width = 300, Height = 16, BackStyle = 0

    ADD OBJECT txtPassword AS TextBox WITH ;
        Top = 218, Left = 400, Width = 300, Height = 26, FontName = "Segoe UI", ;
        FontSize = 10, PasswordChar = "*", Value = "admin123", ;
        BorderColor = RGB(180,189,214), SpecialEffect = 0

    ADD OBJECT cmdLogin AS CommandButton WITH ;
        Caption = "Entrar", Top = 260, Left = 400, Width = 300, Height = 34, ;
        FontName = "Segoe UI", FontSize = 11, BackColor = RGB(37,92,221), ;
        ForeColor = RGB(255,255,255), Default = .T., SpecialEffect = 2

    ADD OBJECT cmdCancel AS CommandButton WITH ;
        Caption = "Cancelar", Top = 302, Left = 400, Width = 300, Height = 28, ;
        FontName = "Segoe UI", FontSize = 9, BackColor = RGB(235,238,249), ;
        ForeColor = RGB(84,96,130), Cancel = .T., SpecialEffect = 1

    * Boton para ir directo al dashboard si ya hay token
    ADD OBJECT cmdDashboard AS CommandButton WITH ;
        Caption = "Ir al Dashboard", Top = 336, Left = 400, Width = 300, Height = 28, ;
        FontName = "Segoe UI", FontSize = 9, BackColor = RGB(235,238,249), ;
        ForeColor = RGB(84,96,130), Visible = .F.

    ADD OBJECT lblStatus AS Label WITH ;
        Caption = "", FontName = "Segoe UI", FontSize = 9, ForeColor = RGB(195,64,64), ;
        Top = 338, Left = 380, Width = 340, Height = 40, WordWrap = .T., ;
        Alignment = 2, BackStyle = 0

    ADD OBJECT lblFooter AS Label WITH ;
        Caption = "(c) 2025 Sistema Gestion OC2. Todos los derechos reservados.", ;
        FontName = "Segoe UI", FontSize = 8, ForeColor = RGB(150,160,190), ;
        Alignment = 2, Top = 382, Left = 20, Width = 720, Height = 20, ;
        BackStyle = 0

    PROCEDURE Activate
        THIS.txtEmail.SetFocus
        * Mostrar opcion de Dashboard si ya existe token
        THIS.cmdDashboard.Visible = !EMPTY(gcToken)
    ENDPROC

    PROCEDURE cmdLogin.Click
        THISFORM.DoLogin()
    ENDPROC

    PROCEDURE cmdCancel.Click
        THIS.Release()
    ENDPROC

    PROCEDURE cmdDashboard.Click
        * Ir directo al menu principal reutilizando el token actual
        THIS.Release()
        DO MenuPpal.prg
    ENDPROC

    PROCEDURE Destroy
        CLEAR EVENTS
    ENDPROC

    PROCEDURE DoLogin
        LOCAL lcEmail, lcPassword, lcPayload, loHTTP, lcResponse, lcMessage, llError
        lcEmail = ALLTRIM(THIS.txtEmail.Value)
        lcPassword = ALLTRIM(THIS.txtPassword.Value)
        llError = .F.

        IF EMPTY(lcEmail) OR EMPTY(lcPassword)
            THIS.ShowStatus("Ingrese sus credenciales para continuar.", .T.)
            THIS.txtEmail.SetFocus
            RETURN .F.
        ENDIF

        lcPayload = '{"email":"' + THIS.SanitizeJson(lcEmail) + '","password":"' + ;
                    THIS.SanitizeJson(lcPassword) + '"}'

        THIS.EnableInputs(.F.)
        THIS.ShowStatus("Validando credenciales...", .F.)
        THIS.MousePointer = 11

        TRY
            loHTTP = CREATEOBJECT("WinHttp.WinHttpRequest.5.1")
        CATCH
            THIS.ShowStatus("No se pudo crear el componente WinHTTP.", .T.)
            llError = .T.
        ENDTRY

        IF llError
            THIS.EnableInputs(.T.)
            THIS.MousePointer = 0
            RETURN .F.
        ENDIF

        TRY
            loHTTP.Open("POST", gcAPIBaseURL + "/Auth/login", .F.)
            loHTTP.SetRequestHeader("Content-Type", "application/json")
            loHTTP.Send(lcPayload)
            lcResponse = loHTTP.ResponseText
        CATCH TO loEx
            THIS.ShowStatus("Error de conexion: " + NVL(loEx.Message, "Desconocido."), .T.)
            llError = .T.
        ENDTRY

        THIS.MousePointer = 0
        THIS.EnableInputs(.T.)

        IF llError
            RETURN .F.
        ENDIF

        DO CASE
            CASE loHTTP.Status = 200
                
                * Extraer token de la respuesta
                gcToken = THIS.JsonGetString(lcResponse, "accessToken")
                
                gcUserEmail = THIS.JsonGetString(lcResponse, "email")
                gcUserRole  = THIS.JsonGetString(lcResponse, "role")

                IF EMPTY(gcToken)
                    THIS.ShowStatus("La respuesta no contiene un token valido. Revisar formato JSON.", .T.)
                    RETURN .F.
                ENDIF

                THIS.ShowStatus("Bienvenido " + gcUserEmail + "!", .F., RGB(40,140,90))
                WAIT WINDOW "Cargando panel principal..." TIMEOUT 0.8
            
                DO MenuPpal.prg
                
                 THIS.Release()

            CASE loHTTP.Status = 401
                lcMessage = THIS.GetErrorMessage(lcResponse, "Credenciales invalidas. Intente nuevamente.")
                THIS.ShowStatus(lcMessage, .T.)

            CASE loHTTP.Status >= 400 AND loHTTP.Status < 500
                lcMessage = THIS.GetErrorMessage(lcResponse, "Solicitud rechazada (" + TRANSFORM(loHTTP.Status) + ").")
                THIS.ShowStatus(lcMessage, .T.)

            CASE loHTTP.Status >= 500
                THIS.ShowStatus("Error en el servidor (" + TRANSFORM(loHTTP.Status) + "). Intente mas tarde.", .T.)

            OTHERWISE
                THIS.ShowStatus("Respuesta inesperada del servidor (" + TRANSFORM(loHTTP.Status) + ").", .T.)
        ENDCASE

        RETURN .T.
    ENDPROC

    PROCEDURE EnableInputs
        LPARAMETERS llEnable
        THIS.txtEmail.Enabled = llEnable
        THIS.txtPassword.Enabled = llEnable
        THIS.cmdLogin.Enabled  = llEnable
        THIS.cmdCancel.Enabled = llEnable
        THIS.cmdDashboard.Enabled = llEnable
    ENDPROC

    PROCEDURE ShowStatus
        LPARAMETERS lcMessage, llError, lnColor
        LOCAL lnForeColor
        lnForeColor = IIF(llError, RGB(195,64,64), IIF(TYPE("lnColor") = "N", lnColor, RGB(56,112,218)))
        THIS.lblStatus.ForeColor = lnForeColor
        THIS.lblStatus.Caption   = lcMessage
    ENDPROC

    PROCEDURE GetErrorMessage
        LPARAMETERS lcJSON, lcDefault
        LOCAL lcMessage
        lcMessage = THIS.JsonGetString(lcJSON, "message")
        IF EMPTY(lcMessage)
            lcMessage = lcDefault
        ENDIF
        RETURN lcMessage
    ENDPROC

	PROCEDURE JsonGetString
	    LPARAMETERS lcJSON, lcKey
	    LOCAL lcValue, lcAltKey, lcQuote, lcNeedle

	    lcQuote = CHR(34)

	    lcNeedle = lcQuote + lcKey + lcQuote + ":" + lcQuote
	    lcValue  = STREXTRACT(lcJSON, lcNeedle, lcQuote, 1, 1)

	    IF EMPTY(lcValue)
	        lcNeedle = lcQuote + lcKey + lcQuote + " : " + lcQuote
	        lcValue  = STREXTRACT(lcJSON, lcNeedle, lcQuote, 1, 1)
	    ENDIF

	    IF EMPTY(lcValue)
	        lcAltKey = UPPER(LEFT(lcKey, 1)) + SUBSTR(lcKey, 2)
	        lcNeedle = lcQuote + lcAltKey + lcQuote + ":" + lcQuote
	        lcValue  = STREXTRACT(lcJSON, lcNeedle, lcQuote, 1, 1)

	        IF EMPTY(lcValue)
	            lcNeedle = lcQuote + lcAltKey + lcQuote + " : " + lcQuote
	            lcValue  = STREXTRACT(lcJSON, lcNeedle, lcQuote, 1, 1)
	        ENDIF
	    ENDIF

    RETURN lcValue
ENDPROC


    PROCEDURE SanitizeJson
        LPARAMETERS lcValue
        lcValue = STRTRAN(lcValue, "\", "\\")
        lcValue = STRTRAN(lcValue, '"', '""')
        RETURN lcValue
    ENDPROC
ENDDEFINE







