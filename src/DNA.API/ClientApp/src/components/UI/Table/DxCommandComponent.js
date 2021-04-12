import React from "react";
import { IconButton } from "@material-ui/core";
import DeleteIcon from '@material-ui/icons/DeleteOutlineTwoTone';
import AddIcon from '@material-ui/icons/AddCircleOutlineTwoTone';
import EditIcon from '@material-ui/icons/EditOutlined';
import CheckIcon from '@material-ui/icons/CheckTwoTone';
import CancelIcon from '@material-ui/icons/CancelSharp';
import Tooltip from '@material-ui/core/Tooltip';

const CommandButton = ({ onExecute, icon, text, hint, color, disabled }) => (
    <IconButton
        color="primary"
        style={{ opacity: 0.64, padding: "6px" }}
        //edge="start"
        onClick={e => {
            onExecute();
            e.stopPropagation();
        }}
        disabled={disabled}>
        <Tooltip title={hint}>
            {React.createElement(icon)}
        </Tooltip>
    </IconButton>
);

const DeleteButton = ({ onExecute }) => (
    <CommandButton
        icon={DeleteIcon}
        hint="Delete"
        onExecute={() => {
            //if (window.confirm("Are you sure you want to delete this row?")) {
            onExecute();
            //}
        }}
    />
);

const AddButton = ({ onExecute }) => (
    <CommandButton
        icon={AddIcon}
        hint="Add new"
        onExecute={onExecute}
    />
);

const EditButton = ({ onExecute }) => (
    <CommandButton
        icon={EditIcon}
        hint="Edit"
        onExecute={onExecute}
    />
);

const CommitButton = ({ onExecute, disabled }) => (
    <CommandButton
        id="commit"
        icon={CheckIcon}
        hint="Commit changes"
        onExecute={onExecute}
        disabled={disabled}
    />
);

const CancelButton = ({ onExecute }) => (
    <CommandButton
        icon={CancelIcon}
        hint="Cancel"
        onExecute={onExecute}
    />
);

const commandComponents = {
    add: AddButton,
    edit: EditButton,
    delete: DeleteButton,
    cancel: CancelButton,
    commit: CommitButton,
};

const Command = ({ id, onExecute, disabled }) => {
    const ButtonComponent = commandComponents[id];
    return <ButtonComponent onExecute={onExecute} disabled={disabled} />;
};

export default Command;
