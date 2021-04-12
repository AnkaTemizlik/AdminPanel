import React, { useState, useEfect, useEffect } from 'react'
import _ from 'lodash'
import { Box, Card, CardActions, CardContent, CardHeader, Chip, Collapse, FormControl, FormControlLabel, Grid, Icon, IconButton, Switch, Tooltip, Zoom } from '@material-ui/core'
import { red } from '@material-ui/core/colors';
import CheckIcon from '@material-ui/icons/Check';
import SendIcon from '@material-ui/icons/Send';
import { useTranslation } from "../../store/i18next";
import SelectField from '../UI/SelectField'
import TextField from '../UI/TextField'
import { OperatorIcon } from '../UI/Icons/Operator';
import OperatorSelect from '../UI/Editors/OperatorSelect';

const AdvancedFilter = ({ fields, onChange, defaultValue }) => {

	const { t } = useTranslation();
	const [conditions, setConditions] = useState([]);
	const [field, setField] = useState("");
	const [column, setColumn] = useState(null);
	const [operator, setOperator] = useState("");
	//const [showOperator, setShowOperator] = useState(true);
	const [value, setValue] = useState(undefined);
	const [edit, setEdit] = useState(true);
	const [valid, setValid] = useState(true);

	const handleAddCondition = () => {

		if (field && operator && (value == !undefined)) {
			setValid(true)
			setConditions((c) => {
				if (!c)
					c = []
				let exists = c.find((r) => r.field == field && r.operator.id == operator.id && r.value == value)
				if (exists)
					return c
				let e = [...c]
				e.push({ field, operator, value })
				return e
			})
			setEdit(false)
		}
		else
			setValid(false)
	}

	const handleConditionDelete = (e) => {
		setConditions((c) => {
			if (!c) return c
			return c.filter((r) => !(r.field == e.field && r.operator == e.operator && r.value == e.value))
		})
	}

	const handleFieldChange = (val) => {
		let selected = _.find(fields, { name: val });
		if (selected && selected.ref)
			selected = _.find(fields, { name: selected.ref });
		setColumn(selected)
		setField(selected.name)
	}

	const handleOperatorChange = (o) => {
		setOperator(o)
	}

	const handleValueChange = (v) => {
		setValue(v)
	}
	const sendValues = () => {
		var expressions = conditions.map((c) => {
			return { field: c.field, operator: c.operator.id, value: c.value }
		})
		onChange && onChange(expressions)
	}

	useEffect(() => {
		setConditions(defaultValue)
	}, [defaultValue]);

	const renderField = () => {
		let textField = <TextField
			label={t("Değer")}
			style={{ marginTop: 4, width: 120 }}
			onChange={(e) => handleValueChange(e.target.value)}
			required />

		if (column) {
			switch (column.type) {
				case "numeric":

					return <TextField
						label={t("Değer")}
						style={{ marginTop: 4, width: 120 }}
						onChange={(e) => handleValueChange(e.target.value)}
						required
						type="number" />
				case "datetime":

					return <TextField
						label={t("Değer")}
						style={{ marginTop: 4, width: 120 }}
						onChange={(e) => handleValueChange(e.target.value)}
						required
					//defaultValue="2017-05-24T10:30"
					//type="datetime-local"
					/>
				case "check":
					return renderCheck()
				default:
					break;
			}
		}
		return textField
	}

	const renderCheck = () => {
		return <FormControl style={{ padding: "4px 0 0 8px" }}>
			<Switch
				defaultChecked={false}
				onChange={(e) => {
					handleValueChange(e.target.checked);
				}}
			/>
		</FormControl>
	}

	return <Card>

		{/* Conditions */}
		<Collapse in={conditions.length > 0}>
			<CardContent style={{ paddingBottom: 8 }}>
				<Grid container spacing={1}>
					{conditions.map((c, i) => {
						return <Tooltip title={c.operator ? (c.operator.title || c.operator.value) : ""} key={i}>
							<Grid item >
								<Chip label={
									<Box display="flex" alignItems="center">
										<span>{c.field}</span>
										<Box style={{ paddingLeft: 4, paddingRight: 4 }} display="flex" alignItems="center">
											<OperatorIcon name={c.operator.value} fontSize="small" />
										</Box>
										<Box pr={1} display="flex" alignItems="center">
											<span>{JSON.stringify(c.value)}</span>
										</Box>
									</Box>
								}
									onDelete={() => handleConditionDelete(c)}
									color="primary"
									variant="outlined" />
							</Grid>
						</Tooltip>
					})}
				</Grid>
			</CardContent>
		</Collapse>

		<CardActions //disableSpacing
		>
			<Box pl={1} flexGrow="1">
				{/* Add Condition fields */}
				<Grid container direction="row" justify="space-between" >

					<Grid item>
						<Grid container spacing={1}>
							<Grid item>
								<SelectField label="Alan"
									items={fields}
									onChange={handleFieldChange}
									valueField="name"
									captionField="title"
									style={{ minWidth: 120 }}
									required
								/>
							</Grid>

							{field &&
								<Grid item>
									<OperatorSelect
										onChange={handleOperatorChange}
										valueField="id"
										captionField="title"
									/>
								</Grid>
							}

							<Grid item>
								{renderField()}
							</Grid>

						</Grid>
					</Grid>

					<Grid item>

					</Grid>

					<Grid item>

						{/* Warning */}
						<Zoom in={!valid}>
							<Tooltip title={t("Gerekli alanları doldurun")}>
								<span>
									<IconButton>
										<Icon style={{ color: red[600] }}>warning</Icon>
									</IconButton>
								</span>
							</Tooltip>
						</Zoom>

						{/* OK */}
						<Tooltip title={t("Ekle")}>
							<span>
								<IconButton onClick={handleAddCondition}>
									<CheckIcon />
								</IconButton>
							</span>
						</Tooltip>

						<Tooltip title={t("Uygula")}>
							<span>
								<IconButton onClick={sendValues}>
									<SendIcon />
								</IconButton>
							</span>
						</Tooltip>

					</Grid>
				</Grid>
			</Box>
		</CardActions>
	</Card>
}

export default AdvancedFilter