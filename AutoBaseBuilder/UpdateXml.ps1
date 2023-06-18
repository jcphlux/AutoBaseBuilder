param ($XmlFilePath, $AssemblyVersion)
$xml = [xml](Get-Content "./$XmlFilePath")
$xml.xml.Version.value = $AssemblyVersion
$xml.Save($XmlFilePath)
