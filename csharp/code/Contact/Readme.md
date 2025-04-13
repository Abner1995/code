# 通讯录

项目使用整洁架构实现通讯录APP  

这个比较失败的项目模仿

## 技术栈
.NET 8 WebAPI + MySQL + MAUI

## 笔记  

1. 项目引用：  
Contact.API引用Contact.Application、Contact.Infrastructure  
Contact.Application引用Contact.Domain  
Contact.Infrastructure引用Contact.Application  

2. Entities实体类放在Contact.Domain  

3. 在Infrastructure(基础设施层)项目中安装EntityFramework包  
> [!NOTE]  
> 由于用的环境是.NET8，EntityFrameworkCore版本也得是8不然就会有冲突  
```bash  
Install-Package Pomelo.EntityFrameworkCore.MySql -Version 8.0.2  
Install-Package Microsoft.EntityFrameworkCore.Relational -Version 8.0.2  
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.2  
```  

4. 在Infrastructure(基础设施层)项目中使用FluentAPI来配置实体关系  

5. 在API项目中安装EntityFramework包  
> [!NOTE]  
> 由于用的环境是.NET8，EntityFrameworkCore版本也得是8不然就会有冲突  
```bash  
Install-Package Microsoft.EntityFrameworkCore.Relational -Version 8.0.2  
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.2  
```  