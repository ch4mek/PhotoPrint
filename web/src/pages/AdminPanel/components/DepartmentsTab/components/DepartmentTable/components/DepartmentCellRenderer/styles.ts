import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { DEPARTMENT_PICKER_BUTTON_CLASS, HEIGHT_CELL } from '../../constants';

import { SMALL_ICON_SIZE } from '@/constants/commonStyles';

export const CollapsibleCellWithMenuTypography = styled(Typography)<{
	isSubRow: boolean;
}>(({ isSubRow, theme }) => ({
	paddingLeft: isSubRow ? theme.spaces.m : 0,
	width: '100%',
	textOverflow: 'ellipsis',
	overflow: 'hidden',
}));

export const DepartmentsCellWrapper = styled.div`
	display: flex;
	justify-content: space-between;
	height: 100%;
	width: 100%;
	position: relative;
`;

export const DepartmentsFieldsWrapper = styled.div({
	height: '100%',
	width: '100%',
	display: 'flex',
	flexDirection: 'column',
});

export const DepartmentsGeneralFields = styled('div')<{ newHeight: number }>(
	({ theme, newHeight }) => ({
		width: '100%',
		flexDirection: 'column',
		justifyContent: 'center',
		height: newHeight,
		minHeight: newHeight,
		padding: `${theme.spaces.xs}px ${theme.spaces.s}px`,
		margin: 0,
	})
);

export const DepartmentsSubFields = styled('div')<{ isAccessCell?: boolean }>(
	({ theme, isAccessCell }) => ({
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'space-between',
		height: `${HEIGHT_CELL}px`,
		paddingLeft: isAccessCell ? theme.spaces.s : theme.spaces.m,
		paddingRight: theme.spaces.s,
		borderTop: `1px solid ${theme.palette['graphics-divider-2']}`,

		'& button, button > svg': {
			width: SMALL_ICON_SIZE,
			height: SMALL_ICON_SIZE,
		},

		[`& .${DEPARTMENT_PICKER_BUTTON_CLASS} > svg`]: {
			width: SMALL_ICON_SIZE,
			height: SMALL_ICON_SIZE,
		},
	})
);

export const DepartmentsGeneralFieldWrapper = styled('div')({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'space-between',
	maxHeight: HEIGHT_CELL,

	'& button, button > svg': {
		width: SMALL_ICON_SIZE,
		height: SMALL_ICON_SIZE,
	},

	[`& .${DEPARTMENT_PICKER_BUTTON_CLASS} > svg`]: {
		width: SMALL_ICON_SIZE,
		height: SMALL_ICON_SIZE,
	},
});
