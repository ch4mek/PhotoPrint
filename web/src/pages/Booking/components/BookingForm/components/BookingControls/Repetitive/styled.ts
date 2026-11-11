import styled from '@emotion/styled';
import Accordion from '@mui/material/Accordion';
import AccordionSummary from '@mui/material/AccordionSummary';

import { switchGridAreas } from '../constants';

export const StyledAccordion = styled(Accordion)({
	gridArea: switchGridAreas.repetativeAccordion,

	backgroundColor: 'transparent',
	'& .MuiPaper-root, & .MuiAccordionSummary-content': {
		margin: 0,
	},
	'& .MuiAccordionSummary-root, & .MuiAccordionDetails-root': {
		padding: 0,
	},
	'&::before': {
		display: 'none',
	},
});

export const StyledAccordionSummary = styled(AccordionSummary)({
	minHeight: 0,
});

export const RepetativeSwitchContainer = styled('label')(({ theme }) => ({
	gridArea: switchGridAreas.repetativeSwitch,
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'center',
	color: theme.palette['text-gray'],
}));
