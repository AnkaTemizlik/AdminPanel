import React, { useState } from "react";
import DeleteIcon from "@material-ui/icons/Delete";
import AddIcon from "@material-ui/icons/Add";
import { Box, IconButton, Tooltip, InputAdornment, FormControl } from "@material-ui/core";
import TextField from "../../../../components/UI/TextField";
import CollapsibleCard from "../../../../components/UI/CollapsibleCard";

const TextArrayEditor = (props) => {
	const { field, onChange, h, loading } = props
	const { fullname, value, options } = field
	const { caption, require, valueCaption, keyCaption, valueType, multiline } = options
	const readOnly = props.readOnly == true || options.readOnly == true

	var [state, setState] = useState({ list: value });

	const addNew = () => {
		if (state.list.length == 0 || state.list[state.list.length - 1] == "") return;
		const list = [...state.list];
		list.push("");
		setState({ list: list });
	};

	const removeItem = (i) => {
		let list = [...state.list];
		list.splice(i, 1);
		setState({ list: list });
		onChange(list);
	};

	const textChanged = (i, val) => {
		let list = [...state.list];
		list.splice(i, 1, val)
		setState({ list: list });
		onChange(list);
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
							<IconButton onClick={addNew} edge="end" disabled={readOnly}>
								<AddIcon />
							</IconButton>
						</span>
					</Tooltip>
				}
			>
				{state.list &&
					state.list.map((a, i) => {
						return (
							<FormControl key={i} fullWidth variant="outlined">
								<TextField
									variant="outlined"
									id={`${fullname}${i}`}
									multiline={multiline == true}
									name={`${fullname}${i}`}
									onChange={(e) => {
										textChanged(i, e.target.value);
									}}
									//onFocus={(e) => setActiveField(e.target)}
									required={require}
									disabled={readOnly}
									value={a}
									InputProps={{
										endAdornment: (
											<InputAdornment position="end">
												<Tooltip title="Sil">
													<span>
														<IconButton
															aria-label="toggle password visibility"
															onClick={() => {
																removeItem(i);
															}}
															edge="end"
															size="small"
															disabled={readOnly}
														>
															<DeleteIcon />
														</IconButton>
													</span>
												</Tooltip>
											</InputAdornment>
										),
									}}
								/>
							</FormControl>
						);
					})}
			</CollapsibleCard>
		</Box>
	);
};

export default TextArrayEditor