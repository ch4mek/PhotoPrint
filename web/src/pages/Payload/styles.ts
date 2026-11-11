import styled from '@emotion/styled';
import { Skeleton } from '@mms/mms-ui-library/components/Skeleton';
import { CoinsIcon } from '@mms/mms-ui-library/icons/base/CoinsIcon';

import { verticalEllipsisStyles } from '@/components/Grid/renderers/styles';
import { SMALL_ICON_SIZE } from '@/constants/commonStyles';
import {
	DEFAULT_DISABLED_GRID_CELL_OPACITY,
	DEFAULT_OPACITY,
} from '@/constants/opacities';
import { createGradient } from '@/helpers/createGradient';
import { YELLOW_TAG_COLOR, ThemeMode } from '@/theme';

import {
	FIRST_PROJECTS_ROW_HEIGHT,
	PROJECTS_ROW_HEIGHT,
	GRID_SUBROW_HEIGHT,
	HEIGHT_TIME_CELL,
} from './contants';
import {
	CellWrapperProps,
	EmployeeFieldProps,
	MonthCellProps,
	ProjectsGeneralFieldsProps,
	ProjectsGeneralFieldWrapperProps,
	TotalProps,
} from './types';

const CELL_BACKGROUND_OPACITY = 0.65;

export const Wrapper = styled('div')(({ theme }) => ({
	height: '100%',
	display: 'flex',
	flexDirection: 'column',
	rowGap: theme.spaces.m,
}));

export const CellWrapper = styled('div')<CellWrapperProps>(
	({
		theme,
		childHeight = '100%',
		childWidth,
		maxVerticalLineCount,
		shouldPaintCell,
	}) => ({
		paddingLeft: theme.spaces.s,
		paddingRight: theme.spaces.s,
		paddingTop: theme.spaces.xs,
		display: 'flex',
		alignItems: 'flex-start',
		width: '100%',
		height: '100%',

		...(shouldPaintCell && {
			background:
				theme.mode === ThemeMode.LIGHT
					? YELLOW_TAG_COLOR
					: createGradient(
							YELLOW_TAG_COLOR,
							theme.palette['background-white'],
							CELL_BACKGROUND_OPACITY
					  ),
		}),
		...theme.typography.body['m-400'],

		'& > *': {
			...(maxVerticalLineCount && verticalEllipsisStyles(maxVerticalLineCount)),

			width: childWidth,
			height: `${childHeight} !important`,
		},
	})
);

export const MonthCellWrapper = styled('div')<TotalProps>(
	({ theme, isTotal }) => ({
		height: '100%',
		width: '100%',
		position: 'relative',
		overflow: 'visible',

		...(isTotal && {
			display: 'flex',
			gap: theme.spaces.xs,
			flexDirection: 'column',
			alignItems: 'flex-end',
			padding: theme.spaces.xs,

			'& > div': {
				width: '100%',
				textAlign: 'end',
			},
		}),
	})
);

export const ProjectsCellWrapper = styled('div')<{ disabled: boolean }>(
	({ disabled }) => ({
		display: 'flex',
		justifyContent: 'space-between',
		height: '100%',
		width: '100%',
		position: 'relative',
		opacity: disabled ? DEFAULT_DISABLED_GRID_CELL_OPACITY : DEFAULT_OPACITY,
	})
);

export const ProjectsFieldsWrapper = styled.div({
	height: '100%',
	width: '100%',
	display: 'flex',
	flexDirection: 'column',
	overflow: 'hidden',
});

export const ProjectsGeneralFields = styled('div')<ProjectsGeneralFieldsProps>(
	({ theme, newHeight }) => ({
		width: '100%',
		flexDirection: 'column',
		height: '100%',
		maxHeight: newHeight,
		padding: `${theme.spaces.xs}px ${theme.spaces.s}px`,
		margin: 0,
		boxShadow: `0 1px 0 0 ${theme.palette['graphics-divider-1']}`,
	})
);

export const ProjectsSubFields = styled('div')<{ showTime: boolean }>(
	({ theme, showTime }) => ({
		display: 'grid',
		gridTemplateColumns: 'auto 40px',
		height: GRID_SUBROW_HEIGHT + (showTime ? HEIGHT_TIME_CELL : 0),
		paddingTop: theme.spaces.xs,
		paddingLeft: theme.spaces.l,
		paddingRight: theme.spaces.s,
		boxShadow: `0 1px 0 0 ${theme.palette['graphics-divider-1']}`,
		borderBottom: '1px solid transparent',
	})
);

export const ProjectsGeneralFieldWrapper = styled(
	'div'
)<ProjectsGeneralFieldWrapperProps>(({ isFirstRow = false }) => ({
	display: 'grid',
	alignItems: 'center',
	gridTemplateColumns: 'auto 20px 20px',
	maxHeight: isFirstRow ? FIRST_PROJECTS_ROW_HEIGHT : PROJECTS_ROW_HEIGHT,

	'& button, button > svg': {
		width: SMALL_ICON_SIZE,
		height: SMALL_ICON_SIZE,
	},
}));

export const MonthHeaderCell = styled('div')<MonthCellProps>(
	({ isCurrentMonth, theme }) => ({
		width: '100%',
		height: '100%',
		display: 'flex',
		justifyContent: 'flex-end',
		alignItems: 'center',
		color: isCurrentMonth ? theme.palette['text-accent'] : 'inherit',
		...theme.typography.body[isCurrentMonth ? 's-700' : 's-600'],
	})
);

export const IdCellWrapper = styled('div')<{ disabled: boolean }>(
	({ theme, disabled }) => ({
		display: 'flex',
		alignItems: 'center',
		gap: theme.spaces.xs,
		opacity: disabled ? DEFAULT_DISABLED_GRID_CELL_OPACITY : DEFAULT_OPACITY,
	})
);

export const EmployeeCellWrapper = styled(CellWrapper)<{ disabled: boolean }>(
	({ disabled }) => ({
		flexDirection: 'column',
		height: 'auto',
		overflow: 'hidden',
		textOverflow: 'ellipsis',
		opacity: disabled ? DEFAULT_DISABLED_GRID_CELL_OPACITY : DEFAULT_OPACITY,
	})
);

export const EmployeeHeaderRow = styled('div')(({ theme }) => ({
	width: '100%',
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	gap: theme.spaces.xs,
}));

export const EmployeeCoinIcon = styled(CoinsIcon)({
	flex: 'none',
});

export const EmployeeField = styled('span')<EmployeeFieldProps>(
	({ theme, isSpecializationField = false }) => ({
		...(isSpecializationField
			? {
					color: theme.palette['text-gray'],
					...theme.typography.body['s-400'],
			  }
			: {}),
		overflow: 'hidden',
		textOverflow: 'ellipsis',
		maxWidth: '100%',
	})
);

export const PayloadSkeletonCell = styled(Skeleton)({
	display: 'block',
});

export const EmployeeFieldName = styled(EmployeeField)<{
	isNeedToUnderlineUser: boolean;
}>(({ isNeedToUnderlineUser }) => ({
	textDecoration: isNeedToUnderlineUser ? 'underline' : 'none',
	cursor: isNeedToUnderlineUser ? 'pointer' : 'auto',
}));
