import React, { useEffect, useState } from "react";
import { useSelector, useDispatch } from 'react-redux';
import { useHistory, useParams, useRouteMatch, useLocation, Link } from "react-router-dom";
import { Box, Grid, Icon, IconButton, Paper, Toolbar, Tooltip } from "@material-ui/core";
import { useTranslation } from "../../../store/i18next";
import { ApiURL } from "../../../store/axios";
import { selectScreen, getById } from './store/screenSlice'
import Scheduler, { Resource } from 'devextreme-react/scheduler';
import createCustomStore from "../../../components/UI/Table/createCustomStore";
import CustomStore from "devextreme/data/custom_store";
import DataSource from "devextreme/data/data_source";
import notify from "devextreme/ui/notify";
import api from '../../../store/api'

// import api from "../../../store/api";

const views = ["day", "week", "month", "agenda"];

const CalendarWrapper = (props) => {
	const { name } = props;
	const { t } = useTranslation();
	const history = useHistory();
	const params = useParams();
	const dispatch = useDispatch();
	const { currentScreen, row } = useSelector(selectScreen)
	const [resources, setResources] = useState(null)
	const [loading, setLoading] = useState(false)
	const [store, setStore] = useState(null)

	//console.warning("CalendarView", calendar, data)
	const onError = (e) => {
		notify(typeof e == "string" ? e : e.Message, "error", 5000)
	}
	useEffect(() => {
		setStore(() => {
			if (Array.isArray(currentScreen.dataSource))
				return currentScreen.dataSource
			else {
				return createCustomStore(currentScreen.dataSource, currentScreen.keyFieldName, null, null, onError)
			}
		})
	}, [currentScreen.dataSource, currentScreen.keyFieldName])

	useEffect(() => {
		if (name && currentScreen) {
			if (currentScreen.calendar) {
				var resourcesCopy = currentScreen.calendar.resources.map(r => { return { ...r } })
				resourcesCopy.map(async (r, i) => {
					let ds = r.dataSource
					if (!Array.isArray(r.dataSource)) {
						let loadUrl = ds && ds.load
							? (`${ds.load}${ds.load.indexOf('?') > -1 ? '&' : '?'}${'name=' + (ds.name)}`)
							: `/api/entity?${'name=' + (ds.name)}`
						let status = await api.execute("GET", loadUrl)
						r.dataSource = status.Resource.Items
					}
				})
				setResources(resourcesCopy)
			}
		}
	}, [currentScreen, name])

	return <>
		<Grid container spacing={2} alignItems="stretch">
			<Grid item xs={12}>
				<Box pl={1} pr={1}>
					{(!loading && currentScreen.calendar)
						? <CalendarView
							name={name}
							keyFieldName={currentScreen.keyFieldName}
							dataSource={store}
							calendar={currentScreen.calendar}
							resources={resources || []}
						/>
						: <div>Loading...</div>
					}
				</Box>
			</Grid>
		</Grid>
	</>
}

const CalendarView = React.memo(({ name, keyFieldName, dataSource, calendar, resources }) => {

	const schedulerRef = React.useRef(null)

	const onAppointmentAdding = (e) => {
		return e;
	}

	const onAppointmentDeleting = (e) => {
		// e.appointmentData
		return e;
	}

	const onAppointmentUpdating = async (e) => {
		return e;
	}

	const onAppointmentUpdated = (e) => {
		return e;
	}

	return <Scheduler
		ref={schedulerRef}
		views={views}
		defaultCurrentView="week"
		dataSource={dataSource}
		editing={calendar.editing}
		startDateExpr={calendar.startDateExpr}
		endDateExpr={calendar.endDateExpr}
		allDayExpr={calendar.allDayExpr}
		textExpr={calendar.textExpr}
		descriptionExpr={calendar.descriptionExpr}
		// height={1024}
		startDayHour={6}
		endDayHour={24}
		firstDayOfWeek={0}
		showAllDayPanel={true}
		//groupByDate={true}
		adaptivityEnabled={true}
		// onAppointmentAdding={onAppointmentAdding}
		// onAppointmentDeleting={onAppointmentDeleting}
		// onAppointmentUpdating={onAppointmentUpdating}
		// onAppointmentUpdated={onAppointmentUpdated}
		onAppointmentFormOpening={(e) => {
			const form = e.form;
			let mainGroupItems = form.itemOption('mainGroup').items;
			if (!mainGroupItems.find(function (i) { return i.dataField === "Note" })) {
				mainGroupItems.push({
					colSpan: 2,
					label: { text: "Note" },
					editorType: "dxTextBox",
					dataField: "Note"
				});
			}
			mainGroupItems[2].items.splice(1, 1)
			form.itemOption('mainGroup', 'items', mainGroupItems);
		}}
		resources={resources}
	>
	</Scheduler>
})

export default CalendarWrapper