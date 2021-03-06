import React, { useState } from "react";
import { useHistory, Link, useRouteMatch } from "react-router-dom";
import { useSelector, useDispatch } from 'react-redux';
import api from "../../../../store/api";
import { isNotEmpty, supplant } from "../../../../store/utils";
import { showMessage } from '../../../../store/slices/alertsSlice'
import { setLoading } from '../store/screenSlice'
import { Box, Icon, IconButton, Tooltip, Typography } from "@material-ui/core";
import Modal from "../../../../components/UI/Modal";
import { useTranslation } from "../../../../store/i18next";
import { Button } from 'devextreme-react/button';
import PopupView from "./PopupView";
import Iconify from "../../../../components/UI/Icons/Iconify";

const ActionsView = React.memo(({ renderActions, actions, refresh, showButtonText }) => {
	const { panel, screenConfig } = useSelector(state => state)
	const { currentScreen: screen, row } = panel.screen
	const { url } = useRouteMatch()
	const dispatch = useDispatch()
	const { t } = useTranslation();
	const history = useHistory()
	const [modalOpen, setModalOpen] = useState(false);
	const [popupViewOpen, setPopupViewOpen] = useState(null);
	const [confirmMessage, setConfirmMessage] = useState(null);
	const [currentAction, setCurrentAction] = useState(null);

	const executeEval = (action) => {
		setCurrentAction(action)
		var command = supplant(action.eval, row)
		// eslint-disable-next-line no-eval
		eval(command)
		let msg = {
			Success: true, Message: supplant(t(action.onSuccess.text))
		}
		dispatch(showMessage(msg))
	}

	const executeApi = (action) => {
		setCurrentAction(action)
		if (action.confirmation) {
			setConfirmMessage(action.confirmation.message)
			setModalOpen(true);
		}
		else {
			runAction(action.request)
		}
	}

	const runAction = (req) => {
		const { method, refreshAfterSuccess, url, data, onError, onSuccess } = req || currentAction.request
		let params = {}
		if (data) {
			console.success("runAction", data)
			if (Array.isArray(data)) {
				data.map(f => params[f] = row[f])
			}
			else {
				if (typeof data == "object") {
					console.success("runAction 1", "object: " + typeof data == "object")
					for (const key in data) {
						console.success("runAction 2", key, data[key], supplant(data[key], row))
						params[key] = supplant(data[key], row)
					}
				}
			}
			console.success("runAction", typeof data == "object", params)
		}
		var preparedUrl = supplant(url, row);
		console.purple("runAction", url, row, preparedUrl, currentAction)
		dispatch(setLoading(true))
		api.actions.run(method, preparedUrl, params)
			.then(status => {
				console.purple("runAction status", status, onSuccess)
				if (status.Success == true) {
					if (onSuccess) {
						dispatch(showMessage({ Success: true, Message: ((onSuccess && t(onSuccess.text)) || "????lem Ba??ar??l??.") }))
						if (onSuccess.route) {
							history.push(supplant(onSuccess.route, row))
						}
						else if (onSuccess.blank) {
							var href = supplant(onSuccess.blank, status.Resource)
							const a = document.createElement("a");
							a.href = href;
							a.target = "_blank";
							document.body.appendChild(a);
							a.click();
						}
					}
					if (refreshAfterSuccess == true)
						refresh && refresh()
				}
				else {
					dispatch(showMessage(status))
				}
				dispatch(setLoading(false))
			})
			.catch(e => {
				dispatch(showMessage({ Success: false, Message: ((onError && t(onError.text)) || "????lem Ba??ar??s??z. ") + " " + e }))
				dispatch(setLoading(false))
			})
	}

	const handleModalConfirm = (e) => {
		setModalOpen(false);
		runAction()
	};

	const handleModalCancel = (params) => {
		setModalOpen(false);
	};

	const isDisable = (dependsOnSelected) => {
		let disabled = isNotEmpty(dependsOnSelected) && dependsOnSelected == false
			? false
			: !row
		return disabled
	}

	const renderRequestAction = (action, i) => {
		let disabled = isDisable(action.dependsOnSelected)

		if (action.executeWhen) {
			if (row) {
				if (action.executeWhen.condition && Array.isArray(action.executeWhen.condition)) {
					var c = action.executeWhen.condition;
					var cond = `{${c[0]}}${c[1]}${c[2]}`
					// eslint-disable-next-line no-eval
					disabled = !eval(supplant(cond, row))
				}
				else if (action.executeWhen.eval) {
					try {
						// eslint-disable-next-line no-eval
						disabled = !eval(supplant(action.executeWhen.eval, row))
						// if (action.onSuccess)
						// 	dispatch(showMessage(supplant(action.onSuccess.text)))
					} catch (error) {
						// console.error(error)
						// if (action.onError)
						// 	dispatch(showMessage(supplant(action.onError.text)))
						disabled = true
					}
				}
			}
			else
				disabled = true
		}
		return getButton(i, disabled, action.text, action.icon, action.dxIcon, () => executeApi(action));
	}

	const renderEvalAction = (action, i) => {
		let disabled = isDisable(action.dependsOnSelected)
		return getButton(i, disabled, action.text, action.icon, action.dxIcon, () => executeEval(action));
	}

	const renderRouteAction = (action, i) => {
		let disabled = isDisable(action.dependsOnSelected)
		return getButton(i, disabled, action.text, action.icon, action.dxIcon, () => {
			var url = supplant(action.route, row)
			history.push(url)
		});
	}

	const getButton = (i, disabled, text, icon, dxIcon, onClick) => {
		return <Button key={i}
			hint={t(text)}
			disabled={disabled}
			onClick={onClick}
			style={{ marginRight: 5 }}
			icon={dxIcon}
			stylingMode="outlined"
		>
			<span style={{ marginBottom: -1.74, marginTop: 0, padding: 0 }}>
				{icon && <Iconify icon={icon} fontSize="1.125rem" />}
				{showButtonText ? <span style={{ paddingLeft: 4 }}>{t(text)}</span> : undefined}
			</span>
		</Button>
	}

	const renderActionButton = (action, i) => {
		if (action.route)
			return renderRouteAction(action, i)
		else if (action.request)
			return renderRequestAction(action, i)
		else if (action.eval)
			return renderEvalAction(action, i)
		else return null
	}

	return (<Box display="flex">

		{screen.subModels && screen.subModels.map((m, i) => {
			let field = "";
			let value = "";

			if (m.visible == false)
				return null;

			if (row) {
				if (Array.isArray(m.relationFieldNames[0])) {
					field = m.relationFieldNames[0][1];
					value = row[m.relationFieldNames[0][0]]
				}
				else {
					field = m.relationFieldNames[1];
					value = row[m.relationFieldNames[0]]
				}
			}
			if (Array.isArray(m.showIn) ? m.showIn.indexOf("toolbar") > -1 : m.showIn == "toolbar") {
				console.info("popup", m)
				return m.showIn.indexOf("popup") > -1
					? getButton(i, m.dependsOnSelected == true ? !row : false, m.name, m.icon, m.dxIcon, (e) => setPopupViewOpen(m))
					// ? <Button key={i}
					// 	hint={t(m.title)}
					// 	disabled={m.dependsOnSelected == true ? !row : false}
					// 	style={{ marginRight: 5 }}
					// 	icon={m.dxIcon}
					// 	onClick={(e) => setPopupViewOpen(m)}
					// >
					// 	<span style={{ display: "flex", alignItems: "center", marginBottom: -1.74, marginTop: 0, padding: 0 }}>
					// 		{m.icon && <Iconify icon={m.icon} fontSize="1.125rem" />}
					// 		{showButtonText ? <span style={{ paddingLeft: 4 }}>{t(m.name)}</span> : undefined}
					// 	</span>
					// </Button>
					: <Button key={i}
						hint={t(m.title)}
						disabled={!row}
						component={Link}
						to={row ? `/panel/screen/${screenConfig.screens[m.name].route}/${field}/${value}` : `${m.route}`}
						style={{ marginRight: 5 }}
						icon={m.dxIcon}
					>
						<span style={{ display: "flex", alignItems: "center", marginBottom: -1.74, marginTop: 0, padding: 0 }}>
							{m.icon && <Iconify icon={m.icon} fontSize="1.125rem" />}
							{showButtonText ? <span style={{ paddingLeft: 4 }}>{t(m.name)}</span> : undefined}
						</span>
					</Button>
			}

			return null
		})}

		{actions && actions.filter(a => a.showInEditColumn !== true && a.visible !== false && a.type != "dx").map((a, i) => {
			return renderActionButton(a, i)
		})}

		{renderActions && renderActions()}

		<Modal
			open={modalOpen}
			onClose={handleModalCancel}
			title="Confirm"
			ok={handleModalConfirm}
			cancel={handleModalCancel}
			okText="Ok"
			cancelText="Cancel"
		>
			<Box>
				<Typography>{t(confirmMessage)}</Typography>
			</Box>
		</Modal>

		{popupViewOpen &&
			<PopupView model={popupViewOpen} onClose={() => setPopupViewOpen(null)} />}

	</Box>
	)
})

export default ActionsView;
