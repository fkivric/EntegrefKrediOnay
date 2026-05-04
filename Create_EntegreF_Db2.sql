USE [master]
GO
/****** Object:  Database [EntegreF]    Script Date: 03-May-26 3:41:37 PM ******/
CREATE DATABASE [EntegreF]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Yonavm_Web_Siparis', FILENAME = N'G:\Sql_Data\EntegreF.mdf' , SIZE = 345088KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Yonavm_Web_Siparis_log', FILENAME = N'H:\Sql_Log\EntegreF_0.ldf' , SIZE = 21287424KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [EntegreF] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [EntegreF].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [EntegreF] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [EntegreF] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [EntegreF] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [EntegreF] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [EntegreF] SET ARITHABORT OFF 
GO
ALTER DATABASE [EntegreF] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [EntegreF] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [EntegreF] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [EntegreF] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [EntegreF] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [EntegreF] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [EntegreF] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [EntegreF] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [EntegreF] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [EntegreF] SET  DISABLE_BROKER 
GO
ALTER DATABASE [EntegreF] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [EntegreF] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [EntegreF] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [EntegreF] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [EntegreF] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [EntegreF] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [EntegreF] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [EntegreF] SET RECOVERY FULL 
GO
ALTER DATABASE [EntegreF] SET  MULTI_USER 
GO
ALTER DATABASE [EntegreF] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [EntegreF] SET DB_CHAINING OFF 
GO
ALTER DATABASE [EntegreF] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [EntegreF] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [EntegreF] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [EntegreF] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'EntegreF', N'ON'
GO
ALTER DATABASE [EntegreF] SET QUERY_STORE = OFF
GO
USE [EntegreF]
GO
/****** Object:  UserDefinedFunction [dbo].[base64_decode]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[base64_decode]
(
  @encoded_text varchar(8000)
)
RETURNS 
          varchar(6000)
AS BEGIN
--local variables
DECLARE
  @output           varchar(8000),
  @block_start      int,
  @encoded_length   int,
  @decoded_length   int,
  @mapr             binary(122)
--IF @encoded_text COLLATE LATIN1_GENERAL_BIN
-- LIKE '%[^ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=]%'
--     COLLATE LATIN1_GENERAL_BIN
--  RETURN NULL
--IF LEN(@encoded_text) & 3 > 0
--  RETURN NULL
SET @output   = ''
-- The nth byte of @mapr contains the base64 value
-- of the character with an ASCII value of n.
-- EG, 65th byte = 0x00 = 0 = value of 'A'
SET @mapr =
  0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF -- 1-33
+ 0xFFFFFFFFFFFFFFFFFFFF3EFFFFFF3F3435363738393A3B3C3DFFFFFF00FFFFFF -- 33-64
+ 0x000102030405060708090A0B0C0D0E0F10111213141516171819FFFFFFFFFFFF -- 65-96
+ 0x1A1B1C1D1E1F202122232425262728292A2B2C2D2E2F30313233 -- 97-122
--get the number of blocks to be decoded
SET @encoded_length = LEN(@encoded_text)
SET @decoded_length = @encoded_length / 4 * 3
--for each block
SET @block_start = 1
WHILE @block_start < @encoded_length BEGIN
  --decode the block and add to output
  --BINARY values between 1 and 4 bytes can be implicitly cast to INT
  SET @output = @output +  CAST(CAST(CAST(
   substring( @mapr, ascii( substring( @encoded_text, @block_start    , 1) ), 1) * 262144
 + substring( @mapr, ascii( substring( @encoded_text, @block_start + 1, 1) ), 1) * 4096
 + substring( @mapr, ascii( substring( @encoded_text, @block_start + 2, 1) ), 1) * 64
 + substring( @mapr, ascii( substring( @encoded_text, @block_start + 3, 1) ), 1) 
   AS INTEGER) AS BINARY(3)) AS VARCHAR(3))
  SET @block_start = @block_start + 4
END
IF RIGHT(@encoded_text, 2) = '=='
 SET @decoded_length = @decoded_length - 2
ELSE IF RIGHT(@encoded_text, 1) = '='
 SET @decoded_length = @decoded_length - 1
--IF SUBSTRING(@output, @decoded_length, 1) = CHAR(0)
-- SET @decoded_length = @decoded_length - 1
--return the decoded string
RETURN LEFT(@output, @decoded_length)
END
GO
/****** Object:  UserDefinedFunction [dbo].[base64_encode]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[base64_encode]
(
  @plain_text varchar(6000)
)
RETURNS 
          varchar(8000)
AS BEGIN
--local variables
DECLARE
  @output            varchar(8000),
  @input_length      integer,
  @block_start       integer,
  @partial_block_start  integer, -- position of last 0, 1 or 2 characters
  @partial_block_length integer,
  @block_val         integer,
  @map               char(64)
SET @map = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/'
--initialise variables
SET @output   = ''
--set length and count
SET @input_length      = LEN( @plain_text + '#' ) - 1
SET @partial_block_length = @input_length % 3
SET @partial_block_start = @input_length - @partial_block_length
SET @block_start       = 1
--for each block
WHILE @block_start < @partial_block_start  BEGIN
  SET @block_val = CAST(SUBSTRING(@plain_text, @block_start, 3) AS BINARY(3))
  --encode the 3 character block and add to the output
  SET @output = @output + SUBSTRING(@map, @block_val / 262144 + 1, 1)
                        + SUBSTRING(@map, (@block_val / 4096 & 63) + 1, 1)
                        + SUBSTRING(@map, (@block_val / 64 & 63  ) + 1, 1)
                        + SUBSTRING(@map, (@block_val & 63) + 1, 1)
  --increment the counter
  SET @block_start = @block_start + 3
END
IF @partial_block_length > 0
BEGIN
  SET @block_val = CAST(SUBSTRING(@plain_text, @block_start, @partial_block_length)
                      + REPLICATE(CHAR(0), 3 - @partial_block_length) AS BINARY(3))
  SET @output = @output
 + SUBSTRING(@map, @block_val / 262144 + 1, 1)
 + SUBSTRING(@map, (@block_val / 4096 & 63) + 1, 1)
 + CASE WHEN @partial_block_length < 2
    THEN REPLACE(SUBSTRING(@map, (@block_val / 64 & 63  ) + 1, 1), 'A', '=')
    ELSE SUBSTRING(@map, (@block_val / 64 & 63  ) + 1, 1) END
 + CASE WHEN @partial_block_length < 3
    THEN REPLACE(SUBSTRING(@map, (@block_val & 63) + 1, 1), 'A', '=')
    ELSE SUBSTRING(@map, (@block_val & 63) + 1, 1) END
END
--return the result
RETURN @output
END
GO
/****** Object:  UserDefinedFunction [dbo].[FK_BKUCUK]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[FK_BKUCUK] (
@kelime NVARCHAR(MAX)  
)  
RETURNS NVARCHAR(MAX)  
AS  
BEGIN  
	DECLARE @girdi NVARCHAR(MAX)  
	DECLARE @sonuc NVARCHAR(MAX)  
	DECLARE @say INTEGER 
	DECLARE @uzunluk INTEGER 
	SET @girdi = LTRIM(@kelime)
	SET @uzunluk=LEN(@girdi)
	SET @say=0
 
	WHILE @uzunluk>@say
	BEGIN
		SET @say=@say+1
		IF @say=1
		BEGIN
			SET @sonuc=
			CASE
				WHEN SUBSTRING (@girdi,@say,1)='ç' THEN  'Ç'
				WHEN SUBSTRING (@girdi,@say,1)='g' THEN  'G'
				WHEN SUBSTRING (@girdi,@say,1)='i' THEN  'I'
				WHEN SUBSTRING (@girdi,@say,1)='ö' THEN  'Ö'
				WHEN SUBSTRING (@girdi,@say,1)='ü' THEN  'Ü'
				ELSE UPPER(SUBSTRING (@girdi,@say,1))
			END	
			SET @say=@say+1
		END 
		IF SUBSTRING(@girdi,@say,1)=' '
		BEGIN
			SET @say=@say+1
			WHILE SUBSTRING(@girdi,@say,1)=' '
			BEGIN
				SET @say=@say+1
			END			
			SET @sonuc=@sonuc+
			CASE
				WHEN SUBSTRING (@girdi,@say,1)='ç' THEN  ' Ç'
				WHEN SUBSTRING (@girdi,@say,1)='g' THEN  ' G'
				WHEN SUBSTRING (@girdi,@say,1)='i' THEN  ' I'
				WHEN SUBSTRING (@girdi,@say,1)='ö' THEN  ' Ö'
				WHEN SUBSTRING (@girdi,@say,1)='ü' THEN  ' Ü'
				ELSE ' ' +UPPER(SUBSTRING (@girdi,@say,1))
			END	
		END
		ELSE
		BEGIN
			SET @sonuc=@sonuc+LOWER(SUBSTRING (@girdi,@say,1))
		END						
	END
RETURN @sonuc
END
GO
/****** Object:  UserDefinedFunction [dbo].[mde_fn_realocate]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[mde_fn_realocate]
(@Sayi as int)


RETURNS @ResAllocate TABLE (Folders int,Files int,Other int) 
  
as 

    BEGIN

	insert into @ResAllocate
	select 
	ceiling(@sayi*1.0/200) klasor,
	floor(@sayi*1.0/(ceiling(@sayi*1.0/200))) dosya,
	@sayi % floor(@sayi*1.0/(ceiling(@sayi*1.0/200))) artik



    RETURN
END
GO
/****** Object:  UserDefinedFunction [dbo].[mde_fn_siplit_string]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[mde_fn_siplit_string] 
( 
   @DelimittedString [varchar](max), 
   @Delimiter [varchar](1) 
) 
RETURNS @Table Table (Value [varchar](100)) 
BEGIN 
   DECLARE @sTemp [varchar](max) 
   SET @sTemp = ISNULL(@DelimittedString,'') 
                + @Delimiter 
   WHILE LEN(@sTemp) > 0 
   BEGIN 
      INSERT INTO @Table 
      SELECT SubString(@sTemp,1,
             CharIndex(@Delimiter,@sTemp)-1) 
      
	  INSERT INTO @Table
      SELECT REPLACE((SubString(@sTemp,1,
             CharIndex(@Delimiter,@sTemp)-1)),'M','R') 
		
      SET @sTemp = RIGHT(@sTemp,
        LEN(@sTemp)-CharIndex(@Delimiter,@sTemp)) 
   END 
   RETURN 
END
GO
/****** Object:  UserDefinedFunction [dbo].[mde_fn_Split]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[mde_fn_Split](@String nvarchar(max), @Delimiter char(1))
RETURNS @Results TABLE (Items nvarchar(4000))
AS


    BEGIN
    DECLARE @INDEX INT
    DECLARE @SLICE nvarchar(4000)
    -- HAVE TO SET TO 1 SO IT DOESNT EQUAL Z
    --     ERO FIRST TIME IN LOOP
    SELECT @INDEX = 1
    -- following line added 10/06/04 as null
    --      values cause issues
    IF @String IS NULL RETURN
    WHILE @INDEX !=0


        BEGIN	
        	-- GET THE INDEX OF THE FIRST OCCURENCE OF THE SPLIT CHARACTER
        	SELECT @INDEX = CHARINDEX(@Delimiter,@String)
        	-- NOW PUSH EVERYTHING TO THE LEFT OF IT INTO THE SLICE VARIABLE
        	IF @INDEX !=0
        		SELECT @SLICE = LEFT(@String,@INDEX - 1)
        	ELSE
        		SELECT @SLICE = @String
        	-- PUT THE ITEM INTO THE RESULTS SET
        	INSERT INTO @Results(Items) VALUES(@SLICE)
        	-- CHOP THE ITEM REMOVED OFF THE MAIN STRING
        	SELECT @String = RIGHT(@String,LEN(@String) - @INDEX)
        	-- BREAK OUT IF WE ARE DONE
        	IF LEN(@String) = 0 BREAK
    END
    RETURN
END
GO
/****** Object:  Table [dbo].[TicimaxKargoTakip]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxKargoTakip](
	[SiparisID] [int] NULL,
	[UrunID] [int] NULL,
	[SiparisFaturaID] [varchar](20) NULL,
	[SiparisFaturaDetayID] [varchar](20) NULL,
	[GöndericiSubeKodu] [varchar](20) NULL,
	[MNGSiparisNumber] [varchar](20) NULL,
	[MNGSiparisID] [varchar](20) NULL,
	[MNGBarkodNumber] [int] NULL,
	[MNGSiparisTarihi] [smalldatetime] NULL,
	[MNGBarkod] [varchar](20) NULL,
	[MNGTakipAdresi] [nvarchar](100) NULL,
	[MNGDurumu] [nvarchar](100) NULL,
	[MNGTeslimDurum] [int] NULL,
	[MNGTeslimTarihi] [smalldatetime] NULL,
	[MNGTeslimAlan] [nvarchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantMagazaIletisim]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantMagazaIletisim](
	[DIVVAL] [varchar](2) NOT NULL,
	[DIVNAME] [varchar](25) NULL,
	[DIVMDRNAME] [varchar](100) NULL,
	[DIVPHN1] [varchar](15) NULL,
	[DIVPHN2] [varchar](15) NULL,
	[DIVADR] [varchar](100) NULL,
	[DIVEMAIL] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicimaxSiparisOdeme]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxSiparisOdeme](
	[BankaKomisyonu] [float] NULL,
	[CheckSum] [nvarchar](255) NULL,
	[HavaleBankaID] [int] NULL,
	[HavaleHesapID] [int] NULL,
	[ID] [int] NULL,
	[KKOdemeBankaID] [int] NULL,
	[KapidaOdemeTutari] [float] NULL,
	[OdemeIndirimi] [float] NULL,
	[OdemeNotu] [nvarchar](255) NULL,
	[OdemeSecenekID] [int] NULL,
	[OdemeTipi] [int] NULL,
	[Onaylandi] [int] NULL,
	[PosReferansID] [nvarchar](255) NULL,
	[SiparisID] [int] NULL,
	[TaksitSayisi] [int] NULL,
	[Tarih] [datetime] NULL,
	[Tutar] [float] NULL,
	[UyeID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicimaxUye]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxUye](
	[CepTelefonu] [nvarchar](255) NULL,
	[CinsiyetID] [int] NULL,
	[DogumTarihi] [datetime] NULL,
	[DuzenlemeTarihi] [datetime] NULL,
	[ID] [int] NULL,
	[Il] [nvarchar](255) NULL,
	[Ilce] [nvarchar](255) NULL,
	[Mahalle] [nvarchar](255) NULL,
	[IlceID] [int] NULL,
	[IlID] [int] NULL,
	[MahalleID] [int] NULL,
	[Isim] [nvarchar](255) NULL,
	[Mail] [nvarchar](255) NULL,
	[MailIzin] [bit] NULL,
	[Meslek] [nvarchar](255) NULL,
	[MusteriKodu] [nvarchar](255) NULL,
	[OgrenimDurumu] [nvarchar](255) NULL,
	[Sifre] [nvarchar](255) NULL,
	[SmsIzin] [bit] NULL,
	[Soyisim] [nvarchar](255) NULL,
	[Telefon] [nvarchar](255) NULL,
	[UyeTuruID] [int] NULL,
	[UyeTCtoVKN] [varchar](11) NULL,
	[UyeVD] [varchar](250) NULL,
	[UyeIsVm] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebSiparisOdemeTipi]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebSiparisOdemeTipi](
	[Ticimax_ID] [int] NOT NULL,
	[Ticimax_Adi] [varchar](60) NULL,
	[Volant_DPYMID] [int] NULL,
	[Volant_DPYMVAL] [varchar](10) NULL,
	[Volant_DPYMNAME] [varchar](60) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicimaxSiparis]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxSiparis](
	[AdiSoyadi] [nvarchar](255) NULL,
	[Durum] [int] NULL,
	[EntegrasyonAktarildi] [bit] NULL,
	[FaturaAdresId] [int] NULL,
	[FaturaAdresi] [nvarchar](255) NULL,
	[FaturaNo] [nvarchar](255) NULL,
	[FaturaTarihi] [datetime] NULL,
	[HediyeCeki] [nvarchar](255) NULL,
	[HediyeCekiTutari] [float] NULL,
	[HadiyePaketiNotu] [nvarchar](255) NULL,
	[HediyePaketiTutari] [float] NULL,
	[HediyePaketiVar] [bit] NULL,
	[ID] [int] NULL,
	[IPAdresi] [nvarchar](255) NULL,
	[IndirimTutari] [float] NULL,
	[KampanyaID] [int] NULL,
	[KargoAdresID] [int] NULL,
	[KargoEntegrasyonID] [nvarchar](255) NULL,
	[KargoEntegrasyonTakipNo] [float] NULL,
	[KargoFirmaId] [int] NULL,
	[KargoFirmaTanim] [varchar](255) NULL,
	[KargoTakipNo] [nvarchar](255) NULL,
	[KargoTutari] [float] NULL,
	[Kaynak] [int] NULL,
	[Kur] [float] NULL,
	[Mail] [nvarchar](255) NULL,
	[Maliyet] [float] NULL,
	[MarketplaceKampanyaKodu] [nvarchar](255) NULL,
	[Odemeler] [nvarchar](255) NULL,
	[OlusturanId] [int] NULL,
	[OzelAlan1] [nvarchar](255) NULL,
	[OzelAlan2] [nvarchar](255) NULL,
	[OzelAlan3] [nvarchar](255) NULL,
	[OzellestirmeTutari] [float] NULL,
	[PaketlemeDurumu] [nvarchar](255) NULL,
	[PaketlemeDurumuID] [int] NULL,
	[ParaBirimi] [nvarchar](255) NULL,
	[PuanIndirimi] [float] NULL,
	[PuanKullanimID] [int] NULL,
	[Referer] [nvarchar](255) NULL,
	[ReklamKaynagi] [nvarchar](255) NULL,
	[SepetKampanyasiIndirimi] [float] NULL,
	[SiparisDurumu] [nvarchar](255) NULL,
	[SiparisKaynagi] [nvarchar](255) NULL,
	[SiparisKodu] [nvarchar](255) NULL,
	[SiparisNo] [nvarchar](255) NULL,
	[SiparisNotu] [nvarchar](255) NULL,
	[SiparisTarihi] [datetime] NULL,
	[SiparisToplamTutari] [float] NULL,
	[StokDustu] [bit] NULL,
	[TeslimatAdresiId] [int] NULL,
	[TeslimatAdresi] [nvarchar](255) NULL,
	[TeslimatGunu] [datetime] NULL,
	[TeslimatSaati] [nvarchar](255) NULL,
	[ToplamKdv] [float] NULL,
	[ToplamTutar] [float] NULL,
	[Tutar] [float] NULL,
	[Urunler] [nvarchar](255) NULL,
	[UyeAdi] [nvarchar](255) NULL,
	[UyeID] [int] NULL,
	[UyeMusteriKodu] [nvarchar](255) NULL,
	[UyeSoyadi] [nvarchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicimaxTeslimatAdres]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxTeslimatAdres](
	[Adres] [nvarchar](255) NULL,
	[AdresTarifi] [nvarchar](255) NULL,
	[AliciAdi] [nvarchar](255) NULL,
	[AliciTelefon] [nvarchar](255) NULL,
	[ID] [int] NULL,
	[Il] [nvarchar](255) NULL,
	[IlId] [int] NULL,
	[IlKodu] [nvarchar](255) NULL,
	[Ilce] [nvarchar](255) NULL,
	[IlceId] [int] NULL,
	[IlceKodu] [nvarchar](255) NULL,
	[PostaKodu] [nvarchar](255) NULL,
	[Ulke] [nvarchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[KargoBarkod]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[KargoBarkod]
AS
SELECT        CONVERT(varchar(10), dbo.TicimaxSiparis.ID) AS ID, dbo.TicimaxSiparis.AdiSoyadi AS Musteri, dbo.TicimaxUye.Telefon AS Telefom, dbo.TicimaxSiparis.TeslimatAdresi AS Adres, 
                         dbo.TicimaxSiparis.KargoFirmaTanim AS KargoFirma, dbo.WebSiparisOdemeTipi.Ticimax_Adi AS OdemeTipi, dbo.TicimaxSiparis.SiparisToplamTutari AS Tutar, dbo.TicimaxSiparis.Urunler AS UrunSayisi, 
                         dbo.TicimaxSiparis.SiparisNotu, dbo.TicimaxKargoTakip.SiparisFaturaID, dbo.TicimaxKargoTakip.GöndericiSubeKodu, dbo.TicimaxKargoTakip.SiparisFaturaDetayID, dbo.TicimaxKargoTakip.MNGSiparisNumber, 
                         dbo.TicimaxKargoTakip.MNGSiparisID, dbo.TicimaxKargoTakip.MNGBarkodNumber, dbo.TicimaxKargoTakip.MNGSiparisTarihi, dbo.TicimaxKargoTakip.MNGBarkod, dbo.TicimaxKargoTakip.MNGTakipAdresi, 
                         dbo.TicimaxKargoTakip.MNGDurumu, dbo.TicimaxKargoTakip.MNGTeslimDurum, dbo.TicimaxKargoTakip.MNGTeslimTarihi, dbo.TicimaxKargoTakip.MNGTeslimAlan, dbo.VolantMagazaIletisim.DIVNAME, 
                         dbo.VolantMagazaIletisim.DIVPHN1, dbo.VolantMagazaIletisim.DIVMDRNAME, dbo.VolantMagazaIletisim.DIVADR, dbo.VolantMagazaIletisim.DIVPHN2, dbo.TicimaxTeslimatAdres.ID AS Expr1, dbo.TicimaxTeslimatAdres.Il, 
                         dbo.TicimaxTeslimatAdres.Ilce, dbo.TicimaxTeslimatAdres.PostaKodu, dbo.TicimaxTeslimatAdres.Ulke
FROM            dbo.TicimaxSiparis INNER JOIN
                         dbo.TicimaxUye ON dbo.TicimaxSiparis.UyeID = dbo.TicimaxUye.ID INNER JOIN
                         dbo.WebSiparisOdemeTipi INNER JOIN
                         dbo.TicimaxSiparisOdeme ON dbo.WebSiparisOdemeTipi.Ticimax_ID = dbo.TicimaxSiparisOdeme.OdemeTipi ON dbo.TicimaxSiparis.ID = dbo.TicimaxSiparisOdeme.SiparisID AND 
                         dbo.TicimaxSiparisOdeme.Onaylandi = 1 INNER JOIN
                         dbo.TicimaxKargoTakip ON dbo.TicimaxSiparis.ID = dbo.TicimaxKargoTakip.SiparisID AND dbo.TicimaxSiparisOdeme.SiparisID = dbo.TicimaxKargoTakip.SiparisID INNER JOIN
                         dbo.TicimaxTeslimatAdres ON dbo.TicimaxSiparis.TeslimatAdresiId = dbo.TicimaxTeslimatAdres.ID LEFT OUTER JOIN
                         dbo.VolantMagazaIletisim ON CASE WHEN dbo.TicimaxSiparis.OzelAlan3 = '' THEN 'WB' ELSE dbo.TicimaxSiparis.OzelAlan3 END = dbo.VolantMagazaIletisim.DIVVAL
GO
/****** Object:  Table [dbo].[TicimaxKargoFirma]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxKargoFirma](
	[ID] [int] NULL,
	[Tanim] [nvarchar](60) NULL,
	[KargoFiyati] [money] NULL,
	[KapidaOdeme] [bit] NULL,
	[KapidaOdemeFiyati] [money] NULL,
	[KapidaOdemeKK] [bit] NULL,
	[KapidaOdemeKKFiyati] [money] NULL,
	[Logo] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicimaxFaturaAdres]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxFaturaAdres](
	[Adres] [nvarchar](255) NULL,
	[AliciTelefon] [nvarchar](255) NULL,
	[EntegrasyonId] [nvarchar](255) NULL,
	[FirmaAdi] [nvarchar](255) NULL,
	[ID] [int] NULL,
	[Il] [nvarchar](255) NULL,
	[IlId] [int] NULL,
	[IlKodu] [nvarchar](255) NULL,
	[Ilce] [nvarchar](255) NULL,
	[IlceId] [int] NULL,
	[IlceKodu] [nvarchar](255) NULL,
	[Ulke] [nvarchar](255) NULL,
	[VergiDairesi] [nvarchar](255) NULL,
	[VergiNo] [nvarchar](255) NULL,
	[isKurumsal] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[SiparisDokum]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[SiparisDokum]
AS
SELECT        CONVERT(varchar(10), dbo.TicimaxSiparis.ID) AS SiparisID, dbo.TicimaxSiparis.AdiSoyadi, dbo.TicimaxFaturaAdres.Adres, dbo.TicimaxFaturaAdres.VergiNo, dbo.TicimaxFaturaAdres.VergiDairesi, dbo.TicimaxFaturaAdres.Il, 
                         dbo.TicimaxFaturaAdres.Ilce, dbo.TicimaxFaturaAdres.FirmaAdi, dbo.TicimaxFaturaAdres.AliciTelefon, dbo.TicimaxTeslimatAdres.Adres AS Expr1, dbo.TicimaxTeslimatAdres.AliciAdi, 
                         dbo.TicimaxTeslimatAdres.AliciTelefon AS Expr2, dbo.TicimaxTeslimatAdres.AdresTarifi, dbo.TicimaxTeslimatAdres.Il AS Expr3, dbo.TicimaxTeslimatAdres.Ilce AS Expr4, dbo.TicimaxSiparis.SiparisToplamTutari, 
                         dbo.TicimaxSiparis.ToplamKdv, dbo.TicimaxSiparis.ToplamTutar, dbo.TicimaxSiparis.OzelAlan3, dbo.TicimaxKargoFirma.Tanim, dbo.TicimaxTeslimatAdres.PostaKodu, ROUND(dbo.TicimaxSiparis.ToplamKdv, 3) 
                         AS SiparisToplamKDV, ROUND(dbo.TicimaxSiparis.ToplamTutar, 3) AS SiparisToplamKDVHaric, ROUND(dbo.TicimaxSiparis.ToplamTutar + dbo.TicimaxSiparis.ToplamKdv, 3) AS SiparisToplamTutar, 
                         ROUND(dbo.TicimaxSiparis.KargoTutari, 0) AS SiparisToplamKargoTutar, CASE WHEN dbo.TicimaxSiparisOdeme.OdemeIndirimi > 0 THEN round(dbo.TicimaxSiparisOdeme.OdemeIndirimi, 3) 
                         ELSE 0 END AS SiparisToplamIndirim, ROUND(dbo.TicimaxSiparis.HediyeCekiTutari, 0) AS SiparisToplaHCTutar, dbo.TicimaxSiparisOdeme.Tutar, dbo.TicimaxSiparis.SiparisTarihi
FROM            dbo.TicimaxSiparis INNER JOIN
                         dbo.TicimaxFaturaAdres ON dbo.TicimaxSiparis.FaturaAdresId = dbo.TicimaxFaturaAdres.ID INNER JOIN
                         dbo.TicimaxTeslimatAdres ON dbo.TicimaxSiparis.TeslimatAdresiId = dbo.TicimaxTeslimatAdres.ID INNER JOIN
                         dbo.TicimaxKargoFirma ON dbo.TicimaxSiparis.KargoFirmaId = dbo.TicimaxKargoFirma.ID INNER JOIN
                         dbo.TicimaxSiparisOdeme ON dbo.TicimaxSiparis.ID = dbo.TicimaxSiparisOdeme.SiparisID
WHERE        (dbo.TicimaxSiparisOdeme.Onaylandi = 1)
GO
/****** Object:  View [dbo].[SiparisOdeme]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[SiparisOdeme]
AS
SELECT        CONVERT(varchar(10), dbo.TicimaxSiparisOdeme.SiparisID) AS SiparisID, dbo.WebSiparisOdemeTipi.Ticimax_Adi, dbo.TicimaxSiparisOdeme.TaksitSayisi, dbo.TicimaxSiparisOdeme.Tutar
FROM            dbo.TicimaxSiparisOdeme INNER JOIN
                         dbo.WebSiparisOdemeTipi ON dbo.TicimaxSiparisOdeme.OdemeTipi = dbo.WebSiparisOdemeTipi.Ticimax_ID
GO
/****** Object:  Table [dbo].[WebSiparisOdeme]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebSiparisOdeme](
	[BankaKomisyonu] [float] NULL,
	[CheckSum] [nvarchar](255) NULL,
	[HavaleBankaID] [int] NULL,
	[HavaleHesapID] [int] NULL,
	[ID] [int] NULL,
	[KKOdemeBankaID] [int] NULL,
	[KapidaOdemeTutari] [float] NULL,
	[OdemeIndirimi] [float] NULL,
	[OdemeNotu] [nvarchar](255) NULL,
	[OdemeSecenekID] [int] NULL,
	[OdemeTipi] [int] NULL,
	[Onaylandi] [int] NULL,
	[PosReferansID] [nvarchar](255) NULL,
	[SiparisID] [int] NULL,
	[TaksitSayisi] [int] NULL,
	[Tarih] [datetime] NULL,
	[Tutar] [float] NULL,
	[UyeID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[SiparisFisi]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[SiparisFisi]
AS
SELECT        TOP (100) PERCENT CONVERT(varchar(10), w.ID) AS SiparisID, m.DIVVAL + SPACE(1) + m.DIVNAME AS TeslimatMagaza, m.DIVMDRNAME AS MüdürAdi, m.DIVADR AS TeslimatMagazaAdres, w.SiparisTarihi AS Tarih, 
                         CASE WHEN isnull(kt.SiparisFaturaID, '') != '' THEN CONVERT(varchar(10), kt.SiparisID) ELSE SiparisNo END AS KarkoBarkod, w.AdiSoyadi AS TeslimatAdSoyad, w.TeslimatAdresi AS TeslimatAdres, 
                         t.Ulke + '/' + t.Il + '/' + t.Ilce AS TeslimatÜlkeSehirIlçe, t.PostaKodu AS TeslimatPostaKodu, t.AliciTelefon AS TeslimatTelefon, u.Mail AS TeslimatEposta, w.SiparisNotu AS TeslimatNot, k.Tanim AS TeslimatKargo, 
                         w.AdiSoyadi AS FaturaAdSoyad, w.FaturaAdresi AS FaturaAdres, f.Ulke + '/' + f.Il + '/' + f.Ilce AS FaturaÜlkeSehirIlçe, f.AliciTelefon AS FaturaTelefon, CASE WHEN f.isKurumsal = 1 THEN f.FirmaAdi ELSE '' END AS FaturaFirma, 
                         CASE WHEN f.isKurumsal = 1 THEN f.VergiNo ELSE '' END AS FaturaVKN, f.VergiDairesi AS FaturaVD, ISNULL(o.KKOdemeBankaID, '') AS KKOdemeBankaID, 
                         ISNULL(CASE WHEN ot.Ticimax_Adi LIKE 'KapidaOdeme%' THEN '12' ELSE o.TaksitSayisi END, '12') AS TaksitSayisi, ISNULL(o.OdemeNotu, '') AS OdemeNotu, ISNULL(o.Tutar, '') AS Tutar, ISNULL(o.OdemeTipi, '') 
                         AS OdemeTipi, ISNULL(CASE WHEN ot.Ticimax_Adi LIKE 'KapidaOdeme%' THEN 'Elden Taksitli' ELSE ot.Ticimax_Adi END, 'Elden Taksitli') AS Ticimax_Adi, ROUND(w.ToplamKdv, 3) AS SiparisToplamKDV, 
                         ROUND(w.ToplamTutar, 3) AS SiparisToplamKDVHaric, ROUND(w.ToplamTutar + w.ToplamKdv, 3) AS SiparisToplamTutar, ROUND(w.KargoTutari, 0) AS SiparisToplamKargoTutar, 
                         CASE WHEN o.OdemeIndirimi > 0 THEN round(o.OdemeIndirimi, 3) ELSE 0 END AS SiparisToplamIndirim, ROUND(w.HediyeCekiTutari, 0) AS SiparisToplaHCTutar
FROM            dbo.TicimaxSiparis AS w LEFT OUTER JOIN
                         dbo.TicimaxUye AS u ON u.ID = w.UyeID LEFT OUTER JOIN
                         dbo.TicimaxFaturaAdres AS f ON f.ID = w.FaturaAdresId LEFT OUTER JOIN
                         dbo.TicimaxTeslimatAdres AS t ON t.ID = w.TeslimatAdresiId LEFT OUTER JOIN
                         dbo.TicimaxKargoFirma AS k ON k.ID = w.KargoFirmaId LEFT OUTER JOIN
                         dbo.WebSiparisOdeme AS o ON o.SiparisID = w.ID AND CASE WHEN OdemeTipi = 0 AND o.Onaylandi = 1 THEN 1 ELSE Onaylandi END = 1 LEFT OUTER JOIN
                         dbo.TicimaxKargoTakip AS kt ON kt.SiparisID = w.ID LEFT OUTER JOIN
                         dbo.WebSiparisOdemeTipi AS ot ON ot.Ticimax_ID = o.OdemeTipi LEFT OUTER JOIN
                         dbo.VolantMagazaIletisim AS m ON m.DIVVAL = w.OzelAlan3
GO
/****** Object:  Table [dbo].[WebSiparisUrun]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebSiparisUrun](
	[Adet] [float] NULL,
	[Barkod] [nvarchar](255) NULL,
	[Durum] [int] NULL,
	[DurumAd] [nvarchar](255) NULL,
	[ID] [int] NULL,
	[IslemAd] [nvarchar](255) NULL,
	[IslemID] [int] NULL,
	[KampanyaID] [int] NULL,
	[KampanyaIndirimTutari] [float] NULL,
	[KdvOrani] [int] NULL,
	[KdvTutari] [float] NULL,
	[MagazaAtamaTarihi] [datetime] NULL,
	[MagazaDurum] [int] NULL,
	[MagazaGonderimTarihi] [datetime] NULL,
	[MagazaID] [int] NULL,
	[MagazaKodu] [nvarchar](255) NULL,
	[Maliyet] [float] NULL,
	[SiparisId] [int] NULL,
	[StokKodu] [nvarchar](255) NULL,
	[TedarikciID] [int] NULL,
	[TedarikciKodu] [nvarchar](255) NULL,
	[TedarikciKodu2] [nvarchar](255) NULL,
	[Tutar] [float] NULL,
	[UrunAdi] [nvarchar](255) NULL,
	[UrunID] [int] NULL,
	[UrunKartiID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[SiparisFisiStok]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[SiparisFisiStok]
AS
SELECT        CONVERT(varchar(10),SiparisId) as SiparisId, StokKodu, UrunAdi, Adet, KdvTutari, Tutar, Barkod
FROM            dbo.WebSiparisUrun
GO
/****** Object:  Table [dbo].[Anket]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Anket](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[UyeId] [int] NULL,
	[SiparisId] [int] NULL,
	[Çalisma Durumu] [nvarchar](50) NULL,
	[Meslek] [nvarchar](50) NULL,
	[Çalisma Süresi] [int] NULL,
	[Gelir Durumu] [nvarchar](50) NULL,
	[Icra Durumu] [int] NULL,
	[Ev Durumu] [int] NULL,
	[Oturma Süresi] [int] NULL,
	[Medeni Durumu] [int] NULL,
	[Kefil Verebilir] [bit] NULL,
	[Pesinat Verebilir] [bit] NULL,
	[E-Devlet Durumu] [bit] NULL,
	[Sözlesme Yeri] [bit] NULL,
	[Anket Notu] [nvarchar](250) NULL,
	[Anket Tarihi] [smalldatetime] NULL,
 CONSTRAINT [PK_Anket] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Anket_List1]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Anket_List1](
	[id] [int] IDENTITY(0,1) NOT NULL,
	[Durum] [nvarchar](50) NULL,
 CONSTRAINT [PK_Anket_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Anket_List2]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Anket_List2](
	[id] [int] IDENTITY(0,1) NOT NULL,
	[Durum] [nvarchar](50) NULL,
 CONSTRAINT [PK_Anket_2] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Anket_List3]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Anket_List3](
	[id] [int] IDENTITY(0,1) NOT NULL,
	[Durum] [nvarchar](50) NULL,
 CONSTRAINT [PK_Anket_3] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Anket_List4]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Anket_List4](
	[id] [int] IDENTITY(0,1) NOT NULL,
	[Durum] [nvarchar](50) NULL,
 CONSTRAINT [PK_Anket_4] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Anket_List5]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Anket_List5](
	[id] [int] IDENTITY(0,1) NOT NULL,
	[Durum] [nvarchar](50) NULL,
 CONSTRAINT [PK_Anket_5] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Anket3]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Anket3](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[UyeId] [int] NULL,
	[SiparisId] [int] NULL,
	[Çalisma Durumu] [nvarchar](50) NULL,
	[Meslek] [nvarchar](50) NULL,
	[Çalisma Süresi] [int] NULL,
	[Gelir Durumu] [nvarchar](50) NULL,
	[Icra Durumu] [int] NULL,
	[Ev Durumu] [int] NULL,
	[Oturma Süresi] [int] NULL,
	[Medeni Durumu] [int] NULL,
	[Kefil Verebilir] [bit] NULL,
	[Pesinat Verebilir] [bit] NULL,
	[Anket Notu] [nvarchar](250) NULL,
	[Anket Tarihi] [smalldatetime] NULL,
	[E-Devlet Durumu] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AsgariUcret]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AsgariUcret](
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Brut] [money] NULL,
	[Net] [money] NULL,
	[Gecerli] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Authorities]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Authorities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[Isim] [nvarchar](100) NULL,
	[Soyisim] [nvarchar](100) NULL,
	[Password] [nvarchar](100) NULL,
	[TicimaxUserid] [int] NULL,
	[TEl1] [varchar](12) NULL,
	[TEl2] [varchar](12) NULL,
	[Vol_SOCODE] [varchar](15) NULL,
	[Vol_SOENTERKEY] [varchar](15) NULL,
	[E-MailAdress] [varchar](150) NULL,
	[E-MailPassword] [varchar](150) NULL,
 CONSTRAINT [PK_Authorities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuthoritiesPotency]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuthoritiesPotency](
	[Userid] [int] NULL,
	[AnaSatis] [bit] NULL,
	[Satis0] [bit] NULL,
	[Satis1] [bit] NULL,
	[Satis2] [bit] NULL,
	[Satis3] [bit] NULL,
	[Satis4] [bit] NULL,
	[Satis5] [bit] NULL,
	[AnaUrun] [bit] NULL,
	[Urun0] [bit] NULL,
	[Urun1] [bit] NULL,
	[Urun2] [bit] NULL,
	[Urun3] [bit] NULL,
	[Urun4] [bit] NULL,
	[Urun5] [bit] NULL,
	[AnaDepoYonetimi] [bit] NULL,
	[Depo0] [bit] NULL,
	[Depo1] [bit] NULL,
	[Depo2] [bit] NULL,
	[Depo3] [bit] NULL,
	[Depo4] [bit] NULL,
	[Depo5] [bit] NULL,
	[AnaCariIslemler] [bit] NULL,
	[Cari0] [bit] NULL,
	[Cari1] [bit] NULL,
	[Cari2] [bit] NULL,
	[Cari3] [bit] NULL,
	[Cari4] [bit] NULL,
	[Cari5] [bit] NULL,
	[AnaEIslem] [bit] NULL,
	[Fatura] [bit] NULL,
	[Arsiv] [bit] NULL,
	[Irsaliye] [bit] NULL,
	[AnaKargo] [bit] NULL,
	[Kargo0] [bit] NULL,
	[Kargo1] [bit] NULL,
	[Kargo2] [bit] NULL,
	[Kargo3] [bit] NULL,
	[Kargo4] [bit] NULL,
	[Kargo5] [bit] NULL,
	[AnaRapor] [bit] NULL,
	[Rapor0] [bit] NULL,
	[Rapor1] [bit] NULL,
	[Rapor2] [bit] NULL,
	[Rapor3] [bit] NULL,
	[Rapor4] [bit] NULL,
	[Rapor5] [bit] NULL,
	[AnaAyarlar] [bit] NULL,
	[AnaTicimax] [bit] NULL,
	[Ticimax0] [bit] NULL,
	[Ticimax1] [bit] NULL,
	[Ticimax2] [bit] NULL,
	[Ticimax3] [bit] NULL,
	[Ticimax4] [bit] NULL,
	[Ticimax5] [bit] NULL,
	[Ticimax6] [bit] NULL,
	[Ticimax7] [bit] NULL,
	[Ticimax8] [bit] NULL,
	[Ticimax9] [bit] NULL,
	[Ticimax10] [bit] NULL,
	[Ticimax11] [bit] NULL,
	[AnaPhone] [bit] NULL,
	[Phone0] [bit] NULL,
	[Phone1] [bit] NULL,
	[Phone2] [bit] NULL,
	[Phone3] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ayarlar]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ayarlar](
	[VKN] [varchar](11) NOT NULL,
	[TicimaxAlanAdi] [varchar](255) NULL,
	[TicimaxYetkiKodu] [varchar](250) NULL,
	[TicimaxUser] [varchar](50) NULL,
	[TicimaxPass] [varchar](50) NULL,
	[EntegreFDataBaseName] [varchar](50) NULL,
	[EntegreFConnectionLocal] [varchar](250) NULL,
	[EntegreFConnectionOutside] [varchar](250) NULL,
	[VolantConnectionLocal] [varchar](250) NULL,
	[VolantConnectionOutside] [varchar](250) NULL,
	[VolantApiUrl] [varchar](250) NULL,
	[VolantFtpHostLocal] [varchar](250) NULL,
	[VolantFtpHostOutside] [varchar](250) NULL,
	[VolantFtpUser] [varchar](250) NULL,
	[VolantFtpPass] [varchar](250) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CompanyImages]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompanyImages](
	[ResimAdi] [nvarchar](200) NOT NULL,
	[ResimVerisi] [varbinary](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CreditSalesCompany]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CreditSalesCompany](
	[CSCID] [int] NOT NULL,
	[CSCNAME] [varchar](150) NOT NULL,
 CONSTRAINT [PK_CreditSalesCompany] PRIMARY KEY CLUSTERED 
(
	[CSCID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CreditSalesUnlock]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CreditSalesUnlock](
	[CSUID] [int] IDENTITY(1,1) NOT NULL,
	[CSUCID] [int] NOT NULL,
	[CSUSALID] [bigint] NOT NULL,
	[CSUUUID] [uniqueidentifier] NOT NULL,
	[CSUSALAMONT] [decimal](18, 2) NOT NULL,
	[CSUCREDITID] [bigint] NOT NULL,
	[CSUCREDITAMOUNT] [decimal](18, 2) NOT NULL,
	[CSUDATE] [date] NOT NULL,
	[CSUSOCODE] [varchar](15) NOT NULL,
 CONSTRAINT [PK_CreditSalesUnlock] PRIMARY KEY CLUSTERED 
(
	[CSUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DEFINVESTIGATION]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DEFINVESTIGATION](
	[DINSID] [int] NOT NULL,
	[DINSNAME] [varchar](120) NULL,
	[DINSTYPE] [int] NULL,
	[DINSACTIVE] [bit] NULL,
 CONSTRAINT [PK_DEFINVESTIGATION] PRIMARY KEY CLUSTERED 
(
	[DINSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EDevlet_MusteriDurum]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EDevlet_MusteriDurum](
	[Musteri_VolID] [bigint] NOT NULL,
	[Musteri_TicID] [bigint] NOT NULL,
	[Musteri_SipID] [bigint] NOT NULL,
	[Kullanici_ID] [bigint] NOT NULL,
	[islemTarihi] [smalldatetime] NOT NULL,
	[Adress] [bit] NULL,
	[DogumTarihi] [smalldatetime] NULL,
	[icraAcik] [bit] NULL,
	[icraAcikOnemli] [int] NULL,
	[icraAcikOnemsiz] [int] NULL,
	[icraKapali] [bit] NULL,
	[icraKapaliOnemli] [int] NULL,
	[icraKapaliOnemsiz] [int] NULL,
	[icraGizli] [bit] NULL,
	[icraGizliOnemli] [int] NULL,
	[icraGizliOnemsiz] [int] NULL,
	[SSK] [bit] NULL,
	[SGKTopSure] [int] NULL,
	[SGKsongiris] [smalldatetime] NULL,
	[SGKsonPrim] [numeric](18, 2) NULL,
	[SGKSonAy] [int] NULL,
	[SGKSonYil] [int] NULL,
	[SGKYilGun] [int] NULL,
	[SGKYilPrim] [numeric](18, 2) NULL,
	[SGKMeslekKodu] [nvarchar](7) NULL,
	[Tapu] [bit] NULL,
	[Mesken] [int] NULL,
	[Tasinmaz] [int] NULL,
	[Mulk] [int] NULL,
	[Arac] [bit] NULL,
	[AracSayi] [int] NULL,
	[Dava] [bit] NULL,
	[DavaOnemli] [int] NULL,
	[DavaOnemsiz] [int] NULL,
	[Emekli] [bit] NULL,
	[EmekliAylik] [numeric](18, 2) NULL,
	[GuncelleyenKullanici] [smalldatetime] NULL,
	[GuncellemeTarihi] [smalldatetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EDevlet_RiskGrubu_1]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EDevlet_RiskGrubu_1](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Tanim] [nvarchar](250) NOT NULL,
	[Zorunlu] [bit] NOT NULL,
 CONSTRAINT [PK_EDevlet_RiskGrubu_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EDevlet_RiskGrubu_2]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EDevlet_RiskGrubu_2](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Parent_id] [int] NOT NULL,
	[Tanim] [nvarchar](250) NOT NULL,
	[Risk] [bit] NOT NULL,
 CONSTRAINT [PK_EDevlet_RiskGrubu_2] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EDevlet_RiskGrubu_3]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EDevlet_RiskGrubu_3](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Parent_id] [int] NOT NULL,
	[Tanim] [nvarchar](250) NOT NULL,
	[Risk] [bit] NOT NULL,
 CONSTRAINT [PK_EDevlet_RiskGrubu_3] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EDevlet_RiskGrubu_Puan]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EDevlet_RiskGrubu_Puan](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Risk_id] [int] NOT NULL,
	[Puan] [numeric](18, 2) NOT NULL,
	[Ceza] [bit] NOT NULL,
 CONSTRAINT [PK_EDevlet_RiskGrubu_Puan] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EDevlet_RiskGrubu_Puan_1]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EDevlet_RiskGrubu_Puan_1](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Risk_id] [int] NOT NULL,
	[Puan] [numeric](18, 2) NOT NULL,
	[Ceza] [bit] NOT NULL,
 CONSTRAINT [PK_EDevlet_RiskGrubu_Puan_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EDevlet_RiskGrubu_Puan_2]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EDevlet_RiskGrubu_Puan_2](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Risk_id] [int] NOT NULL,
	[Puan] [numeric](18, 2) NOT NULL,
	[Ceza] [bit] NOT NULL,
 CONSTRAINT [PK_EDevlet_RiskGrubu_Puan_2] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EDevlet_RiskGrubu_Puan_3]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EDevlet_RiskGrubu_Puan_3](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Risk_id] [int] NOT NULL,
	[Puan] [numeric](18, 2) NOT NULL,
	[Ceza] [bit] NOT NULL,
 CONSTRAINT [PK_EDevlet_RiskGrubu_Puan_3] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EDevlet_RiskGrubu_Yetkilendirme]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EDevlet_RiskGrubu_Yetkilendirme](
	[YetkiDepval] [varchar](10) NOT NULL,
	[RiskUrunGurup1] [bit] NOT NULL,
	[RiskUrunGurup2] [bit] NOT NULL,
	[RiskUrunGurup3] [bit] NOT NULL,
	[RiskUrunGurup4] [bit] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EDEVLETMAIN]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EDEVLETMAIN](
	[EDVID] [bigint] IDENTITY(1,1) NOT NULL,
	[EDVCURID] [bigint] NULL,
	[EDVCURIDENTY] [bigint] NULL,
	[EDVCURGSM] [bigint] NULL,
	[EDVDATE] [date] NULL,
	[EDVDATETIME] [smalldatetime] NULL,
	[EDVSOCODE] [nchar](10) NULL,
	[EDVREPORTSENT] [bit] NULL,
	[EDVREPORTLASTTIME] [time](7) NULL,
	[EDVREPORTSHORTID] [varchar](50) NULL,
	[EDVREPORTID] [varchar](50) NULL,
	[EDVREPORTSTATUS] [varchar](50) NULL,
	[EDVREPORTISDONE] [bit] NULL,
	[EDVRAPORTISDOWNLOAD] [bit] NULL,
 CONSTRAINT [PK_EDEVLETMAIN] PRIMARY KEY CLUSTERED 
(
	[EDVID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EgarantiOrder]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EgarantiOrder](
	[EGORDID] [int] NOT NULL,
	[EGORDCURID] [int] NOT NULL,
	[EGORDSALID] [int] NOT NULL,
	[EGORDLINEID] [int] NOT NULL,
	[EGORDAMOUNT] [numeric](18, 2) NOT NULL,
	[EGORDQUAN] [int] NOT NULL,
	[EGORDSALESMEN] [varchar](10) NOT NULL,
	[EGORDEGNEWLINEID] [int] NULL,
	[EGORDNEWSALID] [int] NULL,
	[EGORDNEWLINEID] [int] NULL,
	[EGORDCANCEL] [bit] NULL,
	[EGORDCANCELSALID] [int] NULL,
	[EGORDCANCELLINEID] [int] NULL,
	[EGORDCANCELNEWSALID] [int] NULL,
	[EGORDCANCELNEWLINEID] [int] NULL,
 CONSTRAINT [PK_EgarantiOrder] PRIMARY KEY CLUSTERED 
(
	[EGORDID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EgarantiOrderCancel]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EgarantiOrderCancel](
	[EGORDCID] [int] NOT NULL,
	[EGORDCEGORDID] [int] NOT NULL,
	[EGORDCEGID] [int] NOT NULL,
	[EGORDCSOCODE] [varchar](15) NULL,
	[EGORDCDATE] [date] NULL,
 CONSTRAINT [PK_EgarantiOrderCancel] PRIMARY KEY CLUSTERED 
(
	[EGORDCID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EgarantiOrderDelete]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EgarantiOrderDelete](
	[EGORDID] [int] NOT NULL,
	[EGORDCURID] [int] NOT NULL,
	[EGORDSALID] [int] NOT NULL,
	[EGORDLINEID] [int] NOT NULL,
	[EGORDAMOUNT] [numeric](18, 2) NOT NULL,
	[EGORDQUAN] [int] NOT NULL,
	[EGORDSALESMEN] [varchar](10) NOT NULL,
	[EGORDEGNEWLINEID] [int] NULL,
	[EGORDNEWSALID] [int] NULL,
	[EGORDNEWLINEID] [int] NULL,
	[EGORDCANCEL] [bit] NULL,
	[EGORDCANCELSALID] [int] NULL,
	[EGORDCANCELLINEID] [int] NULL,
	[EGORDCANCELNEWSALID] [int] NULL,
	[EGORDCANCELNEWLINEID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EgarantiOrderEKLENELER]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EgarantiOrderEKLENELER](
	[EGORDID] [bigint] NULL,
	[EGORDCURID] [bigint] NOT NULL,
	[EGORDSALID] [bigint] NULL,
	[EGORDLINEID] [bigint] NULL,
	[EGORDAMOUNT] [decimal](17, 2) NULL,
	[EGORDQUAN] [decimal](18, 6) NULL,
	[EGORDSALESMEN] [varchar](10) NULL,
	[EGORDEGNEWLINEID] [int] NOT NULL,
	[EGORDNEWSALID] [bigint] NULL,
	[EGORDNEWLINEID] [bigint] NULL,
	[EGORDCANCEL] [int] NULL,
	[EGORDCANCELSALID] [int] NOT NULL,
	[EGORDCANCELLINEID] [int] NOT NULL,
	[EGORDCANCELNEWSALID] [int] NOT NULL,
	[EGORDCANCELNEWLINEID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EgarantiRate]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EgarantiRate](
	[EGRATEID] [int] NULL,
	[EGRATENAME] [varchar](60) NULL,
	[EGRATEPROPROUID] [int] NULL,
	[EGRATEORAN] [numeric](5, 2) NULL,
	[EGRATETYPID] [int] NULL,
	[EGRATEACTIVE] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EgarantiType]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EgarantiType](
	[EGTYPEID] [int] NOT NULL,
	[EGTYPENAME] [varchar](20) NULL,
	[EGTYPEACTIVE] [bit] NULL,
	[EGTYPEADDYEAR] [int] NULL,
	[EGTYPEADDNAME] [varchar](60) NULL,
 CONSTRAINT [PK_EgarantiType] PRIMARY KEY CLUSTERED 
(
	[EGTYPEID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ENTEGREF_AISCORING]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ENTEGREF_AISCORING](
	[AIROW_ID] [int] IDENTITY(1,1) NOT NULL,
	[AIDEF_ID] [int] NULL,
	[AIDEF_SOCODE] [varchar](50) NULL,
	[AIDEF_VAL] [varchar](200) NULL,
	[AIDEF_NAME] [varchar](200) NULL,
	[AIDEF_VALUE] [bigint] NULL,
	[AIDEF_RESULT] [bigint] NULL,
	[AIDEF_BOOL] [bit] NULL,
	[AIDEF_GROUP] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntegreF_Marketplace]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntegreF_Marketplace](
	[MRKID] [int] NOT NULL,
	[MRKEPRID] [int] NOT NULL,
	[MRKNAME] [nvarchar](50) NULL,
	[MRKCLASS] [varchar](10) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntegreF_Marketplace_Settings]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntegreF_Marketplace_Settings](
	[MRKID] [int] NOT NULL,
	[MRKSTSELLERNAME] [nvarchar](50) NULL,
	[MRKSTSELLERID] [nvarchar](50) NULL,
	[MRKSTSELLERAPIKEY] [nvarchar](50) NULL,
	[MRKSTSELLERSECRETKEY] [nvarchar](50) NULL,
	[MRKSTTOKEN] [nvarchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntegreFAvOdemeDusum]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntegreFAvOdemeDusum](
	[EAVCURID] [bigint] NULL,
	[EAVCURVAL] [varchar](20) NULL,
	[EAVPAYTYPE] [varchar](50) NULL,
	[EAVAMOUNT] [numeric](17, 2) NULL,
	[EAVDATE] [date] NULL,
	[EAVPCDSID] [bigint] NULL,
	[EAVINPCDSSID] [bigint] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntegreFHrcOdemeDusum]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntegreFHrcOdemeDusum](
	[EHRCCURID] [bigint] NULL,
	[EHRCCURVAL] [varchar](20) NULL,
	[EHRCTYPE] [bit] NULL,
	[EHRCLATETYPE] [bit] NULL,
	[EHRCPAYTYPE] [varchar](50) NULL,
	[EHRCAMOUNT] [decimal](18, 0) NULL,
	[EHRCDATE] [date] NULL,
	[EHRCPCDSID] [bigint] NULL,
	[EHRCINPCDSSID] [bigint] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ENTEGREFPAZARYERI]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ENTEGREFPAZARYERI](
	[ENTEGREFPAZARYERIID] [int] IDENTITY(1,1) NOT NULL,
	[ENTEGREFPAZARYERINAME] [varbinary](50) NOT NULL,
 CONSTRAINT [PK_ENTEGREFPAZARYERI] PRIMARY KEY CLUSTERED 
(
	[ENTEGREFPAZARYERIID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntegrefPazaryerleri]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntegrefPazaryerleri](
	[id] [int] NOT NULL,
	[VolantSinifid] [int] NOT NULL,
	[PazaryeriAdi] [nvarchar](50) NULL,
 CONSTRAINT [PK_EntegrefPazaryerleri] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntegrefSmsOperator]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntegrefSmsOperator](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[SmsOperatorAdi] [varchar](50) NULL,
	[SmsOperatorApiUrl] [varchar](250) NULL,
 CONSTRAINT [PK_EntegrefSmsOperator] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntegrefSmsOperatorAyarlar]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntegrefSmsOperatorAyarlar](
	[OperatorID] [int] NOT NULL,
	[IsActive] [bit] NULL,
	[IsDefault] [bit] NULL,
	[OperatorUserName] [varchar](50) NULL,
	[OperatorUserPass] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntegreFStokResimleri]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntegreFStokResimleri](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[STOKID] [bigint] NOT NULL,
	[RESIMADI] [varchar](250) NULL,
 CONSTRAINT [PK_EntegreFStokResimleri] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntegrefStokSiniflari]    Script Date: 03-May-26 3:41:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntegrefStokSiniflari](
	[id] [int] NOT NULL,
	[Adi] [nvarchar](50) NULL,
	[Vol_DWPROUNIQ] [varchar](10) NULL,
	[Vol_DWPRONAME] [varchar](50) NULL,
 CONSTRAINT [PK_EntegrefStokSiniflari] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntegreFStokVaryant]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntegreFStokVaryant](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[MAINSTOKID] [bigint] NOT NULL,
	[VARYANTSTOKID] [bigint] NULL,
	[VARYANTID] [int] NOT NULL,
	[VARYANTVAL] [varchar](50) NOT NULL,
	[AKTIF] [bit] NOT NULL,
	[TICIMAXSTOKID] [int] NULL,
	[TICIMAXVAYANTID] [int] NULL,
 CONSTRAINT [PK_EntegreFStokVaryant] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntegreFTrendyolAyarlari]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntegreFTrendyolAyarlari](
	[Pazaryeriid] [int] NOT NULL,
	[MarketAdi] [nvarchar](50) NULL,
	[SaticiID] [nvarchar](50) NULL,
	[ApiKey] [nvarchar](50) NULL,
	[ApiSecretKey] [nvarchar](50) NULL,
	[Token] [nvarchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FaturaAdres]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FaturaAdres](
	[Adres] [nvarchar](255) NULL,
	[AliciTelefon] [nvarchar](255) NULL,
	[EntegrasyonId] [nvarchar](255) NULL,
	[FirmaAdi] [nvarchar](255) NULL,
	[ID] [int] NULL,
	[Il] [nvarchar](255) NULL,
	[IlId] [int] NULL,
	[IlKodu] [nvarchar](255) NULL,
	[Ilce] [nvarchar](255) NULL,
	[IlceId] [int] NULL,
	[IlceKodu] [nvarchar](255) NULL,
	[Ulke] [nvarchar](255) NULL,
	[VergiDairesi] [nvarchar](255) NULL,
	[VergiNo] [nvarchar](255) NULL,
	[isKurumsal] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GorusmeNotu]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GorusmeNotu](
	[UyeId] [int] NOT NULL,
	[SiparisId] [int] NULL,
	[Görüsme Notu] [text] NULL,
	[Not Tarihi] [smalldatetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HBCategory]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HBCategory](
	[categoryId] [int] NULL,
	[name] [nvarchar](max) NULL,
	[parentCategoryId] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HBCategory2]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HBCategory2](
	[categoryId] [int] NULL,
	[name] [nvarchar](max) NULL,
	[displayName] [nvarchar](max) NULL,
	[parentCategoryId] [int] NULL,
	[paths] [nvarchar](max) NULL,
	[status] [nvarchar](max) NULL,
	[type] [nvarchar](max) NULL,
	[sortId] [nvarchar](max) NULL,
	[productTypename] [nvarchar](max) NULL,
	[productTypeId] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HBCategoryMain]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HBCategoryMain](
	[CatogoryName1] [nvarchar](max) NULL,
	[CatogoryName2] [nvarchar](max) NULL,
	[CatogoryName3] [nvarchar](max) NULL,
	[CatogoryName4] [nvarchar](max) NULL,
	[CatogoryName5] [nvarchar](max) NULL,
	[CatogoryName6] [nvarchar](max) NULL,
	[CatogoryName7] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IadeBakiyeDuzeltilenler]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IadeBakiyeDuzeltilenler](
	[INSCINSID] [bigint] NOT NULL,
	[Musteri_Kodu] [varchar](20) NULL,
	[Muster_Adi] [varchar](1000) NULL,
	[Iade_Tarihi] [smalldatetime] NULL,
	[Iade_ID] [bigint] NULL,
	[Hatali_Satis_Tarihi] [smalldatetime] NULL,
	[Hatali_Satis_ID] [bigint] NULL,
	[Dogru_Satis_ID] [bigint] NULL,
	[Taksit_Tutari] [decimal](17, 2) NULL,
	[Iade_Tutari] [decimal](38, 2) NULL,
	[Bakiye] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IadeBakiyeDuzeltilenlerBakiye]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IadeBakiyeDuzeltilenlerBakiye](
	[Musteri_Kodu] [varchar](50) NULL,
	[SATISTOPLAM] [decimal](38, 2) NULL,
	[PESINATTAHSILAT] [decimal](38, 2) NULL,
	[TAKSITTAHSILAT] [decimal](38, 2) NULL,
	[TAKSITBAKIYE] [decimal](38, 2) NULL,
	[TAKSITSAYISI] [int] NULL,
	[MINVADETARIHI] [smalldatetime] NULL,
	[SONODEMEMTARIHI] [smalldatetime] NULL,
	[SONALISVERISTARIHI] [smalldatetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[INVESTIGATIONVALUE]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[INVESTIGATIONVALUE](
	[INVESSALID] [bigint] NULL,
	[INVESCURID] [bigint] NULL,
	[INVESDINSID] [char](10) NULL,
	[INVESSALSTS] [bit] NULL,
	[INVESSALNOTES] [varchar](500) NULL,
	[INVESRISK] [bigint] NULL,
	[INVESWARANTER] [bigint] NULL,
	[INVESCURLATE] [numeric](18, 2) NULL,
	[INVESSOCODE] [varchar](15) NULL,
	[INVESDATE] [smalldatetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[INVESTIGATIONVALUE2]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[INVESTIGATIONVALUE2](
	[INVESSALID] [bigint] NULL,
	[INVESCURID] [bigint] NULL,
	[INVESDINSID] [char](10) NULL,
	[INVESSALSTS] [bit] NULL,
	[INVESSALNOTES] [varchar](500) NULL,
	[INVESRISK] [bigint] NULL,
	[INVESWARANTER] [bigint] NULL,
	[INVESCURLATE] [numeric](18, 2) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IYS]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IYS](
	[IYSCURID] [bigint] NOT NULL,
	[IYSNUMBER] [varchar](10) NOT NULL,
	[IYSSENTDATE] [smalldatetime] NOT NULL,
	[IYSACTIVE] [bit] NOT NULL,
	[IYSPACKAGEID] [varchar](100) NULL,
	[IYSSTATUS] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IYSDAYCOUNT]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IYSDAYCOUNT](
	[IYSDATE] [smalldatetime] NULL,
	[IYSDAYCOUNT] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Kargo]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Kargo](
	[Tici_ID] [int] NOT NULL,
	[Tici_Tanim] [varchar](60) NOT NULL,
	[Tici_KargoFiyati] [money] NULL,
	[Tici_KapidaOdemeNakit] [bit] NULL,
	[Tici_KapidaOdemeNakitFiyat] [money] NULL,
	[Tici_KapidaOdemeKK] [bit] NULL,
	[Tici_KapidaOdemeKKFiyat] [money] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KargoDurum]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KargoDurum](
	[Tici_ID] [int] NOT NULL,
	[Tici_Tanim] [varchar](20) NOT NULL,
	[Tici_Islem] [int] NULL,
	[Aktif] [bit] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KargoEslesme]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KargoEslesme](
	[Vol_DSHIPVAL] [varchar](20) NULL,
	[Tic_ID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KrediPuan_Baslik]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KrediPuan_Baslik](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[BASLIK] [nvarchar](50) NOT NULL,
	[ZORUNLU] [bit] NOT NULL,
 CONSTRAINT [PK_KrediPuan_Baslik] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KrediPuan_BaslikPuan]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KrediPuan_BaslikPuan](
	[BASLIKID] [int] NOT NULL,
	[PUAN] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KrediPuan_Islem]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KrediPuan_Islem](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[KIRILIMID] [int] NOT NULL,
	[ISLEMADI] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_KrediPuan_Islem] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KrediPuan_IslemPuan]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KrediPuan_IslemPuan](
	[ISLEMID] [int] NOT NULL,
	[PUAN] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KrediPuan_Kirilim]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KrediPuan_Kirilim](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[BASLIKID] [int] NOT NULL,
	[KIRILIMADI] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_KrediPuan_Kirilim] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KrediPuan_KirilmPuan]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KrediPuan_KirilmPuan](
	[KIRILIMID] [int] NOT NULL,
	[PUAN] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KrediPuan_RiskSatisGurupPuan]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KrediPuan_RiskSatisGurupPuan](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Risk_Adi] [varchar](30) NULL,
	[Risk_TutarMin] [numeric](18, 2) NOT NULL,
	[Risk_TutarMax] [numeric](18, 2) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KrediPuan_RiskUrunGurup]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KrediPuan_RiskUrunGurup](
	[Risk_id] [int] NOT NULL,
	[VOLUID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KrediPuan_RiskUrunGurupPuan]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KrediPuan_RiskUrunGurupPuan](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Risk_Adi] [varchar](30) NULL,
	[Risk_TutarMin] [numeric](18, 2) NOT NULL,
	[Risk_TutarMax] [numeric](18, 2) NULL,
 CONSTRAINT [PK_KrediPuan_RiskUrunGurup] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MailCC]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MailCC](
	[AdiSoyadi] [nvarchar](50) NOT NULL,
	[Mail] [nvarchar](250) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MesajListesi]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MesajListesi](
	[Mesaj Tipi] [int] NULL,
	[Siparis Durumu] [int] NULL,
	[Mesaj Basligi] [varchar](50) NULL,
	[Mesaj Metini] [text] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MeslekKodu]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeslekKodu](
	[KODU] [nvarchar](7) NULL,
	[ADI] [nvarchar](800) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MNG_API]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MNG_API](
	[API_Active] [bit] NOT NULL,
	[API_Type] [int] NOT NULL,
	[API_Name] [varchar](50) NOT NULL,
	[API_Key] [varchar](50) NOT NULL,
	[API_Secret] [varchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MNG_APITYPE]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MNG_APITYPE](
	[Typeid] [int] NULL,
	[TypeName] [varchar](20) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MNG_URL]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MNG_URL](
	[API_ProductsId] [int] IDENTITY(0,1) NOT NULL,
	[API_ProductsName] [varchar](50) NOT NULL,
	[API_Name] [varchar](50) NOT NULL,
	[API_Base] [varchar](10) NOT NULL,
	[API_Url] [varchar](100) NOT NULL,
 CONSTRAINT [PK_MNG_URL] PRIMARY KEY CLUSTERED 
(
	[API_ProductsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MNG_USER]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MNG_USER](
	[MNG_DIVVAL] [varchar](2) NOT NULL,
	[MNG_Userid] [int] NULL,
	[MNG_Userpass1] [varchar](20) NULL,
	[MNG_Userpass2] [varchar](20) NULL,
	[MNG_Token] [varchar](500) NULL,
	[MNG_Token_ExpireDate] [smalldatetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MusteriEDevlet]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MusteriEDevlet](
	[SipID] [int] NULL,
	[UyeID] [int] NULL,
	[whatsapp_Sent] [bit] NULL,
	[whatsapp_Date] [smalldatetime] NULL,
	[SMS_Sent] [bit] NULL,
	[SMS_Date] [smalldatetime] NULL,
	[SMS_Sonuc] [char](150) NULL,
	[SMS_Durum] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NKOLAYCLIENT]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NKOLAYCLIENT](
	[CLIENTPCDSID] [bigint] NOT NULL,
	[CLIENTSESSION_ID] [varchar](150) NULL,
	[CLIENTTRXID] [varchar](150) NULL,
 CONSTRAINT [PK_NKOLAYCLIENT] PRIMARY KEY CLUSTERED 
(
	[CLIENTPCDSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NKOLAYEXP]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NKOLAYEXP](
	[id] [int] NULL,
	[Kod] [varchar](50) NULL,
	[Açiklama] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NKOLAYPICTURE]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NKOLAYPICTURE](
	[PICCURID] [bigint] NULL,
	[PICIDENTITY] [varchar](max) NULL,
	[PICFTPURL] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NKOLAYSEND]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NKOLAYSEND](
	[SENDPCDSID] [bigint] NOT NULL,
	[SENDAMOUNT] [decimal](17, 2) NULL,
	[SENDCURID] [bigint] NULL,
	[SENDSTATUS] [char](1) NULL,
	[SENDSOCODE] [varchar](15) NULL,
	[SENDDATE] [smalldatetime] NULL,
	[SENDTRANSID] [varchar](50) NULL,
	[SENDINSTITID] [varchar](50) NULL,
	[SENDOK] [bit] NULL,
	[SENDDOCUMENT] [bit] NULL,
 CONSTRAINT [PK_NKOLAYSEND] PRIMARY KEY CLUSTERED 
(
	[SENDPCDSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NKOLAYUSER]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NKOLAYUSER](
	[USRSOCODE] [varchar](15) NOT NULL,
	[USRGSM] [varchar](10) NOT NULL,
	[USRDIVVAL] [varchar](2) NULL,
	[USRNAME] [varchar](15) NULL,
	[USRPASWORD] [varchar](150) NULL,
	[USRTYPE] [bit] NULL,
	[USRACTIVE] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OtoFiyatGuncelle]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OtoFiyatGuncelle](
	[StokID] [int] NOT NULL,
	[IslemTarihi] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OtoFiyatRapor]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OtoFiyatRapor](
	[PROID] [bigint] NOT NULL,
	[PROVAL] [varchar](20) NOT NULL,
	[PRONAME] [varchar](200) NOT NULL,
	[Envanter] [int] NOT NULL,
	[LastPrice] [numeric](18, 2) NOT NULL,
	[NewPrice] [numeric](18, 2) NOT NULL,
	[UpdateDate] [date] NOT NULL,
	[Sonuc] [varchar](20) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ÖdemeDurumuDegiskenleri]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ÖdemeDurumuDegiskenleri](
	[Adi] [nvarchar](255) NULL,
	[Deger] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ÖdemeTipiDegiskenleri]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ÖdemeTipiDegiskenleri](
	[Adi] [nvarchar](255) NULL,
	[Deger] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Personel]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Personel](
	[PRSID] [bigint] IDENTITY(1,1) NOT NULL,
	[PRSKODE] [varchar](8) NULL,
	[PRSNAME] [varchar](50) NOT NULL,
	[PRSSURNAME] [varchar](50) NOT NULL,
	[PRSIDENTY] [varchar](11) NOT NULL,
	[PRSDEVVAL] [varchar](2) NOT NULL,
	[PRSBIRDDATE] [date] NULL,
	[PRSSMENKOD] [varchar](10) NULL,
	[PRSSOCODE] [varchar](15) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PROINVUPDATE]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PROINVUPDATE](
	[UPDPROID] [bigint] NULL,
	[UPDDSTOREID] [int] NULL,
	[UPDPINVLSTQUAN] [decimal](13, 9) NULL,
	[UPDPINVNEWQUAN] [decimal](13, 9) NULL,
	[UPDDATE] [smalldatetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SAFEMOVIE]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SAFEMOVIE](
	[SFCOCEDSID] [int] NULL,
	[SOCODE] [varchar](50) NULL,
	[DIVVAL] [varchar](50) NULL,
	[SABHDATE] [smalldatetime] NULL,
	[SABHAMOUNT] [decimal](17, 2) NULL,
	[VI1] [int] NULL,
	[VI1CH] [int] NULL,
	[VI0] [int] NULL,
	[VI0CH] [int] NULL,
	[DSAFEID] [int] NULL,
	[VK1] [int] NULL,
	[VK1CH] [int] NULL,
	[VK0] [int] NULL,
	[VK0CH] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SatisRaporu]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SatisRaporu](
	[Iptal Aciklama] [varchar](60) NULL,
	[Fatura Satir ID] [bigint] NULL,
	[Kasiyer Kodu] [varchar](10) NULL,
	[SatisTarihi] [smalldatetime] NULL,
	[Kasiyer Adi] [varchar](101) NULL,
	[Iptal Satis ID] [bigint] NULL,
	[UrunID] [bigint] NULL,
	[Urun Kodu] [varchar](200) NULL,
	[Urun Adi] [varchar](200) NULL,
	[Urun Renk Adi] [varchar](600) NULL,
	[Urun Birim ID] [bigint] NULL,
	[Magaza Adi] [varchar](40) NULL,
	[Magaza Bölgesi] [varchar](1000) NULL,
	[Urun Birim] [varchar](3) NULL,
	[Satis ID] [bigint] NULL,
	[Birim Degeri] [varchar](15) NULL,
	[Birim Adi] [varchar](60) NULL,
	[Satis Türü] [varchar](1) NULL,
	[Satis Magaza] [varchar](2) NULL,
	[Musteri Kayit Tarihi] [datetime] NULL,
	[Satis Temsilcisi Kodu] [varchar](10) NULL,
	[Satis Temsilcisi Adi] [varchar](100) NULL,
	[Urun Genislik] [varchar](20) NULL,
	[Urun Derinlik] [varchar](20) NULL,
	[Urun Yukseklik] [varchar](20) NULL,
	[Urun Agirlik] [varchar](20) NULL,
	[Musteri MagazaKodu] [varchar](2) NULL,
	[Musteri MagazaAdi] [varchar](40) NULL,
	[Musteri ID] [bigint] NULL,
	[Musteri Kodu] [varchar](20) NULL,
	[Musteri Adi] [varchar](1000) NULL,
	[Musteri DogumTarihi] [smalldatetime] NULL,
	[Musteri TCKN] [bigint] NULL,
	[Musteri VergiNo] [varchar](20) NULL,
	[Satin AlinanUrun] [bit] NULL,
	[Musteri Ilce] [varchar](60) NULL,
	[Musteri Sehir] [varchar](60) NULL,
	[Musteri Adres1] [varchar](300) NULL,
	[Musteri Adres2] [varchar](300) NULL,
	[Musteri GSM] [varchar](20) NULL,
	[Tedarikci Firma Kodu] [varchar](20) NULL,
	[Tedarikci Firma Adi] [varchar](1000) NULL,
	[Musteri Medeni Durum] [varchar](1) NULL,
	[Musteri Cinsiyet] [varchar](1) NULL,
	[AltGrupKodu] [varchar](100) NULL,
	[AltGrupAdi] [varchar](150) NULL,
	[AltGrupDetayAdi] [varchar](150) NULL,
	[Marka Adi] [varchar](150) NULL,
	[Reyon Adi] [varchar](150) NULL,
	[Yil Adi] [varchar](150) NULL,
	[Tanimli Kampanya Sinif Adi] [varchar](150) NULL,
	[Sozlesme Yer Adi] [varchar](100) NULL,
	[Yil] [int] NULL,
	[Ay] [int] NULL,
	[Gun] [int] NULL,
	[Saat] [int] NULL,
	[Dakika] [int] NULL,
	[Fiyat Tipi] [varchar](20) NULL,
	[Taksit Sayisi] [smallint] NULL,
	[Dip Iskonto Orani] [decimal](17, 2) NULL,
	[Dip Iskonto Tutari] [decimal](17, 2) NULL,
	[Satir Iskonto Orani] [decimal](17, 2) NULL,
	[Satir Iskonto Tutari] [decimal](17, 2) NULL,
	[Kampanya Tutari] [decimal](17, 0) NULL,
	[Satis Miktari] [decimal](17, 2) NULL,
	[Iade Miktari] [decimal](17, 2) NULL,
	[Orijinal Fiyat] [decimal](17, 2) NULL,
	[Para Birimi] [varchar](3) NULL,
	[Doviz Kuru Orani] [decimal](17, 2) NULL,
	[Satis Tutari] [decimal](17, 2) NULL,
	[Iade Tutari] [decimal](17, 2) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SatisRaporu2]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SatisRaporu2](
	[DCANNAME] [varchar](60) NULL,
	[INVCHID] [bigint] NULL,
	[SALCHVAL] [varchar](10) NULL,
	[SALDATE] [smalldatetime] NOT NULL,
	[SONAME] [varchar](101) NULL,
	[SALCANSALID] [bigint] NULL,
	[PROID] [bigint] NULL,
	[PROVAL] [varchar](200) NULL,
	[PRONAME] [varchar](200) NULL,
	[PROCHCOLORNAME] [varchar](600) NULL,
	[PROPROUID] [bigint] NULL,
	[DIVNAME] [varchar](40) NULL,
	[DIVREGION] [varchar](1000) NULL,
	[PROBHUNIT] [varchar](3) NULL,
	[SALID] [bigint] NOT NULL,
	[PROUVAL] [varchar](15) NULL,
	[PROUNAME] [varchar](60) NULL,
	[SALSALEKIND] [varchar](1) NOT NULL,
	[SALDIVISON] [varchar](2) NOT NULL,
	[CUSDATETIME] [datetime] NULL,
	[SMENVAL] [varchar](10) NULL,
	[SMENNAME] [varchar](100) NULL,
	[PROWIDTH] [varchar](20) NULL,
	[PRODEPTH] [varchar](20) NULL,
	[PROHEIGHT] [varchar](20) NULL,
	[PROWEIGHT] [varchar](20) NULL,
	[CARIDIVVAL] [varchar](2) NULL,
	[CARIDIVNAME] [varchar](40) NULL,
	[CUSCURID] [bigint] NULL,
	[CUSCURVAL] [varchar](20) NULL,
	[CUSCURNAME] [varchar](1000) NULL,
	[CUSIDBIRTHDAY] [smalldatetime] NULL,
	[CUSIDTCNO] [bigint] NULL,
	[CURCHWATNO] [varchar](20) NULL,
	[INVCHSHAMEPRODUCT] [bit] NULL,
	[CURCHCOUNTY] [varchar](60) NOT NULL,
	[CURCHCITY] [varchar](60) NOT NULL,
	[CURCHADR1] [varchar](100) NOT NULL,
	[CURCHADR2] [varchar](300) NOT NULL,
	[CURCHGSM1] [varchar](20) NOT NULL,
	[SUPCURVAL] [varchar](20) NULL,
	[SUPCURNAME] [varchar](1000) NULL,
	[CUSIDMARRIED] [varchar](1) NULL,
	[CUSIDSEX] [varchar](1) NULL,
	[ALTGRUPKODU] [varchar](100) NULL,
	[ALTGRUPADI] [varchar](150) NULL,
	[ALTGRUPDTYADI] [varchar](150) NULL,
	[MARKAADI] [varchar](150) NULL,
	[REYONADI] [varchar](150) NULL,
	[YILADI] [varchar](150) NULL,
	[CINSIYETADI] [varchar](150) NULL,
	[DCONTNAME] [varchar](100) NULL,
	[YIL] [int] NULL,
	[AY] [int] NULL,
	[GUN] [int] NULL,
	[SAAT] [int] NULL,
	[DAKIKA] [int] NULL,
	[DPRVAL] [varchar](20) NULL,
	[DPAYPINSCOUNT] [smallint] NULL,
	[DIPORAN] [decimal](5, 2) NULL,
	[DIPTUTAR] [decimal](17, 2) NULL,
	[SATIRORAN] [decimal](5, 2) NULL,
	[SATIRTUTAR] [decimal](17, 2) NULL,
	[KAMPANYATUTAR] [decimal](38, 2) NULL,
	[SATISMIKTAR] [decimal](38, 6) NULL,
	[IADEMIKTAR] [decimal](38, 6) NULL,
	[ORJPRICE] [decimal](38, 9) NULL,
	[INVCHEXCH] [varchar](3) NULL,
	[INVCHEXCHRATE] [decimal](18, 9) NULL,
	[SALAMOUNT] [decimal](17, 2) NOT NULL,
	[SATISTUTAR] [decimal](38, 2) NULL,
	[IADETUTAR] [decimal](38, 2) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sayfa1$]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sayfa1$](
	[PRSNAME] [nvarchar](255) NULL,
	[PRSSURNAME] [nvarchar](255) NULL,
	[PRSIDENTY] [nvarchar](255) NULL,
	[PRSDEVVAL] [nvarchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SILINENINVOICE]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SILINENINVOICE](
	[INVID] [bigint] NOT NULL,
	[INVCOMPANY] [varchar](2) NOT NULL,
	[INVDIVISON] [varchar](2) NOT NULL,
	[INVVATKIND] [varchar](1) NOT NULL,
	[INVSOURCE] [varchar](1) NOT NULL,
	[INVKIND] [varchar](1) NOT NULL,
	[INVSAFEORINV] [varchar](1) NOT NULL,
	[INVTYPE] [varchar](1) NOT NULL,
	[INVTEVVAL] [varchar](6) NULL,
	[INVDATE] [smalldatetime] NOT NULL,
	[INVPAYDATE] [smalldatetime] NOT NULL,
	[INVPRINTDATE] [smalldatetime] NULL,
	[INVSERIAL] [varchar](20) NULL,
	[INVPRINTEDNO] [bigint] NULL,
	[INVVAL] [smallint] NOT NULL,
	[INVBHEVAL] [int] NOT NULL,
	[INVDEEDVAL] [int] NOT NULL,
	[INVPAYVAL] [varchar](3) NOT NULL,
	[INVDIVVAL] [varchar](2) NOT NULL,
	[INVCURID] [bigint] NOT NULL,
	[INVCURKIND] [varchar](1) NOT NULL,
	[INVEXSOID] [bigint] NULL,
	[INVEXCH] [varchar](3) NOT NULL,
	[INVRATE] [decimal](18, 9) NOT NULL,
	[INVACCEXCH] [varchar](3) NOT NULL,
	[INVACCRATE] [decimal](18, 9) NOT NULL,
	[INVACCBALANCE] [decimal](17, 2) NOT NULL,
	[INVDOVATON] [bit] NOT NULL,
	[INVSUMDISC] [decimal](17, 2) NOT NULL,
	[INVPRODISC] [decimal](17, 2) NOT NULL,
	[INVAMOUNT] [decimal](17, 2) NOT NULL,
	[INVDISC] [decimal](17, 2) NOT NULL,
	[INVOTV] [decimal](17, 2) NOT NULL,
	[INVVAT] [decimal](17, 2) NOT NULL,
	[INVTEV] [decimal](17, 2) NOT NULL,
	[INVSTOP] [decimal](17, 2) NOT NULL,
	[INVBALANCE] [decimal](17, 2) NOT NULL,
	[INVSUMDISCEXCH] [decimal](17, 2) NOT NULL,
	[INVPRODISCEXCH] [decimal](17, 2) NOT NULL,
	[INVAMOUNTEXCH] [decimal](17, 2) NOT NULL,
	[INVDISCEXCH] [decimal](17, 2) NOT NULL,
	[INVOTVEXCH] [decimal](17, 2) NOT NULL,
	[INVTEVEXCH] [decimal](17, 2) NOT NULL,
	[INVVATEXCH] [decimal](17, 2) NOT NULL,
	[INVSTOPEXCH] [decimal](17, 2) NOT NULL,
	[INVBALANCEEXCH] [decimal](17, 2) NOT NULL,
	[INVKG] [decimal](15, 6) NOT NULL,
	[INVNOTES] [varchar](1000) NOT NULL,
	[INVADRNO] [int] NULL,
	[INVSALID] [bigint] NULL,
	[INVTIEID] [bigint] NULL,
	[INVEXPOLNO] [bigint] NULL,
	[INVACCKIND] [varchar](1) NOT NULL,
	[INVSPECIALVAL] [varchar](20) NULL,
	[INVSOCODE] [varchar](15) NOT NULL,
	[INVDATETIME] [datetime] NOT NULL,
	[INVPAYORDERSTS] [int] NULL,
	[INVSERIALWARRANT] [int] NULL,
	[INVSERIALWARRANTCOUNT] [int] NULL,
	[INVLAWFNO] [bigint] NULL,
	[INVCONTROLSTS] [bit] NOT NULL,
	[INVCONTROLSOCODE] [varchar](15) NULL,
	[INVCONTROLDATE] [datetime] NULL,
	[INVUSEFILED1] [varchar](1000) NULL,
	[INVBABSSTS] [bit] NOT NULL,
	[INVBABSSOCODE] [varchar](15) NULL,
	[INVBABSDATE] [datetime] NULL,
	[INVSTOPAJLISTSTS] [bit] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SILINENINVOICECHILD]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SILINENINVOICECHILD](
	[INVCHID] [bigint] NOT NULL,
	[INVCHINVID] [bigint] NOT NULL,
	[INVCHSOURCE] [varchar](1) NOT NULL,
	[INVCHNOTES] [varchar](max) NULL,
	[INVCHDIVISON] [varchar](2) NOT NULL,
	[INVCHPAYPID] [bigint] NULL,
	[INVCHSMENID] [bigint] NULL,
	[INVCHINCOID] [bigint] NULL,
	[INVCHDEXPOID] [bigint] NULL,
	[INVCHIFEXPOORINCOQUAN] [smallint] NULL,
	[INVCHQUAN] [decimal](18, 6) NULL,
	[INVCHUNIT] [varchar](3) NULL,
	[INVCHEXSOID] [bigint] NULL,
	[INVCHORJPRICE] [decimal](21, 9) NOT NULL,
	[INVCHORJVATKIND] [varchar](1) NOT NULL,
	[INVCHPRICE] [decimal](21, 9) NULL,
	[INVCHEXCH] [varchar](3) NULL,
	[INVCHEXCHRATE] [decimal](18, 9) NULL,
	[INVCHPRICEKIND] [varchar](1) NULL,
	[INVCHDPRID] [bigint] NULL,
	[INVCHVATRATE] [decimal](5, 2) NULL,
	[INVCHOTVRATE] [decimal](5, 2) NOT NULL,
	[INVCHOTVPRICE] [decimal](18, 6) NOT NULL,
	[INVCHSTOPRATE] [decimal](5, 2) NOT NULL,
	[INVCHAMOUNT] [decimal](17, 2) NOT NULL,
	[INVCHDISC] [decimal](17, 2) NOT NULL,
	[INVCHOTV] [decimal](17, 2) NOT NULL,
	[INVCHVAT] [decimal](17, 2) NOT NULL,
	[INVCHTEV] [decimal](17, 2) NOT NULL,
	[INVCHSTOP] [decimal](17, 2) NOT NULL,
	[INVCHBALANCE] [decimal](17, 2) NOT NULL,
	[INVCHEXCHAMOUNT] [decimal](17, 2) NOT NULL,
	[INVCHEXCHDISC] [decimal](17, 2) NOT NULL,
	[INVCHEXCHOTV] [decimal](17, 2) NOT NULL,
	[INVCHEXCHVAT] [decimal](17, 2) NOT NULL,
	[INVCHEXCHTEV] [decimal](17, 2) NOT NULL,
	[INVCHEXCHSTOP] [decimal](17, 2) NOT NULL,
	[INVCHEXCHBALANCE] [decimal](17, 2) NOT NULL,
	[INVCHTIEEXPOCHID] [bigint] NULL,
	[INVCHCANCELID] [bigint] NULL,
	[INVCHSHAMEPRODUCT] [bit] NULL,
	[INVCHDWETVAL] [varchar](50) NULL,
	[INVCHTEVVAL] [varchar](6) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SILINENPROCEEDS]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SILINENPROCEEDS](
	[PCDSID] [bigint] NOT NULL,
	[PCDSCURID] [bigint] NOT NULL,
	[PCDSDATE] [smalldatetime] NOT NULL,
	[PCDSAMOUNT] [decimal](17, 2) NOT NULL,
	[PCDSLATEINCOME] [decimal](17, 2) NOT NULL,
	[PCDSEARLYPAYDISC] [decimal](17, 2) NOT NULL,
	[PCDSEXTRA] [decimal](17, 2) NOT NULL,
	[PCDSCOMPANY] [varchar](2) NOT NULL,
	[PCDSDIVISON] [varchar](2) NOT NULL,
	[PCDSCASHIER] [varchar](10) NULL,
	[PCDSEXCH] [varchar](3) NOT NULL,
	[PCDSRATE] [decimal](18, 9) NOT NULL,
	[PCDSDC] [varchar](1) NOT NULL,
	[PCDSKIND] [smallint] NOT NULL,
	[PCDSSALID] [bigint] NULL,
	[PCDSLAWCURID] [bigint] NULL,
	[PCDSUSEFIELDS1] [varchar](50) NULL,
	[PCDSSOCODE] [varchar](15) NOT NULL,
	[PCDSDATETIME] [datetime] NOT NULL,
	[PCDSPRINTED] [bit] NULL,
	[PCDSEXCHLIST] [varchar](3) NULL,
	[PCDSEXCHAMOUNT] [decimal](17, 2) NULL,
	[PCDSACCKIND] [varchar](1) NOT NULL,
	[PCDSUSEFIELDS2] [varchar](300) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SILINENPROCEEDSCHILD]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SILINENPROCEEDSCHILD](
	[PCDSCHID] [bigint] NOT NULL,
	[PCDSCHPCDSID] [bigint] NOT NULL,
	[PCDSCHDPYMID] [bigint] NOT NULL,
	[PCDSCHAMOUNT] [decimal](17, 2) NOT NULL,
	[PCDSCHCANCELID] [bigint] NULL,
	[PCDSCHEXPOINVID] [bigint] NULL,
	[PCDSCHCHQID] [bigint] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SILINENSALES]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SILINENSALES](
	[SALID] [bigint] NOT NULL,
	[SALCOMPANY] [varchar](2) NOT NULL,
	[SALDIVISON] [varchar](2) NOT NULL,
	[SALCURID] [bigint] NOT NULL,
	[SALDATE] [smalldatetime] NOT NULL,
	[SALAMOUNT] [decimal](17, 2) NOT NULL,
	[SALCHVAL] [varchar](10) NULL,
	[SALSALEKIND] [varchar](1) NOT NULL,
	[SALSHIPKIND] [varchar](1) NOT NULL,
	[SALINSDIV] [varchar](2) NULL,
	[SALINSSTS] [tinyint] NULL,
	[SALINSVAL] [smallint] NULL,
	[SALINS] [bit] NULL,
	[SALCREDITCHECK] [tinyint] NULL,
	[SALISONDELIVERY] [bit] NULL,
	[SALCANID] [int] NULL,
	[SALCANSALID] [bigint] NULL,
	[SALCHANGEID] [bigint] NULL,
	[SALSTS] [varchar](1) NOT NULL,
	[SALUSEFIELD1] [varchar](50) NULL,
	[SALUSEFIELD2] [varchar](50) NULL,
	[SALUSEFIELD3] [varchar](1000) NULL,
	[SALSOCODE] [varchar](15) NOT NULL,
	[SALDATETIME] [datetime] NOT NULL,
	[SALCONTRACTDIV] [smallint] NULL,
	[SALCONTRACTKIND] [bit] NOT NULL,
	[SALCUROTHERVAL] [varchar](20) NULL,
	[SALCONSULTATOR] [varchar](200) NULL,
	[SALPREFORMNO] [varchar](30) NULL,
	[SALINSDEMAND] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SIPARISDEPO]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SIPARISDEPO](
	[DIVDPVAL] [char](2) NULL,
	[DIVDPNAME] [char](20) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SiparisDurumuDegiskenleri]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SiparisDurumuDegiskenleri](
	[Adi] [nvarchar](50) NULL,
	[Deger] [float] NULL,
	[VolantAdi] [nvarchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SiparisDurumuDegiskenleri_last]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SiparisDurumuDegiskenleri_last](
	[Adi] [nvarchar](50) NULL,
	[Deger] [float] NULL,
	[VolantAdi] [nvarchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SmsDurum]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SmsDurum](
	[Durumid] [int] NULL,
	[DurumAciklama] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[STAFF]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[STAFF](
	[STFID] [bigint] IDENTITY(1,1) NOT NULL,
	[STFCODE] [varchar](8) NULL,
	[STFTYPEID] [int] NULL,
	[STFTYPECODE] [varchar](10) NULL,
	[STFNAME] [varchar](50) NOT NULL,
	[STFSURNAME] [varchar](50) NOT NULL,
	[STFIDENTY] [varchar](11) NOT NULL,
	[STFDIVVAL] [varchar](2) NOT NULL,
	[STFBIRDDATE] [date] NULL,
UNIQUE NONCLUSTERED 
(
	[STFIDENTY] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[STAFFMONTHLYSALARY]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[STAFFMONTHLYSALARY](
	[STFID] [bigint] IDENTITY(1,1) NOT NULL,
	[STFCODE] [varchar](8) NULL,
	[STFNAME] [varchar](50) NOT NULL,
	[STFSURNAME] [varchar](50) NOT NULL,
	[STFIDENTY] [varchar](11) NOT NULL,
	[STFDIVVAL] [varchar](2) NOT NULL,
	[STFBIRDDATE] [date] NULL,
UNIQUE NONCLUSTERED 
(
	[STFIDENTY] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[STAFFPAYMENT]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[STAFFPAYMENT](
	[STFPYMID] [int] IDENTITY(1,1) NOT NULL,
	[STFPYMTYPEID] [int] NULL,
	[STFPYMDIVVAL] [nvarchar](2) NULL,
	[STFPYMSTARTDATE] [datetime] NULL,
	[STFPYMENDDATE] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[STAFFPAYMENTLIST]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[STAFFPAYMENTLIST](
	[STFPYMLISTID] [int] IDENTITY(1,1) NOT NULL,
	[STFPYMLISTPYMID] [int] NULL,
	[STFPYMLISTSTFID] [int] NULL,
	[STFPYMLISTEXPOLID] [bigint] NULL,
	[STFPYMLISTEXPOCHID] [bigint] NULL,
	[STFPYMLISTAMOUNT] [numeric](18, 2) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[STAFFPAYMENTTYPE]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[STAFFPAYMENTTYPE](
	[STFPYMTYPEID] [int] IDENTITY(1,1) NOT NULL,
	[STFPYMTYPENAME] [varchar](8) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[STAFFSALESMEN]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[STAFFSALESMEN](
	[STFSMENID] [bigint] IDENTITY(1,1) NOT NULL,
	[STFSMENTYPEID] [int] NULL,
	[STFSMENTYPECODE] [varchar](10) NULL,
	[STFSMENNAME] [varchar](50) NULL,
	[STFSMENSURNAME] [varchar](100) NULL,
	[STFSMENDIVVAL] [varchar](2) NULL,
	[STFSMENBIRDDATE] [date] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[STAFFTYPE]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[STAFFTYPE](
	[STFTYPEID] [int] IDENTITY(1,1) NOT NULL,
	[STFTYPENAME] [varchar](20) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StokDepo]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StokDepo](
	[DIVDPVAL] [char](2) NULL,
	[DIVDPNAME] [char](30) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TempTanim]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempTanim](
	[ID] [int] NULL,
	[Tanim] [varchar](150) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeslimatAdres]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeslimatAdres](
	[Adres] [nvarchar](255) NULL,
	[AdresTarifi] [nvarchar](255) NULL,
	[AliciAdi] [nvarchar](255) NULL,
	[AliciTelefon] [nvarchar](255) NULL,
	[ID] [int] NULL,
	[Il] [nvarchar](255) NULL,
	[IlId] [int] NULL,
	[IlKodu] [nvarchar](255) NULL,
	[Ilce] [nvarchar](255) NULL,
	[IlceId] [int] NULL,
	[IlceKodu] [nvarchar](255) NULL,
	[PostaKodu] [nvarchar](255) NULL,
	[Ulke] [nvarchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeslimatBekleyenSatislar]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeslimatBekleyenSatislar](
	[Magaza Kodu] [varchar](2) NOT NULL,
	[Magaza Adi] [varchar](40) NULL,
	[Bölge Müdürü] [varchar](1000) NULL,
	[Satis Tarihi] [smalldatetime] NOT NULL,
	[Müsteri Kodu] [varchar](20) NULL,
	[Müsteri Adi] [varchar](1000) NULL,
	[Ürün Grubu] [varchar](60) NULL,
	[Ürün Kodu] [varchar](200) NULL,
	[Ürün Adi] [varchar](200) NULL,
	[Satis Tutari] [decimal](38, 2) NULL,
	[Satis Miktari] [int] NULL,
	[Teslimat Tarihi] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ticimaxil]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ticimaxil](
	[ID] [int] NULL,
	[Tanim] [nvarchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ticimaxilce]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ticimaxilce](
	[ID] [int] NULL,
	[Tanim] [varchar](100) NULL,
	[ILID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicimaxKargoAktif]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxKargoAktif](
	[Tici_ID] [int] NULL,
	[Aktif] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicimaxKargoTakipeski]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxKargoTakipeski](
	[SiparisID] [int] NULL,
	[SiparisFaturaID] [varchar](20) NULL,
	[SiparisFaturaDetayID] [varchar](20) NULL,
	[GöndericiSubeKodu] [varchar](20) NULL,
	[MNGSiparisNumber] [varchar](20) NULL,
	[MNGSiparisID] [varchar](20) NULL,
	[MNGBarkodNumber] [int] NULL,
	[MNGSiparisTarihi] [smalldatetime] NULL,
	[MNGBarkod] [varchar](20) NULL,
	[MNGTakipAdresi] [nvarchar](100) NULL,
	[MNGDurumu] [nvarchar](100) NULL,
	[MNGTeslimDurum] [int] NULL,
	[MNGTeslimTarihi] [smalldatetime] NULL,
	[MNGTeslimAlan] [nvarchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicimaxPuanKullanim]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxPuanKullanim](
	[KazanilanPuanID] [int] NULL,
	[KartNumarasi] [nvarchar](20) NULL,
	[KazanilanPuan] [float] NULL,
	[KazanilanPuanTLKarsiligi] [float] NULL,
	[KullanilanPuan] [float] NULL,
	[KullanilanPuanTLKarsiligi] [float] NULL,
	[KullanimTarihi] [datetime] NULL,
	[PuanBildirildi] [bit] NULL,
	[Tip] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicimaxSiparisKampanya]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxSiparisKampanya](
	[KampanyaID] [int] NULL,
	[SiparisID] [int] NULL,
	[UrunID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicimaxSiparisUrun]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxSiparisUrun](
	[Adet] [float] NULL,
	[Barkod] [nvarchar](255) NULL,
	[Durum] [int] NULL,
	[DurumAd] [nvarchar](255) NULL,
	[ID] [int] NULL,
	[IslemAd] [nvarchar](255) NULL,
	[IslemID] [int] NULL,
	[KampanyaID] [int] NULL,
	[KampanyaIndirimTutari] [float] NULL,
	[KdvOrani] [int] NULL,
	[KdvTutari] [float] NULL,
	[MagazaAtamaTarihi] [datetime] NULL,
	[MagazaDurum] [int] NULL,
	[MagazaGonderimTarihi] [datetime] NULL,
	[MagazaID] [int] NULL,
	[MagazaKodu] [nvarchar](255) NULL,
	[Maliyet] [float] NULL,
	[SiparisId] [int] NULL,
	[StokKodu] [nvarchar](255) NULL,
	[TedarikciID] [int] NULL,
	[TedarikciKodu] [nvarchar](255) NULL,
	[TedarikciKodu2] [nvarchar](255) NULL,
	[Tutar] [float] NULL,
	[UrunAdi] [nvarchar](255) NULL,
	[UrunID] [int] NULL,
	[UrunKartiID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicimaxTeknikDetayUrun]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxTeknikDetayUrun](
	[PROVAL] [varchar](20) NULL,
	[Tic_Group_ID] [int] NULL,
	[Tic_Ozellik_ID] [int] NULL,
	[Tic_Deger_ID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicimaxVaryantDetayID]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicimaxVaryantDetayID](
	[id] [int] IDENTITY(0,1) NOT NULL,
	[Tanim] [varchar](15) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserDepart]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserDepart](
	[USRDPID] [int] IDENTITY(1,1) NOT NULL,
	[USRDBNAME] [nchar](20) NULL,
	[USRDPNAME] [nchar](50) NULL,
	[USRDPFORMID] [int] NULL,
	[USRDPDEPVAL] [nchar](20) NULL,
 CONSTRAINT [PK_UserDepart] PRIMARY KEY CLUSTERED 
(
	[USRDPID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserForm]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserForm](
	[USRFRDPID] [int] IDENTITY(1,1) NOT NULL,
	[USRFRNAME] [nchar](20) NULL,
	[USRFRREGNAME] [varchar](100) NULL,
 CONSTRAINT [PK_UserFORM] PRIMARY KEY CLUSTERED 
(
	[USRFRDPID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserList]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[Isim] [nvarchar](100) NULL,
	[Soyisim] [nvarchar](100) NULL,
	[Password] [nvarchar](100) NULL,
	[TicimaxUserid] [int] NULL,
	[TEl1] [varchar](12) NULL,
	[TEl2] [varchar](12) NULL,
	[Vol_SOCODE] [varchar](15) NULL,
	[Vol_SOENTERKEY] [varchar](15) NULL,
	[E-MailAdress] [varchar](150) NULL,
	[E-MailPassword] [varchar](150) NULL,
 CONSTRAINT [PK_UserList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserPotency]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPotency](
	[Userid] [int] NULL,
	[AnaSatis] [bit] NULL,
	[Satis0] [bit] NULL,
	[Satis1] [bit] NULL,
	[Satis2] [bit] NULL,
	[Satis3] [bit] NULL,
	[Satis4] [bit] NULL,
	[Satis5] [bit] NULL,
	[AnaUrun] [bit] NULL,
	[Urun0] [bit] NULL,
	[Urun1] [bit] NULL,
	[Urun2] [bit] NULL,
	[Urun3] [bit] NULL,
	[Urun4] [bit] NULL,
	[Urun5] [bit] NULL,
	[AnaDepoYonetimi] [bit] NULL,
	[Depo0] [bit] NULL,
	[Depo1] [bit] NULL,
	[Depo2] [bit] NULL,
	[Depo3] [bit] NULL,
	[Depo4] [bit] NULL,
	[Depo5] [bit] NULL,
	[AnaCariIslemler] [bit] NULL,
	[Cari0] [bit] NULL,
	[Cari1] [bit] NULL,
	[Cari2] [bit] NULL,
	[Cari3] [bit] NULL,
	[Cari4] [bit] NULL,
	[Cari5] [bit] NULL,
	[AnaEIslem] [bit] NULL,
	[Fatura] [bit] NULL,
	[Arsiv] [bit] NULL,
	[Irsaliye] [bit] NULL,
	[AnaKargo] [bit] NULL,
	[Kargo0] [bit] NULL,
	[Kargo1] [bit] NULL,
	[Kargo2] [bit] NULL,
	[Kargo3] [bit] NULL,
	[Kargo4] [bit] NULL,
	[Kargo5] [bit] NULL,
	[AnaRapor] [bit] NULL,
	[Rapor0] [bit] NULL,
	[Rapor1] [bit] NULL,
	[Rapor2] [bit] NULL,
	[Rapor3] [bit] NULL,
	[Rapor4] [bit] NULL,
	[Rapor5] [bit] NULL,
	[AnaAyarlar] [bit] NULL,
	[AnaTicimax] [bit] NULL,
	[Ticimax0] [bit] NULL,
	[Ticimax1] [bit] NULL,
	[Ticimax2] [bit] NULL,
	[Ticimax3] [bit] NULL,
	[Ticimax4] [bit] NULL,
	[Ticimax5] [bit] NULL,
	[Ticimax6] [bit] NULL,
	[Ticimax7] [bit] NULL,
	[Ticimax8] [bit] NULL,
	[Ticimax9] [bit] NULL,
	[Ticimax10] [bit] NULL,
	[Ticimax11] [bit] NULL,
	[AnaPhone] [bit] NULL,
	[Phone0] [bit] NULL,
	[Phone1] [bit] NULL,
	[Phone2] [bit] NULL,
	[Phone3] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserTransactionLogMain]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTransactionLogMain](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[IslemZamani] [smalldatetime] NOT NULL,
	[Form] [varchar](50) NOT NULL,
	[Islem] [varchar](50) NOT NULL,
	[IslemAdi] [nvarchar](50) NOT NULL,
	[IslemDetayi] [nvarchar](250) NULL,
	[IslmeSonucu] [bit] NULL,
	[SiparisID] [int] NULL,
	[UyeID] [int] NULL,
 CONSTRAINT [PK_UserTransactionLogMain] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Uye]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Uye](
	[CepTelefonu] [nvarchar](255) NULL,
	[CinsiyetID] [int] NULL,
	[DogumTarihi] [datetime] NULL,
	[DuzenlemeTarihi] [datetime] NULL,
	[ID] [int] NULL,
	[Il] [nvarchar](255) NULL,
	[Ilce] [nvarchar](255) NULL,
	[Mahalle] [nvarchar](255) NULL,
	[IlceID] [int] NULL,
	[IlID] [int] NULL,
	[MahalleID] [int] NULL,
	[Isim] [nvarchar](255) NULL,
	[Mail] [nvarchar](255) NULL,
	[MailIzin] [bit] NULL,
	[Meslek] [nvarchar](255) NULL,
	[MusteriKodu] [nvarchar](255) NULL,
	[OgrenimDurumu] [nvarchar](255) NULL,
	[Sifre] [nvarchar](255) NULL,
	[SmsIzin] [bit] NULL,
	[Soyisim] [nvarchar](255) NULL,
	[Telefon] [nvarchar](255) NULL,
	[UyeTuruID] [int] NULL,
	[UyeTCtoVKN] [varchar](11) NULL,
	[UyeVD] [varchar](250) NULL,
	[UyeIsVm] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantMagazaIletisimeski]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantMagazaIletisimeski](
	[DIVVAL] [varchar](2) NULL,
	[DIVNAME] [varchar](25) NULL,
	[DIVPHN1] [varchar](15) NULL,
	[DIVMDRNAME] [varchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantParaTahsilat]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantParaTahsilat](
	[CompanyId] [int] NOT NULL,
	[CompanyName] [varchar](50) NOT NULL,
	[Url] [varchar](250) NOT NULL,
	[Type] [int] NULL,
	[reCAPTCHA] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantSatisParametleri]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantSatisParametleri](
	[SatisMagaza] [varchar](2) NULL,
	[StokAdi] [char](10) NULL,
	[TaksitliFiyattipi] [varchar](20) NULL,
	[TaksitliOdemeAraci] [varchar](10) NULL,
	[InternetFiyattipi] [varchar](20) NULL,
	[InternetOdemeAraci] [varchar](10) NULL,
	[Pazaryeri1Fiyattipi] [varchar](20) NULL,
	[Pazaryeri1OdemeAraci] [varchar](10) NULL,
	[Pazaryeri2Fiyattipi] [varchar](20) NULL,
	[Pazaryeri2OdemeAraci] [varchar](10) NULL,
	[Pazaryeri3Fiyattipi] [varchar](20) NULL,
	[Pazaryeri3OdemeAraci] [varchar](10) NULL,
	[Pazaryeri4Fiyattipi] [varchar](20) NULL,
	[Pazaryeri4OdemeAraci] [varchar](10) NULL,
	[Pazaryeri5Fiyattipi] [varchar](20) NULL,
	[Pazaryeri5OdemeAraci] [varchar](10) NULL,
	[SaticiMagaza] [varchar](10) NULL,
	[SaticiPazaryeri1] [varchar](10) NULL,
	[SaticiPazaryeri2] [varchar](10) NULL,
	[SaticiPazaryeri3] [varchar](10) NULL,
	[SaticiPazaryeri4] [varchar](10) NULL,
	[SaticiPazaryeri5] [varchar](10) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantSeoParametleri]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantSeoParametleri](
	[SeoType] [varchar](20) NULL,
	[SeoAnahtarkelime] [varchar](1000) NULL,
	[SeoSayfaAciklama] [varchar](1000) NULL,
	[SeoSayfaBaslik] [varchar](1000) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantSet]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantSet](
	[SetKodu] [varchar](20) NULL,
	[SetAdi] [varchar](20) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantStokSiniflari]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantStokSiniflari](
	[Marka] [char](10) NULL,
	[YayinaAçik] [char](10) NULL,
	[Sinif1] [char](10) NULL,
	[Sinif2] [char](10) NULL,
	[Sinif3] [char](10) NULL,
	[Sinif4] [char](10) NULL,
	[Sinif5] [char](10) NULL,
	[Teknik1] [char](10) NULL,
	[Teknik2] [char](10) NULL,
	[Teknik3] [char](10) NULL,
	[Pazaryeri1] [char](10) NULL,
	[Pazaryeri2] [char](10) NULL,
	[Pazaryeri3] [char](10) NULL,
	[Pazaryeri4] [char](10) NULL,
	[Pazaryeri5] [char](10) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantToTicimaxIlveIlce]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantToTicimaxIlveIlce](
	[Vol_DCITYVAL] [int] NULL,
	[Vol_DCITYNAME] [varchar](60) NULL,
	[Vol_DCOUNTYVAL] [int] NULL,
	[Vol_DCOUNTYNAME] [varchar](60) NULL,
	[TicimaxILID] [int] NULL,
	[TicimaxILCEID] [int] NULL,
	[TicimaxILCEADI] [varchar](60) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantToTicimaxKategoriID]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantToTicimaxKategoriID](
	[Vol_WPTREUNIQ] [smallint] NOT NULL,
	[Vol_WPTREVAL] [varchar](100) NOT NULL,
	[Vol_WPTRENAME] [varchar](150) NOT NULL,
	[Vol_WPTRPRNID] [varchar](100) NOT NULL,
	[Tic_ID] [int] NULL,
	[Tic_ParentID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantToTicimaxMarkaID]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantToTicimaxMarkaID](
	[Vol_WPTREUNIQ] [int] NOT NULL,
	[Vol_WPTREVAL] [varchar](100) NOT NULL,
	[Vol_WPTRENAME] [varchar](150) NOT NULL,
	[TicimaxID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantToTicimaxStokAciklama]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantToTicimaxStokAciklama](
	[VOL_PROID] [int] NOT NULL,
	[TicimaxAciklama] [varchar](max) NULL,
	[Kaydeden] [varchar](50) NULL,
 CONSTRAINT [PK_VolantToTicimaxStokAciklama] PRIMARY KEY CLUSTERED 
(
	[VOL_PROID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantToTicimaxStokAciklamaFirma]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantToTicimaxStokAciklamaFirma](
	[VOL_PROID] [int] NOT NULL,
	[TicimaxAciklama] [varchar](max) NULL,
	[Kaydeden] [varchar](50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantToTicimaxStokResim]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantToTicimaxStokResim](
	[VOL_PROID] [int] NOT NULL,
	[TicimaxResimUrl] [varchar](max) NULL,
 CONSTRAINT [PK_VolantToTicimaxStokResim] PRIMARY KEY CLUSTERED 
(
	[VOL_PROID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantToTicimaxTedarikciID]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantToTicimaxTedarikciID](
	[Vol_CURVAL] [varchar](100) NOT NULL,
	[Vol_CURNAME] [varchar](150) NOT NULL,
	[Vol_CURCHEMAIL] [varchar](256) NULL,
	[TicimaxID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantToTicimaxTeknikDetayID]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantToTicimaxTeknikDetayID](
	[Vol_WPTREUNIQ] [smallint] NOT NULL,
	[Vol_WPTREVAL] [varchar](100) NOT NULL,
	[Vol_WPTRENAME] [varchar](150) NOT NULL,
	[Vol_WPTRPRNID] [varchar](100) NOT NULL,
	[Tic_ID] [int] NULL,
	[Tic_ParentID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VolantToTicimaxWebClass]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VolantToTicimaxWebClass](
	[Vol_WPTREVAL] [varchar](100) NOT NULL,
	[DetayClass] [varchar](100) NULL,
	[TeknikClass] [varchar](100) NULL,
 CONSTRAINT [PK_VolantToTicimaxWebClass] PRIMARY KEY CLUSTERED 
(
	[Vol_WPTREVAL] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebKargoFirma]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebKargoFirma](
	[ID] [int] NULL,
	[KapidaOdeme] [bit] NULL,
	[KapidaOdemeFiyati] [float] NULL,
	[KapidaOdemeKK] [bit] NULL,
	[KapidaOdemeKKFiyati] [float] NULL,
	[Tanim] [nvarchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebSiparis]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebSiparis](
	[AdiSoyadi] [nvarchar](255) NULL,
	[Durum] [int] NULL,
	[EntegrasyonAktarildi] [bit] NULL,
	[FaturaAdresId] [int] NULL,
	[FaturaAdresi] [nvarchar](255) NULL,
	[FaturaNo] [nvarchar](255) NULL,
	[FaturaTarihi] [datetime] NULL,
	[HediyeCeki] [nvarchar](255) NULL,
	[HediyeCekiTutari] [float] NULL,
	[HadiyePaketiNotu] [nvarchar](255) NULL,
	[HediyePaketiTutari] [float] NULL,
	[HediyePaketiVar] [bit] NULL,
	[ID] [int] NULL,
	[IPAdresi] [nvarchar](255) NULL,
	[IndirimTutari] [float] NULL,
	[KargoAdresID] [int] NULL,
	[KargoEntegrasyonID] [nvarchar](255) NULL,
	[KargoEntegrasyonTakipNo] [float] NULL,
	[KargoFirmaId] [int] NULL,
	[KargoTakipNo] [nvarchar](255) NULL,
	[KargoTutari] [float] NULL,
	[Kaynak] [int] NULL,
	[Kur] [float] NULL,
	[Mail] [nvarchar](255) NULL,
	[Maliyet] [float] NULL,
	[Odemeler] [nvarchar](255) NULL,
	[ParaBirimi] [nvarchar](255) NULL,
	[Referer] [nvarchar](255) NULL,
	[ReklamKaynagi] [nvarchar](255) NULL,
	[SepetKampanyasiIndirimi] [float] NULL,
	[SiparisDurumu] [nvarchar](255) NULL,
	[SiparisKaynagi] [nvarchar](255) NULL,
	[SiparisNotu] [nvarchar](255) NULL,
	[SiparisTarihi] [datetime] NULL,
	[SiparisToplamTutari] [float] NULL,
	[StokDustu] [bit] NULL,
	[TeslimatAdresi] [nvarchar](255) NULL,
	[TeslimatGunu] [datetime] NULL,
	[TeslimatSaati] [nvarchar](255) NULL,
	[ToplamKdv] [float] NULL,
	[ToplamTutar] [float] NULL,
	[Tutar] [float] NULL,
	[Urunler] [nvarchar](255) NULL,
	[UyeAdi] [nvarchar](255) NULL,
	[UyeID] [int] NULL,
	[UyeMusteriKodu] [nvarchar](255) NULL,
	[UyeSoyadi] [nvarchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebSiparisIptalDurum]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebSiparisIptalDurum](
	[SipID] [int] NULL,
	[IptalNedeni] [varchar](50) NULL,
	[IptalAciklamasi] [text] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[Anket_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Anket_insert]
(
	@UyeId int,
	@SipId	int,
	@Çalisma_Durumu nvarchar(50),
	@Meslek nvarchar(50),
	@Çalisma_Süresi int,
	@Gelir_Durumu nvarchar(50),
	@Icra_Durumu int,
	@Ev_Durumu int,
	@Oturma_Süresi int,
	@Medeni_Durumu int,
	@Kefil_Verebilir bit,
	@Pesinat_Verebilir bit,
	@Anket_Notu nvarchar(250),
	@AnketTarihi smalldatetime,
	@ReturnDesc  nvarchar(255) output
)
AS
if (select count(*) from Anket where UyeId = @UyeId and SiparisId = @SipId) = 0
begin
INSERT INTO [dbo].[Anket]
           ([UyeId]
		   ,SiparisId
           ,[Çalisma Durumu]
           ,[Meslek]
           ,[Çalisma Süresi]
           ,[Gelir Durumu]
           ,[Icra Durumu]
           ,[Ev Durumu]
           ,[Oturma Süresi]
           ,[Medeni Durumu]
           ,[Kefil Verebilir]
           ,[Pesinat Verebilir]
           ,[Anket Notu]
		   ,[Anket Tarihi])
     VALUES
           (@UyeId
		   ,@SipId
           ,@Çalisma_Durumu
           ,@Meslek
           ,@Çalisma_Süresi
           ,@Gelir_Durumu
           ,@Icra_Durumu
           ,@Ev_Durumu
           ,@Oturma_Süresi
           ,@Medeni_Durumu
           ,@Kefil_Verebilir
           ,@Pesinat_Verebilir
           ,@Anket_Notu
		   ,@AnketTarihi
		   )
	set @ReturnDesc = '1'
end
else
begin
	UPDATE [dbo].[Anket]
	SET [Çalisma Durumu] = @Çalisma_Durumu
		,[Meslek] = @Meslek
		,[Çalisma Süresi] = @Çalisma_Süresi
		,[Gelir Durumu] =@Gelir_Durumu
		,[Icra Durumu] = @Icra_Durumu
		,[Ev Durumu] = @Ev_Durumu
		,[Oturma Süresi] = @Oturma_Süresi
		,[Medeni Durumu] = @Medeni_Durumu
		,[Kefil Verebilir] = @Kefil_Verebilir
		,[Pesinat Verebilir] = @Pesinat_Verebilir
		,[Anket Notu] = @Anket_Notu
		,[Anket Tarihi] = @AnketTarihi
	WHERE [UyeId] = @UyeId and SiparisId = @SipId
	set @ReturnDesc = 'Var olan Kayit Güncellendi'
end
GO
/****** Object:  StoredProcedure [dbo].[Anket_insert2]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Anket_insert2]
(
	@UyeId int,
	@SipId	int,
	@Çalisma_Durumu nvarchar(50),
	@Meslek nvarchar(50),
	@Çalisma_Süresi int,
	@Gelir_Durumu nvarchar(50),
	@Icra_Durumu int,
	@Ev_Durumu int,
	@Oturma_Süresi int,
	@Medeni_Durumu int,
	@Kefil_Verebilir bit,
	@Pesinat_Verebilir bit,
	@Edevlet_Verebilir bit,
	@SozlesmeYeri	bit,
	@Anket_Notu nvarchar(250),
	@AnketTarihi smalldatetime,
	@ReturnDesc  nvarchar(255) output
)
AS
if (select count(*) from Anket where UyeId = @UyeId and SiparisId = @SipId) = 0
begin
INSERT INTO [dbo].[Anket]
           ([UyeId]
		   ,SiparisId
           ,[Çalisma Durumu]
           ,[Meslek]
           ,[Çalisma Süresi]
           ,[Gelir Durumu]
           ,[Icra Durumu]
           ,[Ev Durumu]
           ,[Oturma Süresi]
           ,[Medeni Durumu]
           ,[Kefil Verebilir]
           ,[Pesinat Verebilir]
           ,[Anket Notu]
		   ,[Anket Tarihi]
		   ,[E-Devlet Durumu]
		   ,[Sözlesme Yeri])
     VALUES
           (@UyeId
		   ,@SipId
           ,@Çalisma_Durumu
           ,@Meslek
           ,@Çalisma_Süresi
           ,@Gelir_Durumu
           ,@Icra_Durumu
           ,@Ev_Durumu
           ,@Oturma_Süresi
           ,@Medeni_Durumu
           ,@Kefil_Verebilir
           ,@Pesinat_Verebilir
           ,@Anket_Notu
		   ,@AnketTarihi
		   ,@Edevlet_Verebilir
		   ,@SozlesmeYeri
		   )
	set @ReturnDesc = '1'
end
else
begin
	UPDATE [dbo].[Anket]
	SET [Çalisma Durumu] = @Çalisma_Durumu
		,[Meslek] = @Meslek
		,[Çalisma Süresi] = @Çalisma_Süresi
		,[Gelir Durumu] =@Gelir_Durumu
		,[Icra Durumu] = @Icra_Durumu
		,[Ev Durumu] = @Ev_Durumu
		,[Oturma Süresi] = @Oturma_Süresi
		,[Medeni Durumu] = @Medeni_Durumu
		,[Kefil Verebilir] = @Kefil_Verebilir
		,[Pesinat Verebilir] = @Pesinat_Verebilir
		,[Anket Notu] = @Anket_Notu
		,[Anket Tarihi] = @AnketTarihi
		,[Sözlesme Yeri] = @SozlesmeYeri
		,[E-Devlet Durumu] = @Edevlet_Verebilir
	WHERE [UyeId] = @UyeId and SiparisId = @SipId
	set @ReturnDesc = 'Var olan Kayit Güncellendi'
end
GO
/****** Object:  StoredProcedure [dbo].[DurumGuncelle]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[DurumGuncelle]
(
	@Durum	nvarchar(20),
	@sipID	int
)
AS
declare @Deger	int,
		@Vol_Adi	varchar(50)
if @Durum = '20'
begin	
	update s set Durum = 20,SiparisDurumu = 'Takipli Satis' from WebSiparis s where s.ID = @sipID	
end
else
begin
select @Deger=Deger, @Vol_Adi = VolantAdi from SiparisDurumuDegiskenleri where Adi = @Durum


update s set Durum = @Deger,SiparisDurumu = @Vol_Adi from WebSiparis s where s.ID = @sipID	
end
GO
/****** Object:  StoredProcedure [dbo].[DurumGuncelle2]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[DurumGuncelle2]
(
	@Durum	int,
	@sipID	int
)
AS
declare @Deger	int,
		@Vol_Adi	varchar(50)

	select @Vol_Adi=VolantAdi from SiparisDurumuDegiskenleri where Deger = @Durum
	update s set Durum = @Durum,SiparisDurumu = @Vol_Adi from WebSiparis s where s.ID = @sipID
GO
/****** Object:  StoredProcedure [dbo].[FaturaAdres_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[FaturaAdres_insert]
(
			@Adres nvarchar(255)
           ,@AliciTelefon nvarchar(255)
           ,@EntegrasyonId nvarchar(255) = null
           ,@FirmaAdi nvarchar(255)
           ,@ID int
           ,@Il nvarchar(255)
           ,@IlId int
           ,@IlKodu nvarchar(255)
           ,@Ilce nvarchar(255)
           ,@IlceId int
           ,@IlceKodu nvarchar(255)
           ,@Ulke nvarchar(255)
           ,@VergiDairesi nvarchar(255)
           ,@VergiNo nvarchar(255)
           ,@isKurumsal bit
		   ,@ReturnDesc  nvarchar(255) output
)
AS
if (select COUNT(*) from FaturaAdres where ID = @ID) = 0
begin
	INSERT INTO [dbo].[FaturaAdres]
			   ([Adres]
			   ,[AliciTelefon]
			   ,[EntegrasyonId]
			   ,[FirmaAdi]
			   ,[ID]
			   ,[Il]
			   ,[IlId]
			   ,[IlKodu]
			   ,[Ilce]
			   ,[IlceId]
			   ,[IlceKodu]
			   ,[Ulke]
			   ,[VergiDairesi]
			   ,[VergiNo]
			   ,[isKurumsal])
		 VALUES
			   (@Adres
			   ,@AliciTelefon
			   ,@EntegrasyonId
			   ,@FirmaAdi
			   ,@ID
			   ,@Il
			   ,@IlId
			   ,@IlKodu
			   ,@Ilce
			   ,@IlceId
			   ,@IlceKodu
			   ,@Ulke
			   ,@VergiDairesi
			   ,@VergiNo
			   ,@isKurumsal)

	set @ReturnDesc = '1'
end
else
begin
	update FaturaAdres set 
	Adres=@Adres,
	AliciTelefon=@AliciTelefon,
	EntegrasyonId=@EntegrasyonId,
	FirmaAdi=@FirmaAdi,
	Il=@Il,
	IlId=@IlId,
	IlKodu=@IlKodu,
	Ilce=@Ilce,
	IlceId=@IlceId,
	IlceKodu=@IlceKodu,
	Ulke=@Ulke,
	VergiDairesi=@VergiDairesi,
	VergiNo=@VergiNo,
	isKurumsal=@isKurumsal	
	where ID = @ID
	set @ReturnDesc = '2'
end
GO
/****** Object:  StoredProcedure [dbo].[FaturaAdres_Pazaryeri_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[FaturaAdres_Pazaryeri_insert]
(
			@Adres nvarchar(255)
           ,@AliciTelefon nvarchar(255)
           ,@EntegrasyonId nvarchar(255) = null
           ,@FirmaAdi nvarchar(255)
           ,@ID int
           ,@Il nvarchar(255)
           ,@IlId int
           ,@IlKodu nvarchar(255)
           ,@Ilce nvarchar(255)
           ,@IlceId int
           ,@IlceKodu nvarchar(255)
           ,@Ulke nvarchar(255)
           ,@VergiDairesi nvarchar(255)
           ,@VergiNo nvarchar(255)
           ,@isKurumsal bit
		   ,@ReturnDesc  nvarchar(255) output
)
AS
	INSERT INTO [dbo].[FaturaAdres]
			   ([Adres]
			   ,[AliciTelefon]
			   ,[EntegrasyonId]
			   ,[FirmaAdi]
			   ,[ID]
			   ,[Il]
			   ,[IlId]
			   ,[IlKodu]
			   ,[Ilce]
			   ,[IlceId]
			   ,[IlceKodu]
			   ,[Ulke]
			   ,[VergiDairesi]
			   ,[VergiNo]
			   ,[isKurumsal])
		 VALUES
			   (@Adres
			   ,@AliciTelefon
			   ,@EntegrasyonId
			   ,@FirmaAdi
			   ,@ID
			   ,@Il
			   ,@IlId
			   ,@IlKodu
			   ,@Ilce
			   ,@IlceId
			   ,@IlceKodu
			   ,@Ulke
			   ,@VergiDairesi
			   ,@VergiNo
			   ,@isKurumsal)

	set @ReturnDesc = '1'
GO
/****** Object:  StoredProcedure [dbo].[Fk_sp_calculate_inventory]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Fk_sp_calculate_inventory]
(
	@PROVAL	nvarchar(20)
)
--with encryption
as
Set NoCount On;
set dateformat dmy; 


CREATE TABLE #TEMP
(
A1 VARCHAR(20),
A2 INT,
A3 VARCHAR(20),
A4 INT,
A5 VARCHAR(20),
A6 INT,
)
insert into #temp values ('ADAPAZARI',0,'ESKISEHIR',0,'MENEMEN',0)
insert into #temp values ('AFYON',0,'GEBZE',0,'ORDU',0)
insert into #temp values ('ANTALYA',0,'GIRESUN',0,'RIZE',0)
insert into #temp values ('BURSA',0,'ISPARTA',0,'SIVAS',0)
insert into #temp values ('ÇANAKKALE',0,'ISTANBUL-ANADOLU',0,'TEKIRDAG',0)
insert into #temp values ('ÇORUM',0,'ISTANBUL-AVRUPA',0,'TOKAT',0)
insert into #temp values ('DENIZLI',0,'IZMIT',0,'USAK',0)
insert into #temp values ('EDIRNE',0,'KAYSERI',0,'YOZGAT',0)
insert into #temp values ('ELAZIG',0,'LÜLEBURGAZ',0,'ZONGULDAK',0)


update #TEMP set A2 = CASE WHEN env >= 1 THEN env ELSE 1 END from (
select DIVDPNAME as mgz,SUM(PINVQUAN) AS env from VOLANT.VDB_YON01.dbo.PRODUCTS
left outer join VOLANT.VDB_YON01.dbo.PROINV on PINVPROID = PROID
left outer join VOLANT.VDB_YON01.dbo.DEFSTORAGE on DSTORID = PINVSTORID
left outer join VOLANT.MDE_GENEL.DBO.SIPARISDEPO on DIVDPVAL = DSTORDIVISON
where PINVYEAR != 0
and PROVAL = @PROVAL
GROUP BY DIVDPNAME
) net
inner join #TEMP on A1 = mgz

update #TEMP set A4 = CASE WHEN env >= 1 THEN env ELSE 1 END from (
select DIVDPNAME as mgz,SUM(PINVQUAN) AS env from VOLANT.VDB_YON01.dbo.PRODUCTS
left outer join VOLANT.VDB_YON01.dbo.PROINV on PINVPROID = PROID
left outer join VOLANT.VDB_YON01.dbo.DEFSTORAGE on DSTORID = PINVSTORID
left outer join VOLANT.MDE_GENEL.DBO.SIPARISDEPO on DIVDPVAL = DSTORDIVISON
where PINVYEAR != 0
and PROVAL = @PROVAL
GROUP BY DIVDPNAME
) net
inner join #TEMP on A3 = mgz

update #TEMP set A6 = CASE WHEN env >= 1  THEN env ELSE 1 END from (
select DIVDPNAME as mgz,SUM(PINVQUAN) AS env from VOLANT.VDB_YON01.dbo.PRODUCTS
left outer join VOLANT.VDB_YON01.dbo.PROINV on PINVPROID = PROID
left outer join VOLANT.VDB_YON01.dbo.DEFSTORAGE on DSTORID = PINVSTORID
left outer join VOLANT.MDE_GENEL.DBO.SIPARISDEPO on DIVDPVAL = DSTORDIVISON
where PINVYEAR != 0
and PROVAL = @PROVAL
GROUP BY DIVDPNAME
) net
inner join #TEMP on A5 = mgz

Declare @Body varchar(max),
		@TableHead varchar(max),
		@TableTail varchar(max)

------------------------------------------------------------------------------------------------------- 
begin	
					

        
--------------------------------------------------------------------------------                  
	
Set @TableTail = '</table><p>Stoklarimizda bulunmayan ürünler diger magazalarimizdan veya üretici firmadan tedarik edilebilir.</p></body></html>';
Set @TableHead = '<html><head>' +
                  '<style>' +
                  'td {border: solid black 1px;padding-left:5px;padding-right:5px;padding-top:1px;padding-bottom:1px;font-size:8pt;} ' +
                  '</style>' +
                  '</head><body>'+
                  '<table cellpadding=5 cellspacing=0 border=0>' +
                  '<td align=left><b>IL</b></td>' +
                  '<td align=center><b>ADET</b></td>' +
                  '<td align=left><b>IL</b></td>' +
                  '<td align=center><b>ADET</b></td>' +
                  '<td align=left><b>IL</b></td>' +
                  '<td align=center><b>ADET</b></td></tr>' ;
				                    	
-------------------------------------------------------------------------------------------------------------------
Select @Body = (
-------------------------------------------------------------------------------------------------------------------

select A1 td,A2 td, A3 td, A4 td, A5 td, A6 td from #TEMP


For XML raw('tr'), Elements )
-------------------------------------------------------------------------------------------------------------------
	if @body !='' or not @body is null 
	begin 

	Set @Body = Replace(@Body, '_x0020_', space(1))
	Set @Body = Replace(@Body, '_x003D_', '=')
	Set @Body = Replace(@Body, '<tr><TRRow>1</TRRow>', '<tr bgcolor=#C6CFFF>')
	Set @Body = Replace(@Body, '<TRRow>0</TRRow>', '')
	
	Select @Body = @TableHead + @Body + @TableTail 


	select @Body
	union
	select TicimaxAciklama from VolantToTicimaxStokAciklama t
	inner join VOLANT.VDB_YON01.dbo.PRODUCTS p on p.PROID = t.VOL_PROID
	where PROVAL = @PROVAL

	
drop table #TEMP	
	end
	end
		


GO
/****** Object:  StoredProcedure [dbo].[InsertUyeler]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[InsertUyeler]
(
@UserName	nvarchar(100),
@Isim			nvarchar(100),
@Soyisim		nvarchar(100),
@Password		nvarchar(100),
@TicimaxUserid int
)
as
insert into UserList (UserName,Isim,Soyisim,Password,TicimaxUserid) values (@UserName,@Isim,@Soyisim,@Password,@TicimaxUserid)

GO
/****** Object:  StoredProcedure [dbo].[RETORNID]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[RETORNID]
(
    @End   INT,
    @Start INT,
    @Adet  INT,
    @IDName NVARCHAR(100),
    @DBName NVARCHAR(100),
    @TableName NVARCHAR(100)
)
as
    DECLARE @sql NVARCHAR(MAX);

    SET @sql = '
    ;WITH Numbers AS
    (
        SELECT ' + CAST(@Start AS NVARCHAR) + ' AS Number
        UNION ALL
        SELECT Number + 1
        FROM Numbers
        WHERE Number + 1 <= ' + CAST(@End AS NVARCHAR) + '
    )
    SELECT TOP (' + CAST(@Adet AS NVARCHAR) + ')
           n.Number AS EksikID
    FROM Numbers n
    LEFT JOIN ' + @DBName + '..' + @TableName + ' t ON t.'+@IDName+' = n.Number
    WHERE t.'+@IDName+' IS NULL
    ORDER BY n.Number
    OPTION (MAXRECURSION 0);';

    EXEC sp_executesql @sql;
GO
/****** Object:  StoredProcedure [dbo].[Siparis_Cek]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Siparis_Cek]
(
	@bsTarih	smalldatetime,
	@btTarih smalldatetime
)
AS
select s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(numeric(12,2),SiparisToplamTutari) as SiparisToplamTutari,SiparisDurumu,
case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriHesapBeyani,
s.SiparisNotu
from WebSiparis s
left outer join Uye u on u.ID = s.UyeID
where siparisTarihi between @bsTarih and @btTarih
order by 1 desc
GO
/****** Object:  StoredProcedure [dbo].[Siparis_Cek_Islenen]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Siparis_Cek_Islenen]
(
	@bsTarih	smalldatetime,
	@btTarih smalldatetime,
	@sipID		int
)
AS


if @sipID = 0
begin
/*
	select TOP 100 s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(int,ROUND(SiparisToplamTutari,0)) as SiparisToplamTutari,SiparisDurumu,
	case when f.AliciTelefon != '' then f.AliciTelefon else CepTelefonu end CepTelefonu,
	case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriNo, 
	case when (select COUNT(*) from Anket where UyeId = u.ID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
		when (select COUNT(*) from Anket where UyeId = u.ID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
	end as AnketSonucu, '' as EDevletVar,
	(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = u.MusteriKodu ) as Vol_CURID,
	s.TeslimatAdresi, f.Il,f.Ilce,FaturaNo 
	from WebSiparis s
	left outer join Uye u on u.ID = s.UyeID
	left outer join FaturaAdres f on f.ID = s.FaturaAdresId
	where Durum not in(0,4,8,9)
*/
select TOP 100 s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
convert(int,ROUND(SiparisToplamTutari,0)) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
	end,'Anket Yok') as AnketSonucu,
case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
	end as AnketDurumu
--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
,FaturaAdresId
,s.UyeMusteriKodu as VOL_CURVAL,
case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
	end as AnketDurumu,u.MailIzin
from
WebSiparis s
left outer join Uye u on u.ID = s.UyeID
left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
outer apply(
select CURID,SALDATE,
case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
from VOLANT.VDB_YON01.dbo.CURRENTS c
inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
and SALID = s.FaturaNo
) volant
	where Durum not in(0,3,7,8,9)
	and EntegrasyonAktarildi = 1
	and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
	order by 1 desc option (fast 100)
end
else
begin
/*
	select TOP 100 s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(int,ROUND(SiparisToplamTutari,0)) as SiparisToplamTutari,SiparisDurumu,
	case when f.AliciTelefon != '' then f.AliciTelefon else CepTelefonu end CepTelefonu,
	case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriNo, 
	case when (select COUNT(*) from Anket where UyeId = u.ID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
		when (select COUNT(*) from Anket where UyeId = u.ID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
	end as AnketSonucu, '' as EDevletVar
	,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = u.MusteriKodu ) as Vol_CURID,
	s.TeslimatAdresi, f.Il,f.Ilce,FaturaNo 
	from WebSiparis s
	left outer join Uye u on u.ID = s.UyeID
	left outer join FaturaAdres f on f.ID = s.FaturaAdresId
	where Durum not in(0,4,8,9)
*/
select TOP 100 s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
convert(int,ROUND(SiparisToplamTutari,0)) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
	end,'Anket Yok') as AnketSonucu,
case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
	end as AnketDurumu
--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
,FaturaAdresId
,s.UyeMusteriKodu as VOL_CURVAL,
case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
	end as AnketDurumu,u.MailIzin
from
WebSiparis s
left outer join Uye u on u.ID = s.UyeID
left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
outer apply(
select CURID,SALDATE,
case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
from VOLANT.VDB_YON01.dbo.CURRENTS c
inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
and SALID = s.FaturaNo
) volant
	where Durum not in(0,3,7,8,9)
	and EntegrasyonAktarildi = 1
	and s.ID = @sipID
	order by 1 desc option (fast 100)
end

--select * from SiparisDurumuDegiskenleri
GO
/****** Object:  StoredProcedure [dbo].[Siparis_Cek_Islenen2]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Siparis_Cek_Islenen2]
(
	@bsTarih	smalldatetime,
	@btTarih	smalldatetime,
	@sipID		int,
	@islenen	int
)
AS

if @islenen = 0
begin
	if @sipID = 0
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL
		from
		WebSiparis s
		left outer join Uye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where Durum in(2,5,16,17)
			and EntegrasyonAktarildi = 1
			and SiparisTarihi between @bsTarih and @btTarih
			--and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
			order by 1 desc option (fast 100)
	end
	else
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL
		from
		WebSiparis s
		left outer join Uye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where 
			--Durum in(2,5,16,17) and
			EntegrasyonAktarildi = 1
			and s.ID = @sipID
			order by 1 desc option (fast 100)
	end
end
if @islenen = 1
begin

	if @sipID = 0
	begin
	select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
	ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
	isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
		end,'Anket Yok') as AnketSonucu,
	case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu
	--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
	,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
	,FaturaAdresId
	,s.UyeMusteriKodu as VOL_CURVAL,
	u.MailIzin,DIVVAL as VOL_DIVVAL
	from
	WebSiparis s
	left outer join Uye u on u.ID = s.UyeID
	left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
	outer apply(
	select CURID,SALDATE,
	case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
	from VOLANT.VDB_YON01.dbo.CURRENTS c
	inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
	inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
	where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
	and SALID = s.FaturaNo
	) volant
		where Durum in (7,10)
		and EntegrasyonAktarildi = 1
		and SiparisTarihi between @bsTarih and @btTarih
		--and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL
		from
		WebSiparis s
		left outer join Uye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where Durum in (7,10)
			and EntegrasyonAktarildi = 1
			and s.ID = @sipID
			order by 1 desc option (fast 100)
	end
end
if @islenen = 2
begin

	if @sipID = 0
	begin
	select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
	ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,d.VolantAdi as SiparisDurumu ,FaturaNo,
	isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
		end,'Anket Yok') as AnketSonucu,
	case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu
	--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
	,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
	,FaturaAdresId
	,s.UyeMusteriKodu as VOL_CURVAL,
	u.MailIzin,DIVVAL as VOL_DIVVAL
	from
	WebSiparis s
	left outer join Uye u on u.ID = s.UyeID
	left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
	outer apply(
	select CURID,SALDATE,
	SALDIVISON + ' - ' + DIVNAME as DIVVAL
	from VOLANT.VDB_YON01.dbo.CURRENTS c
	inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
	inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
	where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
	and SALID = s.FaturaNo
	) volant
		where Durum in (4,6,11,12,13,14,15)
		and EntegrasyonAktarildi = 1
		and SiparisTarihi between @bsTarih and @btTarih
		--and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL
		from
		WebSiparis s
		left outer join Uye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON + ' - ' + DIVNAME as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where Durum in (4,6,11,12,13,14,15)
			and EntegrasyonAktarildi = 1
			and s.ID = @sipID
			order by 1 desc option (fast 100)
	end
end
--select * from SiparisDurumuDegiskenleri
GO
/****** Object:  StoredProcedure [dbo].[Siparis_Cek_Islenen3]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Siparis_Cek_Islenen3]
(
	@bsTarih	smalldatetime,
	@btTarih	smalldatetime,
	@sipID		int
)
AS

	if @sipID = 0
	begin
	select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
	convert(int,ROUND(SiparisToplamTutari,0)) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
	isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
		end,'Anket Yok') as AnketSonucu,
	case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu
	--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
	,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
	,FaturaAdresId
	,s.UyeMusteriKodu as VOL_CURVAL,
	case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu,u.MailIzin,DIVVAL as VOL_DIVVAL
	from
	TicimaxSiparis s
	left outer join TicimaxUye u on u.ID = s.UyeID
	left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
	outer apply(
	select CURID,SALDATE,
	case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
	from VOLANT.VDB_YON01.dbo.CURRENTS c
	inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
	inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
	where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
	and SALID = s.FaturaNo
	) volant
		where Durum in (20)
		--and EntegrasyonAktarildi = 1
		and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		convert(int,ROUND(SiparisToplamTutari,0)) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu,u.MailIzin,DIVVAL as VOL_DIVVAL
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where Durum in (20)
			--and EntegrasyonAktarildi = 1
			and s.ID = @sipID
			order by 1 desc option (fast 100)
	end
--select * from SiparisDurumuDegiskenleri
GO
/****** Object:  StoredProcedure [dbo].[Siparis_Cek_ilk]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Siparis_Cek_ilk]
(
	@bsTarih	smalldatetime,
	@btTarih smalldatetime
)
AS
select s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(numeric(12,2),SiparisToplamTutari) as SiparisToplamTutari,SiparisDurumu,
case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriHesapBeyani,
s.SiparisNotu,Mahalle, s.UyeMusteriKodu as CURVAL,
case when (select max([Not Tarihi]) from GorusmeNotu n where n.SiparisId = s.ID and u.ID = n.UyeId) is null then '0' else '1' end as Notlar
from WebSiparis s
left outer join Uye u on u.ID = s.UyeID
left outer join WebSiparisOdeme o on o.SiparisID = s.ID
where  Durum in(0,3,18,19)
and o.OdemeTipi in (2,3)
and siparisTarihi between @bsTarih and @btTarih
order by 1 desc
GO
/****** Object:  StoredProcedure [dbo].[Siparis_Cek_iptal]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Siparis_Cek_iptal]
(
	@bsTarih	smalldatetime,
	@btTarih smalldatetime
)
AS
select s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(numeric(12,2),SiparisToplamTutari) as SiparisToplamTutari,SiparisDurumu,
case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriHesapBeyani,
s.SiparisNotu
from WebSiparis s
left outer join Uye u on u.ID = s.UyeID
where  Durum in (8)
and siparisTarihi between @bsTarih and @btTarih
order by 1 desc
GO
/****** Object:  StoredProcedure [dbo].[Siparis_Cek_Pazaryeri_ilk]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[Siparis_Cek_Pazaryeri_ilk]
(
	@bsTarih	smalldatetime,
	@btTarih smalldatetime
)
AS
select s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(numeric(12,2),SiparisToplamTutari) as SiparisToplamTutari,Durum,SiparisDurumu,
case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriHesapBeyani,
s.SiparisNotu,Mahalle, s.UyeMusteriKodu as CURVAL,
case when (select max([Not Tarihi]) from GorusmeNotu n where n.SiparisId = s.ID and u.ID = n.UyeId) is null then '0' else '1' end as Notlar,
SiparisKaynagi
from WebSiparis s
left outer join Uye u on u.ID = s.UyeID
left outer join WebSiparisOdeme o on o.SiparisID = s.ID
where EntegrasyonAktarildi = 0
and SiparisKaynagi != 'TicimaxWeb'
and siparisTarihi between @bsTarih and @btTarih
order by 1 desc

GO
/****** Object:  StoredProcedure [dbo].[Siparis_Cek_Pazaryeri_ilk2]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[Siparis_Cek_Pazaryeri_ilk2]
(
	@bsTarih	smalldatetime,
	@btTarih smalldatetime
)
AS
select s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(numeric(12,2),SiparisToplamTutari) as SiparisToplamTutari,Durum,SiparisDurumu,
case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriHesapBeyani,
s.SiparisNotu,Mahalle, s.UyeMusteriKodu as CURVAL,
case when (select max([Not Tarihi]) from GorusmeNotu n where n.SiparisId = s.ID and u.ID = n.UyeId) is null then '0' else '1' end as Notlar,
SiparisKaynagi
from TicimaxSiparis s
left outer join TicimaxUye u on u.ID = s.UyeID
left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
where EntegrasyonAktarildi = 0
and SiparisKaynagi != 'TicimaxWeb'
and FaturaNo = ''
and siparisTarihi between @bsTarih and @btTarih
order by 1 desc

GO
/****** Object:  StoredProcedure [dbo].[Siparis_Cek_Pesin_Islenen]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Siparis_Cek_Pesin_Islenen]
(
	@bsTarih	smalldatetime,
	@btTarih	smalldatetime,
	@sipID		int,
	@islenen	int
)
AS

if @islenen = 0
begin
	if @sipID = 0
	begin
		select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL
		from WebSiparis s
		left outer join Uye u on u.ID = s.UyeID
		left outer join WebSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1
		and volant.DIVVAL = 'WB'
		and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
	select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL
		from WebSiparis s
		left outer join Uye u on u.ID = s.UyeID
		left outer join WebSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1
		and volant.DIVVAL = 'WB'
		and s.ID = @sipID
		order by 1 desc option (fast 100)
	end
end
if @islenen = 1
begin

	if @sipID = 0
	begin
	select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL
		from WebSiparis s
		left outer join Uye u on u.ID = s.UyeID
		left outer join WebSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		--and EntegrasyonAktarildi = 1
		and volant.DIVVAL != 'WB'
		and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
	select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL
		from WebSiparis s
		left outer join Uye u on u.ID = s.UyeID
		left outer join WebSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1
		and volant.DIVVAL != 'WB'
		and s.ID = @sipID
		order by 1 desc option (fast 100)
	end
end
if @islenen = 2
begin

	if @sipID = 0
	begin
	select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
	ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,d.VolantAdi as SiparisDurumu ,FaturaNo,
	isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
		end,'Anket Yok') as AnketSonucu,
	case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu
	--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
	,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
	,FaturaAdresId
	,s.UyeMusteriKodu as VOL_CURVAL,
	u.MailIzin,DIVVAL as VOL_DIVVAL
	from
	WebSiparis s
	left outer join Uye u on u.ID = s.UyeID
	left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
	outer apply(
	select CURID,SALDATE,
	SALDIVISON + ' - ' + DIVNAME as DIVVAL
	from VOLANT.VDB_YON01.dbo.CURRENTS c
	inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
	inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
	where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
	and SALID = s.FaturaNo
	) volant
		where Durum in(6,11,12,13,14,15,15,21)
		and EntegrasyonAktarildi = 1
		and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL
		from
		WebSiparis s
		left outer join Uye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON + ' - ' + DIVNAME as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where Durum in(6,11,12,13,14,15,15,21)
			and EntegrasyonAktarildi = 1
			and s.ID = @sipID
			order by 1 desc option (fast 100)
	end
end
--select * from SiparisDurumuDegiskenleri
GO
/****** Object:  StoredProcedure [dbo].[Siparis_Cek_Pesin_Islenen2]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Siparis_Cek_Pesin_Islenen2]
(
	@bsTarih	smalldatetime,
	@btTarih	smalldatetime,
	@sipID		int,
	@islenen	int
)
AS

if @islenen = 0
begin
	if @sipID = 0
	begin
		select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL
		from TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1
		and volant.DIVVAL = 'WB'
		and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
	select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL
		from TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1
		and volant.DIVVAL = 'WB'
		and s.ID = @sipID
		order by 1 desc option (fast 100)
	end
end
if @islenen = 1
begin

	if @sipID = 0
	begin
	select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL
		from TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		--and EntegrasyonAktarildi = 1
		and volant.DIVVAL != 'WB'
		and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
	select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL
		from TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1
		and volant.DIVVAL != 'WB'
		and s.ID = @sipID
		order by 1 desc option (fast 100)
	end
end
if @islenen = 2
begin

	if @sipID = 0
	begin
	select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
	ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,d.VolantAdi as SiparisDurumu ,FaturaNo,
	isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
		end,'Anket Yok') as AnketSonucu,
	case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu
	--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
	,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
	,FaturaAdresId
	,s.UyeMusteriKodu as VOL_CURVAL,
	u.MailIzin,DIVVAL as VOL_DIVVAL
	from
	TicimaxSiparis s
	left outer join TicimaxUye u on u.ID = s.UyeID
	left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
	outer apply(
	select CURID,SALDATE,
	SALDIVISON + ' - ' + DIVNAME as DIVVAL
	from VOLANT.VDB_YON01.dbo.CURRENTS c
	inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
	inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
	where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
	and SALID = s.FaturaNo
	) volant
		where Durum in(6,11,12,13,14,15,15,21)
		and EntegrasyonAktarildi = 1
		and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON + ' - ' + DIVNAME as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where Durum in(6,11,12,13,14,15,15,21)
			and EntegrasyonAktarildi = 1
			and s.ID = @sipID
			order by 1 desc option (fast 100)
	end
end
--select * from SiparisDurumuDegiskenleri
GO
/****** Object:  StoredProcedure [dbo].[Siparis_Cek_Pesin_ilk]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Siparis_Cek_Pesin_ilk]
(
	@bsTarih	smalldatetime,
	@btTarih smalldatetime
)
AS
select s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(numeric(12,2),SiparisToplamTutari) as SiparisToplamTutari,Durum,SiparisDurumu,
case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriHesapBeyani,
s.SiparisNotu,Mahalle, s.UyeMusteriKodu as CURVAL,
case when (select max([Not Tarihi]) from GorusmeNotu n where n.SiparisId = s.ID and u.ID = n.UyeId) is null then '0' else '1' end as Notlar,
SiparisKaynagi
from WebSiparis s
left outer join Uye u on u.ID = s.UyeID
left outer join WebSiparisOdeme o on o.SiparisID = s.ID
where  o.Onaylandi = 1 and KKOdemeBankaID != 0
and EntegrasyonAktarildi = 0
and siparisTarihi between @bsTarih and @btTarih
order by 1 desc

GO
/****** Object:  StoredProcedure [dbo].[Siparis_Cek_Pesin_ilk2]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[Siparis_Cek_Pesin_ilk2]
(
	@bsTarih	smalldatetime,
	@btTarih smalldatetime
)
AS
select s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(numeric(12,2),SiparisToplamTutari) as SiparisToplamTutari,Durum,SiparisDurumu,
case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriHesapBeyani,
s.SiparisNotu,Mahalle, s.UyeMusteriKodu as CURVAL,
case when (select max([Not Tarihi]) from GorusmeNotu n where n.SiparisId = s.ID and u.ID = n.UyeId) is null then '0' else '1' end as Notlar,
SiparisKaynagi
from TicimaxSiparis s
left outer join TicimaxUye u on u.ID = s.UyeID
left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
where  o.Onaylandi = 1 and KKOdemeBankaID != 0
and EntegrasyonAktarildi = 0
and FaturaNo = ''
and siparisTarihi between @bsTarih and @btTarih
order by 1 desc

GO
/****** Object:  StoredProcedure [dbo].[Siparis_Cek_Sonuc]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Siparis_Cek_Sonuc]
(
	@bsTarih	smalldatetime,
	@btTarih smalldatetime,
	@sipID		int
)
AS


if @sipID = 0
begin
/*
	select TOP 100 s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(int,ROUND(SiparisToplamTutari,0)) as SiparisToplamTutari,SiparisDurumu,
	case when f.AliciTelefon != '' then f.AliciTelefon else CepTelefonu end CepTelefonu,
	case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriNo, 
	case when (select COUNT(*) from Anket where UyeId = u.ID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
		when (select COUNT(*) from Anket where UyeId = u.ID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
	end as AnketSonucu, '' as EDevletVar,
	(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = u.MusteriKodu ) as Vol_CURID,
	s.TeslimatAdresi, f.Il,f.Ilce,FaturaNo 
	from WebSiparis s
	left outer join Uye u on u.ID = s.UyeID
	left outer join FaturaAdres f on f.ID = s.FaturaAdresId
	where Durum not in(0,4,8,9)
*/
select s.ID,UyeID as UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
	end,'Anket Yok') as AnketSonucu,
case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
	end as AnketDurumu,s.UyeMusteriKodu
,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
,FaturaAdresId,
case when u.MailIzin = '0' and (select [E-Devlet Durumu] from Anket where UyeId = s.UyeID and SiparisId = s.ID )  is null then 0 
when u.MailIzin = '0' and (select [E-Devlet Durumu] from Anket where UyeId = s.UyeID and SiparisId = s.ID ) = '0' then 0
when u.MailIzin = '0' and (select [E-Devlet Durumu] from Anket where UyeId = s.UyeID and SiparisId = s.ID ) = '1' then 2
else MailIzin
end as MailIzin
from 
WebSiparis s
left outer join Uye u on u.ID = s.UyeID
left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
	where Durum in (1) --in (1,4,10)--
	and EntegrasyonAktarildi = 0	
	and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
	--and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between '01/08/2023' and '31/12/2023'
	order by 1 desc option (fast 100)
end
else
begin
/*
	select TOP 100 s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(int,ROUND(SiparisToplamTutari,0)) as SiparisToplamTutari,SiparisDurumu,
	case when f.AliciTelefon != '' then f.AliciTelefon else CepTelefonu end CepTelefonu,
	case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriNo, 
	case when (select COUNT(*) from Anket where UyeId = u.ID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
		when (select COUNT(*) from Anket where UyeId = u.ID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
	end as AnketSonucu, '' as EDevletVar
	,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = u.MusteriKodu ) as Vol_CURID,
	s.TeslimatAdresi, f.Il,f.Ilce,FaturaNo 
	from WebSiparis s
	left outer join Uye u on u.ID = s.UyeID
	left outer join FaturaAdres f on f.ID = s.FaturaAdresId
	where Durum not in(0,4,8,9)
*/
select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
	end,'Anket Yok') as AnketSonucu,
case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
	end as AnketDurumu,s.UyeMusteriKodu
,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
,FaturaAdresId,
case when u.MailIzin = '0' and (select [E-Devlet Durumu] from Anket where UyeId = s.UyeID and SiparisId = s.ID )  is null then 0 
when u.MailIzin = '0' and (select [E-Devlet Durumu] from Anket where UyeId = s.UyeID and SiparisId = s.ID ) = '0' then 0
when u.MailIzin = '0' and (select [E-Devlet Durumu] from Anket where UyeId = s.UyeID and SiparisId = s.ID ) = '1' then 2
else MailIzin
end as MailIzin
from 
WebSiparis s
left outer join Uye u on u.ID = s.UyeID
left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
	where Durum not in (0,3,8,9,7,18,19) --in (1,4,10)--
	and EntegrasyonAktarildi = 0
	and s.ID = @sipID
	order by 1 desc option (fast 100)
end

--select * from SiparisDurumuDegiskenleri
GO
/****** Object:  StoredProcedure [dbo].[StokAciklama]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[StokAciklama]
(
	@id		int,
	@aciklama	varchar(max),
	@User		varchar(15),
	@ReturnDesc	varchar(500) output
)
as

	if exists(select * from VolantToTicimaxStokAciklama where VOL_PROID = @id)
	begin
	update VolantToTicimaxStokAciklama set TicimaxAciklama = @aciklama,Kaydeden = @User where VOL_PROID = @id
	set @ReturnDesc = 'Update'
	end
	else
	begin
	insert into VolantToTicimaxStokAciklama(VOL_PROID,TicimaxAciklama,Kaydeden) values (@id,@aciklama,@User)
	set @ReturnDesc = 'insert'
	end
GO
/****** Object:  StoredProcedure [dbo].[TeslimatAdres_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[TeslimatAdres_insert]
 (
		  @Adres nvarchar(255)
		 ,@AdresTarifi nvarchar(255)
		 ,@AliciAdi nvarchar(255)
		 ,@AliciTelefon nvarchar(255)
		 ,@ID int
		 ,@Il nvarchar(255)
		 ,@IlId int
		 ,@IlKodu nvarchar(255)
		 ,@Ilce nvarchar(255)
		 ,@IlceId int
		 ,@IlceKodu nvarchar(255)
		 ,@PostaKodu nvarchar(255)
		 ,@Ulke nvarchar(255)
		 ,@ReturnDesc  nvarchar(255) output
)
AS
if (select count(*) from TeslimatAdres where ID = @ID) = 0
begin
INSERT INTO [dbo].[TeslimatAdres]
           ([Adres]
           ,[AdresTarifi]
           ,[AliciAdi]
           ,[AliciTelefon]
           ,[ID]
           ,[Il]
           ,[IlId]
           ,[IlKodu]
           ,[Ilce]
           ,[IlceId]
           ,[IlceKodu]
           ,[PostaKodu]
           ,[Ulke])
     VALUES
           (@Adres
           ,@AdresTarifi
           ,@AliciAdi
           ,@AliciTelefon
           ,@ID
           ,@Il
           ,@IlId
           ,@IlKodu
           ,@Ilce
           ,@IlceId
           ,@IlceKodu
           ,@PostaKodu
           ,@Ulke)
	set @ReturnDesc = '1'
end
else
begin
	set @ReturnDesc = 'Kayit Var'
end
GO
/****** Object:  StoredProcedure [dbo].[TicimaxDurumGuncelle]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[TicimaxDurumGuncelle]
(
	@Durum	nvarchar(20),
	@sipID	int
)
AS
declare @Deger	int,
		@Vol_Adi	varchar(50)
if @Durum = '20'
begin	
	update s set Durum = 20,SiparisDurumu = 'Takipli Satis' from TicimaxSiparis s where s.ID = @sipID	
end
else if @Durum = '21'
begin
	update s set Durum = 21,SiparisDurumu = 'Kargo Alim Bekliyor' from TicimaxSiparis s where s.ID = @sipID	
end
else if @Durum = '1'
begin
	
	select @Deger=Deger, @Vol_Adi = VolantAdi from SiparisDurumuDegiskenleri where Adi = @Durum
	update s set Durum = @Deger,SiparisDurumu = @Vol_Adi,EntegrasyonAktarildi = 0, FaturaNo = '' from TicimaxSiparis s where s.ID = @sipID	
end
else
begin
select @Deger=Deger, @Vol_Adi = VolantAdi from SiparisDurumuDegiskenleri where Adi = @Durum


update s set Durum = @Deger,SiparisDurumu = @Vol_Adi from TicimaxSiparis s where s.ID = @sipID	
end
GO
/****** Object:  StoredProcedure [dbo].[TicimaxFaturaAdres_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[TicimaxFaturaAdres_insert]
(
			@Adres nvarchar(255)
           ,@AliciTelefon nvarchar(255)
           ,@EntegrasyonId nvarchar(255) = null
           ,@FirmaAdi nvarchar(255)
           ,@ID int
           ,@Il nvarchar(255)
           ,@IlId int
           ,@IlKodu nvarchar(255)
           ,@Ilce nvarchar(255)
           ,@IlceId int
           ,@IlceKodu nvarchar(255)
           ,@Ulke nvarchar(255)
           ,@VergiDairesi nvarchar(255)
           ,@VergiNo nvarchar(255)
           ,@isKurumsal bit
		   ,@ReturnDesc  nvarchar(255) output
)
AS
if (select COUNT(*) from TicimaxFaturaAdres where ID = @ID) = 0
begin
	INSERT INTO [dbo].[TicimaxFaturaAdres]
			   ([Adres]
			   ,[AliciTelefon]
			   ,[EntegrasyonId]
			   ,[FirmaAdi]
			   ,[ID]
			   ,[Il]
			   ,[IlId]
			   ,[IlKodu]
			   ,[Ilce]
			   ,[IlceId]
			   ,[IlceKodu]
			   ,[Ulke]
			   ,[VergiDairesi]
			   ,[VergiNo]
			   ,[isKurumsal])
		 VALUES
			   (@Adres
			   ,@AliciTelefon
			   ,@EntegrasyonId
			   ,@FirmaAdi
			   ,@ID
			   ,@Il
			   ,@IlId
			   ,@IlKodu
			   ,@Ilce
			   ,@IlceId
			   ,@IlceKodu
			   ,@Ulke
			   ,@VergiDairesi
			   ,@VergiNo
			   ,@isKurumsal)

	set @ReturnDesc = '1'
end
else
begin
	update TicimaxFaturaAdres set 
	Adres=@Adres,
	AliciTelefon=@AliciTelefon,
	EntegrasyonId=@EntegrasyonId,
	FirmaAdi=@FirmaAdi,
	Il=@Il,
	IlId=@IlId,
	IlKodu=@IlKodu,
	Ilce=@Ilce,
	IlceId=@IlceId,
	IlceKodu=@IlceKodu,
	Ulke=@Ulke,
	VergiDairesi=@VergiDairesi,
	VergiNo=@VergiNo,
	isKurumsal=@isKurumsal	
	where ID = @ID
	set @ReturnDesc = 'Fatura Adresi Güncellendi'
end
GO
/****** Object:  StoredProcedure [dbo].[TicimaxOdeme_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[TicimaxOdeme_insert]
(
			@BankaKomisyonu float
           ,@CheckSum nvarchar(255) = null
           ,@HavaleBankaID int
           ,@HavaleHesapID int
           ,@ID int
           ,@KKOdemeBankaID int
           ,@KapidaOdemeTutari float
           ,@OdemeIndirimi float
           ,@OdemeNotu nvarchar(255)
           ,@OdemeSecenekID int
           ,@OdemeTipi int
           ,@Onaylandi int
           ,@PosReferansID nvarchar(255)
           ,@SiparisID int
           ,@TaksitSayisi int
           ,@Tarih datetime
           ,@Tutar float
           ,@UyeID int
		   ,@ReturnDesc  nvarchar(255) output
)
AS
if (select count(*) from TicimaxSiparisOdeme where ID = @ID) = 0
begin
INSERT INTO [dbo].[TicimaxSiparisOdeme]
           ([BankaKomisyonu]
           ,[CheckSum]
           ,[HavaleBankaID]
           ,[HavaleHesapID]
           ,[ID]
           ,[KKOdemeBankaID]
           ,[KapidaOdemeTutari]
           ,[OdemeIndirimi]
           ,[OdemeNotu]
           ,[OdemeSecenekID]
           ,[OdemeTipi]
           ,[Onaylandi]
           ,[PosReferansID]
           ,[SiparisID]
           ,[TaksitSayisi]
           ,[Tarih]
           ,[Tutar]
           ,[UyeID])
     VALUES
           (@BankaKomisyonu
           ,@CheckSum
           ,@HavaleBankaID
           ,@HavaleHesapID
           ,@ID
           ,@KKOdemeBankaID
           ,@KapidaOdemeTutari
           ,@OdemeIndirimi
           ,@OdemeNotu
           ,@OdemeSecenekID
           ,@OdemeTipi
           ,@Onaylandi
           ,@PosReferansID
           ,@SiparisID
           ,@TaksitSayisi
           ,@Tarih
           ,@Tutar
           ,@UyeID)
	set @ReturnDesc = '1'
end
else
begin
	set @ReturnDesc = 'Ödeme Kaydi Var'
end
GO
/****** Object:  StoredProcedure [dbo].[TicimaxPazaryeri_ilk]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create Proc [dbo].[TicimaxPazaryeri_ilk]
(
	@bsTarih	smalldatetime,
	@btTarih smalldatetime
)
AS
select s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(numeric(12,2),SiparisToplamTutari) as SiparisToplamTutari,Durum,SiparisDurumu,
case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriHesapBeyani,
s.SiparisNotu,Mahalle, s.UyeMusteriKodu as CURVAL,
case when (select max([Not Tarihi]) from GorusmeNotu n where n.SiparisId = s.ID and u.ID = n.UyeId) is null then '0' else '1' end as Notlar,
SiparisKaynagi
from TicimaxSiparis s
left outer join TicimaxUye u on u.ID = s.UyeID
left outer join WebSiparisOdeme o on o.SiparisID = s.ID
where EntegrasyonAktarildi = 0
and SiparisKaynagi != 'TicimaxWeb'
and siparisTarihi between @bsTarih and @btTarih
order by 1 desc

GO
/****** Object:  StoredProcedure [dbo].[TicimaxPesin_Islenen]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[TicimaxPesin_Islenen]
(
	@bsTarih	smalldatetime,
	@btTarih	smalldatetime,
	@sipID		int,
	@islenen	int
)
AS

if @islenen = 0
begin
	if @sipID = 0
	begin
		select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL,t.*
		from TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		left outer join TicimaxKargoTakip t on t.SiparisID = s.ID
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1
		and volant.DIVVAL = 'WB'
		and SiparisTarihi between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
	select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL,t.*
		from TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		left outer join TicimaxKargoTakip t on t.SiparisID = s.ID
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1
		and volant.DIVVAL = 'WB'
		and s.ID = @sipID
		order by 1 desc option (fast 100)
	end
end
if @islenen = 1
begin

	if @sipID = 0
	begin
	select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL,t.*
		from TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		left outer join TicimaxKargoTakip t on t.SiparisID = s.ID
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1
		and volant.DIVVAL != 'WB'
		and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
	select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL,t.*
		from TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		left outer join TicimaxKargoTakip t on t.SiparisID = s.ID
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1		
		and volant.DIVVAL != 'WB'
		and s.ID = @sipID
		order by 1 desc option (fast 100)
	end
end
if @islenen = 2
begin

	if @sipID = 0
		begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,d.VolantAdi as SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL,t.*
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		left outer join TicimaxKargoTakip t on t.SiparisID = s.ID
		outer apply(
		select CURID,SALDATE,
		SALDIVISON + ' - ' + DIVNAME as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
		where Durum in(6,11,12,13,14,15,15,21)
		and EntegrasyonAktarildi = 1
		and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL,t.*
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		left outer join TicimaxKargoTakip t on t.SiparisID = s.ID
		outer apply(
		select CURID,SALDATE,
		SALDIVISON + ' - ' + DIVNAME as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where Durum in(6,11,12,13,14,15,15,21)
			and EntegrasyonAktarildi = 1
			and s.ID = @sipID
			order by 1 desc option (fast 100)
	end
end
--select * from SiparisDurumuDegiskenleri
GO
/****** Object:  StoredProcedure [dbo].[TicimaxPesin_ilk]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[TicimaxPesin_ilk]
(
	@bsTarih	smalldatetime,
	@btTarih	smalldatetime,
	@OdemeTipi	int
)
AS
select s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(numeric(12,2),SiparisToplamTutari) as SiparisToplamTutari,Durum,SiparisDurumu,
case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriHesapBeyani,
s.SiparisNotu,Mahalle, s.UyeMusteriKodu as CURVAL,VergiNo,
case when (select max([Not Tarihi]) from GorusmeNotu n where n.SiparisId = s.ID and u.ID = n.UyeId) is null then '0' else '1' end as Notlar,
SiparisKaynagi
from TicimaxSiparis s
left outer join TicimaxUye u on u.ID = s.UyeID
left outer join TicimaxFaturaAdres f on f.ID = s.FaturaAdresId
left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
where  o.Onaylandi = 1 and KKOdemeBankaID != 0
and EntegrasyonAktarildi = 0
and Durum = 0
and o.OdemeTipi = @OdemeTipi
and siparisTarihi between @bsTarih and @btTarih
order by 1 desc
GO
/****** Object:  StoredProcedure [dbo].[TicimaxPesin_Kargo]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[TicimaxPesin_Kargo]
(
	@bsTarih	smalldatetime,
	@btTarih	smalldatetime,
	@sipID		int,
	@islenen	int
)
AS

if @islenen = 0
begin
	if @sipID = 0
	begin
		select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL
		from TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1
		and Durum = 6
		and volant.DIVVAL = 'WB'
		--and SiparisTarihi between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
	select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL
		from TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1
		and volant.DIVVAL = 'WB'
		and Durum = 6
		and s.ID = @sipID
		order by 1 desc option (fast 100)
	end
end
if @islenen = 1
begin

	if @sipID = 0
	begin
	select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL
		from TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1
		and volant.DIVVAL != 'WB'
		and Durum = 6
		and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
	select s.ID,s.UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,s.Durum as Durum,SiparisDurumu ,FaturaNo
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,
		DIVVAL + ' - ' + DIVNAME as VOL_DIVVAL
		from TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join TicimaxSiparisOdeme o on o.SiparisID = s.ID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON as DIVVAL, DIVNAME
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),s.UyeID )
		and SALID = s.FaturaNo 
		) volant
		where  o.Onaylandi = 1 and KKOdemeBankaID != 0
		and EntegrasyonAktarildi = 1
		and volant.DIVVAL != 'WB'
		and Durum = 6
		and s.ID = @sipID
		order by 1 desc option (fast 100)
	end
end
if @islenen = 2
begin

	if @sipID = 0
	begin
	select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
	ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,d.VolantAdi as SiparisDurumu ,FaturaNo,
	isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
		end,'Anket Yok') as AnketSonucu,
	case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu
	--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
	,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
	,FaturaAdresId
	,s.UyeMusteriKodu as VOL_CURVAL,
	u.MailIzin,DIVVAL as VOL_DIVVAL
	from
	TicimaxSiparis s
	left outer join TicimaxUye u on u.ID = s.UyeID
	left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
	outer apply(
	select CURID,SALDATE,
	SALDIVISON + ' - ' + DIVNAME as DIVVAL
	from VOLANT.VDB_YON01.dbo.CURRENTS c
	inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
	inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
	where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
	and SALID = s.FaturaNo
	) volant
		where Durum in(6,11,12,13,14,15,15,21)
		and EntegrasyonAktarildi = 1
		and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON + ' - ' + DIVNAME as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where Durum in(6,11,12,13,14,15,15,21)
			and EntegrasyonAktarildi = 1
			and s.ID = @sipID
			order by 1 desc option (fast 100)
	end
end
--select * from SiparisDurumuDegiskenleri
GO
/****** Object:  StoredProcedure [dbo].[TicimaxPuan_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[TicimaxPuan_insert]
(
		@KartNumarasi nvarchar(20),
		@KazanilanPuan float,
		@KazanilanPuanTLKarsiligi float,
		@KullanilanPuan float,
		@KullanilanPuanTLKarsiligi float,
		@KullanimTarihi datetime,
		@PuanBildirildi bit,
		@Tip int,
		@ReturnDesc  nvarchar(255) output
)
AS
if (select count(*) from TicimaxPuanKullanim where KartNumarasi = @KartNumarasi and KullanimTarihi = @KullanimTarihi) = 0
begin
INSERT INTO [dbo].[TicimaxPuanKullanim]
			([KazanilanPuanID],
			[KartNumarasi],
			[KazanilanPuan],
			[KazanilanPuanTLKarsiligi],	
			[KullanilanPuan],
			[KullanilanPuanTLKarsiligi], 
			[KullanimTarihi],
			[PuanBildirildi],
			[Tip])
     VALUES
(			(select count(*) from TicimaxPuanKullanim)+1,
			@KartNumarasi,
			@KazanilanPuan,
			@KazanilanPuanTLKarsiligi,	
			@KullanilanPuan,
			@KullanilanPuanTLKarsiligi, 
			@KullanimTarihi,
			@PuanBildirildi,
			@Tip
)
	set @ReturnDesc = (select count(*) from TicimaxPuanKullanim)+1
end
else
begin
	set @ReturnDesc = (select KazanilanPuanID from TicimaxPuanKullanim where KartNumarasi = @KartNumarasi and KullanimTarihi = @KullanimTarihi)
end
GO
/****** Object:  StoredProcedure [dbo].[TicimaxSiparis_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[TicimaxSiparis_insert]
(
	@AdiSoyadi nvarchar(255),
	@Durum int,
	@EntegrasyonAktarildi bit,
	@FaturaAdresId int,
	@FaturaAdresi nvarchar(255),
	@FaturaNo nvarchar(255),
	@FaturaTarihi datetime,
	@HediyeCeki nvarchar(255),
	@HediyeCekiTutari float,
	@HadiyePaketiNotu nvarchar(255),
	@HediyePaketiTutari float,
	@HediyePaketiVar bit,
	@ID int,
	@IPAdresi nvarchar(255),
	@IndirimTutari float,
	@KampanyaID	int,
	@KargoAdresID int,
	@KargoEntegrasyonID int,
	@KargoEntegrasyonTakipNo nvarchar(255),
	@KargoFirmaId int,
	@KargoFirmaTanim nvarchar(255),
	@KargoTakipNo nvarchar(255),
	@KargoTutari float,
	@Kaynak int,
	@Kur float,
	@Mail nvarchar(255),
	@Maliyet float,
	@MarketplaceKampanyaKodu nvarchar(255),
	@Odemeler nvarchar(255),
	@OlusturanId int,
	@OzelAlan1 nvarchar(255),
	@OzelAlan2 nvarchar(255),
	@OzelAlan3 nvarchar(255),
	@OzellestirmeTutari float,
	@PaketlemeDurumu nvarchar(255),
	@PaketlemeDurumuID int,
	@ParaBirimi nvarchar(255),
	@PuanIndirimi  nvarchar(255),
	@PuanKullanimID int,
	@Referer nvarchar(255),
	@ReklamKaynagi nvarchar(255),
	@SepetKampanyasiIndirimi float,
	@SiparisDurumu nvarchar(255),
	@SiparisKaynagi nvarchar(255),
	@SiparisKodu nvarchar(255),
	@SiparisNo nvarchar(255),
	@SiparisNotu nvarchar(255),
	@SiparisTarihi datetime,
	@SiparisToplamTutari float,
	@StokDustu bit,
	@TeslimatAdresiId int,
	@TeslimatAdresi nvarchar(255),
	@TeslimatGunu datetime,
	@TeslimatSaati nvarchar(255),
	@ToplamKdv float,
	@ToplamTutar float,
	@Tutar float,
	@Urunler nvarchar(255),
	@UyeAdi nvarchar(255),
	@UyeID int,
	@UyeMusteriKodu nvarchar(255),
	@UyeSoyadi nvarchar(255),
	@ReturnDesc  nvarchar(255) output
)
AS
if (select count(*) from TicimaxSiparis where ID = @ID) = 0
begin
INSERT INTO [dbo].[TicimaxSiparis]
           ([AdiSoyadi]
		  ,[Durum]
		  ,[EntegrasyonAktarildi]
		  ,[FaturaAdresId]
		  ,[FaturaAdresi]
		  ,[FaturaNo]
		  ,[FaturaTarihi]
		  ,[HediyeCeki]
		  ,[HediyeCekiTutari]
		  ,[HadiyePaketiNotu]
		  ,[HediyePaketiTutari]
		  ,[HediyePaketiVar]
		  ,[ID]
		  ,[IPAdresi]
		  ,[IndirimTutari]
		  ,[KampanyaID]
		  ,[KargoAdresID]
		  ,[KargoEntegrasyonID]
		  ,[KargoEntegrasyonTakipNo]
		  ,[KargoFirmaId]
		  ,[KargoFirmaTanim]
		  ,[KargoTakipNo]
		  ,[KargoTutari]
		  ,[Kaynak]
		  ,[Kur]
		  ,[Mail]
		  ,[Maliyet]
		  ,[MarketplaceKampanyaKodu]
		  ,[Odemeler]
		  ,[OlusturanId]
		  ,[OzelAlan1]
		  ,[OzelAlan2]
		  ,[OzelAlan3]
		  ,[OzellestirmeTutari]
		  ,[PaketlemeDurumu]
		  ,[PaketlemeDurumuID]
		  ,[ParaBirimi]
		  ,[PuanIndirimi]
		  ,[PuanKullanimID]
		  ,[Referer]
		  ,[ReklamKaynagi]
		  ,[SepetKampanyasiIndirimi]
		  ,[SiparisDurumu]
		  ,[SiparisKaynagi]
		  ,[SiparisKodu]
		  ,[SiparisNo]
		  ,[SiparisNotu]
		  ,[SiparisTarihi]
		  ,[SiparisToplamTutari]
		  ,[StokDustu]
		  ,[TeslimatAdresiId]
		  ,[TeslimatAdresi]
		  ,[TeslimatGunu]
		  ,[TeslimatSaati]
		  ,[ToplamKdv]
		  ,[ToplamTutar]
		  ,[Tutar]
		  ,[Urunler]
		  ,[UyeAdi]
		  ,[UyeID]
		  ,[UyeMusteriKodu]
		  ,[UyeSoyadi])
     VALUES
           (@AdiSoyadi
			,@Durum
			,@EntegrasyonAktarildi
			,@FaturaAdresId
			,@FaturaAdresi
			,@FaturaNo
			,@FaturaTarihi
			,@HediyeCeki
			,@HediyeCekiTutari
			,@HadiyePaketiNotu
			,@HediyePaketiTutari
			,@HediyePaketiVar
			,@ID
			,@IPAdresi
			,@IndirimTutari
			,@KampanyaID
			,@KargoAdresID
			,@KargoEntegrasyonID
			,@KargoEntegrasyonTakipNo
			,@KargoFirmaId
			,@KargoFirmaTanim
			,@KargoTakipNo
			,@KargoTutari
			,@Kaynak
			,@Kur
			,@Mail
			,@Maliyet
			,@MarketplaceKampanyaKodu
			,@Odemeler
			,@OlusturanId
			,@OzelAlan1
			,@OzelAlan2
			,@OzelAlan3
			,@OzellestirmeTutari
			,@PaketlemeDurumu
			,@PaketlemeDurumuID
			,@ParaBirimi
			,@PuanIndirimi
			,@PuanKullanimID
			,@Referer
			,@ReklamKaynagi
			,@SepetKampanyasiIndirimi
			,@SiparisDurumu
			,@SiparisKaynagi
			,@SiparisKodu
			,@SiparisNo
			,@SiparisNotu
			,@SiparisTarihi
			,@SiparisToplamTutari
			,@StokDustu
			,@TeslimatAdresiId
			,@TeslimatAdresi
			,@TeslimatGunu
			,@TeslimatSaati
			,@ToplamKdv
			,@ToplamTutar
			,@Tutar
			,@Urunler
			,@UyeAdi
			,@UyeID
			,@UyeMusteriKodu
			,@UyeSoyadi)
	set @ReturnDesc = '1'
end
else
begin
	--update WebSiparis set TeslimatAdresi = @TeslimatAdresi,FaturaAdresi = @FaturaAdresi, Durum = @Durum, SiparisDurumu = @SiparisDurumu where ID = @ID
	set @ReturnDesc = 'Siparis Daha Önce Eklenmis'
end
GO
/****** Object:  StoredProcedure [dbo].[TicimaxTaksitli_ilk]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[TicimaxTaksitli_ilk]
(
	@bsTarih	smalldatetime,
	@btTarih smalldatetime
)
AS
select s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(numeric(12,2),SiparisToplamTutari) as SiparisToplamTutari,SiparisDurumu,
case when MusteriKodu ='' then 'Yok' else 'WEB-'+cast(s.UyeID as char(7)) end as MusteriHesapBeyani,
s.SiparisNotu,Mahalle, s.UyeMusteriKodu as CURVAL,
case when (select max([Not Tarihi]) from GorusmeNotu n where n.SiparisId = s.ID and u.ID = n.UyeId) is null then '0' else '1' end as Notlar
from TicimaxSiparis s
left outer join TicimaxUye u on u.ID = s.UyeID
left outer join WebSiparisOdeme o on o.SiparisID = s.ID
where  Durum in(0,3,18,19)
and o.OdemeTipi in (2,3)
and siparisTarihi between @bsTarih and @btTarih
order by 1 desc
GO
/****** Object:  StoredProcedure [dbo].[TicimaxTaksitli_iptal]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[TicimaxTaksitli_iptal]
(
	@bsTarih	smalldatetime,
	@btTarih smalldatetime
)
AS
select s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(numeric(12,2),SiparisToplamTutari) as SiparisToplamTutari,SiparisDurumu,
case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriHesapBeyani,
s.SiparisNotu,i.IptalAciklamasi,i.IptalNedeni,case when EntegrasyonAktarildi = 0 and FaturaNo = ''  then 'Volantta YOK'
 when EntegrasyonAktarildi = 1 and FaturaNo = '' then 'Satis isleme Tamamlanmamis' else 'SATIS ISLENMIS' end as Durum
from TicimaxSiparis s
left outer join TicimaxUye u on u.ID = s.UyeID
left outer join WebSiparisIptalDurum i on i.SipID = s.ID
where  Durum in (8)
and siparisTarihi between @bsTarih and @btTarih
order by 1 desc
GO
/****** Object:  StoredProcedure [dbo].[TicimaxTaksitli_Sonuc]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[TicimaxTaksitli_Sonuc]
(
	@bsTarih	smalldatetime,
	@btTarih smalldatetime,
	@sipID		int
)
AS


if @sipID = 0
begin
/*
	select TOP 100 s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(int,ROUND(SiparisToplamTutari,0)) as SiparisToplamTutari,SiparisDurumu,
	case when f.AliciTelefon != '' then f.AliciTelefon else CepTelefonu end CepTelefonu,
	case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriNo, 
	case when (select COUNT(*) from Anket where UyeId = u.ID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
		when (select COUNT(*) from Anket where UyeId = u.ID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
	end as AnketSonucu, '' as EDevletVar,
	(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = u.MusteriKodu ) as Vol_CURID,
	s.TeslimatAdresi, f.Il,f.Ilce,FaturaNo 
	from TicimaxSiparis s
	left outer joinTicimaxUyeu on u.ID = s.UyeID
	left outer join FaturaAdres f on f.ID = s.FaturaAdresId
	where Durum not in(0,4,8,9)
*/
select s.ID,UyeID as UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
	end,'Anket Yok') as AnketSonucu,
case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
	end as AnketDurumu,s.UyeMusteriKodu
,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
,FaturaAdresId,
case when u.MailIzin = '0' and (select [E-Devlet Durumu] from Anket where UyeId = s.UyeID and SiparisId = s.ID )  is null then 0 
when u.MailIzin = '0' and (select [E-Devlet Durumu] from Anket where UyeId = s.UyeID and SiparisId = s.ID ) = '0' then 0
when u.MailIzin = '0' and (select [E-Devlet Durumu] from Anket where UyeId = s.UyeID and SiparisId = s.ID ) = '1' then 2
else MailIzin
end as MailIzin
from 
TicimaxSiparis s
left outer join TicimaxUye u on u.ID = s.UyeID
left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
	where Durum in (1) --in (1,4,10)--
	and EntegrasyonAktarildi = 0	
	and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
	--and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between '01/08/2023' and '31/12/2024'
	order by 1 desc option (fast 100)
end
else
begin
/*
	select TOP 100 s.ID,s.UyeID,SiparisTarihi,Upper(AdiSoyadi) as AdiSoyadi,convert(int,ROUND(SiparisToplamTutari,0)) as SiparisToplamTutari,SiparisDurumu,
	case when f.AliciTelefon != '' then f.AliciTelefon else CepTelefonu end CepTelefonu,
	case when MusteriKodu='' then 'Yok' else MusteriKodu end as MusteriNo, 
	case when (select COUNT(*) from Anket where UyeId = u.ID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
		when (select COUNT(*) from Anket where UyeId = u.ID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
	end as AnketSonucu, '' as EDevletVar
	,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = u.MusteriKodu ) as Vol_CURID,
	s.TeslimatAdresi, f.Il,f.Ilce,FaturaNo 
	from TicimaxSiparis s
	left outer joinTicimaxUyeu on u.ID = s.UyeID
	left outer join FaturaAdres f on f.ID = s.FaturaAdresId
	where Durum not in(0,4,8,9)
*/
select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
	end,'Anket Yok') as AnketSonucu,
case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
		when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
	end as AnketDurumu,s.UyeMusteriKodu
,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
,FaturaAdresId,
case when u.MailIzin = '0' and (select [E-Devlet Durumu] from Anket where UyeId = s.UyeID and SiparisId = s.ID )  is null then 0 
when u.MailIzin = '0' and (select [E-Devlet Durumu] from Anket where UyeId = s.UyeID and SiparisId = s.ID ) = '0' then 0
when u.MailIzin = '0' and (select [E-Devlet Durumu] from Anket where UyeId = s.UyeID and SiparisId = s.ID ) = '1' then 2
else MailIzin
end as MailIzin
from 
TicimaxSiparis s
left outer join TicimaxUye u on u.ID = s.UyeID
left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
	where Durum not in (0,3,8,9,7,18,19) --in (1,4,10)--
	and EntegrasyonAktarildi = 0
	and s.ID = @sipID
	order by 1 desc option (fast 100)
end

--select * from SiparisDurumuDegiskenleri
GO
/****** Object:  StoredProcedure [dbo].[TicimaxTaksitli_TeslimatBekleyen]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[TicimaxTaksitli_TeslimatBekleyen]
(
	@bsTarih	smalldatetime,
	@btTarih	smalldatetime,
	@sipID		int,
	@islenen	int
)
AS

if @islenen = 0
begin
	if @sipID = 0
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu,
		(select top 1 CURID from VOLANT.VDB_YON01.dbo.CURRENTS with (nolock) where CURVAL = UyeMusteriKodu) as Vol_CURID,
		volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL,
		case when (select count(SALID) from VOLANT.VDB_YON01.dbo.SALES where SALCANSALID = s.FaturaNo) = 1 then 'Iptal Olmus Degisim Kontol Et' else
		case when sum(ORDCHBALANCEQUAN) != 0 then Cast(sum(ORDCHBALANCEQUAN) as varchar(10)) else 'Teslim Olmus' end end as ORDCHBALANCEQUAN
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select SALDATE,
		case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL,ORDCHBALANCEQUAN
		from VOLANT.VDB_YON01.dbo.SALES
		inner join VOLANT.VDB_YON01.dbo.ORDERS on ORDSALID = SALID
		inner join VOLANT.VDB_YON01.dbo.ORDERSCHILD on ORDCHORDID = ORDID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where SALID = s.FaturaNo
		) volant
			where Durum in(2,5,16,17)
			and EntegrasyonAktarildi = 1
			--and SiparisTarihi between @bsTarih and @btTarih
			--and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
			group by 
			s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi),
		SiparisToplamTutari ,d.Adi,SiparisDurumu ,FaturaNo, SALDATE,FaturaAdresId,s.UyeMusteriKodu,u.MailIzin,DIVVAL
			order by 1 desc 
			option (fast 100)
	end
	else
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu,
		(select top 1 CURID from VOLANT.VDB_YON01.dbo.CURRENTS with (nolock) where CURVAL = UyeMusteriKodu) as Vol_CURID,
		volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL,
		case when (select count(SALID) from VOLANT.VDB_YON01.dbo.SALES where SALCANSALID = s.FaturaNo) = 1 then 'Iptal Olmus Degisim Kontol Et' else
		case when sum(ORDCHBALANCEQUAN) != 0 then Cast(sum(ORDCHBALANCEQUAN) as varchar(10)) else 'Teslim Olmus' end end as ORDCHBALANCEQUAN
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select SALDATE,
		case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL,ORDCHBALANCEQUAN
		from VOLANT.VDB_YON01.dbo.SALES
		inner join VOLANT.VDB_YON01.dbo.ORDERS on ORDSALID = SALID
		inner join VOLANT.VDB_YON01.dbo.ORDERSCHILD on ORDCHORDID = ORDID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where SALID = s.FaturaNo
		) volant
			where Durum in(2,5,16,17)
			and EntegrasyonAktarildi = 1
			and s.ID = @sipID
			--and SiparisTarihi between @bsTarih and @btTarih
			--and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
			group by 
			s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi),
		SiparisToplamTutari ,d.Adi,SiparisDurumu ,FaturaNo, SALDATE,FaturaAdresId,s.UyeMusteriKodu,u.MailIzin,DIVVAL
			order by 1 desc 
			option (fast 100)
	end
end
if @islenen = 1
begin

	if @sipID = 0
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu,
		(select top 1 CURID from VOLANT.VDB_YON01.dbo.CURRENTS with (nolock) where CURVAL = UyeMusteriKodu) as Vol_CURID,
		volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL,
		case when (select count(SALID) from VOLANT.VDB_YON01.dbo.SALES where SALCANSALID = s.FaturaNo) = 1 then 'Iptal Olmus Degisim mi Kontol Et' else 'Satista'	end as ORDCHBALANCEQUAN	
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select SALDATE,
		case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
		from VOLANT.VDB_YON01.dbo.SALES
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where SALID = s.FaturaNo
		) volant
		where Durum in (7,10)
		and EntegrasyonAktarildi = 1
		and SiparisTarihi between @bsTarih and @btTarih
		--and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
			group by 
			s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi),
		SiparisToplamTutari ,d.Adi,SiparisDurumu ,FaturaNo, SALDATE,FaturaAdresId,s.UyeMusteriKodu,u.MailIzin,DIVVAL
		order by 1 desc option (fast 100)
	end
	else
	begin
		
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu,
		(select top 1 CURID from VOLANT.VDB_YON01.dbo.CURRENTS with (nolock) where CURVAL = UyeMusteriKodu) as Vol_CURID,
		volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL,
		case when (select count(SALID) from VOLANT.VDB_YON01.dbo.SALES where SALCANSALID = s.FaturaNo) = 1 then 'Iptal Olmus Degisim mi Kontol Et' else 'Satista'	end as ORDCHBALANCEQUAN	
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select SALDATE,
		case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
		from VOLANT.VDB_YON01.dbo.SALES
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where SALID = s.FaturaNo
		) volant
		where Durum in (7,10)
		and EntegrasyonAktarildi = 1
		and SiparisTarihi between @bsTarih and @btTarih		
		and s.ID = @sipID
		--and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
			group by 
			s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi),
		SiparisToplamTutari ,d.Adi,SiparisDurumu ,FaturaNo, SALDATE,FaturaAdresId,s.UyeMusteriKodu,u.MailIzin,DIVVAL
		order by 1 desc option (fast 100)
	end
end
if @islenen = 2
begin
	if @sipID = 0
	begin
	select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
	ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,d.VolantAdi as SiparisDurumu ,FaturaNo,
	isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
		end,'Anket Yok') as AnketSonucu,
	case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu
	--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
	,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
	,FaturaAdresId
	,s.UyeMusteriKodu as VOL_CURVAL,
	u.MailIzin,DIVVAL as VOL_DIVVAL
	from
	TicimaxSiparis s
	left outer join TicimaxUye u on u.ID = s.UyeID
	left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
	outer apply(
	select CURID,SALDATE,
	SALDIVISON + ' - ' + DIVNAME as DIVVAL
	from VOLANT.VDB_YON01.dbo.CURRENTS c
	inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
	inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
	where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
	and SALID = s.FaturaNo
	) volant
		where Durum in (4,6,11,12,13,14,15)
		and EntegrasyonAktarildi = 1
		--and SiparisTarihi between @bsTarih and @btTarih
		--and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON + ' - ' + DIVNAME as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where Durum in (4,6,11,12,13,14,15)
			and EntegrasyonAktarildi = 1
			and s.ID = @sipID
			order by 1 desc option (fast 100)
	end
end
--select * from SiparisDurumuDegiskenleri



GO
/****** Object:  StoredProcedure [dbo].[TicimaxTaksitli_TeslimOlanlar]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[TicimaxTaksitli_TeslimOlanlar]
(
	@bsTarih	smalldatetime,
	@btTarih	smalldatetime,
	@sipID		int,
	@islenen	int
)
AS

if @islenen = 0
begin
	if @sipID = 0
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where Durum in(2,5,16,17)
			--and EntegrasyonAktarildi = 1
			and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
			order by 1 desc option (fast 100)
	end
	else
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where 
			--Durum in(2,5,16,17) and
			EntegrasyonAktarildi = 1
			and s.ID = 20292
			order by 1 desc option (fast 100)
	end
end
if @islenen = 1
begin

	if @sipID = 0
	begin
	select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
	ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
	isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
		end,'Anket Yok') as AnketSonucu,
	case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu
	--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
	,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
	,FaturaAdresId
	,s.UyeMusteriKodu as VOL_CURVAL,
	u.MailIzin,DIVVAL as VOL_DIVVAL
	from
	TicimaxSiparis s
	left outer join TicimaxUye u on u.ID = s.UyeID
	left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
	outer apply(
	select CURID,SALDATE,
	case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
	from VOLANT.VDB_YON01.dbo.CURRENTS c
	inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
	inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
	where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
	and SALID = s.FaturaNo
	) volant
		where Durum in (7,10)
		and EntegrasyonAktarildi = 1
		and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		case when SALDIVISON = 'WB' then 'Atanammais' else SALDIVISON + ' - ' + DIVNAME end as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where Durum in (7,10)
			and EntegrasyonAktarildi = 1
			and s.ID = @sipID
			order by 1 desc option (fast 100)
	end
end
if @islenen = 2
begin

	if @sipID = 0
	begin
	select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
	ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,d.VolantAdi as SiparisDurumu ,FaturaNo,
	isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
		end,'Anket Yok') as AnketSonucu,
	case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
			when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
		end as AnketDurumu
	--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
	,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
	,FaturaAdresId
	,s.UyeMusteriKodu as VOL_CURVAL,
	u.MailIzin,DIVVAL as VOL_DIVVAL
	from
	TicimaxSiparis s
	left outer join TicimaxUye u on u.ID = s.UyeID
	left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
	outer apply(
	select CURID,SALDATE,
	SALDIVISON + ' - ' + DIVNAME as DIVVAL
	from VOLANT.VDB_YON01.dbo.CURRENTS c
	inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
	inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
	where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
	and SALID = s.FaturaNo
	) volant
		where Durum in (4,6,11,12,13,14,15)
		and EntegrasyonAktarildi = 1
		and convert(smalldatetime,CONVERT(char(10),SiparisTarihi,103)) between @bsTarih and @btTarih
		order by 1 desc option (fast 100)
	end
	else
	begin
		select s.ID,UyeID,SiparisTarihi,upper(AdiSoyadi) as AdiSoyadi,
		ROUND(SiparisToplamTutari,2) as SiparisToplamTutari,d.Adi as Durum,SiparisDurumu ,FaturaNo,
		isnull(case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then 'Bu Siparise Anket Yapilmis.'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then 'Müsteri Geçmis Anketi Var Sadece'
			end,'Anket Yok') as AnketSonucu,
		case when (select COUNT(*) from Anket where UyeId = s.UyeID and SiparisId = s.ID)  = 1 then '0'
				when (select COUNT(*) from Anket where UyeId = s.UyeID)  = 1 then '1'
			end as AnketDurumu
		--,(select CURID from VOLANT.VDB_YON01.dbo.CURRENTS where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )) as Vol_CURID
		,volant.CURID as Vol_CURID,volant.SALDATE as Vol_SALDATE
		,FaturaAdresId
		,s.UyeMusteriKodu as VOL_CURVAL,
		u.MailIzin,DIVVAL as VOL_DIVVAL
		from
		TicimaxSiparis s
		left outer join TicimaxUye u on u.ID = s.UyeID
		left outer join SiparisDurumuDegiskenleri d on d.Deger = s.Durum
		outer apply(
		select CURID,SALDATE,
		SALDIVISON + ' - ' + DIVNAME as DIVVAL
		from VOLANT.VDB_YON01.dbo.CURRENTS c
		inner join VOLANT.VDB_YON01.dbo.SALES i on i.SALCURID = c.CURID
		inner join VOLANT.VDB_YON01.dbo.DIVISON on DIVVAL = SALDIVISON
		where CURUSEFIELD3 = 'WEB-'+ convert(char(10),UyeID )
		and SALID = s.FaturaNo
		) volant
			where Durum in (4,6,11,12,13,14,15)
			and EntegrasyonAktarildi = 1
			and s.ID = @sipID
			order by 1 desc option (fast 100)
	end
end
--select * from SiparisDurumuDegiskenleri
GO
/****** Object:  StoredProcedure [dbo].[TicimaxTeslimatAdres_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[TicimaxTeslimatAdres_insert]
 (
		  @Adres nvarchar(255)
		 ,@AdresTarifi nvarchar(255)
		 ,@AliciAdi nvarchar(255)
		 ,@AliciTelefon nvarchar(255)
		 ,@ID int
		 ,@Il nvarchar(255)
		 ,@IlId int
		 ,@IlKodu nvarchar(255)
		 ,@Ilce nvarchar(255)
		 ,@IlceId int
		 ,@IlceKodu nvarchar(255)
		 ,@PostaKodu nvarchar(255)
		 ,@Ulke nvarchar(255)
		 ,@ReturnDesc  nvarchar(255) output
)
AS
if (select count(*) from TicimaxTeslimatAdres where ID = @ID) = 0
begin
INSERT INTO [dbo].[TicimaxTeslimatAdres]
           ([Adres]
           ,[AdresTarifi]
           ,[AliciAdi]
           ,[AliciTelefon]
           ,[ID]
           ,[Il]
           ,[IlId]
           ,[IlKodu]
           ,[Ilce]
           ,[IlceId]
           ,[IlceKodu]
           ,[PostaKodu]
           ,[Ulke])
     VALUES
           (@Adres
           ,@AdresTarifi
           ,@AliciAdi
           ,@AliciTelefon
           ,@ID
           ,@Il
           ,@IlId
           ,@IlKodu
           ,@Ilce
           ,@IlceId
           ,@IlceKodu
           ,@PostaKodu
           ,@Ulke)
	set @ReturnDesc = '1'
end
else
begin
	update TicimaxTeslimatAdres set 
	 Adres=@Adres
	,AdresTarifi=@AdresTarifi
	,AliciAdi=@AliciAdi
	,AliciTelefon=@AliciTelefon
	,ID=@ID
	,Il=@Il
	,IlId=@IlId
	,IlKodu=@IlKodu
	,Ilce=@Ilce
	,IlceId=@IlceId
	,IlceKodu=@IlceKodu
	,PostaKodu=@PostaKodu
	,Ulke=@Ulke
	where ID = @ID
	set @ReturnDesc = 'Teslimat Adresi Güncellendi'
end
GO
/****** Object:  StoredProcedure [dbo].[TicimaxUrun_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[TicimaxUrun_insert]
(
  @Adet float
 ,@Barkod nvarchar(255)
 ,@Durum int
 ,@DurumAd nvarchar(255)
 ,@ID int
 ,@IslemAd nvarchar(255)
 ,@IslemID int
 ,@KampanyaID int
 ,@KampanyaIndirimTutari float
 ,@KdvOrani int
 ,@KdvTutari float
 ,@MagazaAtamaTarihi datetime
 ,@MagazaDurum int
 ,@MagazaGonderimTarihi datetime
 ,@MagazaID int
 ,@MagazaKodu nvarchar(255)
 ,@Maliyet float
 ,@SiparisId int
 ,@StokKodu nvarchar(255)
 ,@TedarikciID int
 ,@TedarikciKodu nvarchar(255)
 ,@TedarikciKodu2 nvarchar(255)
 ,@Tutar float
 ,@UrunAdi nvarchar(255)
 ,@UrunID int
 ,@UrunKartiID int
 ,@ReturnDesc  nvarchar(255) output)
AS
if (select count(*) from TicimaxSiparisUrun where SiparisId = @SiparisId and UrunID = @UrunID) = 0
begin
INSERT INTO [dbo].[TicimaxSiparisUrun]
           ([Adet]
           ,[Barkod]
           ,[Durum]
           ,[DurumAd]
           ,[ID]
           ,[IslemAd]
           ,[IslemID]
           ,[KampanyaID]
           ,[KampanyaIndirimTutari]
           ,[KdvOrani]
           ,[KdvTutari]
           ,[MagazaAtamaTarihi]
           ,[MagazaDurum]
           ,[MagazaGonderimTarihi]
           ,[MagazaID]
           ,[MagazaKodu]
           ,[Maliyet]
           ,[SiparisId]
           ,[StokKodu]
           ,[TedarikciID]
           ,[TedarikciKodu]
           ,[TedarikciKodu2]
           ,[Tutar]
           ,[UrunAdi]
           ,[UrunID]
           ,[UrunKartiID])
     VALUES
           (@Adet
           ,@Barkod
           ,@Durum
           ,@DurumAd
           ,@ID
           ,@IslemAd
           ,@IslemID
           ,@KampanyaID
           ,@KampanyaIndirimTutari
           ,@KdvOrani
           ,@KdvTutari
           ,@MagazaAtamaTarihi
           ,@MagazaDurum
           ,@MagazaGonderimTarihi
           ,@MagazaID
           ,@MagazaKodu
           ,@Maliyet
           ,@SiparisId
           ,@StokKodu
           ,@TedarikciID
           ,@TedarikciKodu
           ,@TedarikciKodu2
           ,@Tutar
           ,@UrunAdi
           ,@UrunID
           ,@UrunKartiID)
	set @ReturnDesc = '1'
end
else
begin
	set @ReturnDesc = 'Satistaki Ürün Daha Önce Eklenmis'
end
GO
/****** Object:  StoredProcedure [dbo].[TicimaxUye_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[TicimaxUye_insert]
(			@CepTelefonu nvarchar(255)
           ,@CinsiyetID int
           ,@DogumTarihi datetime
           ,@DuzenlemeTarihi datetime
           ,@ID int
           ,@Il nvarchar(255)
           ,@Ilce nvarchar(255)
           ,@IlceID int
           ,@IlID int
           ,@Isim nvarchar(255)
           ,@Mail nvarchar(255)
           ,@MailIzin bit
           ,@Meslek nvarchar(255)
           ,@MusteriKodu nvarchar(255)
           ,@OgrenimDurumu nvarchar(255)
           ,@Sifre nvarchar(255)
           ,@SmsIzin bit
           ,@Soyisim nvarchar(255)
           ,@Telefon nvarchar(255)
           ,@UyeTuruID int
		   ,@UyeTCtoVKN varchar(11)
		   ,@UyeVD		varchar(250)	
		   ,@UyeIsVm	bit
		   ,@ReturnDesc  nvarchar(255) output
)
AS
if (select count(*) from TicimaxUye where ID = @ID) = 0
begin
INSERT INTO [dbo].[TicimaxUye]
           ([CepTelefonu]
           ,[CinsiyetID]
           ,[DogumTarihi]
           ,[DuzenlemeTarihi]
           ,[ID]
           ,[Il]
           ,[Ilce]
           ,[IlceID]
           ,[IlID]
           ,[Isim]
           ,[Mail]
           ,[MailIzin]
           ,[Meslek]
           ,[MusteriKodu]
           ,[OgrenimDurumu]
           ,[Sifre]
           ,[SmsIzin]
           ,[Soyisim]
           ,[Telefon]
           ,[UyeTuruID]
		   ,[UyeTCtoVKN]
		   ,[UyeVD]
		   ,[UyeIsVm])
     VALUES
           (@CepTelefonu
           ,@CinsiyetID
           ,@DogumTarihi
           ,@DuzenlemeTarihi
           ,@ID
           ,@Il
           ,@Ilce
           ,@IlceID
           ,@IlID
           ,@Isim
           ,@Mail
           ,@MailIzin
           ,@Meslek
           ,@MusteriKodu
           ,@OgrenimDurumu
           ,@Sifre
           ,@SmsIzin
           ,@Soyisim
           ,@Telefon
           ,@UyeTuruID
		   ,@UyeTCtoVKN 
		   ,@UyeVD		
		   ,@UyeIsVm)
	set @ReturnDesc = '1'
end
else
begin
	update TicimaxUye set 
	 [CepTelefonu] = @CepTelefonu
	,[CinsiyetID] = @CinsiyetID
	,[DogumTarihi] = @DogumTarihi
	,[DuzenlemeTarihi] = @DuzenlemeTarihi
	,[ID] = @ID
	,[Il] = @Il
	,[Ilce] = @Ilce
	,[IlceID] = @IlceID
	,[IlID] = @IlID
	,[Isim] = @Isim
	,[Mail] = @Mail
	,[MailIzin] = @MailIzin
	,[Meslek] = @Meslek
	,[MusteriKodu] = @MusteriKodu
	,[OgrenimDurumu] = @OgrenimDurumu
	,[Sifre] = @Sifre
	,[SmsIzin] = @SmsIzin
	,[Soyisim] = @Soyisim
	,[Telefon] = @Telefon
	,[UyeTuruID] = @UyeTuruID
	,[UyeTCtoVKN] = @UyeTCtoVKN 
	,[UyeVD] = @UyeVD
	,[UyeIsVm] = @UyeIsVm
	where ID = @ID
	set @ReturnDesc = 'Üye Bilgisi Güncellendi'
end
GO
/****** Object:  StoredProcedure [dbo].[Urun_Cek]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Urun_Cek]
(
	@ID	int
)
AS
	select u.StokKodu,upper(u.UrunAdi)UrunAdi,u.Tutar+u.KdvTutari as Tutar,u.Adet from TicimaxSiparis s
	inner join TicimaxSiparisUrun u on u.SiparisId = s.ID
	where SiparisId = @ID
	order by 3 desc,4 desc
GO
/****** Object:  StoredProcedure [dbo].[Uye_Cek]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Uye_Cek]
(
	@ID	int
)
AS
select distinct u.ID,s.AdiSoyadi,
case when u.CepTelefonu = '' then AliciTelefon else u.CepTelefonu end CepTelefonu,VergiNo,s.FaturaAdresi as TeslimatAdresi,tc.Il,tc.Ilce,Mahalle,Durum,convert(char(10),DogumTarihi,103) as DogumTarihi 
from WebSiparis s with(nolock)
inner join Uye u on u.ID = s.UyeID
outer apply(select distinct VergiNo,f.Il,f.Ilce,AliciTelefon from FaturaAdres f with(nolock) where f.ID = s.FaturaAdresId) tc
where s.ID = @ID
GO
/****** Object:  StoredProcedure [dbo].[Uye_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Uye_insert]
(			@CepTelefonu nvarchar(255)
           ,@CinsiyetID int
           ,@DogumTarihi datetime
           ,@DuzenlemeTarihi datetime
           ,@ID int
           ,@Il nvarchar(255)
           ,@Ilce nvarchar(255)
           ,@IlceID int
           ,@IlID int
           ,@Isim nvarchar(255)
           ,@Mail nvarchar(255)
           ,@MailIzin bit
           ,@Meslek nvarchar(255)
           ,@MusteriKodu nvarchar(255)
           ,@OgrenimDurumu nvarchar(255)
           ,@Sifre nvarchar(255)
           ,@SmsIzin bit
           ,@Soyisim nvarchar(255)
           ,@Telefon nvarchar(255)
           ,@UyeTuruID int
		   ,@UyeTCtoVKN varchar(11)
		   ,@UyeVD		varchar(250)	
		   ,@UyeIsVm	bit
		   ,@ReturnDesc  nvarchar(255) output
)
AS
if (select count(*) from Uye where ID = @ID) = 0
begin
INSERT INTO [dbo].[Uye]
           ([CepTelefonu]
           ,[CinsiyetID]
           ,[DogumTarihi]
           ,[DuzenlemeTarihi]
           ,[ID]
           ,[Il]
           ,[Ilce]
           ,[IlceID]
           ,[IlID]
           ,[Isim]
           ,[Mail]
           ,[MailIzin]
           ,[Meslek]
           ,[MusteriKodu]
           ,[OgrenimDurumu]
           ,[Sifre]
           ,[SmsIzin]
           ,[Soyisim]
           ,[Telefon]
           ,[UyeTuruID]
		   ,[UyeTCtoVKN]
		   ,[UyeVD]
		   ,[UyeIsVm])
     VALUES
           (@CepTelefonu
           ,@CinsiyetID
           ,@DogumTarihi
           ,@DuzenlemeTarihi
           ,@ID
           ,@Il
           ,@Ilce
           ,@IlceID
           ,@IlID
           ,@Isim
           ,@Mail
           ,@MailIzin
           ,@Meslek
           ,@MusteriKodu
           ,@OgrenimDurumu
           ,@Sifre
           ,@SmsIzin
           ,@Soyisim
           ,@Telefon
           ,@UyeTuruID
		   ,@UyeTCtoVKN 
		   ,@UyeVD		
		   ,@UyeIsVm)
	set @ReturnDesc = '1'
end
else
begin
	update Uye set 
	 [CepTelefonu] = @CepTelefonu
	,[CinsiyetID] = @CinsiyetID
	,[DogumTarihi] = @DogumTarihi
	,[DuzenlemeTarihi] = @DuzenlemeTarihi
	,[ID] = @ID
	,[Il] = @Il
	,[Ilce] = @Ilce
	,[IlceID] = @IlceID
	,[IlID] = @IlID
	,[Isim] = @Isim
	,[Mail] = @Mail
	,[MailIzin] = @MailIzin
	,[Meslek] = @Meslek
	,[MusteriKodu] = @MusteriKodu
	,[OgrenimDurumu] = @OgrenimDurumu
	,[Sifre] = @Sifre
	,[SmsIzin] = @SmsIzin
	,[Soyisim] = @Soyisim
	,[Telefon] = @Telefon
	,[UyeTuruID] = @UyeTuruID
	,[UyeTCtoVKN] = @UyeTCtoVKN 
	,[UyeVD] = @UyeVD
	,[UyeIsVm] = @UyeIsVm
	where ID = @ID
	set @ReturnDesc = 'Içerde Var'
end
GO
/****** Object:  StoredProcedure [dbo].[Uye_insert_Son]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[Uye_insert_Son]
(			@CepTelefonu nvarchar(255)
           ,@CinsiyetID int
           ,@DogumTarihi datetime
           ,@DuzenlemeTarihi datetime
           ,@ID int
           ,@Il nvarchar(255)
           ,@Ilce nvarchar(255)
           ,@Mahalle nvarchar(255)
           ,@IlceID int
           ,@IlID int
           ,@MahalleID int
           ,@Isim nvarchar(255)
           ,@Mail nvarchar(255)
           ,@MailIzin bit
           ,@Meslek nvarchar(255)
           ,@MusteriKodu nvarchar(255)
           ,@OgrenimDurumu nvarchar(255)
           ,@Sifre nvarchar(255)
           ,@SmsIzin bit
           ,@Soyisim nvarchar(255)
           ,@Telefon nvarchar(255)
           ,@UyeTuruID int
		   ,@UyeTCtoVKN varchar(11)
		   ,@UyeVD		varchar(250)	
		   ,@UyeIsVm	bit
		   ,@ReturnDesc  nvarchar(255) output
)
AS
if (select count(*) from Uye where ID = @ID) = 0
begin
INSERT INTO [dbo].[Uye]
           ([CepTelefonu]
           ,[CinsiyetID]
           ,[DogumTarihi]
           ,[DuzenlemeTarihi]
           ,[ID]
           ,[Il]
           ,[Ilce]
           ,[IlceID]
		   ,[Mahalle]
           ,[IlID]
		   ,[MahalleID]
           ,[Isim]
           ,[Mail]
           ,[MailIzin]
           ,[Meslek]
           ,[MusteriKodu]
           ,[OgrenimDurumu]
           ,[Sifre]
           ,[SmsIzin]
           ,[Soyisim]
           ,[Telefon]
           ,[UyeTuruID]
		   ,[UyeTCtoVKN]
		   ,[UyeVD]
		   ,[UyeIsVm])
     VALUES
           (@CepTelefonu
           ,@CinsiyetID
           ,@DogumTarihi
           ,@DuzenlemeTarihi
           ,@ID
           ,@Il
           ,@Ilce
           ,@Mahalle
           ,@IlceID
           ,@IlID
           ,@MahalleID
           ,@Isim
           ,@Mail
           ,@MailIzin
           ,@Meslek
           ,@MusteriKodu
           ,@OgrenimDurumu
           ,@Sifre
           ,@SmsIzin
           ,@Soyisim
           ,@Telefon
           ,@UyeTuruID
		   ,@UyeTCtoVKN 
		   ,@UyeVD		
		   ,@UyeIsVm)
	set @ReturnDesc = '1'
end
else
begin
	update Uye set 
	 [CepTelefonu] = @CepTelefonu
	,[CinsiyetID] = @CinsiyetID
	,[DogumTarihi] = @DogumTarihi
	,[DuzenlemeTarihi] = @DuzenlemeTarihi
	,[ID] = @ID
	,[Il] = @Il
	,[Ilce] = @Ilce
	,[Mahalle] = @Mahalle
	,[IlceID] = @IlceID
	,[IlID] = @IlID
	,[MahalleID] = @MahalleID
	,[Isim] = @Isim
	,[Mail] = @Mail
	,[MailIzin] = @MailIzin
	,[Meslek] = @Meslek
	,[MusteriKodu] = @MusteriKodu
	,[OgrenimDurumu] = @OgrenimDurumu
	,[Sifre] = @Sifre
	,[SmsIzin] = @SmsIzin
	,[Soyisim] = @Soyisim
	,[Telefon] = @Telefon
	,[UyeTuruID] = @UyeTuruID
	,[UyeTCtoVKN] = @UyeTCtoVKN 
	,[UyeVD] = @UyeVD
	,[UyeIsVm] = @UyeIsVm
	where ID = @ID
	set @ReturnDesc = 'Içerde Var'
end
GO
/****** Object:  StoredProcedure [dbo].[WebSiparis_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[WebSiparis_insert]
(
	@AdiSoyadi nvarchar(255),
	@Durum int,
	@EntegrasyonAktarildi bit,
	@FaturaAdresId int,
	@FaturaAdresi nvarchar(255),
	@FaturaNo nvarchar(255),
	@FaturaTarihi datetime,
	@HediyeCeki nvarchar(255),
	@HediyeCekiTutari float,
	@HadiyePaketiNotu nvarchar(255),
	@HediyePaketiTutari float,
	@HediyePaketiVar bit,
	@ID int,
	@IPAdresi nvarchar(255),
	@IndirimTutari float,
	@KargoAdresID int,
	@KargoEntegrasyonID int,
	@KargoEntegrasyonTakipNo nvarchar(255),
	@KargoFirmaId int,
	@KargoTakipNo nvarchar(255),
	@KargoTutari float,
	@Kaynak int,
	@Kur float,
	@Mail nvarchar(255),
	@Maliyet float,
	@Odemeler nvarchar(255),
	@ParaBirimi nvarchar(255),
	@Referer nvarchar(255),
	@ReklamKaynagi nvarchar(255),
	@SepetKampanyasiIndirimi float,
	@SiparisDurumu nvarchar(255),
	@SiparisKaynagi nvarchar(255),
	@SiparisNotu nvarchar(255),
	@SiparisTarihi datetime,
	@SiparisToplamTutari float,
	@StokDustu bit,
	@TeslimatAdresi nvarchar(255),
	@TeslimatGunu datetime,
	@TeslimatSaati nvarchar(255),
	@ToplamKdv float,
	@ToplamTutar float,
	@Tutar float,
	@Urunler nvarchar(255),
	@UyeAdi nvarchar(255),
	@UyeID int,
	@UyeMusteriKodu nvarchar(255),
	@UyeSoyadi nvarchar(255),
	@ReturnDesc  nvarchar(255) output
)
AS
if (select count(*) from WebSiparis where ID = @ID) = 0
begin
INSERT INTO [dbo].[WebSiparis]
           ([AdiSoyadi]
           ,[Durum]
           ,[EntegrasyonAktarildi]
           ,[FaturaAdresId]
           ,[FaturaAdresi]
           ,[FaturaNo]
           ,[FaturaTarihi]
           ,[HediyeCeki]
           ,[HediyeCekiTutari]
           ,[HadiyePaketiNotu]
           ,[HediyePaketiTutari]
           ,[HediyePaketiVar]
           ,[ID]
           ,[IPAdresi]
           ,[IndirimTutari]
           ,[KargoAdresID]
           ,[KargoEntegrasyonID]
           ,[KargoEntegrasyonTakipNo]
           ,[KargoFirmaId]
           ,[KargoTakipNo]
           ,[KargoTutari]
           ,[Kaynak]
           ,[Kur]
           ,[Mail]
           ,[Maliyet]
           ,[Odemeler]
           ,[ParaBirimi]
           ,[Referer]
           ,[ReklamKaynagi]
           ,[SepetKampanyasiIndirimi]
           ,[SiparisDurumu]
           ,[SiparisKaynagi]
           ,[SiparisNotu]
           ,[SiparisTarihi]
           ,[SiparisToplamTutari]
           ,[StokDustu]
           ,[TeslimatAdresi]
           ,[TeslimatGunu]
           ,[TeslimatSaati]
           ,[ToplamKdv]
           ,[ToplamTutar]
           ,[Tutar]
           ,[Urunler]
           ,[UyeAdi]
           ,[UyeID]
           ,[UyeMusteriKodu]
           ,[UyeSoyadi])
     VALUES
           (@AdiSoyadi,
            @Durum,
            @EntegrasyonAktarildi, 
            @FaturaAdresId,
            @FaturaAdresi,
            @FaturaNo,
            @FaturaTarihi,
            @HediyeCeki, 
            @HediyeCekiTutari,
            @HadiyePaketiNotu,
            @HediyePaketiTutari,
            @HediyePaketiVar,
            @ID,
            @IPAdresi,
            @IndirimTutari,
            @KargoAdresID,
            @KargoEntegrasyonID,
            @KargoEntegrasyonTakipNo,
            @KargoFirmaId,
            @KargoTakipNo,
            @KargoTutari,
            @Kaynak,
            @Kur,
            @Mail,
            @Maliyet,
            @Odemeler,
            @ParaBirimi,
            @Referer,
            @ReklamKaynagi,
            @SepetKampanyasiIndirimi,
            @SiparisDurumu,
            @SiparisKaynagi,
            @SiparisNotu,
            @SiparisTarihi,
            @SiparisToplamTutari,
            @StokDustu,
            @TeslimatAdresi,
            @TeslimatGunu,
            @TeslimatSaati,
            @ToplamKdv,
            @ToplamTutar,
            @Tutar,
            @Urunler,
            @UyeAdi,
            @UyeID,
            @UyeMusteriKodu,
            @UyeSoyadi
			)
	set @ReturnDesc = '1'
end
else
begin
	--update WebSiparis set TeslimatAdresi = @TeslimatAdresi,FaturaAdresi = @FaturaAdresi, Durum = @Durum, SiparisDurumu = @SiparisDurumu where ID = @ID
	set @ReturnDesc = '2'
end
GO
/****** Object:  StoredProcedure [dbo].[WebSiparisOdeme_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[WebSiparisOdeme_insert]
(
			@BankaKomisyonu float
           ,@CheckSum nvarchar(255) = null
           ,@HavaleBankaID int
           ,@HavaleHesapID int
           ,@ID int
           ,@KKOdemeBankaID int
           ,@KapidaOdemeTutari float
           ,@OdemeIndirimi float
           ,@OdemeNotu nvarchar(255)
           ,@OdemeSecenekID int
           ,@OdemeTipi int
           ,@Onaylandi int
           ,@PosReferansID nvarchar(255)
           ,@SiparisID int
           ,@TaksitSayisi int
           ,@Tarih datetime
           ,@Tutar float
           ,@UyeID int
		   ,@ReturnDesc  nvarchar(255) output
)
AS
if (select count(*) from WebSiparisOdeme where ID = @ID) = 0
begin
INSERT INTO [dbo].[WebSiparisOdeme]
           ([BankaKomisyonu]
           ,[CheckSum]
           ,[HavaleBankaID]
           ,[HavaleHesapID]
           ,[ID]
           ,[KKOdemeBankaID]
           ,[KapidaOdemeTutari]
           ,[OdemeIndirimi]
           ,[OdemeNotu]
           ,[OdemeSecenekID]
           ,[OdemeTipi]
           ,[Onaylandi]
           ,[PosReferansID]
           ,[SiparisID]
           ,[TaksitSayisi]
           ,[Tarih]
           ,[Tutar]
           ,[UyeID])
     VALUES
           (@BankaKomisyonu
           ,@CheckSum
           ,@HavaleBankaID
           ,@HavaleHesapID
           ,@ID
           ,@KKOdemeBankaID
           ,@KapidaOdemeTutari
           ,@OdemeIndirimi
           ,@OdemeNotu
           ,@OdemeSecenekID
           ,@OdemeTipi
           ,@Onaylandi
           ,@PosReferansID
           ,@SiparisID
           ,@TaksitSayisi
           ,@Tarih
           ,@Tutar
           ,@UyeID)
	set @ReturnDesc = '1'
end
else
begin
	set @ReturnDesc = 'Kayit Var'
end
GO
/****** Object:  StoredProcedure [dbo].[WebSiparisPuan_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[WebSiparisPuan_insert]
(
		@KartNumarasi nvarchar(20),
		@KazanilanPuan float,
		@KazanilanPuanTLKarsiligi float,
		@KullanilanPuan float,
		@KullanilanPuanTLKarsiligi float,
		@KullanimTarihi datetime,
		@PuanBildirildi bit,
		@Tip int,
		@ReturnDesc  nvarchar(255) output
)
AS
if (select count(*) from TicimaxPuanKullanim where KartNumarasi = @KartNumarasi and KullanimTarihi = @KullanimTarihi) = 0
begin
INSERT INTO [dbo].[TicimaxPuanKullanim]
			([KazanilanPuanID],
			[KartNumarasi],
			[KazanilanPuan],
			[KazanilanPuanTLKarsiligi],	
			[KullanilanPuan],
			[KullanilanPuanTLKarsiligi], 
			[KullanimTarihi],
			[PuanBildirildi],
			[Tip])
     VALUES
(			(select count(*) from TicimaxPuanKullanim)+1,
			@KartNumarasi,
			@KazanilanPuan,
			@KazanilanPuanTLKarsiligi,	
			@KullanilanPuan,
			@KullanilanPuanTLKarsiligi, 
			@KullanimTarihi,
			@PuanBildirildi,
			@Tip
)
	set @ReturnDesc = (select count(*) from TicimaxPuanKullanim)+1
end
else
begin
	set @ReturnDesc = (select KazanilanPuanID from TicimaxPuanKullanim where KartNumarasi = @KartNumarasi and KullanimTarihi = @KullanimTarihi)
end
GO
/****** Object:  StoredProcedure [dbo].[WebSiparisUrun_insert]    Script Date: 03-May-26 3:41:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Proc [dbo].[WebSiparisUrun_insert]
(
  @Adet float
 ,@Barkod nvarchar(255)
 ,@Durum int
 ,@DurumAd nvarchar(255)
 ,@ID int
 ,@IslemAd nvarchar(255)
 ,@IslemID int
 ,@KampanyaID int
 ,@KampanyaIndirimTutari float
 ,@KdvOrani int
 ,@KdvTutari float
 ,@MagazaAtamaTarihi datetime
 ,@MagazaDurum int
 ,@MagazaGonderimTarihi datetime
 ,@MagazaID int
 ,@MagazaKodu nvarchar(255)
 ,@Maliyet float
 ,@SiparisId int
 ,@StokKodu nvarchar(255)
 ,@TedarikciID int
 ,@TedarikciKodu nvarchar(255)
 ,@TedarikciKodu2 nvarchar(255)
 ,@Tutar float
 ,@UrunAdi nvarchar(255)
 ,@UrunID int
 ,@UrunKartiID int
 ,@ReturnDesc  nvarchar(255) output)
AS
if (select count(*) from WebSiparisUrun where SiparisId = @SiparisId and UrunID = @UrunID) = 0
begin
INSERT INTO [dbo].[WebSiparisUrun]
           ([Adet]
           ,[Barkod]
           ,[Durum]
           ,[DurumAd]
           ,[ID]
           ,[IslemAd]
           ,[IslemID]
           ,[KampanyaID]
           ,[KampanyaIndirimTutari]
           ,[KdvOrani]
           ,[KdvTutari]
           ,[MagazaAtamaTarihi]
           ,[MagazaDurum]
           ,[MagazaGonderimTarihi]
           ,[MagazaID]
           ,[MagazaKodu]
           ,[Maliyet]
           ,[SiparisId]
           ,[StokKodu]
           ,[TedarikciID]
           ,[TedarikciKodu]
           ,[TedarikciKodu2]
           ,[Tutar]
           ,[UrunAdi]
           ,[UrunID]
           ,[UrunKartiID])
     VALUES
           (@Adet
           ,@Barkod
           ,@Durum
           ,@DurumAd
           ,@ID
           ,@IslemAd
           ,@IslemID
           ,@KampanyaID
           ,@KampanyaIndirimTutari
           ,@KdvOrani
           ,@KdvTutari
           ,@MagazaAtamaTarihi
           ,@MagazaDurum
           ,@MagazaGonderimTarihi
           ,@MagazaID
           ,@MagazaKodu
           ,@Maliyet
           ,@SiparisId
           ,@StokKodu
           ,@TedarikciID
           ,@TedarikciKodu
           ,@TedarikciKodu2
           ,@Tutar
           ,@UrunAdi
           ,@UrunID
           ,@UrunKartiID)
	set @ReturnDesc = '1'
end
else
begin
	set @ReturnDesc = 'Kayit Var'
end
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TicimaxSiparis"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 321
               Right = 276
            End
            DisplayFlags = 280
            TopColumn = 42
         End
         Begin Table = "TicimaxUye"
            Begin Extent = 
               Top = 6
               Left = 546
               Bottom = 295
               Right = 726
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "WebSiparisOdemeTipi"
            Begin Extent = 
               Top = 6
               Left = 764
               Bottom = 331
               Right = 959
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TicimaxSiparisOdeme"
            Begin Extent = 
               Top = 6
               Left = 314
               Bottom = 312
               Right = 508
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TicimaxKargoTakip"
            Begin Extent = 
               Top = 6
               Left = 997
               Bottom = 327
               Right = 1195
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TicimaxTeslimatAdres"
            Begin Extent = 
               Top = 6
               Left = 1441
               Bottom = 329
               Right = 1611
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "VolantMagazaIletisim"
            Begin Extent = 
               Top = 6
               Left = 1233
               Bottom = 245
         ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'KargoBarkod'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'      Right = 1403
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 5160
         Alias = 2070
         Table = 2595
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'KargoBarkod'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'KargoBarkod'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = -96
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TicimaxSiparis"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 313
               Right = 275
            End
            DisplayFlags = 280
            TopColumn = 8
         End
         Begin Table = "TicimaxFaturaAdres"
            Begin Extent = 
               Top = 6
               Left = 314
               Bottom = 331
               Right = 505
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TicimaxTeslimatAdres"
            Begin Extent = 
               Top = 6
               Left = 522
               Bottom = 316
               Right = 692
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TicimaxKargoFirma"
            Begin Extent = 
               Top = 6
               Left = 730
               Bottom = 282
               Right = 936
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TicimaxSiparisOdeme"
            Begin Extent = 
               Top = 6
               Left = 974
               Bottom = 299
               Right = 1168
            End
            DisplayFlags = 280
            TopColumn = 5
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 4410
         Alias = 3285
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
     ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SiparisDokum'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'    Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SiparisDokum'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SiparisDokum'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TicimaxSiparisOdeme"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 290
               Right = 232
            End
            DisplayFlags = 280
            TopColumn = 5
         End
         Begin Table = "WebSiparisOdemeTipi"
            Begin Extent = 
               Top = 6
               Left = 270
               Bottom = 277
               Right = 465
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SiparisOdeme'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SiparisOdeme'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = -864
         Left = 0
      End
      Begin Tables = 
         Begin Table = "w"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 1106
               Right = 276
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "u"
            Begin Extent = 
               Top = 954
               Left = 319
               Bottom = 1084
               Right = 499
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "f"
            Begin Extent = 
               Top = 0
               Left = 437
               Bottom = 130
               Right = 607
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "t"
            Begin Extent = 
               Top = 806
               Left = 324
               Bottom = 936
               Right = 494
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "k"
            Begin Extent = 
               Top = 326
               Left = 560
               Bottom = 456
               Right = 766
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "o"
            Begin Extent = 
               Top = 0
               Left = 756
               Bottom = 417
               Right = 950
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "kt"
            Begin Extent = 
               Top = 256
               Left = 360
               Bottom = 386
               Right = 558
            End
            DisplayFlags = 280
            TopColumn = 2
        ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SiparisFisi'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' End
         Begin Table = "ot"
            Begin Extent = 
               Top = 169
               Left = 993
               Bottom = 495
               Right = 1188
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "m"
            Begin Extent = 
               Top = 597
               Left = 342
               Bottom = 727
               Right = 512
            End
            DisplayFlags = 280
            TopColumn = 3
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 10155
         Alias = 3225
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SiparisFisi'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SiparisFisi'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "WebSiparisUrun"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 251
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SiparisFisiStok'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SiparisFisiStok'
GO
USE [master]
GO
ALTER DATABASE [EntegreF] SET  READ_WRITE 
GO
