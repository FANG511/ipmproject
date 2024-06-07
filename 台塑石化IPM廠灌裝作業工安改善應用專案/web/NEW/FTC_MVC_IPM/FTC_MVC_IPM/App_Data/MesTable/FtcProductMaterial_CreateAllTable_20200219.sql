CREATE TABLE [dbo].[FTCBARCODEPRINTERJOB](
	[SYSID] [nvarchar](200) NOT NULL,
	[COMPANYID] [nvarchar](50) NULL,
	[FACTORYID] [nvarchar](50) NULL,
	[TEMPLATEID] [nvarchar](50) NULL,
	[TARGETTYPE] [nvarchar](50) NULL,
	[TARGET] [nvarchar](1000) NULL,
	[TARGETKEY] [nvarchar](1000) NULL,
	[STATUS] [nvarchar](50) NULL,
	[PRINTERNAME] [nvarchar](50) NULL,
	[PRINTERCOUNT] [int] NULL,
	[MODIFYUSER] [nvarchar](50) NULL,
	[MODIFYTIME] [datetime] NULL,
 CONSTRAINT [PK_FTCBARCODEPRINTERJOB] PRIMARY KEY CLUSTERED 
(
	[SYSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[FTCBARCODEPRINTERTEMPLATE](
	[TEMPLATEID] [nvarchar](50) NOT NULL,
	[TEMPLATENAME] [nvarchar](50) NULL,
	[MODIFYUSER] [nvarchar](50) NULL,
	[MODIFYTIME] [datetime] NULL,
	[UPLOADFILE] [nvarchar](50) NULL,
 CONSTRAINT [PK_FTCBARCODETEMPLATE] PRIMARY KEY CLUSTERED 
(
	[TEMPLATEID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[FTCPRODUCTMATERIALPLAN](
	[SYSID] [nvarchar](50) NOT NULL,
	[PLANID] [nvarchar](50) NULL,
	[FACTORID] [nvarchar](50) NULL,
	[FACTORDATAID] [nvarchar](500) NULL,
	[EFFECTIVEDATE] [datetime] NULL,
	[USAGEQTY] [int] NULL,
	[TOTALPACKAGE] [int] NULL,
	[STATUS] [nvarchar](50) NULL,
	[MODIFYUSER] [nvarchar](50) NULL,
	[MODIFYTIME] [datetime] NULL,
	[ITEMCOUT] [int] NULL,
 CONSTRAINT [PK_FTCPRODUCTMATERIALPLAN] PRIMARY KEY CLUSTERED 
(
	[SYSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Index [IX_FTCBARCODEPRINTERJOB]    Script Date: 2020/2/19 上午 11:18:58 ******/
CREATE NONCLUSTERED INDEX [IX_FTCBARCODEPRINTERJOB] ON [dbo].[FTCBARCODEPRINTERJOB]
(
	[TEMPLATEID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

/****** Object:  Index [IX_FTCBARCODEPRINTERJOB_1]    Script Date: 2020/2/19 上午 11:18:58 ******/
CREATE NONCLUSTERED INDEX [IX_FTCBARCODEPRINTERJOB_1] ON [dbo].[FTCBARCODEPRINTERJOB]
(
	[STATUS] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

/****** Object:  Index [IX_FTCPRODUCTMATERIALPLAN]    Script Date: 2020/2/19 上午 11:18:58 ******/
CREATE NONCLUSTERED INDEX [IX_FTCPRODUCTMATERIALPLAN] ON [dbo].[FTCPRODUCTMATERIALPLAN]
(
	[FACTORID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

/****** Object:  Index [IX_FTCPRODUCTMATERIALPLAN_1]    Script Date: 2020/2/19 上午 11:18:58 ******/
CREATE NONCLUSTERED INDEX [IX_FTCPRODUCTMATERIALPLAN_1] ON [dbo].[FTCPRODUCTMATERIALPLAN]
(
	[USAGEQTY] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

ALTER TABLE [dbo].[FTCBARCODEPRINTERJOB] ADD  CONSTRAINT [DF_FTCBARCODEPRINTERJOB_PRINTERNAME]  DEFAULT ('') FOR [PRINTERNAME]

ALTER TABLE [dbo].[FTCBARCODEPRINTERJOB] ADD  CONSTRAINT [DF_FTCBARCODEPRINTERJOB_PRINTERCOUNT]  DEFAULT ((1)) FOR [PRINTERCOUNT]

ALTER TABLE [dbo].[FTCPRODUCTMATERIALPLAN] ADD  CONSTRAINT [DF_FTCPRODUCTMATERIALPLAN_STATUS]  DEFAULT ('0') FOR [STATUS]

ALTER TABLE [dbo].[FTCPRODUCTMATERIALPLAN] ADD  CONSTRAINT [DF_FTCPRODUCTMATERIALPLAN_ITEMCOUT]  DEFAULT ((1)) FOR [ITEMCOUT]

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系統唯一值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERJOB', @level2type=N'COLUMN',@level2name=N'SYSID'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'公司編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERJOB', @level2type=N'COLUMN',@level2name=N'COMPANYID'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'公廠編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERJOB', @level2type=N'COLUMN',@level2name=N'FACTORYID'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'範本編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERJOB', @level2type=N'COLUMN',@level2name=N'TEMPLATEID'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'有以下來源類別，可自行定義如LOT,EQP,EDC,EDC_P' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERJOB', @level2type=N'COLUMN',@level2name=N'TARGETTYPE'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'可依TargetType設定對到該類別的屬性,Ex: TargetType=LOT,Target=ThAvg' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERJOB', @level2type=N'COLUMN',@level2name=N'TARGET'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'依TargetType及Target取得資料來源的唯一值查詢條件' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERJOB', @level2type=N'COLUMN',@level2name=N'TARGETKEY'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'列印狀態0-尚未列印,9-完成列印,1~8錯誤次數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERJOB', @level2type=N'COLUMN',@level2name=N'STATUS'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'指定印表機名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERJOB', @level2type=N'COLUMN',@level2name=N'PRINTERNAME'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'列印張數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERJOB', @level2type=N'COLUMN',@level2name=N'PRINTERCOUNT'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人員' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERJOB', @level2type=N'COLUMN',@level2name=N'MODIFYUSER'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERJOB', @level2type=N'COLUMN',@level2name=N'MODIFYTIME'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'範本編號，唯一值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERTEMPLATE', @level2type=N'COLUMN',@level2name=N'TEMPLATEID'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'範本名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERTEMPLATE', @level2type=N'COLUMN',@level2name=N'TEMPLATENAME'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人員' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERTEMPLATE', @level2type=N'COLUMN',@level2name=N'MODIFYUSER'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERTEMPLATE', @level2type=N'COLUMN',@level2name=N'MODIFYTIME'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上傳檔案名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCBARCODEPRINTERTEMPLATE', @level2type=N'COLUMN',@level2name=N'UPLOADFILE'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'唯一值' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCPRODUCTMATERIALPLAN', @level2type=N'COLUMN',@level2name=N'SYSID'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'計畫編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCPRODUCTMATERIALPLAN', @level2type=N'COLUMN',@level2name=N'PLANID'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'FK 連結FTCMEDCFACTOR.FACTORID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCPRODUCTMATERIALPLAN', @level2type=N'COLUMN',@level2name=N'FACTORID'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'FK 連結FTCMEDCFACTORTYPE.FACTORDATAID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCPRODUCTMATERIALPLAN', @level2type=N'COLUMN',@level2name=N'FACTORDATAID'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'生效日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCPRODUCTMATERIALPLAN', @level2type=N'COLUMN',@level2name=N'EFFECTIVEDATE'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'剩餘包裝量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCPRODUCTMATERIALPLAN', @level2type=N'COLUMN',@level2name=N'USAGEQTY'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'配料總量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCPRODUCTMATERIALPLAN', @level2type=N'COLUMN',@level2name=N'TOTALPACKAGE'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'狀態0-尚未完成,9-完成' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCPRODUCTMATERIALPLAN', @level2type=N'COLUMN',@level2name=N'STATUS'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人員' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCPRODUCTMATERIALPLAN', @level2type=N'COLUMN',@level2name=N'MODIFYUSER'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCPRODUCTMATERIALPLAN', @level2type=N'COLUMN',@level2name=N'MODIFYTIME'

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'單項多批數量' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FTCPRODUCTMATERIALPLAN', @level2type=N'COLUMN',@level2name=N'ITEMCOUT'

