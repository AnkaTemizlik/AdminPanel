import React, { useState } from 'react';
import maxBy from 'lodash/maxBy'
import { withStyles } from '@material-ui/core/styles';
import {
	Chart,
	BarSeries,
	LineSeries,
	AreaSeries,
	PieSeries,
	ArgumentAxis,
	ValueAxis,
	Tooltip,
	Legend
} from '@devexpress/dx-react-chart-material-ui';

import { Animation, Stack, EventTracker, HoverState, ValueScale } from '@devexpress/dx-react-chart';
import { useTranslation } from '../../../store/i18next'

const legendStyles = () => ({
	root: {
		display: 'flex',
		margin: 'auto',
		flexDirection: 'row',
		' & li': {
			paddingLeft: 4,
			paddingRight: 4
		},
		paddingBottom: 0
	},
});
const legendRootBase = ({ classes, ...restProps }) => (
	<Legend.Root {...restProps} className={classes.root} />
);
const Root = withStyles(legendStyles, { name: 'LegendRoot' })(legendRootBase);

const legendLabelStyles = () => ({
	label: {
		whiteSpace: 'nowrap',
		paddingLeft: 4
	},
});
const legendLabelBase = ({ classes, ...restProps }) => (
	<Legend.Label className={classes.label} {...restProps} />
);
const Label = withStyles(legendLabelStyles, { name: 'LegendLabel' })(legendLabelBase);

const DxChart = ({ data, series, argumentField, rotated }) => {

	const { t } = useTranslation()
	const [tooltipTarget, setTooltipTarget] = useState(null)

	const changeHover = target => {
		setTooltipTarget(target ? { series: target.series, point: target.point } : null)
	};

	function getArgumentAxis() {
		for (let i = 0; i < series.length; i++) {
			const serie = series[i];
			if (serie.type != "pie")
				return <ArgumentAxis
					tickFormat={(x) => {
						console.info("DxChart tickFormat", x)
					}}
				/>
		}
		return null
	}

	function getValueAxis() {
		for (let i = 0; i < series.length; i++) {
			const serie = series[i];
			if (serie.type != "pie") {
				let max = maxBy(data, serie.valueField)[serie.valueField];
				max = max - (max % 10) + 10
				return <>
					<ValueAxis />
					<ValueScale modifyDomain={(domain) => {
						return [0, max]
					}} />
				</>
			}
		}
		return null
	}

	const TooltipContent = (props) => {
		const { targetItem, text, ...restProps } = props;
		return (
			// <div>
			// 	<div>
			<Tooltip.Content
				{...restProps}
				style={{
					fontWeight: 'bold',
					paddingBottom: 0
				}}
				text={text}
			/>
			//	</div>
			//	<div>
			//		<Tooltip.Content
			//			{...restProps}
			//			style={tooltipContentBodyStyle}
			//			text={data[targetItem.point].Count}
			//		/>
			//	</div>
			//</div> 
		);
	};

	return <Chart
		rotated={rotated == true}
		height="300"
		data={data}
	>
		{getArgumentAxis()}
		{getValueAxis()}

		{series.map((s, i) => {

			if (s.type == "bar")
				return <BarSeries key={i}
					name={t(s.title)}
					valueField={s.valueField}
					argumentField={argumentField}
					color={s.color}
				/>

			if (s.type == "line")
				return <LineSeries key={i}
					name={t(s.title)}
					valueField={s.valueField}
					argumentField={argumentField}
					color={s.color}
				/>

			if (s.type == "area")
				return <AreaSeries key={i}
					name={t(s.title)}
					valueField={s.valueField}
					argumentField={argumentField}
					color={s.color}
				/>

			if (s.type == "pie") {
				return <PieSeries key={i}
					name={t(s.title)}
					valueField={s.valueField}
					argumentField={argumentField}
					color={s.color}
				//onClick={(e) => handleClick(s, argumentField)}
				/>
			}
		})}
		<Stack />
		<EventTracker />
		<Tooltip
			targetItem={tooltipTarget}
			contentComponent={TooltipContent}
		/>
		<HoverState
			hover={tooltipTarget}
			onHoverChange={changeHover}
		/>
		<Legend position="bottom" rootComponent={Root} labelComponent={Label} />
		<Animation />
	</Chart>
}


export default DxChart