/*
Navicat MySQL Data Transfer

Source Server         : Teste MySQL
Source Server Version : 50625
Source Host           : localhost:3306
Source Database       : main_v101

Target Server Type    : MYSQL
Target Server Version : 50625
File Encoding         : 65001

Date: 2015-08-21 16:01:55
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for v101_tab_modulo
-- ----------------------------
DROP TABLE IF EXISTS `v101_tab_modulo`;
CREATE TABLE `v101_tab_modulo` (
  `MOD.ID` int(11) NOT NULL,
  `MOD.TIPO` varchar(10) NOT NULL,
  `MOD.NOME` varchar(6) NOT NULL,
  `MOD.DESCRICAO` varchar(50) NOT NULL,
  `MOD.PRIVILEGIO` varchar(10) NOT NULL,
  `MOD.PAI` varchar(6) NOT NULL,
  PRIMARY KEY (`MOD.NOME`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for v101_tab_usuario
-- ----------------------------
DROP TABLE IF EXISTS `v101_tab_usuario`;
CREATE TABLE `v101_tab_usuario` (
  `USU.ID` int(1) NOT NULL,
  `USU.USUARIO` varchar(50) NOT NULL,
  `USU.PASSWORD` varchar(50) NOT NULL,
  `USU.NOME` varchar(50) NOT NULL,
  `USU.PRIVILEGIO` varchar(5) NOT NULL,
  `USU.ESTADO` int(1) NOT NULL,
  PRIMARY KEY (`USU.USUARIO`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Procedure structure for V101_ENTRADA_USUARIO
-- ----------------------------
DROP PROCEDURE IF EXISTS `V101_ENTRADA_USUARIO`;
DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `V101_ENTRADA_USUARIO`(IN `p_USUARIO` varchar(50),OUT `o_USUARIO` varchar(50),OUT `o_PASSWORD` varchar(50),OUT `o_NOME` varchar(50),OUT `o_PRIVILEGIO` varchar(5),OUT `o_ESTADO` varchar(1))
BEGIN
	SELECT `USU.USUARIO` ,
				 `USU.PASSWORD` ,	
				 `USU.NOME` ,
				 `USU.PRIVILEGIO` ,
				 `USU.ESTADO`
	INTO	 	o_USUARIO ,
					o_PASSWORD ,
					o_NOME ,
					o_PRIVILEGIO ,
					o_ESTADO
	FROM    `v101_tab_usuario`	
	WHERE	  `USU.USUARIO` = p_USUARIO;
	
	SELECT  o_USUARIO ,
					o_PASSWORD ,
					o_NOME ,
					o_PRIVILEGIO ,
					o_ESTADO;
END
;;
DELIMITER ;

-- ----------------------------
-- Procedure structure for V113_CARREGAR_MENU
-- ----------------------------
DROP PROCEDURE IF EXISTS `V113_CARREGAR_MENU`;
DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `V113_CARREGAR_MENU`(IN `i_mod_pai` varchar(6),IN `i_mod_priv` varchar(10),OUT `o_mod_tipo` varchar(50),OUT `o_mod_nome` varchar(50),OUT  `o_mod_descricacao` varchar(50),OUT `o_mod_privilegio` varchar(50),OUT `o_mod_pai` varchar(50))
BEGIN
	-- V113_CARREGAR_MENU
  -- 20/08/2015 - Carrega os modulos StoredProcedure
	SELECT 
				 `MOD.TIPO`,
				 `MOD.NOME`	,
				 `MOD.DESCRICAO` ,
		     `MOD.PRIVILEGIO` ,
         `MOD.PAI` 
	INTO		
			   o_mod_tipo ,
				 o_mod_nome ,
				 o_mod_descricacao ,
				 o_mod_privilegio ,
				 o_mod_pai
  FROM   
         `v101_tab_modulo`
  WHERE  	
			   `MOD.PAI` =  i_mod_pai
  AND			
			   `MOD.PRIVILEGIO` LIKE CONCAT('%',  i_mod_priv ,'%') < 1000;
  SELECT
				 o_mod_tipo ,
				 o_mod_nome ,
				 o_mod_descricacao ,
				 o_mod_privilegio ,
				 o_mod_pai;		
END
;;
DELIMITER ;

-- ----------------------------
-- Procedure structure for v113_GET_MENU_PAI
-- ----------------------------
DROP PROCEDURE IF EXISTS `v113_GET_MENU_PAI`;
DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `v113_GET_MENU_PAI`(IN `i_nome` varchar(10),OUT `o_pai` varchar(10))
BEGIN
	-- v113_GET_MENU_PAI
	-- retorna o Pai do menu

	SELECT 	
					`MOD.PAI`
	INTO		
				  o_pai
  FROM	
					`v101_tab_modulo`
  WHERE	
					`MOD.NOME` = i_nome;
	
	SELECT o_pai;
END
;;
DELIMITER ;
