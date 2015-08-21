/*
Navicat MySQL Data Transfer

Source Server         : Teste MySQL
Source Server Version : 50625
Source Host           : localhost:3306
Source Database       : main_v101

Target Server Type    : MYSQL
Target Server Version : 50625
File Encoding         : 65001

Date: 2015-08-21 15:36:46
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
-- Records of v101_tab_modulo
-- ----------------------------
INSERT INTO `v101_tab_modulo` VALUES ('7', 'APLICACAO', '', '', '', '');
INSERT INTO `v101_tab_modulo` VALUES ('2', 'MENU', 'CAD000', 'Cadastro', 'AB', 'MNU000');
INSERT INTO `v101_tab_modulo` VALUES ('3', 'APLICACAO', 'CLI100', 'Cadastro de Cliente(s)', 'AB', 'CAD000');
INSERT INTO `v101_tab_modulo` VALUES ('4', 'MENU', 'EST000', 'Estoque', 'AB', 'MNU000');
INSERT INTO `v101_tab_modulo` VALUES ('5', 'APLICACAO', 'EST100', 'Entrada e Saida de Estoque', 'AB', 'EST000');
INSERT INTO `v101_tab_modulo` VALUES ('1', 'APLICACAO', 'MDM001', 'Manutencao de modulos', 'ABCDF', 'SIS000');
INSERT INTO `v101_tab_modulo` VALUES ('6', 'MENU', 'PDV000', 'Menu do Ponto de Venda', 'A', 'MNU000');
INSERT INTO `v101_tab_modulo` VALUES ('7', 'APLICACAO', 'PDV100', 'Ponto de Venda', 'A', 'PDV000');
INSERT INTO `v101_tab_modulo` VALUES ('8', 'MENU', 'SIS000', 'Gerenciar Sistema', 'A', 'MNU000');

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
-- Records of v101_tab_usuario
-- ----------------------------
INSERT INTO `v101_tab_usuario` VALUES ('1', 'ADMIN', 'QVkAFUTL1DTaINlPUubyog==', 'Rafael Fernandes Motta', 'ABCDF', '1');

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
