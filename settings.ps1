# Solution parameters
$SolutionPrefix = "mch.habitat.v91"
$SitePostFix = "dev.local"
$webroot = "d:\work\sitecore"

$SitecoreVersion = "9.1.0 rev. 001564"
$IdentityServerVersion = "2.0.0 rev. 00157"
$InstallerVersion = "2.0.0"

# Assets and prerequisites
$AssetsRoot = "$PSScriptRoot\build\assets"
$AssetsConfigRoot = "$PSScriptRoot\build\AssetsConfiguration"
$AssetsPSRepository = "https://sitecore.myget.org/F/sc-powershell/api/v2/"
$AssetsPSRepositoryName = "SitecoreGallery"

$LicenseFile = "$AssetsRoot\license.xml"

# Certificates
$CertPath = Join-Path "$AssetsRoot" "Certificates"

# SQL Parameters
$SqlServer = ".\SQLServer2016"
$SqlAdminUser = "sa"
$SqlAdminPassword = "test12345"
# Prerequisities Check
$PrerequisitiesConfiguration = "$AssetsConfigRoot\Prerequisites.json"

# XP0 Single Developer Parameters
$SingleDeveloperConfiguration = "$AssetsConfigRoot\XP0-SingleDeveloper.json"

# Sitecore Parameters
$SitecorePackage = "$AssetsRoot\Sitecore $SitecoreVersion (OnPrem)_single.scwdp.zip"
$SitecoreSiteName = "$SolutionPrefix.$SitePostFix"
$SitecoreSiteUrl = "http://$SitecoreSiteName"
$SitecoreSiteName = "$SolutionPrefix.$SitePostFix"
#$SitecoreSiteRoot = Join-Path $webroot -ChildPath $SitecoreSiteName
$SitecoreSiteRoot = $webroot
#$SiteRoot = $webroot
$SitecoreAdminPassword = "b"

# XConnect Parameters
$XConnectPackage = "$AssetsRoot\Sitecore $SitecoreVersion (OnPrem)_xp0xconnect.scwdp.zip"
$XConnectSiteName = "${SolutionPrefix}_xconnect.$SitePostFix"
$XConnectSiteUrl = "https://$XConnectSiteName"
$XConnectSiteRoot = Join-Path $webroot -ChildPath $XConnectSiteName

$XConnectSqlCollectionUser = "collectionuser"
$XConnectSqlCollectionPassword = "test12345"

# Identity Server Parameters
$IdentityServerSiteName = "${SolutionPrefix}_IdentityServer.$SitePostFix"
$IdentityServerUrl = "https://$IdentityServerName"
$IdentityServerPackage = "$AssetsRoot\Sitecore.IdentityServer $IdentityServerVersion (OnPrem)_identityserver.scwdp.zip"
$IdentityClientSecret = "SPDHZpF6g8EXq5F7C5EhPQdsC1UbvTU3"
$IdentityAllowedCorsOrigins = $SitecoreSiteUrl
$IdentityServerSiteRoot = Join-Path $webroot -ChildPath $IdentityServerSiteName

# Solr Parameters
$SolrUrl = "https://localhost:8983/solr"
$SolrRoot = "d:\solr\solr-7.2.1"
$SolrService = "solr-7.2.1"
