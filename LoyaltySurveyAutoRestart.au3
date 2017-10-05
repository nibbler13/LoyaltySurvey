#Region ;**** Directives created by AutoIt3Wrapper_GUI ****
#AutoIt3Wrapper_Icon=LoyaltySurveyAutoRestartIcon.ico
#EndRegion ;**** Directives created by AutoIt3Wrapper_GUI ****
#pragma compile(ProductVersion, 1.0)
#pragma compile(UPX, true)
#pragma compile(CompanyName, 'ООО Клиника ЛМС')
#pragma compile(FileDescription, Приложение для перезапуска процесса монитора лояльности)
#pragma compile(LegalCopyright, Грашкин Павел Павлович - Нижний Новгород - 31-555 - nn-admin@budzdorov.ru)
#pragma compile(ProductName, InfomatAutoRestart)

Local $sProcessName = "LoyaltySurvey.exe"
Local $sWindowTitle = "MainWindow"

While 1
	If Not ProcessExists($sProcessName) Then
		ShellExecute($sProcessName)
	Else
		If Not WinActive($sWindowTitle) Then
			WinActivate($sWindowTitle)
			Send("!{TAB}")
		EndIf

	EndIf

	Sleep(30 * 1000)
WEnd