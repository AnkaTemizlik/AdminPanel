import React, { useState } from "react";
import DeleteIcon from "@material-ui/icons/Delete";
import AddIcon from "@material-ui/icons/Add";
import { Box, FormControl, IconButton, InputLabel, OutlinedInput, Tooltip } from "@material-ui/core";
import TextField from "../../../../components/UI/TextField";
import CollapsibleCard from "../../../../components/UI/CollapsibleCard";
import { useTranslation } from "../../../../store/i18next";

const KeyValueEditor = ({ field, onChange, h, loading }) => {
	const { t } = useTranslation()
	const { fullname, value, options } = field
	const { caption, require, valueCaption, keyCaption, valueType } = options

	var [list, setState] = useState(value);

	const addNew = () => {
		if (list.length > 0) {
			let last = list[list.length - 1]
			if (last.Key == "" && last.Value == "")
				return
		}
		let l = [...list];
		l.push({ Key: "", Value: "" });
		setState(l);
	};

	const removeItem = (i) => {
		let l = [...list];
		l.splice(i, 1);
		setState(l);
		onChange(l);
	};

	const nameChanged = (i, val) => {
		let l = [...list];
		let value = l[i].Value
		l.splice(i, 1, { Key: val, Value: value })
		setState(l);
		onChange(l);
	};

	const valueChanged = (i, val) => {
		let l = [...list];
		let key = list[i].Key
		l.splice(i, 1, { Key: key, Value: val })
		setState(l);
		onChange(l);
	};

	return (
		<Box mb={2} mt={1}>
			<CollapsibleCard
				title={caption}
				header={h}
				exp={h == "h6"}
				addComponent={
					<Tooltip title="Yeni">
						<span>
							<IconButton onClick={addNew} edge="end">
								<AddIcon />
							</IconButton>
						</span>
					</Tooltip>
				}
			>
				{list &&
					list.map((a, i) => {
						return (
							<Box key={i} display="flex"  >

								<FormControl size="small" margin="normal" variant="outlined">
									<InputLabel id={fullname}>{t(keyCaption)}</InputLabel>
									<OutlinedInput
										type={options.inputType || "text"}
										labelId={fullname}
										label={keyCaption}
										id={`${fullname}${i}name`}
										name={`${fullname}${i}name`}
										onChange={(e) => {
											nameChanged(i, e.target.value);
										}}
										required={require}
										disabled={loading}
										value={a.Key}
									/>
								</FormControl>

								<FormControl size="small" margin="normal" variant="outlined" style={{ marginLeft: 4 }}>
									<InputLabel id={fullname}>{t(valueCaption)}</InputLabel>
									<OutlinedInput
										id={`${fullname}${i}value`}
										label={valueCaption}
										name={`${fullname}${i}value`}
										onChange={(e) => {
											valueChanged(i, e.target.value);
										}}
										required={require}
										disabled={loading}
										value={a.Value}
										type={valueType || "text"}
									/>
								</FormControl>
								{i > 0 && <Box pt={1.5}>
									<Tooltip title={t("Delete")}>
										<span>
											<IconButton
												onClick={() => {
													removeItem(i);
												}}
												edge="end"
												disabled={loading}
											>
												<DeleteIcon />
											</IconButton>
										</span>
									</Tooltip>
								</Box>
								}
							</Box>
						);
					})}
			</CollapsibleCard>
		</Box>
	);
};

export default KeyValueEditor