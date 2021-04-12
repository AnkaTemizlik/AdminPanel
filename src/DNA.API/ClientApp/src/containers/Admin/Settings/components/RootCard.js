
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { Grid, Paper, Box } from "@material-ui/core";
import Field from './Field'
import ObjectCard from './ObjectCard'
import FormCard from './FormCard'

// TODO: 
const RootCard = (props) => {
	return <div>ROOT CARD {props.children}</div>;
};

export default RootCard;