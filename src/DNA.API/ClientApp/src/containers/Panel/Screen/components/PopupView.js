import React, { useState, useEffect } from "react";
import { Box } from "@material-ui/core";
import Modal from "../../../../components/UI/Modal";
import notify from 'devextreme/ui/notify'
import TableView from "./TableView";

const PopupView = ({ model, onClose }) => {
	const [modalOpen, setModalOpen] = useState(false);
	const [error, setError] = useState(null);

	const handleModalConfirm = (e) => {
		setModalOpen(false);
		onClose()
	};

	const handleModalCancel = (params) => {
		setModalOpen(false);
		onClose()
	};

	useEffect(() => {
		setModalOpen(model != null)
	}, [model]);

	useEffect(() => {
		if (error) {
			notify(error, "error")
			setError(null)
		}
	}, [error]);

	return <Modal
		open={modalOpen}
		onClose={handleModalCancel}
		title={model.title}
		ok={handleModalConfirm}
		cancel={handleModalCancel}
		okText="Close"
	>
		<Box>
			<TableView model={model} />
		</Box>
	</Modal>

}

export default PopupView;