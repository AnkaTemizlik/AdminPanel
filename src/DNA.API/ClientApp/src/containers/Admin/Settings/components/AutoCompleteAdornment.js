import React, { useEffect, useState } from 'react'
import { IconButton, InputAdornment, Tooltip, TextField, Chip, Box, FormControl, Paper, Tabs, Tab, Toolbar } from '@material-ui/core'
import { HighlightWithinTextarea } from "react-highlight-within-textarea";
import orderBy from 'lodash/orderBy'
import { JsonCodeIcon, CodeBracesIcon } from "../../../../components/UI/Icons";
import Modal from "../../../../components/UI/Modal";
import { TabPanel } from "../../../../components/UI/Tab";
import { useTranslation } from "../../../../store/i18next";
import { splitCamelCase } from "../../../../store/utils";

const AutoCompleteAdornment = ({ value, caption, autoComplete, handleChange }) => {
	const { t } = useTranslation();
	const [open, setOpen] = useState(false);
	const [val, setVal] = useState(value || '');
	const [modelTabIndex, setModelTabIndex] = React.useState(0);

	const highlight = [{
		highlight: /{\w+}/gi,
		className: 'blue'
	}, {
		highlight: /{\w+\.\w+}/gi,
		className: 'blue'
	}]

	useEffect(() => {
		setVal(value)
	}, [value])

	if (!autoComplete)
		return null;

	const handleModalCancel = () => {
		setVal(value)
		setOpen(false)
	};

	const handleModalConfirm = () => {
		if (value != val)
			handleChange(val)
		setOpen(false)
	};

	const renderTabs = (autoComplete) => {
		return <>
			<Tabs
				value={modelTabIndex}
				indicatorColor="primary"
				textColor="primary"
				onChange={(e, val) => setModelTabIndex(val)}
			>
				{autoComplete.map((m, i) => <Tab key={i} label={m.model} disableRipple style={{ textTransform: "none" }} />)}
			</Tabs>
			<Box style={{
				height: "200px",
				overflowY: 'scroll',
			}}>
				{autoComplete.map((m, i) => (
					<TabPanel key={i} value={modelTabIndex} index={i}>
						{renderChips(m, true)}
					</TabPanel>
				))}
			</Box>
		</>
	}

	const renderChips = (autoComplete, full) => {
		return orderBy(autoComplete.fields).map((fieldName, i) => {
			var tag = full ? `{${autoComplete.model}.${fieldName}}` : `{${fieldName}}`;
			var caption = `${autoComplete.model}.${fieldName}`;
			return (
				<Tooltip title={t(caption)} key={i}>
					<Chip
						label={fieldName}
						style={{ margin: 2 }}
						clickable
						onClick={(x, y) => setVal(val + tag)}
						color="primary"
						variant="outlined"
						size="small"
					/>
				</Tooltip>
			);
		})
	}

	return <InputAdornment position="end">
		<Tooltip title={t("AutoComplete")}>
			<span>
				<IconButton edge="end" size="small" onClick={() => setOpen(true)}>
					{Array.isArray(autoComplete)
						? <JsonCodeIcon />
						: <CodeBracesIcon />}
				</IconButton>
			</span>
		</Tooltip>

		<Modal
			open={open}
			onClose={handleModalCancel}
			title={caption}
			ok={handleModalConfirm}
			cancel={handleModalCancel}
			okText="Ok"
			cancelText="Cancel"
		>
			<Box pt={1} pb={2}>
				<Paper variant="outlined" style={{ padding: 4 }}>
					<HighlightWithinTextarea
						className="MuiInputBase-root MuiOutlinedInput-root"
						containerStyle={{
							width: "100%",
							height: '130px',
						}}
						style={{
							flex: 1,
							width: "100%",
							height: '130px',
							resize: "none",
							border: 'unset'
						}}
						value={val || ''}
						onChange={(e) => {
							setVal(e.target.value)
						}}
						highlight={highlight} />

					{/* <TextField
					variant="outlined"
					label={caption}
					//defaultValue={value}
					value={val || ''}
					onChange={(e) => {
						setVal(e.target.value)
					}}
					multiline
					fullWidth
				/> */}

				</Paper>
			</Box>
			<Paper variant="outlined">
				{Array.isArray(autoComplete)
					? renderTabs(autoComplete)
					: <Box style={{
						height: "200px",
						overflowY: 'scroll',
					}}>
						{renderChips(autoComplete)}
					</Box>
				}
			</Paper>
		</Modal>
	</InputAdornment>
}

export default AutoCompleteAdornment

