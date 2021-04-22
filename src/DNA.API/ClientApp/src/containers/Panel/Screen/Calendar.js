import React, { useEffect, useState } from "react";
import { useSelector, useDispatch } from 'react-redux';
import { useHistory, useParams, useRouteMatch, useLocation, Link } from "react-router-dom";
import { Box, Grid, Icon, IconButton, Paper, Toolbar, Tooltip } from "@material-ui/core";
import { useTranslation } from "../../../store/i18next";
import { selectScreen, getById } from './store/screenSlice'
import Scheduler, { Resource } from 'devextreme-react/scheduler';
import createCustomStore from "../../../components/UI/Table/createCustomStore";
import CustomStore from "devextreme/data/custom_store";
import DataSource from "devextreme/data/data_source";
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

	useEffect(() => {
		setStore(() => {
			return Array.isArray(currentScreen.dataSource)
				? currentScreen.dataSource
				: createCustomStore(currentScreen.dataSource, currentScreen.keyFieldName, null, null)
		})
	}, [currentScreen.dataSource, currentScreen.keyFieldName])

	useEffect(() => {
		setLoading(true)
		if (name && currentScreen) {
			if (currentScreen.calendar) {
				var resourcesCopy = currentScreen.calendar.resources.map(r => { return { ...r } })
				resourcesCopy.map((r, i) => {
					let ds = Array.isArray(r.dataSource)
						? r.dataSource
						: new DataSource({ store: createCustomStore(r.dataSource, r.valueExpr), paginate: false })
					r.dataSource = ds
					console.success("CalendarView resourcesCopy", r)
				})
				setResources(resourcesCopy)
			}
		}
		setLoading(false)
	}, [currentScreen, name])

	return <>
		<Grid container spacing={2} alignItems="stretch">
			<Grid item xs={12} lg={10} xg={8}>
				<Box pl={1} pr={1}>
					{(!loading && currentScreen.calendar && resources)
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
		// e.appointmentData
		console.info("CalendarView onAppointmentAdding", e)
		return e;
	}

	const onAppointmentDeleting = (e) => {
		// e.appointmentData
		console.info("CalendarView onAppointmentDeleting", e)
		return e;
	}

	const onAppointmentUpdating = async (e) => {
		// e.newData, e.oldData
		// let data = { ...e.newData }
		// let id = data[keyFieldName]
		// delete data[keyFieldName]
		// delete data.CreationTime
		// delete data.UpdateTime
		// delete data.UpdatedBy
		// delete data.CreatedBy
		// console.info("CalendarView onAppointmentUpdating", data, e)
		// var status = await dataSource.update(id, data)
		// console.success("CalendarView", status)
		//setData(status.Resource)
		return e;
	}

	const onAppointmentUpdated = (e) => {
		// row: e.appointmentData
		console.purple("CalendarView onAppointmentUpdated", e)

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
		resources={resources}
	>
	</Scheduler>
})

export default CalendarWrapper