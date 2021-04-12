import React, { useState, useEffect } from "react";
import { FormControl, MenuItem, Select, InputLabel } from "@material-ui/core";

const SelectField = (props) => {
	const { children, items, size, multiline, margin, valueField, captionField, defaultValue, ...rest } = props;
	const [val, setVal] = useState("")

	useEffect(() => {
		if (defaultValue)
			setVal((v) => {
				return defaultValue != v ? defaultValue : v
			})
	}, [defaultValue])

	return (
		<FormControl style={{ marginTop: 4 }} variant="outlined" fullWidth size={size || "small"}>
			<InputLabel id={props.id || props.label}>{props.label}</InputLabel>
			<Select {...rest}
				margin={margin || "dense"}
				value={val || ""}
				onChange={(e) => {
					setVal(e.target.value);
					props.onChange && props.onChange(e.target.value)
				}}
				required={props.required === true}
			>
				{children
					? children
					: [
						<MenuItem value="" key="-1">
							<em></em>
						</MenuItem>,
						items &&
						items.map((item, i) => {
							return (
								<MenuItem value={item[valueField] || item.value} key={i}>
									{item[captionField] || item.caption || item.value}
								</MenuItem>
							);
						})
					]
				}
			</Select>
		</FormControl>
	);
};

export default SelectField;
