
import React from "react";
import { useDispatch } from "react-redux";
import { useHistory } from "react-router-dom";
import { SnackbarProvider } from "notistack";
import IconButton from "@material-ui/core/IconButton";
import Close from "@material-ui/icons/Close";
import { Box } from "@material-ui/core";
import { ThemeProvider } from "@material-ui/styles";
import ScrollTop from "./components/UI/ScrollTop";
import api from './store/api'
import { loadMenu } from './store/slices/menuSlice'
import { applyGlobalSettings } from './store/slices/settingsSlice'
import Loadable from 'react-loadable';
import Fallback from './components/UI/Fallback'
import { loadMessages } from "devextreme/localization";
import { changeDxTheme } from "./plugins";
import dxMessages from 'devextreme/localization/messages/tr.json'
//import menus from './constants/menus'

require('devextreme/dist/css/dx.common.css')
require('./index.css')
require('moment/locale/tr')

const LoadableApp = Loadable.Map({
	loader: {
		Root: () => import('./containers/Root'),
		i18n: () => import('./store/i18n'),
		createTheme: () => import('./theme'),
		momentLocale: () => import('moment/locale/tr'),
		locales: () => api.actions.run("GET", `/api/locales`).then((status) => { if (status.Success) return status.Resource; else throw status.Message }),
		settingsStatus: () => api.actions.run("GET", `/api/settings`).then((status) => { if (status.Success) return status; else throw status.Message }),
		menusStatus: () => api.actions.run("GET", `/api/menus`).then((status) => { if (status.Success) return status; else throw status.Message }),
	},
	loading: Fallback,
	//timeout: 10000,
	render(loaded, props) {
		const notistackRef = React.createRef();
		const Root = loaded.Root.default
		const i18n = loaded.i18n.default
		const locales = loaded.locales
		const createTheme = loaded.createTheme.default
		const menusStatus = loaded.menusStatus
		const settingsStatus = loaded.settingsStatus
		const { Plugin, MultiLanguage } = settingsStatus.Resource.configs

		props.dispatch(loadMenu(menusStatus))

		console.warning("menus", menusStatus)

		changeDxTheme(Plugin.Color)

		loadMessages(dxMessages);

		// i18n ****************
		if (locales && locales.Translations) {
			i18n.addResourceBundle('tr', 'common', locales.Translations.tr)
			if (locales.Translations.en)
				i18n.addResourceBundle('en', 'common', locales.Translations.en)
		}
		console.success('i18n', i18n, MultiLanguage.Languages)
		// ************

		const onSnackbarClickDismiss = (key) => () => {
			notistackRef.current.closeSnackbar(key);
		};


		props.dispatch(applyGlobalSettings(settingsStatus))

		return <ThemeProvider theme={createTheme(Plugin)}>
			<SnackbarProvider
				ref={notistackRef}
				maxSnack={3}
				anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
				//preventDuplicate
				action={(key) => (
					<IconButton onClick={onSnackbarClickDismiss(key)}>
						<Close />
					</IconButton>
				)}
			>
				<Box height="100vh">
					<span id="back-to-top-anchor" style={{ visibility: "hidden", width: 0, height: 0 }}></span>
					<ScrollTop id="back-to-top-anchor" />

					<Root settingsStatus={settingsStatus} />

				</Box>
			</SnackbarProvider>
		</ThemeProvider>
	}
});

const App = (props) => {
	window.development = process.env && process.env.NODE_ENV === "development";
	const history = useHistory();
	history.listen((location, action) => {
		window.scrollTo(0, 0);
	});
	return (
		<LoadableApp dispatch={useDispatch()} />
	);
};

export default App
