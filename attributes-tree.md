# 特性标记应用场景

## Wxck.AdminTemplate.Api/Filters
  - **LogRequestResponseAttribute.cs**：用于记录API请求和响应日志，帮助开发人员追踪请求和响应的内容，通常用于调试和监控。
  - **SignAttribute.cs**：用于在请求中验证签名，确保请求的合法性，通常用于API的安全性验证，防止数据篡改。
  - **UserRoleAttribute.cs**：用于验证用户角色，确保请求的用户拥有执行某些操作的权限，通常用于权限控制和访问管理。
  - **UserStatusAttribute.cs**：用于检查用户状态（例如是否激活、是否被冻结等），确保用户的操作合法，通常用于身份验证和状态管理。

## Wxck.AdminTemplate.Application/Validators/Attributes
  - **FileMetadataAttribute.cs**：用于验证文件的元数据，如文件类型、大小等，通常用于上传文件的场景，确保文件符合预期要求。
  - **ImageAttribute.cs**：用于验证文件是否为图像类型（例如JPEG、PNG等），常用于确保上传的文件是有效的图片。
  - **VerificationCaptchaAttribute.cs**：用于验证验证码字段，确保用户输入的验证码正确，通常用于防止自动化的垃圾提交或机器人操作。

## Wxck.AdminTemplate.Application/Validators/Attributes/User
- 用于在`LoginMethod`字段中标记验证，检查`LoginMethod`字段的值是否合法
  - **LoginMethodValidAttribute.cs**：用于验证登录方法的合法性，例如检查登录方式是否支持（如用户名/密码、社交账户等），确保用户选择的登录方式有效。
- 通常用于在`UserCode`字段中标记验证`UserCode`的内容是否已存在
  - **UserCodeExistsAttribute.cs**：用于验证用户输入的`UserCode`（例如用户编号或注册代码）是否已经在数据库中存在，防止重复注册或使用。
  - **UserNameExistsAttribute.cs**：用于验证用户名是否已被其他用户使用，确保每个用户名的唯一性。
  - **UserPhoneExistsAttribute.cs**：用于验证用户手机号码是否已注册，避免重复使用相同的电话号码进行注册。

## Wxck.AdminTemplate.Domain/Attributes
  - **ExcludeOnUpdateAttribute.cs**：用于标记某个字段在更新操作时应该被排除，通常用于标记不需要修改的字段，或不应更新的系统字段（如创建时间等）。
  - **InjectableRepositoryAttribute.cs**：用于标记该类或接口是可注入的Repository，通常用于依赖注入（DI）系统中，确保Repository能够在需要的地方自动注入。
  - **InjectableServiceAttribute.cs**：用于标记该类或接口是可注入的Service，类似于`InjectableRepositoryAttribute`，但用于业务逻辑层或其他服务类。
  - **InsertOrUpdataAttribute.cs**：用于标记字段或方法，在插入或更新数据时执行的操作，通常用于标记需要在数据库操作时同时处理的字段。
  - **MaskPhoneAttribute.cs**：用于对用户的电话号码进行遮罩处理，只显示部分数字，保护用户隐私，常用于展示时的隐私保护。
  - **UpdataAttribute.cs**：用于标记字段是否可在更新操作中进行修改，常用于控制哪些字段在更新时可以被修改。
  - **UpdateByAttribute.cs**：用于标记字段的更新来源，例如记录哪个用户或系统进行的更新，通常用于审计跟踪或数据变更历史记录。
