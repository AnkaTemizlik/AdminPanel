import React, { useState } from 'react'
import Modal from './Modal'
import { Typography, Box, IconButton, Tooltip, List, ListItem, ListItemText, ListItemSecondaryAction, Divider } from '@material-ui/core';
import Alert from '@material-ui/lab/Alert';
import DeleteIcon from '@material-ui/icons/Delete';
import AddIcon from '@material-ui/icons/Add';
import CheckIcon from '@material-ui/icons/Check';
import TextField from './TextField';
import { Trans, Tr, useTranslation } from '../../store/i18next'

const EmailAddressesEditModal = (props) => {

	const { t } = useTranslation()
	const { open, ok, cancel, title } = props

	const [modalOpen, setModalOpen] = useState({ open: open });
	const [addresses, setAddresses] = useState([]);
	const [address, setAddress] = useState(null);
	const [error, setError] = useState(null);

	const handleModalCancel = () => {
		cancel();
	}

	const handleModalConfirm = () => {
		ok(addresses);
	};

	const removeItem = (i) => {
		let list = [...addresses];
		list.splice(i, 1);
		setAddresses(list);
		setError(null)
	}

	const addNew = (i) => {
		setError(null)
		if (!address) {
			setAddress({ name: "", email: "" })
		}
	}

	const saveNew = () => {
		setError(null)
		if (!address.email) {
			setError(t("Type an e-mail address"))
			return;
		}
		addresses.push(address);
		setAddresses(addresses);
		setAddress(null)
	}

	return <Modal
		open={open}
		onClose={handleModalCancel}
		title={title}
		params={modalOpen}
		ok={handleModalConfirm}
		cancel={handleModalCancel}
		okText="Ok"
		cancelText="Cancel"
	>
		{error && <Box pb={2}><Alert severity="warning"><Tr>{error}</Tr></Alert></Box>}

		<List dense>
			{addresses.map((a, i) => {
				return <ListItem key={i} >
					<ListItemText
						primary={a.name}
						secondary={a.email}
					/>
					<ListItemSecondaryAction>
						<Tooltip title={t("Remove")}>
							<span>
								<IconButton edge="end" 
									onClick={() => { removeItem(i) }}
								>
									<DeleteIcon />
								</IconButton>
							</span>
						</Tooltip>
					</ListItemSecondaryAction>
				</ListItem>
			})}

			{addresses.length > 0 && <Divider component="li" />}

			{!address && <ListItem>
				<ListItemText
					primary="&nbsp;"
					secondary="&nbsp;"
				/>
				<ListItemSecondaryAction>
					<Tooltip title={t("Add New")}>
						<span>
							<IconButton edge="end" onClick={addNew}>
								<AddIcon />
							</IconButton>
						</span>
					</Tooltip>
				</ListItemSecondaryAction>
			</ListItem>}


			{address && <ListItem>
				<ListItemText>
					<Box display="flex">
						<Box>
							<TextField variant="outlined"
								id="name"
								label={t("Name")}
								name="name"
								onChange={(e) => setAddress({ ...address, name: e.target.value })}
								required
							/>
						</Box>
						<Box pl={1}>
							<TextField variant="outlined"
								id="email"
								label={t("Email")}
								name="email"
								onChange={(e) => setAddress({ ...address, email: e.target.value })}
								required
							/>
						</Box>
					</Box>
				</ListItemText>

				<ListItemSecondaryAction>
					<Tooltip title={t("Save")}>
						<span>
							<IconButton edge="end" onClick={saveNew}>
								<CheckIcon />
							</IconButton>
						</span>
					</Tooltip>
				</ListItemSecondaryAction>
			</ListItem>}

		</List>

	</Modal >
}

export default EmailAddressesEditModal