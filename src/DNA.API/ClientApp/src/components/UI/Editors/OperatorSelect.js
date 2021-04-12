import React, { useState, useEffect } from 'react'
import { makeStyles, withStyles } from "@material-ui/core/styles";
import { connect, useDispatch, useSelector } from "react-redux";
import { Box, Card, CardActions, CardContent, CardHeader, Chip, Collapse, Grid, Icon, IconButton, ListItemIcon, MenuItem, Tooltip, Typography, Zoom } from '@material-ui/core'
import { OperatorIcon } from '../Icons/Operator';
import { useTranslation } from "../../../store/i18next";
import SelectField from '../SelectField';


const useStyles = makeStyles((theme) => ({
	select: {
		minWidth: 160
	},
}));

const OperatorSelect = (props) => {

	const { t } = useTranslation();
	const classes = useStyles();
	const { lists } = useSelector((state) => state.screenConfig)

	const { onChange, ...others } = props;
	const [operators] = useState(lists.OperatorTypes);
	const [operator, setOperator] = useState("");

	const handlerOperatorChange = (e) => {
		setOperator(e)
		onChange && onChange(e);
	}

	// useEffect(() => {
	// 	if (operators)
	// 		setOperator((v) => {
	// 			let d = operators[0]
	// 			if (v != d)
	// 				onChange(d);
	// 			return d
	// 		})
	// }, [operators, onChange])

	return <SelectField
		{...others}
		label="Operatör"
		className={classes.select}
		items={operators}
		onChange={handlerOperatorChange}
		required
		//value={operator || ""}
		//defaultValue={operators[0]}
		renderValue={(e) => {
			return [
				<IconButton style={{ margin: -4, padding: 0 }} key={0}>
					<OperatorIcon name={operator.value} fontSize="small" />
				</IconButton>,
				<Typography style={{ paddingLeft: 8, fontSize: '0.91rem' }} variant="inherit" component="span" key={1}>
					{t(e.value)}
				</Typography>
			]
		}}

	>
		{/* 
		<MenuItem value="">
			<em></em>
		</MenuItem> */}
		{operators.map((item, i) => {
			return (
				<MenuItem value={item} key={i}>
					<ListItemIcon>
						<OperatorIcon name={item.value} fontSize="small" />
					</ListItemIcon>
					<Typography variant="body2">{t(item.value)}</Typography>
				</MenuItem>
			);
		})}

	</SelectField>
}

export default OperatorSelect