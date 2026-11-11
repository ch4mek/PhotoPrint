import styled from '@emotion/styled';

import { popUpBreakpoints } from '@/constants/popUp';

import { ProjectFormGridArea } from './constants';

const { InputFields, RadioFields, ColorsFields } = ProjectFormGridArea;

const INPUT_FIELDS_WIDTH = 350;
const RADIO_FIELDS_WIDTH = 383;

const Column = styled('div')({
	display: 'flex',
	flexDirection: 'column',
});

export const FormContentWrapper = styled(Column)(({ theme }) => ({
	width: '100%',
	gap: theme.spaces.l,

	[theme.breakpoints.up(popUpBreakpoints.tablet)]: {
		display: 'grid',
		gridTemplateColumns: `${INPUT_FIELDS_WIDTH}px auto`,
		columnGap: theme.spaces.xl4,
		gridTemplateAreas: `
		'${InputFields} ${RadioFields}'
		'${InputFields} ${RadioFields}'
		'${InputFields} ${RadioFields}'
		'${InputFields} ${RadioFields}'
		'${ColorsFields} ${RadioFields}'`,
	},

	[theme.breakpoints.up(popUpBreakpoints.tabletXL)]: {
		gridTemplateColumns: `${INPUT_FIELDS_WIDTH}px ${RADIO_FIELDS_WIDTH}px`,
	},
}));

export const InputFieldsWrapper = styled(Column)(({ theme }) => ({
	gridArea: InputFields,
	gap: theme.spaces.l,
}));

export const RadioFieldsWrapper = styled(Column)(({ theme }) => ({
	gridArea: RadioFields,
	gap: theme.spaces.m,
}));

export const ColorsFieldsWrapper = styled(Column)(({ theme }) => ({
	gridArea: ColorsFields,
	gap: theme.spaces.m,
}));

export const ProjectTypeWrapper = styled(Column)(({ theme }) => ({
	gap: theme.spaces.s,
	color: theme.palette['text-gray'],
}));

export const RadioButtonsWrapper = styled('div')<{ gap?: number }>(
	({ gap }) => ({
		display: 'flex',
		alignItems: 'center',
		gap: gap || 0,
	})
);
