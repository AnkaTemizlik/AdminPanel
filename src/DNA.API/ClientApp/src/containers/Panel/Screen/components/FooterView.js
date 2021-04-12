import { useSelector } from "react-redux";
import React, { useEffect, useState, useCallback } from "react";
import { Tooltip, Grid, Toolbar, Box, Button, IconButton, Badge, Paper, Typography, Hidden, Tabs, Tab, AppBar, Icon } from "@material-ui/core";
import { TabPanel } from "../../../../components/UI/Tab";
import { useTranslation } from "../../../../store/i18next";
import withSnack from "../../../../store/snack";
import RowFieldsView from "./RowFieldsView";
import ModelView from "./ModelView";
import TableView from "./TableView";
import { rolesAllowed } from "../../../../store/utils";

const FooterView = React.memo(({ row, screen, name }) => {
	const { t } = useTranslation();
	const [tabIndex, setTabIndex] = useState(0)
	const [models, setModels] = useState([])
	const user = useSelector(state => state.auth.user)

	const tabChange = (e, i) => {
		setTabIndex(i)
	}

	useEffect(() => {
		setModels(screen.subModels.filter((m) =>
			rolesAllowed(user.Roles, m.roles) && (Array.isArray(m.showIn)
				? m.showIn.indexOf("tab") > -1
				: m.showIn == "tab")))
	}, [screen, user.Roles])

	return (<>
		<Paper>
			<Toolbar>
				<Tabs value={tabIndex} onChange={tabChange}>
					{models.map((m, i) => {
						return <Tab key={i} icon={<Icon>{m.icon}</Icon>} label={t(m.title || m.name)} />
					})}
					{screen.hideDetails !== true &&
						<Tab icon={<Icon>format_list_bulleted</Icon>} label={t("Details")} />}
				</Tabs>
			</Toolbar>

			{models.map((m, i) => {
				return <TabPanel key={i} value={tabIndex} index={i}>
					{row && m.type == "property" &&
						<ModelView model={m} row={row} screen={screen} />}
					{row && m.type == "list" &&
						<TableView model={m} row={row} screen={screen} />}
				</TabPanel>
			})}


			<TabPanel value={tabIndex} index={models.length}>
				<RowFieldsView row={row} columns={screen.columns || []} name={name} />
			</TabPanel>

		</Paper>
	</>
	)
})

export default FooterView;
