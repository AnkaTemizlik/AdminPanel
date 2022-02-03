import { useSelector } from "react-redux";
import React, { useEffect, useState, useCallback } from "react";
import { Tooltip, Grid, Toolbar, Box, Button, IconButton, Badge, Paper, Typography, Hidden, Tabs, Tab, AppBar, Icon } from "@material-ui/core";
import { TabPanel } from "../../../../components/UI/Tab";
import { useTranslation } from "../../../../store/i18next";
import withSnack from "../../../../store/snack";
import RowFieldsView from "./RowFieldsView";
import ModelView from "./ModelView";
import TableView from "./TableView";
import TileView from "./TileView";
import Iconify from "../../../../components/UI/Icons/Iconify";
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
			rolesAllowed(user.Roles, m.roles) && (m.visible !== false) && (
				Array.isArray(m.showIn)
					? m.showIn.indexOf("tab") > -1
					: m.showIn == "tab"
			)
		))
	}, [screen, user.Roles])

	return (<>
		<Paper>
			<Toolbar>
				<Tabs value={tabIndex} onChange={tabChange}>
					{screen.hideDetails !== true &&
						<Tab icon={<Iconify icon={"format_list_bulleted"} />} label={t("Details")} />}
					{models.map((m, i) => {
						return <Tab key={i} icon={<Iconify icon={m.icon} />} label={t(m.title || m.name)} />
					})}
				</Tabs>
			</Toolbar>
                                                                                     
			{screen.hideDetails !== true &&
				<TabPanel value={tabIndex} index={0}>
					<RowFieldsView row={row} columns={screen.columns || []} name={name} />
				</TabPanel>}

			{models.map((m, i) => {
				return <TabPanel key={i} value={tabIndex} index={i + (screen.hideDetails === true ? 0 : 1)}>
					{row && m.type == "property" && <ModelView model={m} row={row} />}
					{row && m.type == "list" && <TableView model={m} row={row} />}
					{row && m.type == "gallery" && <TileView model={m} row={row} />}
				</TabPanel>
			})}
		</Paper>
	</>
	)
})

export default FooterView;
