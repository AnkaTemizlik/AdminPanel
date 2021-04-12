import React, { Suspense } from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom'
import App from './App'
import store from './store'
import { Provider } from 'react-redux'
import * as serviceWorker from './serviceWorker';
import Fallback from './components/UI/Fallback'
import "./store/i18n";
import 'moment/locale/tr';
import { locale, loadMessages } from "devextreme/localization";
import trMessages from "devextreme/localization/messages/tr.json";
import './index.css';
import 'devextreme/dist/css/dx.common.css';

loadMessages(trMessages);

ReactDOM.render(
	<Provider store={store}>
		<BrowserRouter>
			<Suspense fallback={<Fallback />}>
				<App />
			</Suspense>
		</BrowserRouter>
	</Provider>
	,
	document.getElementById('root')
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
