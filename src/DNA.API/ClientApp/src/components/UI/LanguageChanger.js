import React, { useEffect, useCallback } from "react";
import { useTranslation, Trans } from "react-i18next";
import moment from "moment";
import { makeStyles } from "@material-ui/core/styles";
import SvgIcon from "@material-ui/core/SvgIcon";
import { Avatar, Button, IconButton, ListItemIcon, ListItemText, Menu, MenuItem } from "@material-ui/core";
import { Drafts, Send } from "@material-ui/icons";
import Plugin from "../../plugins";
import axios from "../../store/axios";

const TrIcon = (props) => {
	return (
		<SvgIcon {...props}>
			<svg xmlns="http://www.w3.org/2000/svg" id="flag-icon-css-tr" viewBox="0 0 512 512">
				<g fillRule="evenodd">
					<path fill="#e30a17" d="M0 0h512v512H0z" />
					<path fill="#fff" d="M348.8 264c0 70.6-58.3 127.9-130.1 127.9s-130.1-57.3-130.1-128 58.2-127.8 130-127.8S348.9 193.3 348.9 264z" />
					<path fill="#e30a17" d="M355.3 264c0 56.5-46.6 102.3-104.1 102.3s-104-45.8-104-102.3 46.5-102.3 104-102.3 104 45.8 104 102.3z" />
					<path fill="#fff" d="M374.1 204.2l-1 47.3-44.2 12 43.5 15.5-1 43.3 28.3-33.8 42.9 14.8-24.8-36.3 30.2-36.1-46.4 12.8-27.5-39.5z" />
				</g>
			</svg>
		</SvgIcon>
	);
};

const TrWideIcon = (props) => {
	return (
		<SvgIcon {...props}>
			<svg xmlns="http://www.w3.org/2000/svg" id="flag-icon-css-tr" viewBox="0 0 640 480">
				<g fillRule="evenodd">
					<path fill="#e30a17" d="M0 0h640v480H0z" />
					<path fill="#fff" d="M407 247.5c0 66.2-54.6 119.9-122 119.9s-122-53.7-122-120 54.6-119.8 122-119.8 122 53.7 122 119.9z" />
					<path fill="#e30a17" d="M413 247.5c0 53-43.6 95.9-97.5 95.9s-97.6-43-97.6-96 43.7-95.8 97.6-95.8 97.6 42.9 97.6 95.9z" />
					<path fill="#fff" d="M430.7 191.5l-1 44.3-41.3 11.2 40.8 14.5-1 40.7 26.5-31.8 40.2 14-23.2-34.1 28.3-33.9-43.5 12-25.8-37z" />
				</g>
			</svg>
		</SvgIcon>
	);
};

const EnIcon = (props) => {
	return (
		<SvgIcon {...props}>
			<svg xmlns="http://www.w3.org/2000/svg" id="flag-icon-css-gb" viewBox="0 0 512 512">
				<path fill="#012169" d="M0 0h512v512H0z" />
				<path fill="#FFF" d="M512 0v64L322 256l190 187v69h-67L254 324 68 512H0v-68l186-187L0 74V0h62l192 188L440 0z" />
				<path fill="#C8102E" d="M184 324l11 34L42 512H0v-3l184-185zm124-12l54 8 150 147v45L308 312zM512 0L320 196l-4-44L466 0h46zM0 1l193 189-59-8L0 49V1z" />
				<path fill="#FFF" d="M176 0v512h160V0H176zM0 176v160h512V176H0z" />
				<path fill="#C8102E" d="M0 208v96h512v-96H0zM208 0v512h96V0h-96z" />
			</svg>
		</SvgIcon>
	);
};

const EnWideIcon = (props) => {
	return (
		<SvgIcon {...props}>
			<svg xmlns="http://www.w3.org/2000/svg" id="flag-icon-css-gb" viewBox="0 0 640 480">
				<path fill="#012169" d="M0 0h640v480H0z" />
				<path fill="#FFF" d="M75 0l244 181L562 0h78v62L400 241l240 178v61h-80L320 301 81 480H0v-60l239-178L0 64V0h75z" />
				<path fill="#C8102E" d="M424 281l216 159v40L369 281h55zm-184 20l6 35L54 480H0l240-179zM640 0v3L391 191l2-44L590 0h50zM0 0l239 176h-60L0 42V0z" />
				<path fill="#FFF" d="M241 0v480h160V0H241zM0 160v160h640V160H0z" />
				<path fill="#C8102E" d="M0 193v96h640v-96H0zM273 0v480h96V0h-96z" />
			</svg>
		</SvgIcon>
	);
};

const useStyles = makeStyles((theme) => ({
	small: {
		width: theme.spacing(3),
		height: theme.spacing(3),
		border: "1px solid white",
	},
}));

var languageNames = {
	tr: "Türkçe",
	en: "English",
};

const LanguageChanger = () => {
	const { i18n } = useTranslation();
	const [anchorEl, setAnchorEl] = React.useState(null);
	const classes = useStyles();

	const changeLanguage = useCallback(
		(lng) => {
			handleClose();
			i18n.changeLanguage(lng);
			moment.locale(lng);
			axios.defaults.headers.common["Accept-Language"] = lng;
		},
		[i18n]
	);

	const handleClick = (event) => {
		setAnchorEl(event.currentTarget);
	};

	const handleClose = () => {
		setAnchorEl(null);
	};

	useEffect(() => {
		let lng = i18n.language;
		// tr-TR şeklinde gelirse değiştir.
		if (i18n.language.indexOf("-") > -1) lng = i18n.language.split("-")[0];
		changeLanguage(lng);
	}, [changeLanguage, i18n.language]);

	return Plugin.Languages.length > 1 ? (
		<>
			<IconButton edge="end" onClick={handleClick}>
				<Avatar alt="Language" className={classes.small}>
					{i18n.language == "tr" && <TrIcon style={{ width: 22, height: 22 }} />}
					{i18n.language == "en" && <EnIcon style={{ width: 22, height: 22 }} />}
				</Avatar>
			</IconButton>
			<Menu
				id="language-menu"
				anchorEl={anchorEl}
				keepMounted
				open={Boolean(anchorEl)}
				onClose={handleClose}
				getContentAnchorEl={null}
				disableScrollLock
				anchorOrigin={{
					vertical: "bottom",
					horizontal: "center",
				}}
				transformOrigin={{
					vertical: "top",
					horizontal: "center",
				}}
			>
				{Plugin.Languages.map((l, i) => (
					<MenuItem dense onClick={() => changeLanguage(l)} key={i}>
						<ListItemIcon>
							{l == "tr" && <TrWideIcon />}
							{l == "en" && <EnWideIcon />}
						</ListItemIcon>
						<ListItemText primary={languageNames[l]} />
					</MenuItem>
				))}
			</Menu>
		</>
	) : null;
};

export default LanguageChanger;
