# WebAPI  

[返回主目录](../../../README.md)  

## 包安装  
```bash
Microsoft.AspNetCore.Authentication.JwtBearer  
Microsoft.IdentityModel.Tokens  
System.IdentityModel.Tokens.Jwt  
```  

## jwt配置appsettings.json
```
"Authentication":{
	"SecretKey":"",
	"Issuer":"",
	"Audience":"",
}
```