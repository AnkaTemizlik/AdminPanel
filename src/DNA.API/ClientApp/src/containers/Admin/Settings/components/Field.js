
import React, { useEffect, useState } from "react";
import { FormControl, FormControlLabel, InputLabel, Select, MenuItem, Switch, OutlinedInput } from "@material-ui/core";
import PasswordField from "../../../../components/UI/PasswordField";
import AutoCompleteAdornment from "./AutoCompleteAdornment";
import { useTranslation } from "../../../../store/i18next";

const Field = ({ field, handleChange, loading }) => {
	const { t } = useTranslation()
	const { value, name, fullname, _, options } = field
	const { caption, password, require, selectList, autoComplete, textArea, check, action, multiline, inputType } = options
	const [activeField, setActiveField] = useState(null);
	const [val, setVal] = useState(undefined);

	useEffect(() => {
		setVal(value)
	}, [value])

	const renderSelect = () => {
		return <FormControl variant="outlined" fullWidth margin="normal" size="small" key={fullname}>
			<InputLabel id={fullname}>{t(caption)}</InputLabel>
			<Select
				labelId={fullname}
				id={fullname}
				defaultValue={value}
				onChange={(e) => {
					handleChange(fullname, e.target.value);
				}}
				onFocus={(e) => setActiveField(e.target)}
				label={caption}
			>
				<MenuItem value="">
					<em></em>
				</MenuItem>
				{selectList.map((item, i) => {
					return (
						<MenuItem value={item.value} key={i}>
							{item.caption}
						</MenuItem>
					);
				})}
			</Select>
		</FormControl>
	}

	const renderPassword = () => {
		return <PasswordField
			variant="outlined"
			autoComplete="off"
			id={fullname}
			key={fullname}
			label={t(caption)}
			name={fullname}
			onChange={(e) => {
				handleChange(fullname, e.target.value);
			}}
			onFocus={(e) => setActiveField(e.target)}
			required={require}
			disabled={loading}
			defaultValue={"********"}
			multiline={false}
		/>
	}

	const renderCheck = () => {
		return <FormControl component="fieldset" key={fullname} size="small" margin="normal" >
			<FormControlLabel
				label={t(caption)}
				control={
					<Switch
						id={fullname}
						defaultChecked={value}
						onChange={(e) => {
							handleChange(fullname, e.target.checked);
						}}
						name={fullname}
						disabled={loading}
					/>
				}
			/>
		</FormControl>
	}

	const renderText = () => {
		//let multiline = typeof value == "string";
		return <FormControl fullWidth size="small" variant="outlined" margin="normal">
			<InputLabel htmlFor={fullname}>{t(caption)}</InputLabel>
			<OutlinedInput
				type={options.inputType || "text"}
				id={fullname}
				key={fullname}
				label={caption}
				name={fullname}
				onChange={(e) => {
					setVal(e.target.value)
					handleChange(fullname, e.target.value);
				}}
				onFocus={(e) => {
					setActiveField(e.target.name)
				}}
				required={require}
				disabled={loading}
				value={val || ''}
				multiline={textArea || multiline}
				endAdornment={(autoComplete && activeField === fullname)
					? <AutoCompleteAdornment
						value={val || ''}
						caption={caption}
						autoComplete={autoComplete}
						handleChange={(v) => {
							setVal(v)
							handleChange(fullname, v)
						}}
					/>
					: null
				}
			/>
		</FormControl>
	}

	return selectList
		? renderSelect()
		: password
			? renderPassword()
			: (check || typeof value == "boolean")
				? renderCheck()
				: renderText()
};

export default Field