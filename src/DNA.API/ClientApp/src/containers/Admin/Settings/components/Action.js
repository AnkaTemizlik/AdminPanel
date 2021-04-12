
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useHistory } from "react-router-dom";
import { Grid, Icon, IconButton, Tooltip } from "@material-ui/core";
import { Box } from "@material-ui/core";
import { useTranslation } from '../../../../store/i18next'
import api from "../../../../store/api";
import withSnack from "../../../../store/snack";

const Action = ({ text, icon, route, apiUrl, snack, externalUrl }) => {

	const { t } = useTranslation()
	const history = useHistory()
	const [loading, setLoading] = useState(false)

	const run = async () => {
		if (apiUrl) {
			setLoading(true)
			var status = await api.actions.run("POST", apiUrl)
			setLoading(false)
			snack.show(status);
		}
		else if (externalUrl) {
			let win = window.open(externalUrl, '_blank');
			win.focus();
		}
		else if (route) {
			history.push(route)
		}
	}

	return text
		? <Tooltip title={t(text)}>
			<span>
				<IconButton edge="end" onClick={run} disabled={loading}>
					<Icon>{icon}</Icon>
				</IconButton>
			</span>
		</Tooltip>
		: null
}

export default withSnack(Action)