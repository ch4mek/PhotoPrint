import styled from '@emotion/styled';
import { Form } from 'formik';

import { popUpBreakpoints } from '@/constants/popUp';

import {
	COLUMN_GAP,
	MainFormField,
	MAIN_FORM_GRID_DEFAULT_AREAS,
	MAIN_FORM_GRID_FOURTH_ROW_AREAS,
	MAIN_FORM_GRID_TEMPLATE_AREAS,
	MAIN_FORM_GRID_TEMPLATE_COLUMNS,
	MAIN_FORM_GRID_THIRD_ROW_AREAS,
} from './constants';
import { getGridAreaStyles } from './utils/getGridAreaStyles';

export const FormContent = styled(Form)(({ theme }) => ({
	display: 'grid',
	flexWrap: 'wrap',
	rowGap: theme.spaces.l,
	columnGap: COLUMN_GAP,
	gridTemplateColumns: MAIN_FORM_GRID_TEMPLATE_COLUMNS.default,
	gridTemplateAreas: MAIN_FORM_GRID_TEMPLATE_AREAS.default,

	...getGridAreaStyles(MAIN_FORM_GRID_DEFAULT_AREAS),

	'div:nth-of-type(8)': {
		gridArea: MainFormField.THIRD_ROW,
		display: 'grid',
		columnGap: COLUMN_GAP,
		gridTemplateColumns: MAIN_FORM_GRID_TEMPLATE_COLUMNS.defaultThirdRow,
		gridTemplateAreas: MAIN_FORM_GRID_TEMPLATE_AREAS.defaultThirdRow,
		...getGridAreaStyles(MAIN_FORM_GRID_THIRD_ROW_AREAS),
	},

	'div:nth-of-type(9)': {
		gridArea: MainFormField.FOURTH_ROW,
		display: 'grid',
		columnGap: COLUMN_GAP,
		gridTemplateColumns: MAIN_FORM_GRID_TEMPLATE_COLUMNS.equalRow,
		gridTemplateAreas: MAIN_FORM_GRID_TEMPLATE_AREAS.defaultFourthRow,
		...getGridAreaStyles(MAIN_FORM_GRID_FOURTH_ROW_AREAS),
	},

	'div:nth-of-type(10)': {
		gridArea: MainFormField.CHARACTERISTICS,
	},

	'div:nth-of-type(11)': {
		gridArea: MainFormField.JIRA_TASK,
	},

	'div:nth-of-type(12)': {
		gridArea: MainFormField.COMMENT,
	},

	[theme.breakpoints.down(popUpBreakpoints.desktopXS)]: {
		gridTemplateColumns: MAIN_FORM_GRID_TEMPLATE_COLUMNS.tablet,
		gridTemplateAreas: MAIN_FORM_GRID_TEMPLATE_AREAS.tablet,

		...getGridAreaStyles(MAIN_FORM_GRID_DEFAULT_AREAS),

		'div:nth-of-type(8)': {
			gridArea: MainFormField.THIRD_ROW,
			display: 'grid',
			rowGap: theme.spaces.l,
			gridTemplateColumns: MAIN_FORM_GRID_TEMPLATE_COLUMNS.tablet,
			gridTemplateAreas: MAIN_FORM_GRID_TEMPLATE_AREAS.tabletThirdRow,
			...getGridAreaStyles(MAIN_FORM_GRID_THIRD_ROW_AREAS),
		},

		'div:nth-of-type(9)': {
			gridArea: MainFormField.FOURTH_ROW,
			display: 'grid',
			rowGap: theme.spaces.l,
			gridTemplateColumns: MAIN_FORM_GRID_TEMPLATE_COLUMNS.tablet,
			gridTemplateAreas: MAIN_FORM_GRID_TEMPLATE_AREAS.tabletFourthRow,
			...getGridAreaStyles(MAIN_FORM_GRID_FOURTH_ROW_AREAS),
		},

		'div:nth-of-type(10)': {
			gridArea: MainFormField.CHARACTERISTICS,
		},

		'div:nth-of-type(11)': {
			gridArea: MainFormField.JIRA_TASK,
		},

		'div:nth-of-type(12)': {
			gridArea: MainFormField.COMMENT,
		},
	},

	[theme.breakpoints.down(popUpBreakpoints.tabletL)]: {
		gridTemplateColumns: MAIN_FORM_GRID_TEMPLATE_COLUMNS.mobile,
		gridTemplateAreas: MAIN_FORM_GRID_TEMPLATE_AREAS.mobile,

		'div:nth-of-type(8)': {
			gridArea: MainFormField.THIRD_ROW,
			display: 'grid',
			rowGap: theme.spaces.l,
			gridTemplateColumns: MAIN_FORM_GRID_TEMPLATE_COLUMNS.mobile,
			gridTemplateAreas: MAIN_FORM_GRID_TEMPLATE_AREAS.mobileThirdRow,
			...getGridAreaStyles(MAIN_FORM_GRID_THIRD_ROW_AREAS),
		},

		'div:nth-of-type(9)': {
			gridArea: MainFormField.FOURTH_ROW,
			display: 'grid',
			rowGap: theme.spaces.l,
			gridTemplateColumns: MAIN_FORM_GRID_TEMPLATE_COLUMNS.mobile,
			gridTemplateAreas: MAIN_FORM_GRID_TEMPLATE_AREAS.mobileFourthRow,
			...getGridAreaStyles(MAIN_FORM_GRID_FOURTH_ROW_AREAS),
		},

		'div:nth-of-type(10)': {
			gridArea: MainFormField.CHARACTERISTICS,
		},

		'div:nth-of-type(11)': {
			gridArea: MainFormField.JIRA_TASK,
		},

		'div:nth-of-type(12)': {
			gridArea: MainFormField.COMMENT,
		},
	},
}));
