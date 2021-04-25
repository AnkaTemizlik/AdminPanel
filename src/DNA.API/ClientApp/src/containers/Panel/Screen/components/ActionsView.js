import React, { useState } from "react";
import { useHistory, Link, useRouteMatch } from "react-router-dom";
import { useSelector, useDispatch } from 'react-redux';
import api from "../../../../store/api";
import { isNotEmpty, supplant } from "../../../../store/utils";
import { showMessage } from '../../../../store/slices/alertsSlice'
import { Box, Icon, IconButton, Tooltip, Typography } from "@material-ui/core";
import Modal from "../../../../components/UI/Modal";
import { useTranslation } from "../../../../store/i18next";
import { Button } from 'devextreme-react/button';

const ActionsView = ({ renderActions, actions, refresh }) => {
	const { panel, screenConfig } = useSelector(state => state)
	const { currentScreen: screen, row } = panel.screen
	const { url } = useRouteMatch()
	const dispatch = useDispatch()
	const { t } = useTranslation();
	const history = useHistory()
	const [modalOpen, setModalOpen] = useState(false);
	const [confirmMessage, setConfirmMessage] = useState(null);
	const [currentAction, setCurrentAction] = useState(null);

	const executeEval = (action) => {
		setCurrentAction(action)
		var command = supplant(action.eval, row)
		// eslint-disable-next-line no-eval
		eval(command)
		let msg = {
			Success: true, Message: supplant(action.onSuccess.text)
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
			data.map(f => params[f] = row[f])
		}
		var preparedUrl = supplant(url, row);
		console.purple("runAction", url, row, preparedUrl, currentAction)
		api.actions.run(method, preparedUrl, params)
			.then(status => {

				if (status.Success) {
					dispatch(showMessage({ Success: true, Message: ((onSuccess && onSuccess.message) || "İşlem Başarılı.") }))
					if (refreshAfterSuccess == true)
						refresh && refresh()
				}
				else
					dispatch(showMessage({ Success: false, Message: ((onError && onError.message) || "İşlem Başarısız. ") + (status.Message || status.Error) }))
			})
			.catch(e => {
				dispatch(showMessage({ Success: false, Message: ((onError && onError.message) || "İşlem Başarısız. ") + e }))
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
					var cond = '{' + c[0] + '}' + c[1] + c[2]
					// eslint-disable-next-line no-eval
					disabled = !eval(supplant(cond, row))
				}
				else if (action.executeWhen.eval) {
					try {
						// eslint-disable-next-line no-eval
						disabled = !eval(supplant(action.executeWhen.eval, row))
						if (action.onSuccess)
							dispatch(showMessage(supplant(action.onSuccess.text)))
					} catch (error) {
						console.error(error)
						if (action.onError)
							dispatch(showMessage(supplant(action.onError.text)))
						disabled = true
					}
				}
			}
			else
				disabled = true
		}

		return <Tooltip title={action.text} key={i}>
			<span>
				<Button key={i}
					disabled={disabled}
					onClick={() => executeApi(action)}
					style={{ marginRight: 5 }}
					icon={action.dxIcon}
				>
					{action.icon
						&& <Icon style={{ fontSize: "1.125rem" }}>{action.icon}</Icon>}
				</Button>
			</span>
		</Tooltip>
	}

	const renderEvalAction = (action, i) => {
		let disabled = isDisable(action.dependsOnSelected)

		return <Tooltip title={action.text} key={i}>
			<span>
				<Button key={i}
					disabled={disabled}
					onClick={() => executeEval(action)}
					style={{ marginRight: 5 }}
					icon={action.dxIcon}
				>
					{action.icon
						&& <Icon style={{ fontSize: "1.125rem" }}>{action.icon}</Icon>}
				</Button>
			</span>
		</Tooltip>
	}

	const renderRouteAction = (action, i) => {
		let disabled = isDisable(action.dependsOnSelected)

		return <Tooltip title={action.text} key={i} >
			<span >
				<Button key={i}
					disabled={disabled}
					onClick={() => {
						var url = supplant(action.route, row)
						history.push(url)
					}}
					style={{ marginRight: 5 }}
					icon={action.dxIcon}
				>
					{action.icon
						&& <Icon style={{ fontSize: "1.125rem" }}>{action.icon}</Icon>}
				</Button>
			</span>
		</Tooltip>
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

	return (<Box display="flex" >

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

			return (Array.isArray(m.showIn) ? m.showIn.indexOf("toolbar") > -1 : m.showIn == "toolbar")
				? <Button key={i}
					disabled={!row}
					component={Link}
					to={row ? `/panel/screen/${screenConfig.screens[m.name].route}/${field}/${value}` : `${url}`}
					style={{ marginRight: 5 }}
					icon={m.dxIcon}
				>
					{m.icon
						&& <Icon style={{ fontSize: "1.125rem" }}>{m.icon}</Icon>}
				</Button>
				: null
		})}

		{actions && actions.filter(a => a.visible !== false && a.type != "dx").map((a, i) => {
			return renderActionButton(a, i)
		})}

		{renderActions()}

		<Modal
			open={modalOpen}
			onClose={handleModalCancel}
			title={t("Confirm")}
			ok={handleModalConfirm}
			cancel={handleModalCancel}
			okText={t("Ok")}
			cancelText={t("Cancel")}
		>
			<Box>
				<Typography>{t(confirmMessage)}</Typography>
			</Box>
		</Modal>

	</Box>
	)
}

export default ActionsView;
