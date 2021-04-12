import React, { useEffect, useState } from "react";
import TextArrayEditor from './TextArrayEditor'
import KeyValueEditor from './KeyValueEditor'

const FormCard = (props) => {
	const { field, h, handleChange, loading } = props
	const { fullname, options } = field
	const { textArray, keyValue, visible } = options

	if (!visible)
		return null;

	return (
		<>
			{textArray ? (
				<TextArrayEditor
					field={field}
					h={h}
					loading={loading}
					onChange={(v) => {
						handleChange(fullname, v);
					}}
				/>
			) : null}
			{keyValue ? (
				<KeyValueEditor
					field={field}
					loading={loading}
					h={h}
					onChange={(v) => {
						handleChange(fullname, v);
					}}
				/>
			) : null}
		</>
	);
};

export default FormCard